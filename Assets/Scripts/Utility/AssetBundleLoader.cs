using UnityEngine;
using System.Collections.Generic;
using System.IO;


public class AssetBundleLoader : BaseSingleton<AssetBundleLoader>
{

    public class LoadedAssetBundle
    {
        public AssetBundle m_assetBundle;
        public int m_referencedCount;

        public LoadedAssetBundle(AssetBundle assetBundle)
        {
            m_assetBundle = assetBundle;
            m_referencedCount = 1;
        }
    }

    /* Typedefs and Enums */


    /* Constants */


    /* Data Members */
    // SDN server URL
    private string m_rootDownloadURL = "http://static-ff.igg.com/";

    //private static Dictionary<string, WWW> m_DownloadingWWWMap = new Dictionary<string, WWW>();
    //private static Dictionary<string, string> m_DownloadingWWWErrorMap = new Dictionary<string, string>();

    // <asset bundle name, asset bundle>
    private Dictionary<string, LoadedAssetBundle> m_loadedAssetBundleMap = new Dictionary<string, LoadedAssetBundle>();
    // <asset bundle name, dependency asset bundle name array>
    private Dictionary<string, string[]> m_dependencyMap = new Dictionary<string, string[]>();

    private AssetBundleManifest m_assetBundleManifest = null;

    //
    //private string m_assetBundleAnalysisDataName = "AssetBundleAnalysisData";
#if LOAD_FROM_ASSETBUNDLE
    // asset bundle CRC map <asset bundle name, CRC string>
    private Dictionary<string, string> m_assetBundleCRCMap = new Dictionary<string, string>();
#endif
    // <asset name, asset bundle name>
    private Dictionary<string, string> m_objectBelongAssetBundleMap = new Dictionary<string, string>();

#if LOAD_FROM_ASSETBUNDLE
    // need to update asset bundle name list
    private List<string> m_waitForUpdateAssetBundleNameList = new List<string>();
#endif

    private string m_assetBundleFolderPath = Application.persistentDataPath + "/AssetBundles";


    /* Constructor */
    private AssetBundleLoader()
    {
    }

    /* Destructor */
    ~AssetBundleLoader()
    {
        releaseAllAssetBundle();
    }

    private void releaseAllAssetBundle()
    {
        var iter = m_loadedAssetBundleMap.GetEnumerator();
        while (iter.MoveNext() == true)
        {
            if (iter.Current.Value == null)
                continue;
            if (iter.Current.Value.m_assetBundle == null)
                continue;

            iter.Current.Value.m_assetBundle.Unload(false);
        }

        m_loadedAssetBundleMap.Clear();
        m_dependencyMap.Clear();
#if LOAD_FROM_ASSETBUNDLE
        m_assetBundleCRCMap.clear();
#endif
        m_objectBelongAssetBundleMap.Clear();
#if LOAD_FROM_ASSETBUNDLE
        m_waitForUpdateAssetBundleNameList.clear();
#endif
    }

    /* Creators */
    public override void init()
    {
        // TODO : 片段程式碼function化
#if LOAD_FROM_ASSETBUNDLE
        // update analysis data asset bundle
        //updateAssetBundle(m_assetBundleAnalysisDataName);
        // get analysis data asset bundle
        LoadedAssetBundle analysisDatabundle = loadAssetBundle(m_assetBundleAnalysisDataName);
        if (analysisDatabundle == null)
        {
            DebugerManager.logError("[AssetBundleLoader] init - Load Asset Bundle Analysis Data Fail : " + m_assetBundleAnalysisDataName);
            return;
        }
        // load asset bundle dependency txt
        TextAsset analysisDataTXT = analysisDatabundle.m_assetBundle.LoadAsset<TextAsset>(m_assetBundleAnalysisDataName);
        if (analysisDataTXT == null)
        {
            DebugerManager.logError("[AssetBundleLoader] init - Load AssetBundle Analysis Data TXT Fail : " + m_assetBundleAnalysisDataName);
            return;
        }
        // analyze asset bundle analysis data
        analyzeAssetBundleAnalysisData(ref m_assetBundleCRCMap, ref m_objectBelongAssetBundleMap, analysisDataTXT);
        // release asset bundle
        unloadAssetBundle(m_assetBundleAnalysisDataName);


        // compare asset bundle CRC
        checkNeedToUpateAssetBundle(m_assetBundleFolderPath, m_assetBundleCRCMap, ref m_waitForUpdateAssetBundleNameList);

        // call update & save update
        // ...


        // load main manifest
        RuntimePlatform assetBundlePlatform;
        switch (Application.platform)
        {
            default:
                assetBundlePlatform = Application.platform;
                break;
            case RuntimePlatform.WindowsEditor:
                assetBundlePlatform = RuntimePlatform.Android;
                break;
            case RuntimePlatform.OSXEditor:
                assetBundlePlatform = RuntimePlatform.IPhonePlayer;
                break;
        }
        string manifestAssetBundleName = getPlatformNameForAssetBundles(assetBundlePlatform);
        LoadedAssetBundle mainManifestbundle = loadAssetBundle(manifestAssetBundleName);
        m_assetBundleManifest = mainManifestbundle.m_assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        // release asset bundle
        unloadAssetBundle(manifestAssetBundleName);

#endif
    }

