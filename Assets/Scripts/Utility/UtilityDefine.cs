using System;
//using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

#region Const variable

public static class UtilityDefine
{

    // const char
    public const char HASHTAG_CHAR = '#'; 

    // const int
    public const int ONE_MILLISECOND_EQUAL_SECOND = 1000;

    // racing map info data path
    //public static string racingMapInfoDataPath = @"Assets/StreamingAssets/RacingMapInfo.asset";

}


#endregion


#region Enum variable
public enum UnityLayerOrder
{
    LayerOrder_None = 0,

    LayerOrder_UI = 5,

    LayerOrder_Enemy = 13,
    LayerOrder_Wall = 14,
}

public enum GameState
{
    GameState_None = 0,
    GameState_PVE,
    GameState_PVP,
    GameState_Co_PVE,
    GameState_Co_PVP,
    GameState_Max,
}


#endregion


#region Structure and class variable

[Serializable]
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class GameConfiguration
{
    public int lapsNumber;
    public int playerInstantiationDelay;    // milliseconds
}

[Serializable]
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class PlayerInfo
{
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    public string name;
    public int velocity;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
    public string colorString;    // need safety check
    public string iconPath;
}


[System.Serializable]
public class RacingMapInfo : ScriptableObject
{
    public int mapID;
    public string mapName;
    public List<Vector2> nodePositionList;
}




#endregion


