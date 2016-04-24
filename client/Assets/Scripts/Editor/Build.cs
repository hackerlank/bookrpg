using UnityEngine;
using UnityEditor;
using System.Collections;

public class Build : ScriptableObject {

    [MenuItem("Test/Build Asset Bundles")]
    static void BuildABs() {
        // Put the bundles in a folder called "ABs" within the
        // Assets folder.
        BuildPipeline.BuildAssetBundles("Assets/abs");
    }
}