    /* Methods */
    public void Test()
    {
        /* ok
        // get analyze data asset bundle
        LoadedAssetBundle analyzeDatabundle = loadAssetBundle(m_assetBundleAnalysisDataName);
        if (analyzeDatabundle == null)
        {
            //Debug.LogError("Asset Bundle Init Fail : Load Asset Bundle Analyze Data Fail !");
            return;
        }
        // load asset bundle dependency txt
        TextAsset analyzeDataTXT = analyzeDatabundle.m_assetBundle.LoadAsset<TextAsset>(m_assetBundleAnalysisDataName);
        // handle asset bundle analyze data
        analyzeAssetBundleAnalysisData(ref m_assetBundleCRCMap, ref m_objectBelongAssetBundleMap, analyzeDataTXT);
        // release asset bundle
        unloadAssetBundle(m_assetBundleAnalysisDataName);
        */

        /* ok
        // test load and save asset bundle
        string path = "file:///" + m_assetBundleFolderPath + "/" + m_assetBundleAnalysisDataName;
        WWW www = new WWW(path);
        File.WriteAllBytes(m_assetBundleFolderPath + "/" + "ABC", www.bytes);
        www.Dispose();
        */

        /* ok
        // test load main manifest
        string manifestAssetBundleName = getPlatformNameForAssetBundles(Application.platform);
        LoadedAssetBundle mainManifestbundle = loadAssetBundle(manifestAssetBundleName);
        m_assetBundleManifest = mainManifestbundle.m_assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        // release asset bundle
        unloadAssetBundle(manifestAssetBundleName);
  
        // test main manifest data 
        string[] dependencies = m_assetBundleManifest.GetAllDependencies("scene-bundle");
        //Debug.Log(dependencies[0]);
        */

        /* ok
        // test 
        string path1 = "file:///" + m_assetBundleFolderPath + "/" + m_assetBundleAnalysisDataName;
        string path2 = "Assets/" + m_assetBundleAnalysisDataName;

        byte[] aaa = File.ReadAllBytes(path2);
        var str1 = System.Text.Encoding.Default.GetString(aaa);
        var str2 = System.BitConverter.ToString(aaa);
        var str3 = System.Text.Encoding.ASCII.GetString(aaa);
        */


        /* ok
        // test load prefab
        GameObject cube = loadPrefabSync("Cube.prefab");
        GameObject cube2 = Instantiate(cube);
        unloadAssetBundleByFile("Cube.prefab");
        */

        /*
        // test load scene
        loadSceneSync("Test.unity");
        SceneManager.LoadScene("Test");
        unloadAssetBundleByFile("Test.unity");
        */


        // test Caching.compressionEnabled
        //string assetBundlePath = m_assetBundleFolderPath + "/scenetest";
        /*
        //Caching.compressionEnabled = true;     
        LoadedAssetBundle bundle1 = null;
        bundle1 = new loadedAssetBundle(AssetBundle.LoadFromFile(assetBundlePath));
        m_loadedAssetBundleMap.Add("scenetest", bundle1);
        */
        /*
        Caching.compressionEnabled = false;
        LoadedAssetBundle bundle2 = null;
        bundle2 = new loadedAssetBundle(AssetBundle.LoadFromFile(assetBundlePath));
        m_loadedAssetBundleMap.Add("scenetest", bundle2);
        */

        //string assetBundlePath = "file:///" + m_AssetBundleFolderPath + "/" + m_AssetBundleAnalyzeDataName;
        /*
        Caching.compressionEnabled = false; 
        WWW www1 = new WWW(assetBundlePath);
        //Debug.Log(www1.bytes.Length);
        */


        /*
        string assetBundlePath = "file:///" + m_assetBundleFolderPath + "/talentskilldata";

        Caching.compressionEnabled = false;
        WWW www1 = new WWW(assetBundlePath);
        //Debug.Log(www1.bytes.Length);


        TextAsset abc = www1.assetBundle.LoadAsset<TextAsset>("TalentSkill.txt");
        //Debug.Log(abc.bytes.Length);
        */
    }


