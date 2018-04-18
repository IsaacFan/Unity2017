using System.IO;
using UnityEditor;
using UnityEngine;


public class AutoSetAssetPostprocessor : AssetPostprocessor
{

    private const string ANIMATIONCLIP_EXTENSION = ".anim";
    private const string ANIMATIONCLIP_FOLDER_NAME = "Animation";
    private const string ANIMATIONCLIP_FOLDER_PATH = "Assets/Resources/" + ANIMATIONCLIP_FOLDER_NAME + "/";

    private const string MODLE_EXTENSION = ".fbx";
    private const string SCRIPT_EXTENSION = ".cs";
    private const string SMCS_EXTENSION = ".rsp";
    private const string SCENE_EXTENSION = ".unity";
    

    // import model
    void OnPostprocessModel(GameObject go)
    {
        // does model put into the animation folder
        if (assetPath.Contains(ANIMATIONCLIP_FOLDER_NAME) == true)
        {
            // copy AnimationClip from Model
            copyAnimationClipFromModel(assetPath);
        }
        // only model file(model should be without animation clip)
        else
        {
            // set model import
            setModelImportSettings(assetImporter);
            /*
            // remove Animator dependency
            Animator animator = go.GetComponent<Animator>();
            animator.runtimeAnimatorController = null;
            animator.avatar = null;

            // remove SkinnedMeshRenderer dependency
            SkinnedMeshRenderer skinnedMeshRenderer = go.GetComponentInChildren<SkinnedMeshRenderer>();
            skinnedMeshRenderer.materials = new Material[0];
            skinnedMeshRenderer.sharedMesh = null;
            skinnedMeshRenderer.rootBone = null;
            */
        }      
    }
    private void copyAnimationClipFromModel(string assetPath)
    {
        AnimationClip sourcesClip = null;
        AnimationClip newClip = null;
        string modelFileName = "";
        string modelAnimationFolerPath = "";

        // get model file name 
        modelFileName = Path.GetFileNameWithoutExtension(assetPath);
        // get model animation file folder path
        modelAnimationFolerPath = ANIMATIONCLIP_FOLDER_PATH + modelFileName;
        // recreate model animation folder
        if (Directory.Exists(modelAnimationFolerPath) == true)
            Directory.Delete(modelAnimationFolerPath);
        Directory.CreateDirectory(modelAnimationFolerPath);

        // get all modle objects
        Object[] objects = AssetDatabase.LoadAllAssetsAtPath(assetPath);
        foreach (Object data in objects)
        {
            if (data.GetType() != typeof(AnimationClip))
                continue;

            // filter unity preview animation
            sourcesClip = data as AnimationClip;
            if (sourcesClip.name.Contains("preview") == true)       // use "preview" or "__"
                continue;

            // copy animation
            newClip = new AnimationClip();
            EditorUtility.CopySerialized(sourcesClip, newClip);

            // output animation
            AssetDatabase.CreateAsset(newClip, modelAnimationFolerPath + Path.DirectorySeparatorChar + newClip.name + ANIMATIONCLIP_EXTENSION);
        }

        AssetDatabase.Refresh();
    }
    private void setModelImportSettings(AssetImporter assetImporter)
    {
        // set model to old animation system
        ModelImporter modelImporter = assetImporter as ModelImporter;
        // setting Model
        modelImporter.globalScale = 1.0f;
        modelImporter.meshCompression = ModelImporterMeshCompression.Off;
        modelImporter.isReadable = false;
        modelImporter.optimizeMesh = true;
        modelImporter.importBlendShapes = true;
        modelImporter.swapUVChannels = false;
        modelImporter.generateSecondaryUV = false;
        //modelImporter.normalImportMode = ModelImporterTangentSpaceMode.Import;
        //modelImporter.tangentImportMode = ModelImporterTangentSpaceMode.Calculate;
        modelImporter.importMaterials = true;
        modelImporter.materialName = ModelImporterMaterialName.BasedOnTextureName;
        modelImporter.materialSearch = ModelImporterMaterialSearch.RecursiveUp;

        // setting Rig
        modelImporter.animationType = ModelImporterAnimationType.Generic;
        modelImporter.optimizeGameObjects = false;

        // setting Aniamtions
        modelImporter.importAnimation = false;

        AssetDatabase.Refresh();
    }


    // import sound and music
    void OnPostprocessAudio(AudioClip audioClip)
    {
        AudioImporter audioImporter = assetImporter as AudioImporter;

        audioImporter.forceToMono = false;
        audioImporter.loadInBackground = false;
        audioImporter.preloadAudioData = true;
        /*
        AudioImporterSampleSettings audioImporterSampleSettings = audioImporter.defaultSampleSettings;
        audioImporterSampleSettings.loadType = AudioClipLoadType.DecompressOnLoad;
        audioImporter.SetOverrideSampleSettings("Android", audioImporterSampleSettings);
        */
    }


