using UnityEngine;
using System;
using System.Collections.Generic;

/*
1.播放音樂的function參數目前適用string要改成用編號
2.與AssetBundle合作load音樂檔的方法要改掉, 要再進入場景前先load好
*/

public class AudioManager : BaseSingleton<AudioManager>
{
    #region Typedefs and Enums
    private struct SoundSourceData
    {
        //public int serialNo;
        public int playCount;
        public AudioSource soundAudioSource;
    }
    #endregion


    #region Constants
    private const string AUDIO_MUSIC_FOLDED_PATH = "Audio/Music/";
    private const string AUDIO_SOUND_FOLDED_PATH = "Audio/Sound/";

    private const float DEFAULT_GLOBAL_VALUE = 0.8f;
    private const float DEFAULT_MUSIC_VALUE = 0.8f;
    private const float DEFAULT_SOUND_VALUE = 0.6f;

    protected int MAX_SOUND_AUDIOSOURCE_NUMBER = 20;


    private const int MUSIC_FADE_OUT_COUNT = 10;
    #endregion


    #region Data Members
    private GameObject audioGamgeObject;
    private AudioSource musicAudioSource;

    private float[] musicFadeOutValues = new float[3];        // 0:next decrease volume time.  1:fade out distance time.  2:decrease volume
    private float checkSequenceMusicTime = 0;

    private List<string> musicPlayList = new List<string>();
    private List<SoundSourceData> soundSourceDataList = new List<SoundSourceData>();

   
    private float globalVolume;
    public float GlobalVolume
    {
        get { return globalVolume; }
        set
        {
            saveConfig();
            globalVolume = value;
            resetMusicVolume();
            resetSoundVolume();
        }
    }
    private float musicVolume;
    public float MusicVolume
    {
        get { return musicVolume; }
        set
        {
            saveConfig();
            musicVolume = value;
            resetMusicVolume();
        }
    }
    private float soundVolume;
    public float SoundVolume
    {
        get { return soundVolume; }
        set
        {
            saveConfig();
            soundVolume = value;
            resetSoundVolume();
        }
    }


    protected int m_soundAudioSourceLimit = 20;
    //protected int m_audioSourceSerialNumber = 0;
    /*
    public int audioSourceSerialNo
    {
        get
        {
            return m_audioSourceSerialNumber;
        }
        set
        {
            if (value >= AUDIO_SOURCE_SERIAL_NO_LIMIT)
                value = 1;

            m_audioSourceSerialNumber = value;
        }
    }
    */
    #endregion

    
    #region Constructor
    private AudioManager()
    {
    }
    #endregion
    

    #region Destructor
    ~AudioManager() 
    {
        musicPlayList.Clear();
        soundSourceDataList.Clear();
    }
    #endregion


    #region Override Fuctions
    public override void init()
    {
        loadConfig();

        // init music use audio
        if (audioGamgeObject == null)
        {
            audioGamgeObject = new GameObject("AudioPlayer");
            UnityEngine.Object.DontDestroyOnLoad(audioGamgeObject);
        } 

        musicAudioSource = audioGamgeObject.AddComponent<AudioSource>();

        resetMusicVolume();
        resetSoundVolume();

        GameCore.Instace.registerUpdateAction(update);
    }
    public void destroy()
    {

    }
    #endregion


