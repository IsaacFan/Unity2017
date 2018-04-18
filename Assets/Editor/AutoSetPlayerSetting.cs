using UnityEditor;


public class BuildDefine
{
    public static string[] sm_version = new string[]
    {
        "ANDROID_DEBUG_TW",
        //"ANDROID_INNER_RELEASE_TW",
        //"ANDROID_RELEASE_TW",
        //"IOS_DEBUG_TW",
        //"IOS_INNER_RELEASE_TW",
        //"IOS_RELEASE_TW",
    };

    public static string[] sm_androidDebugTW_DefineLabels = new string[]
    {
        "DEBUG_MODE",
        "SHOW_LOGIN_LIST",

        //"RUN_PROJECT_AFTER_BUILD",
        //"LOAD_FROM_ASSETBUNDLE",
	};
    public static string[] sm_androidInnerReleaseTW_DefineLabels = new string[]
    {
		//"DEBUG_MODE",
        //"SHOW_LOGIN_LIST",

        //"RUN_PROJECT_AFTER_BUILD",
        //"LOAD_FROM_ASSETBUNDLE",
    };
    public static string[] sm_androidReleaseTW_DefineLabels = new string[]
    {
		//"DEBUG_MODE",
        //"SHOW_LOGIN_LIST",

        //"RUN_PROJECT_AFTER_BUILD",
        //"LOAD_FROM_ASSETBUNDLE",
    };

#if ANDROID_DEBUG_TW
    public static string[] sm_versionDefineLabels = sm_androidDebugTW_DefineLabels;
#elif ANDROID_TEST_RELEASE_TW
    public static string[] sm_versionDefineLabels = sm_androidInnerReleaseTW_DefineLabels;
#elif ANDROID_RELEASE_TW
    public static string[] sm_versionDefineLabels = sm_androidReleaseTW_DefineLabels;
#else
    public static string[] sm_versionDefineLabels = new string[0];
#endif

}


#if UNITY_EDITOR
[InitializeOnLoad]
public class AutoSetUpPlayerSetting
{
    static AutoSetUpPlayerSetting()
    {
        // set "Scripting Define Symbols"
        string defineCommand = "";

        for (int i = 0; i < BuildDefine.sm_version.Length; i++)
        {
            defineCommand = defineCommand + BuildDefine.sm_version[i] + ";";
        }

        for (int i = 0; i < BuildDefine.sm_versionDefineLabels.Length; i++)
        {
            defineCommand = defineCommand + BuildDefine.sm_versionDefineLabels[i] + ";";
        }


#if UNITY_ANDROID
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, defineCommand);
#elif UNITY_IOS
		PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, defineCommand);	
#elif UNITY_STANDALONE
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, defineCommand);
#endif


#if UNITY_ANDROID
        // set "Identification"

        // display orientation
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;

        // bundle ID
        PlayerSettings.applicationIdentifier = "Isaacs.Unity2017.Demo";
        //  App Name
        PlayerSettings.productName = "Demo";

        // version header + version middle
        PlayerSettings.bundleVersion = "1.0";
        // version end
        PlayerSettings.Android.bundleVersionCode = 1;
        /*
        // bundle version(big bundle version)
        PlayerSettings.bundleVersion = GlobalTypeDefine.VERSION_INFO.bundleVersion;
        // bundle version code(little bundle version)
        PlayerSettings.Android.bundleVersionCode = GlobalTypeDefine.VERSION_INFO.bundleVersionCode; 
        */       
#elif UNITY_IOS
        // bundle ID
        PlayerSettings.bundleIdentifier = GlobalTypeDefine.VERSION_INFO.bundleIdentifier;
        //  App Name
        PlayerSettings.productName = GlobalTypeDefine.VERSION_INFO.productName;

		// version header + version middle
        PlayerSettings.bundleVersion = GlobalTypeDefine.VERSION_INFO.versionHeader.ToString() + "." + GlobalTypeDefine.VERSION_INFO.versionMiddle.ToString();
        // version end
        PlayerSettings.iOS.buildNumber = GlobalTypeDefine.VERSION_INFO.versionEnd.ToString();
#elif UNITY_STANDALONE
    
#endif

    }
}
#endif


