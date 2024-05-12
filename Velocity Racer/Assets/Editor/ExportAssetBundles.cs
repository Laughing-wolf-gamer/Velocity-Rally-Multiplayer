using System.IO;
using UnityEditor;
using UnityEngine;

public class ExportAssetBundles {
    [MenuItem("Assets/Build AssetBundle")]
    public static void ExportResource() {
        string folderName = "AssetBundles";
        string filePath = Path.Combine(Application.streamingAssetsPath, folderName);

        //Build for Windows platform
        // BuildPipeline.BuildAssetBundles(filePath, BuildAssetBundleOptions.None, BuildTarget.Android);

        //Uncomment to build for other platforms
        //BuildPipeline.BuildAssetBundles(filePath, BuildAssetBundleOptions.None, BuildTarget.iOS);
        BuildPipeline.BuildAssetBundles(filePath, BuildAssetBundleOptions.None,BuildTarget.Android);
        //BuildPipeline.BuildAssetBundles(filePath, BuildAssetBundleOptions.None, BuildTarget.WebGL);
        //BuildPipeline.BuildAssetBundles(filePath, BuildAssetBundleOptions.None, BuildTarget.StandaloneOSX);

        //Refresh the Project folder
        AssetDatabase.Refresh();
    }
}