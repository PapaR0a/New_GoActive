#if UNITY_IOS
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using UnityEditor.iOS.Xcode;

public class HAPostProcessBuild
{
    private const string CAMERA_USAGE_DESCRIPTION = "This app require to use camera";
    private const string LOCATION_USAGE_DESCRIPTION = "This app require to use location";
    [PostProcessBuild(999)]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
    {
        if (buildTarget == BuildTarget.iOS)
        {
            string projectPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";
            string plistPath = Path.Combine(path, "Info.plist");

            PBXProject pbxProject = new PBXProject();
            pbxProject.ReadFromFile(projectPath);

            string target = pbxProject.GetUnityMainTargetGuid();
            // ==== Binary compiled with ARC and Stack smashing flag
            PlistDocument plist = new PlistDocument();
            plist.ReadFromString(File.ReadAllText(plistPath));
            PlistElementDict rootDict = plist.root;

          
            //Set NSCameraUsageDescription
            rootDict.SetString("NSCameraUsageDescription", CAMERA_USAGE_DESCRIPTION);
            //Set NSLocationWhenInUseUsageDescription
            rootDict.SetString("NSLocationWhenInUseUsageDescription", LOCATION_USAGE_DESCRIPTION);
        }
    }
}
#endif