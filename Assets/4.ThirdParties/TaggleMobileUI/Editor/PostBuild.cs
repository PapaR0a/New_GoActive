#if UNITY_IOS
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using BeliefEngine.UnityEditor.Xcode;
using System.IO;

public class HAPostProcessBuild
{
    
	[PostProcessBuild(999)]
    public static void OnPostProcessBuild( BuildTarget buildTarget, string path)
    {
        if(buildTarget == BuildTarget.iOS)
        {
            string projectPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";

            PBXProject pbxProject = new PBXProject();
            pbxProject.ReadFromFile(projectPath);

            string targetName = PBXProject.GetUnityTargetName();
            string target = pbxProject.TargetGuidByName(targetName);            
            pbxProject.SetBuildProperty(target, "ENABLE_BITCODE", "NO");
            pbxProject.AddFrameworkToProject(target, "CoreBluetooth.framework", false);

            // reference: http://educoelho.com/unity/2015/06/15/automating-unity-builds-with-cocoapods/
            // add cocoa support for firebase
            pbxProject.AddBuildProperty (target, "HEADER_SEARCH_PATHS", "$(inherited)");
            pbxProject.AddBuildProperty (target, "FRAMEWORK_SEARCH_PATHS", "$(inherited)");
            pbxProject.AddBuildProperty (target, "OTHER_CFLAGS", "$(inherited)");
            pbxProject.AddBuildProperty (target, "OTHER_LDFLAGS", "$(inherited)");

            // Copy and replace pod file for firebase
            string podFilePath = "Assets/Firebase/Podfile";
            string destPath = Path.Combine (path, "Pods", Path.GetFileName (podFilePath));
            // if (File.Exists(destPath))
            FileUtil.ReplaceFile(podFilePath, destPath);
            // else
            //     File.Copy(podFilePath, destPath);

            pbxProject.WriteToFile (projectPath);

            // write entitlements
            string entitlementPath = pbxProject.GetBuildPropertyForConfig(target, "CODE_SIGN_ENTITLEMENTS");
            var capabilityManager = new ProjectCapabilityManager(projectPath, entitlementPath, targetName);
            capabilityManager.AddAssociatedDomains(new string[]{
                "applinks:tagglehealthapp.page.link",
                "applinks:taggle.page.link"
            });

            // 
            capabilityManager.WriteToFile();
        }
    }
}
#endif