using System;
using System.IO;
using UnityEngine;
using UnityEditor;

public static class GameObjectHelper {

    /// <returns>return File reference</returns>
    public static T CreateFile_ScriptableObject<T>(string dir, string fileNameWithoutExt, string ext = ".asset") where T : ScriptableObject {
        var so = ScriptableObject.CreateInstance<T>();
        so.name = fileNameWithoutExt;
        AssetDatabase.CreateAsset(so, dir + so.name + ext);
        AssetDatabase.SaveAssets();

        so = AssetDatabase.LoadAssetAtPath<T>(dir + so.name + ext);
        return so;
    }

}