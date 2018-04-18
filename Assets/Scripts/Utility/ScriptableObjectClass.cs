//using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ScriptableObjectClass : ScriptableObject {
    public int intData;
    public float floatData;
    public string stringData;

    public int[] intArrayData;
    public List<Vector2> listData;
    public List<ScriptableObjectSubClass> subClassListData;
}

[System.Serializable]
public class ScriptableObjectSubClass {
    public int intData;
    public string stringData;
}