    private void updateAssetBundle(string assetBundleName)
    {
        // download asset bundle
        string donwloadURL = m_rootDownloadURL + assetBundleName;
        WWW www = new WWW(donwloadURL);
        //m_www = WWW.LoadFromCacheOrDownload(m_rootDownloadURL + m_assetBundleAnalysisDataName, VersionNumber);
        //yield return m_www;
        if (www.error != null)
        {
            //DebugerManager.logError("[AssetBundleLoader] updateAssetBundle - Download Cause Error : " + assetBundleName);
            return;
        }

        // save www data to local(overwrite)
        File.WriteAllBytes(m_assetBundleFolderPath + "/" + assetBundleName, www.bytes);

        // unload www data
        www.assetBundle.Unload(false);
        www.Dispose();
    }

    public GameObject loadPrefabSync(string prefabPath)
    {
#if LOAD_FROM_ASSETBUNDLE
        // get prefab name
        string prefabName = Path.GetFileName(prefabPath + GlobalTypeDefine.PREFAB_EXTENSION);
        // get assetbundle name
        string assetBundleName = getAssetBundleName(prefabName);
        if (assetBundleName == "")
            return null;

        // loaded asset bundle
        LoadedAssetBundle bundle = loadAssetBundle(assetBundleName);
        if (bundle.m_assetBundle == null)
            return null;

        return bundle.m_assetBundle.LoadAsset<GameObject>(prefabName);
#else
        return Resources.Load<GameObject>(prefabPath);
#endif
    }
    /*
    public IEnumerator loadPrefabAsync(string prefabName)
    {
        string assetBundleName = GetAssetBundleName(prefabName);
        if (assetBundleName == "")
        {
            yield break;
        }
            

        // This is simply to get the elapsed time for this phase of AssetLoading.
        float startTime = Time.realtimeSinceStartup;

        // Load asset from assetBundle.
        AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetAsync(assetBundleName, prefabName, typeof(GameObject));
        if (request == null)
            yield break;
        yield return StartCoroutine(request);

        // Get the asset.
        GameObject prefab = request.GetAsset<GameObject>();

        if (prefab != null)
            GameObject.Instantiate(prefab);

        // Calculate and display the elapsed time.
        float elapsedTime = Time.realtimeSinceStartup - startTime;
        //Debug.Log(prefabName + (prefab == null ? " was not" : " was") + " loaded successfully in " + elapsedTime + " seconds");
    }
    */
    public Texture loadTextureSync(string texturePath)
    {
#if LOAD_FROM_ASSETBUNDLE
        // get texture name
        string textureName = Path.GetFileName(texturePath + GlobalTypeDefine.PNG_EXTENSION);
        // get assetbundle name
        string assetBundleName = getAssetBundleName(textureName);
        if (assetBundleName == "")
            return null;

        // loaded asset bundle
        LoadedAssetBundle bundle = loadAssetBundle(assetBundleName);
        if (bundle.m_assetBundle == null)
            return null;

        return bundle.m_assetBundle.LoadAsset<Texture>(textureName);
#else
        return Resources.Load<Texture>(texturePath);
#endif
    }
    public Sprite loadSpriteSync(string spritePath)
    {
#if LOAD_FROM_ASSETBUNDLE
        // get sprite name
        string spriteName = Path.GetFileName(spritePath + GlobalTypeDefine.PNG_EXTENSION);
        // get assetbundle name
        string assetBundleName = getAssetBundleName(spriteName);
        if (assetBundleName == "")
            return null;

        // loaded asset bundle
        LoadedAssetBundle bundle = loadAssetBundle(assetBundleName);
        if (bundle.m_assetBundle == null)
            return null;

        return bundle.m_assetBundle.LoadAsset<Sprite>(spriteName);
#else
        return Resources.Load<Sprite>(spritePath);
#endif
    }
    public TextAsset loadBytesSync(string dataPath)
    {
#if LOAD_FROM_ASSETBUNDLE
        // get file name
        string dataName = Path.GetFileName(dataPath + GlobalTypeDefine.BYTES_EXTENSION);
        // get assetbundle name
        string assetBundleName = getAssetBundleName(dataName);
        if (assetBundleName == "")
            return null;

        // loaded asset bundle
        LoadedAssetBundle bundle = loadAssetBundle(assetBundleName);
        if (bundle.m_assetBundle == null)
            return null;

        return bundle.m_assetBundle.LoadAsset<TextAsset>(dataName);
#else
        return Resources.Load<TextAsset>(dataPath);
#endif
    }