    // import texture
    void OnPostprocessTexture(Texture2D texture)
    {
        // texture belond UI
        if (assetPath.Contains("UI/Texture/") == true)
        {
            // get folder name
            string spritePackintTag = new DirectoryInfo(Path.GetDirectoryName(assetPath)).Name;
            // set UI texture import
            setUITextureImportSettings(assetPath, assetImporter, spritePackintTag);
        }
        // other texture
        else
        {

        }
    }
    /*
    // texture with alpha safety check
    private void checkTextureWidthHeightByETC2(Texture2D texture)
    {
        int width = texture.width;
        int height = texture.height;
  
        // system limit
        if (((width % 4) != 0) || ((height % 4) != 0))
            Debug.LogError("Texture Size Not Multiple Of 4, File :" + assetPath);      
    }
    */
    private void setUITextureImportSettings(string assetPath, AssetImporter assetImporter, string spritePackintTag)
    {
        TextureImporter textureImporter = assetImporter as TextureImporter;
        if (textureImporter == null)
            return;

        textureImporter.maxTextureSize = 1024;
        textureImporter.textureCompression = TextureImporterCompression.Compressed;
        switch (EditorUserBuildSettings.activeBuildTarget)
        {
            case BuildTarget.Android:              
                textureImporter.textureType = TextureImporterType.Sprite;
                //textureImporter.spriteImportMode = SpriteImportMode.Single;       // set from artist
                setTexturePackingTag(textureImporter, spritePackintTag);
                textureImporter.mipmapEnabled = false;
                textureImporter.filterMode = FilterMode.Bilinear;

                //textureImporter.maxTextureSize = 1024;
                //textureImporter.textureFormat = TextureImporterFormat.AutomaticCompressed;
                textureImporter.compressionQuality = 100;       // Best
                //textureImporter.SetPlatformTextureSettings();
                //textureImporter.SetPlatformTextureSettings("Android", 1024, TextureImporterFormat.AutomaticCompressed, 100, true);
                //textureImporter.SetAllowsAlphaSplitting(true);
                
                break;

            case BuildTarget.iOS:
                /*
                if (assetPath.Contains("Art/UI/Photo/") == true)
                {
                    textureImporter.textureType = TextureImporterType.Advanced;
                    textureImporter.textureFormat = TextureImporterFormat.PVRTC_RGBA4;
                    textureImporter.maxTextureSize = 1024;
                    textureImporter.mipmapEnabled = false;
                }
                else if (assetPath.Contains("Art/Effects/"))
                {
                    textureImporter.textureType = TextureImporterType.Advanced;
                    textureImporter.npotScale = TextureImporterNPOTScale.ToNearest;
                    textureImporter.textureFormat = TextureImporterFormat.PVRTC_RGBA4;
                    textureImporter.maxTextureSize = 1024;
                    textureImporter.mipmapEnabled = false;
                }
                */
                break;

            default:
                /*
                //textureImporter.textureType = TextureImporterType.Sprite;
                //textureImporter.spriteImportMode = SpriteImportMode.Single;
                textureImporter.spritePackingTag = spritePackerTag;
                textureImporter.mipmapEnabled = false;
                textureImporter.filterMode = FilterMode.Bilinear;

                textureImporter.maxTextureSize = 1024;
                textureImporter.textureFormat = TextureImporterFormat.AutomaticCompressed;
                */
                break;
        }
    }
    private static void setTexturePackingTag(TextureImporter textureImporter, string spritePackintTag)
    {
        if (textureImporter == null)
            return;

        textureImporter.spritePackingTag = spritePackintTag;
    }




    // import all datas
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        // import assets
        for (int i = 0; i < importedAssets.Length; i++)
        {
            // skip folder || skip script || skip rsp || scene
            if ((Directory.Exists(importedAssets[i]) == true) ||
                (importedAssets[i].EndsWith(SCRIPT_EXTENSION) == true) ||
                (importedAssets[i].EndsWith(SMCS_EXTENSION) == true) ||
                (importedAssets[i].EndsWith(SCENE_EXTENSION) == true))
            {
                // fix assetbundle name empty
                //setAssetBundleNameEmpty(importedAssets[i]);
                continue;
            }

            // delete model in animation folder
            if (importedAssets[i].Contains(ANIMATIONCLIP_FOLDER_PATH) == true)
            {
                if (importedAssets[i].EndsWith(MODLE_EXTENSION) == true)
                {
                    File.Delete(importedAssets[i]);
                    continue;
                }
            }

            AssetImporter assetImporter = AssetImporter.GetAtPath(importedAssets[i]);

            string assetBundleName = "";
            // textures use sprite packer 
            if (importedAssets[i].Contains("UI/Texture/") == true)
            {
                // use folder name as AssetBundle name
                assetBundleName = new DirectoryInfo(Path.GetDirectoryName(importedAssets[i])).Name;
            }
            // UI prefab pack with dependent textures
            else if (importedAssets[i].Contains("UI/Prefab/") == true)
            {
                // use folder name as AssetBundle name
                assetBundleName = new DirectoryInfo(Path.GetDirectoryName(importedAssets[i])).Name;
            }

            // particle and dependent textures pack together
            else if (importedAssets[i].Contains("Particle/") == true)
            {
                // set and check particle import
                setParticleImportSettings(importedAssets[i]);

                // use folder name as AssetBundle name
                assetBundleName = new DirectoryInfo(Path.GetDirectoryName(importedAssets[i])).Name;
            }
            /*
            // TODO: model and its texture and material need to be packed together
            else if (assetPath.Contains("Model/") == true)
            {
                // use folder name as AssetBundle name
                assetBundleName = new DirectoryInfo(Path.GetDirectoryName(assetPath)).Name;
            }
            */
            // other resources
            else
            {
                // use file name as AssetBundle name
                assetBundleName = Path.GetFileNameWithoutExtension(importedAssets[i]);
            }

            setAssetBundleName(assetImporter, assetBundleName);
        }

