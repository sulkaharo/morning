using UnityEngine;
using UnityEditor;
using System;
 
public class CreateScriptableObject
{  
    [MenuItem("Assets/Create/ScriptableObject")]
    public static void CreateMyAsset()
    {
        TasksResource asset = new TasksResource();  //scriptable object
		string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath("Assets/Resources/" + "Tasks.asset");
		AssetDatabase.CreateAsset(asset, assetPathAndName);
        AssetDatabase.SaveAssets();
    }
}