    public TextAsset loadTXTSync(string dataPath)
    {
#if LOAD_FROM_ASSETBUNDLE
        // get file name
        string dataName = Path.GetFileName(dataPath + GlobalTypeDefine.TXT_EXTENSION);
        // get assetbundle name
        string assetBundleName = getAssetBundleName(dataName);
        if (assetBundleName == "")
            return null;

        // loaded asset bundle
        LoadedAssetBundle bundle = loadAssetBundle(assetBundleName);
        if (bundle.m_assetBundle == null)
            return null;

        return bundle.m_assetBundle.LoadAsset<TextAsset>(dataName);
#else
        return Resources.Load<TextAsset>(dataPath);
#endif
    }
    public TextAsset loadJSONSync(string dataPath)
    {
#if LOAD_FROM_ASSETBUNDLE
        // get file name
        string dataName = Path.GetFileName(dataPath + GlobalTypeDefine.JSON_EXTENSION);
        // get assetbundle name
        string assetBundleName = getAssetBundleName(dataName);
        if (assetBundleName == "")
            return null;

        // loaded asset bundle
        LoadedAssetBundle bundle = loadAssetBundle(assetBundleName);
        if (bundle.m_assetBundle == null)
            return null;

        return bundle.m_assetBundle.LoadAsset<TextAsset>(dataName);
#else
        return Resources.Load<TextAsset>(dataPath);
#endif
    }
    public AudioClip loadAudioClipSync(string audioClipPath)
    {
#if LOAD_FROM_ASSETBUNDLE
        // get file name
        string audioClipName = Path.GetFileName(audioClipPath + GlobalTypeDefine.AUDIO_EXTENSION);
        // get assetbundle name
        string assetBundleName = getAssetBundleName(audioClipName);
        if (assetBundleName == "")
            return null;

        // loaded asset bundle
        LoadedAssetBundle bundle = loadAssetBundle(assetBundleName);
        if (bundle.m_assetBundle == null)
            return null;

        return bundle.m_assetBundle.LoadAsset<AudioClip>(audioClipName);
#else
        return Resources.Load<AudioClip>(audioClipPath);
#endif
    }
    public TerrainData loadTerrainDataSync(string dataPath)
    {
#if LOAD_FROM_ASSETBUNDLE
        // get file name
        string dataName = Path.GetFileName(dataPath + GlobalTypeDefine.TERRAINDATA_EXTENSION);
        // get assetbundle name
        string assetBundleName = getAssetBundleName(dataName);
        if (assetBundleName == "")
            return null;

        // loaded asset bundle
        LoadedAssetBundle bundle = loadAssetBundle(assetBundleName);
        if (bundle.m_assetBundle == null)
            return null;

        return bundle.m_assetBundle.LoadAsset<TerrainData>(dataName);
#else
        return Resources.Load<TerrainData>(dataPath);
#endif
    }
    public RuntimeAnimatorController loadRuntimeAnimatorControllerSync(string dataPath)
    {
#if LOAD_FROM_ASSETBUNDLE
        // get file name
        string dataName = Path.GetFileName(dataPath + GlobalTypeDefine.ANIMATOR_EXTENSION);
        // get assetbundle name
        string assetBundleName = getAssetBundleName(dataName);
        if (assetBundleName == "")
            return null;

        // loaded asset bundle
        LoadedAssetBundle bundle = loadAssetBundle(assetBundleName);
        if (bundle.m_assetBundle == null)
            return null;

        return bundle.m_assetBundle.LoadAsset<RuntimeAnimatorController>(dataName);
#else
        return Resources.Load<RuntimeAnimatorController>(dataPath);
#endif
    }
    public void loadSceneSync(string sceneName)
    {
#if LOAD_FROM_ASSETBUNDLE
        // get variant name
        string assetBundleName = getAssetBundleName(sceneName + GlobalTypeDefine.SCENE_EXTENSION);
        if (assetBundleName == "")
            return;

        // loaded asset bundle
        LoadedAssetBundle bundle = loadAssetBundle(assetBundleName);
        if (bundle.m_assetBundle == null)
            return;
#else
        // do nothing(because scene in build)
#endif
    }
    /*
    protected IEnumerator initializeLevelAsync(string levelName, bool isAdditive)
    {
        // This is simply to get the elapsed time for this phase of AssetLoading.
        float startTime = Time.realtimeSinceStartup;

        // Load level from assetBundle.
        AssetBundleLoadOperation request = AssetBundleManager.LoadLevelAsync("abc", levelName, isAdditive);

        

        if (request == null)
            yield break;
        yield return StartCoroutine(request);

        // Calculate and display the elapsed time.
        float elapsedTime = Time.realtimeSinceStartup - startTime;
        //Debug.Log("Finished loading scene " + levelName + " in " + elapsedTime + " seconds");
    }
    */