        /*
        // delete assets
        foreach (var assetPath in deletedAssets)
        for (int i = 0; i < deletedAssets.Length; i++)
        {
            //Debug.Log("Deleted Asset : " + deletedAssets[i]);
        }
        */

        // move assets
        for (int i = 0; i < movedAssets.Length; i++)
        {
            // get TextureImporter
            AssetImporter assetImporter = AssetImporter.GetAtPath(movedAssets[i]);
            TextureImporter textureImporter = assetImporter as TextureImporter;
            // get folder name
            string spritePackintTag = new DirectoryInfo(Path.GetDirectoryName(movedAssets[i])).Name;
            // set sprite packing tag
            setTexturePackingTag(textureImporter, spritePackintTag);

            //Debug.Log("Moved Asset : " + movedAssets[i] + " From: " + movedFromAssetPaths[i]);
        }
        
        AssetDatabase.Refresh();
    }
    private static void setAssetBundleNameEmpty(string assetPath)
    {
        AssetImporter assetImporter = AssetImporter.GetAtPath(assetPath);
        setAssetBundleName(assetImporter, "");
    }
    private static void setAssetBundleName(AssetImporter assetImporter, string assetBundleName)
    {
        if (assetImporter == null)
            return;

        assetImporter.SetAssetBundleNameAndVariant(assetBundleName, "");
    }

    private static void setParticleImportSettings(string assetPath)
    {
        GameObject particleGameObject = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
        if (particleGameObject == null)
            return;

        ParticleSystem particleSystem = particleGameObject.GetComponent<ParticleSystem>();
        if (particleSystem.main.maxParticles > 50)
            Debug.LogError("Max Particle Is Too Much, File : " + assetPath);

        ParticleSystemRenderer particleSystemRenderer = particleGameObject.GetComponent<ParticleSystemRenderer>();
        particleSystemRenderer.receiveShadows = false;
        particleSystemRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    }


    


    /*
    static void getTextureWidthHeight(TextureImporter textureImporter, out int width, out int height)
    {
        object[] args = new object[2] { 0, 0 };
        MethodInfo methodInfo = typeof(TextureImporter).GetMethod("GetWidthAndHeight", BindingFlags.NonPublic | BindingFlags.Instance);
        methodInfo.Invoke(textureImporter, args);

        width = (int)args[0];
        height = (int)args[1];
    }

    static bool checkTextureWidthHeight(TextureImporter textureImporter, string assetPath)
    {
        int width, height;
        getTextureWidthHeight(textureImporter, out width, out height);

        switch (EditorUserBuildSettings.activeBuildTarget)
        {
            case BuildTarget.Android:
                // system limit
                if (((width % 4) != 0) || ((height % 4) != 0))
                {
                    Debug.LogError("Texture Size Not Multiple Of 4, File :" + assetPath);
                    return false;
                }

                return true;

            case BuildTarget.iOS:
                // no limit?
                return true;

            default:
                return true;
        }
    }
    */

    /*
    static void setParticalTextureImportSettings(TextureImporter textureImporter, string sritePackerTag)
    {
        // TODO
    }

    static void setModelTextureImportSettings(TextureImporter textureImporter, string sritePackerTag)
    {
        // TODO
    }
    */

    /*
    // auto set Texture max size
    static int[] TextureSizes = new int[] {
        32,
        64,
        128,
        256,
        512,
        1024,
        2048,
        4096
    };
    static int checkTextureMaxSize(TextureImporter textureImporter, TextureImporter importer)
    {
        int width, height;
        getTextureWidthHeight(textureImporter, out width, out height);

        int bigSideSize = Mathf.Max(width, height);

        int maxSize = 1024;   // default size
        for (int i = 0; i < TextureSizes.Length; i++)
        {
            if (TextureSizes[i] >= bigSideSize)
            {
                maxSize = TextureSizes[i];
                break;
            }
        }

        return maxSize;
    }
    */

}