    #region Fuctions
    private void loadConfig()
    {
        globalVolume = PlayerPrefs.GetFloat("GlobalVolume", DEFAULT_GLOBAL_VALUE);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", DEFAULT_MUSIC_VALUE);
        soundVolume = PlayerPrefs.GetFloat("SoundVolume", DEFAULT_SOUND_VALUE);
    }
    private void saveConfig()
    {
        PlayerPrefs.SetFloat("GlobalVolume", globalVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("SoundVolume", soundVolume);
    }

    private void resetMusicVolume()
    {
        if (musicAudioSource == null)
            return;

        musicAudioSource.volume = globalVolume * musicVolume;
    }
    private void resetSoundVolume()
    {
        for (int i = 0; i < soundSourceDataList.Count; i++)
        {
            soundSourceDataList[i].soundAudioSource.volume = globalVolume * soundVolume;
        }
    }


    private void update()
    {
        // fade out music
        if (musicFadeOutValues[0] != 0)
        {
            if (Time.time >= musicFadeOutValues[0])
            {
                musicFadeOutValues[0] += musicFadeOutValues[1];

                volumeDown(musicFadeOutValues[2]);
            }
        }

        if (checkSequenceMusicTime != 0)
        {
            if (Time.time >= checkSequenceMusicTime)
            {
                checkSequenceMusic();
            }
        }
    }

    private void volumeDown(float decreaseVolume)
    {
        musicAudioSource.volume -= decreaseVolume;
        if (musicAudioSource.volume <= 0)
            checkSequenceMusic();
    }

    private void checkSequenceMusic()
    {
        checkSequenceMusicTime = 0;

        if (musicPlayList.Count <= 0)
            return;

        playNextMusic(musicPlayList[0]);
        musicPlayList.RemoveAt(0);
    }

    public void stopMusic()
    {
        musicAudioSource.Stop();

        checkSequenceMusicTime = 0;

        // stop music fade out -> clear music fade out data
        Array.Clear(musicFadeOutValues, 0, musicFadeOutValues.Length);
    }


    public void playMusic(string fileName, bool isLoop = false)
    {
        AudioClip audioClip = loadAudioClip(AUDIO_MUSIC_FOLDED_PATH + fileName);
        if (audioClip == null)
        {
            //DebugerManager.logError("[AudioManager] playMusic - Load Music Fail : " + fileName);
            return;
        }

        // clear sequence
        musicPlayList.Clear();
        stopAndResetVolume();

        musicAudioSource.clip = audioClip;
        musicAudioSource.loop = isLoop;
        musicAudioSource.Play();

        checkSequenceMusicTime = Time.time + audioClip.length;

        unloadAudioClip(fileName);
    }
    public void playMusicInSequence(string fileName)
    {
        musicPlayList.Add(fileName);
    }
    public void fadeOutMusic(float fadeOutTime)
    {
        // ensure music left time greater than fade out time
        if ((musicAudioSource.clip.length - musicAudioSource.time) <= fadeOutTime)
            return;

        musicFadeOutValues[1] = fadeOutTime / MUSIC_FADE_OUT_COUNT;
        musicFadeOutValues[0] = Time.time + musicFadeOutValues[1];
        musicFadeOutValues[2] = musicVolume / MUSIC_FADE_OUT_COUNT;
    }
    private void playNextMusic(string fileName)
    {
        AudioClip audioClip = loadAudioClip(AUDIO_MUSIC_FOLDED_PATH + fileName);
        if (audioClip == null)
        {
            //DebugerManager.logError("[AudioManager] playNextMusic - Load Music Fail : " + fileName);
            return;
        }

        stopAndResetVolume();

        musicAudioSource.clip = audioClip;
        musicAudioSource.loop = false;
        musicAudioSource.Play();

        checkSequenceMusicTime = Time.time + audioClip.length;

        unloadAudioClip(fileName);
    }
    private void stopAndResetVolume()
    {
        stopMusic();
        resetMusicVolume();
    }


    public void playSound(string fileName, int playCount = 1)
    {
        AudioClip audioClip = loadAudioClip(AUDIO_SOUND_FOLDED_PATH + fileName);
        if (audioClip == null)
        {
            //DebugerManager.logError("[AudioManager] playSound - Load Sound Fail : " + fileName);
            return;
        }

        SoundSourceData data;
        // Search Standby AudioSource
        for (int i = 0; i < soundSourceDataList.Count; i++)
        {
            if (soundSourceDataList[i].soundAudioSource.isPlaying == false)
            {
                data = soundSourceDataList[i];
                //data.serialNo = 0;
                data.playCount = playCount;
                data.soundAudioSource.volume = soundVolume;
                data.soundAudioSource.clip = audioClip;
                if (playCount > 1)
                    data.soundAudioSource.loop = true;
                else
                    data.soundAudioSource.loop = false;
                data.soundAudioSource.Play();

                soundSourceDataList[i] = data;

                unloadAudioClip(fileName);
                return;
            }
        }

        // no standby AudioSource & AudioSource amount is full
        if (soundSourceDataList.Count >= MAX_SOUND_AUDIOSOURCE_NUMBER)
        {
            //DebugerManager.logError("[AudioManager] playSound - Sound AudioSource Number Running Limit : " + m_soundSourceDataList.Count);
            return;
        }

        // no standby AudioSource, create one
        //data.serialNo = 0;
        data.playCount = playCount;
        data.soundAudioSource = audioGamgeObject.AddComponent<AudioSource>();
        data.soundAudioSource.volume = soundVolume;
        data.soundAudioSource.clip = audioClip;
        if (playCount > 1)
            data.soundAudioSource.loop = true;
        else
            data.soundAudioSource.loop = false;
        data.soundAudioSource.Play();

        soundSourceDataList.Add(data);

        unloadAudioClip(fileName);
    }
    /*
    public void stopSound(int serialNo)
    {
        for (int i = 0; i < m_soundSourceDataList.Count; i++)
        {
            if (m_soundSourceDataList[i].soundAudioSource.isPlaying == true)
            {
                if (m_soundSourceDataList[i].serialNo == serialNo)
                {
                    m_soundSourceDataList[i].soundAudioSource.Stop();

                    break;
                }
            }
        }
    }
    */
    public void stopAllSound()
    {
        for (int i = 0; i < soundSourceDataList.Count; i++)
        {
            if (soundSourceDataList[i].soundAudioSource.isPlaying == true)
            {
                soundSourceDataList[i].soundAudioSource.Stop();
            }
        }
    }
    



    private AudioClip loadAudioClip(string audioPath)
    {
        return AssetBundleLoader.Instance.loadAudioClipSync(audioPath);
    }
    private void unloadAudioClip(string audioPath)
    {
        AssetBundleLoader.Instance.unloadAudioClip(audioPath);
    }
    

    

    #endregion
    

}
