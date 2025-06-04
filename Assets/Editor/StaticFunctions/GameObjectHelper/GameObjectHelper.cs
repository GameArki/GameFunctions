using System;
using System.IO;
using UnityEngine;
using UnityEditor;

public static class GameObjectHelper {

    /// <summary>
    /// Create a GameObject with the specified name and parent. eg:
    /// <para>dir: "Assets/Res_Runtime"</para>
    /// <para>fileName: "MyFileName"</para>
    /// <para>ext: ".asset"</para>
    /// </summary>
    /// <returns>return the File reference</returns>
    public static T CreateFile_ScriptableObject<T>(string dir, string fileNameWithoutExt, string ext = ".asset") where T : ScriptableObject {
        var so = ScriptableObject.CreateInstance<T>();
        so.name = fileNameWithoutExt;
        string file = Path.Combine(dir, so.name + ext);
        AssetDatabase.CreateAsset(so, file);
        AssetDatabase.SaveAssets();

        so = AssetDatabase.LoadAssetAtPath<T>(file);
        return so;
    }

}