    // TODO : 考慮在load的function提供參數, 可以在load完的時候直接試放掉
    //       (因為很多東西是在init scene的時候就都load好了,遊戲進行中並不會在load到)
    //       (而要使用者自己控制assetbundle什麼時候要是放好像有點麻煩, 只讓使用者控制遊戲進行中還會load到的assetbundle應該會比較好)

    public void unloadPrefab(string prefabPath)
    {
#if LOAD_FROM_ASSETBUNDLE
        // get prefab name
        string prefabName = Path.GetFileName(prefabPath + GlobalTypeDefine.TXT_EXTENSION);
        // get assetbundle name
        string assetBundleName = getAssetBundleName(prefabName);
        if (assetBundleName == "")
            return;

        unloadAssetBundle(assetBundleName);
#endif
    }
    public void unloadTexture(string texturePath)
    {
#if LOAD_FROM_ASSETBUNDLE
        // get texture name
        string textureName = Path.GetFileName(texturePath + GlobalTypeDefine.PNG_EXTENSION);
        // get assetbundle name
        string assetBundleName = getAssetBundleName(textureName);
        if (assetBundleName == "")
            return;

        unloadAssetBundle(assetBundleName);
#endif
    }
    public void unloadSprite(string spritePath)
    {
#if LOAD_FROM_ASSETBUNDLE
        // get sprite name
        string spriteName = Path.GetFileName(spritePath + GlobalTypeDefine.PNG_EXTENSION);
        // get assetbundle name
        string assetBundleName = getAssetBundleName(spriteName);
        if (assetBundleName == "")
            return;

        unloadAssetBundle(assetBundleName);
#endif
    }
    public void unloadBytes(string dataPath)
    {
#if LOAD_FROM_ASSETBUNDLE
        // get file name
        string dataName = Path.GetFileName(dataPath + GlobalTypeDefine.TXT_EXTENSION);
        // get assetbundle name
        string assetBundleName = getAssetBundleName(dataName);
        if (assetBundleName == "")
            return;

        unloadAssetBundle(assetBundleName);
#endif
    }
    public void unloadTXT(string dataPath)
    {
#if LOAD_FROM_ASSETBUNDLE
        // get file name
        string dataName = Path.GetFileName(dataPath + GlobalTypeDefine.TXT_EXTENSION);
        // get assetbundle name
        string assetBundleName = getAssetBundleName(dataName);
        if (assetBundleName == "")
            return;

        unloadAssetBundle(assetBundleName);
#endif
    }
    public void unloadAudioClip(string audioPath)
    {
#if LOAD_FROM_ASSETBUNDLE
        // get audio clip name
        string audioClipName = Path.GetFileName(audioPath + GlobalTypeDefine.TXT_EXTENSION);
        // get assetbundle name
        string assetBundleName = getAssetBundleName(audioClipName + GlobalTypeDefine.AUDIO_EXTENSION);
        if (assetBundleName == "")
            return;

        unloadAssetBundle(assetBundleName);
#endif
    }
    public void unloadScene(string sceneName)
    {
#if LOAD_FROM_ASSETBUNDLE
        string assetBundleName = getAssetBundleName(sceneName + GlobalTypeDefine.SCENE_EXTENSION);
        if (assetBundleName == "")
            return;

        unloadAssetBundle(assetBundleName);
#endif
    }




    private string getPlatformNameForAssetBundles(RuntimePlatform platform)
    {
        switch (platform)
        {
            case RuntimePlatform.Android:
                return "Android";
            case RuntimePlatform.IPhonePlayer:
                return "iOS";
            default:
                return null;
        }
    }

    private void analyzeAssetBundleAnalysisData(ref Dictionary<string, string> crcMap, ref Dictionary<string, string> belongMap, TextAsset analysisDataTXT)        
    {
        //DebugerManager.logConsole("[AssetBundleLoader] - Analyze Asset Bundle Analysis Data TXT Start");

        MemoryStream stream = new MemoryStream(analysisDataTXT.bytes);
        StreamReader reader = new StreamReader(stream);

        string stringLine = "";
        string assetBundleName = "";
        bool getAssetBundleName = false;
        string crcString = "";
        bool getCRCString = false;
        string assetName = "";

        stringLine = reader.ReadLine();
        while (stringLine != null)
        {
            assetBundleName = "";
            getAssetBundleName = false;
            crcString = "";
            getCRCString = false;

            // Format : [asset bundel name, CRC string, asset1, asset2, asset3, .....]
            for (int i = 0; i < stringLine.Length; i++)
            {
                // get asset bundle name
                if (getAssetBundleName == false)
                {
                    if (stringLine[i] == ',')
                        getAssetBundleName = true;
                    else
                        assetBundleName += stringLine[i];
                }
                // get CRC string
                else if (getCRCString == false)
                {
                    if (stringLine[i] == ',')
                    {
                        getCRCString = true;
                        // save CRC
                        crcMap.Add(assetBundleName, crcString);
                    }
                    else
                        crcString += stringLine[i];
                }
                // get assets
                else
                {
                    if (stringLine[i] == ',')
                    {
                        // security code
                        if (belongMap.ContainsKey(assetName) == false)
                        {
                            // save object-assetbundle data
                            belongMap.Add(assetName, assetBundleName);
                        }
                        assetName = "";
                    }
                    else
                        assetName += stringLine[i];
                }
            }

            stringLine = reader.ReadLine();
        }

        //DebugerManager.logConsole("[AssetBundleLoader] - Analyze Asset Bundle Analysis Data TXT Success");
    }

    private void checkNeedToUpateAssetBundle(string localAssetBundlePath, Dictionary<string, string> newCRCMap, ref List<string> waitForUpdateList)
    {
        StreamReader reader = null;
        string text = null;
        string localCRCString = "";
        int crcIndex = 0;
        string sdnCRCString = "";

        // check all local manifest
        string fileName = "";       // without extension 
        foreach (string fileFullName in Directory.GetFiles(localAssetBundlePath))
        {
            if (fileFullName.EndsWith(".manifest") == false)
                continue;

            reader = new StreamReader(fileFullName);

            text = reader.ReadLine();
            while (text != null)
            {
                if (text.Contains("CRC:") == true)
                {
                    crcIndex = text.IndexOf(" ");
                    localCRCString = text.Substring(crcIndex + 1, text.Length - crcIndex - 1);

                    fileName = Path.GetFileNameWithoutExtension(fileFullName);
                    // local exist file, CDN did not
                    if (newCRCMap.TryGetValue(fileName, out sdnCRCString) == false)
                    {

                    }
                    // local CRC == CDN CRC
                    else if (localCRCString.Equals(sdnCRCString) == true)
                    {
                        // do nothing
                    }
                    // local CRC != CDN CRC -> need to update
                    else
                    {
                        waitForUpdateList.Add(fileName);
                    }

                    break;
                }

                text = reader.ReadLine();
            }
        }
    }

    private string getAssetBundleName(string fileName)
    {
        if (m_objectBelongAssetBundleMap.ContainsKey(fileName) == false)
        {
            //DebugerManager.logError("[AssetBundleLoader] getAssetBundleName - Can Not Find Name In Object Belong AssetBundle Map : " + m_assetBundleAnalysisDataName);
            return "";
        }

        return m_objectBelongAssetBundleMap[fileName];
    }

    private LoadedAssetBundle loadAssetBundle(string assetBundleName)
    {
        // get already loaded
        LoadedAssetBundle bundle = null;
        m_loadedAssetBundleMap.TryGetValue(assetBundleName, out bundle);
        if (bundle != null)
        {
            bundle.m_referencedCount++;
            return bundle;
        }

        // load asset bundle
        string assetBundlePath = m_assetBundleFolderPath + "/" + assetBundleName;
        bundle = new LoadedAssetBundle(AssetBundle.LoadFromFile(assetBundlePath));
        if (bundle.m_assetBundle == null)
        {
            //DebugerManager.logError("[AssetBundleLoader] loadAssetBundle - Load Local AssetBundle Fail : " + assetBundleName);
            return null;
        }
        else
        {
            /*
            // 解密 : 上方load asset bundle改成
            string path = "file:///" + m_AssetBundleFolderPath + "/" + assetBundleName;
            WWW www = new WWW(path);
            byte[] encryptedData = www.bytes;
            byte[] decryptedData = YourDecryptionMethod(encryptedData);
            bundle.m_AssetBundle = AssetBundle.LoadFromMemory(decryptedData);
            www.Dispose();
            */

            m_loadedAssetBundleMap.Add(assetBundleName, bundle);
        }

        // check dependency
        string[] dependencies = null;
        if (m_assetBundleManifest != null)
            dependencies = m_assetBundleManifest.GetAllDependencies(assetBundleName);
        if (dependencies == null)
            return bundle;
        //if (dependencies.Length == 0)
        //    return bundle;

        // get all dependency name
        for (int i = 0; i < dependencies.Length; i++)
            dependencies[i] = getAssetBundleName(dependencies[i]);

        // save data and load all dependency asset bundle
        m_dependencyMap.Add(assetBundleName, dependencies);
        for (int i = 0; i < dependencies.Length; i++)
            loadAssetBundle(dependencies[i]);

        return bundle;
    }
    private void unloadAssetBundle(string assetBundleName)
    {
#if LOAD_FROM_ASSETBUNDLE
        // check loaded asset bundle map
        LoadedAssetBundle bundle = null;
        m_loadedAssetBundleMap.TryGetValue(assetBundleName, out bundle);
        if (bundle != null)
        {
            // no reference -> release asset bundle
            if (bundle.m_referencedCount <= 1)
            {
                bundle.m_assetBundle.Unload(false);
                m_loadedAssetBundleMap.Remove(assetBundleName);
            }
            else
            {
                bundle.m_referencedCount--;
            }
        }

        // check dependency map
        string[] dependencies = null;
        m_dependencyMap.TryGetValue(assetBundleName, out dependencies);
        if (dependencies != null)
        {
            for (int i = 0; i < dependencies.Length; i++)
            {
                unloadAssetBundle(dependencies[i]);
            }
        }
#endif
    }





}





