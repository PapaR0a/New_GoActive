#if UNITY_IOS
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using UnityEditor.iOS.Xcode;

public class IHealthPostBuild
{
    private const string BLUETOOTH_USAGE_DESCRIPTION = "Get data from wearble devices";
    
    [PostProcessBuild(200)]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
    {
        if (buildTarget == BuildTarget.iOS)
        {
            string projectPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";
            string plistPath = Path.Combine(path, "Info.plist");

            PBXProject pbxProject = new PBXProject();
            pbxProject.ReadFromFile(projectPath);
           
            string target = PBXProject.GetUnityTargetName(); 
            pbxProject.AddFrameworkToProject(target, "CoreBluetooth.framework", false);
            pbxProject.AddFrameworkToProject(target, "UIKit.framework", false);
            pbxProject.AddFrameworkToProject(target, "Foundation.framework", false);
            pbxProject.AddFrameworkToProject(target, "Security.framework", false);
            pbxProject.AddFrameworkToProject(target, "MediaPlayer.framework", false);
            pbxProject.AddFrameworkToProject(target, "SystemConfiguration.framework", false);
            pbxProject.AddFrameworkToProject(target, "Accelerate.framework", false);
            pbxProject.AddFrameworkToProject(target, "MobileCoreServices.framework", false);
            pbxProject.AddFrameworkToProject(target, "CFNetwork.framework", false);
            pbxProject.AddFrameworkToProject(target, "UserNotificationsUI.framework", false);
            pbxProject.AddFrameworkToProject(target, "UserNotifications.framework", false);
            pbxProject.AddFrameworkToProject(target, "ExternalAccessory.framework", false);
            pbxProject.WriteToFile(projectPath);

            // reference: http://educoelho.com/unity/2015/06/15/automating-unity-builds-with-cocoapods/
            // add cocoa support for firebase
            // pbxProject.AddBuildProperty(target, "HEADER_SEARCH_PATHS", "$(inherited)");
            // pbxProject.AddBuildProperty(target, "FRAMEWORK_SEARCH_PATHS", "$(inherited)");
            // pbxProject.AddBuildProperty(target, "OTHER_CFLAGS", "$(inherited)");
            // pbxProject.AddBuildProperty(target, "OTHER_LDFLAGS", "$(inherited)");

            // ==== Swift
            // pbxProject.SetBuildProperty(target, "ENABLE_BITCODE", "NO");
            // pbxProject.SetBuildProperty(target, "SWIFT_OBJC_BRIDGING_HEADER", "$(PROJECT_DIR)/Libraries/TaggleIHealthLabs/Plugins/iOS/IHealthLabs/Source/IHealthLabs-Bridging-Header.h");
            // pbxProject.SetBuildProperty(target, "SWIFT_OBJC_INTERFACE_HEADER_NAME", "IHealthLabs-Swift.h");
            // pbxProject.AddBuildProperty(target, "LD_RUNPATH_SEARCH_PATHS", "@executable_path/Frameworks");
            // pbxProject.AddBuildProperty(target, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");
            // pbxProject.AddBuildProperty(target, "LD_RUNPATH_SEARCH_PATHS","@executable_path/Frameworks $(PROJECT_DIR)/lib/$(CONFIGURATION) $(inherited)");
            // pbxProject.AddBuildProperty(target, "FRAMERWORK_SEARCH_PATHS","$(inherited) $(PROJECT_DIR) $(PROJECT_DIR)/Frameworks");
            // pbxProject.AddBuildProperty(target, "DYLIB_INSTALL_NAME_BASE", "@rpath");
            // pbxProject.AddBuildProperty(target, "LD_DYLIB_INSTALL_NAME",	"@executable_path/../Frameworks/$(EXECUTABLE_PATH)");

            // pbxProject.AddBuildProperty(target, "DEFINES_MODULE", "YES");
            // pbxProject.AddBuildProperty(target, "SWIFT_VERSION", "5");
            // pbxProject.AddBuildProperty(target, "COREML_CODEGEN_LANGUAGE", "Swift");


            // // Enable clang module to use Swift SDK in Objective-C++.
            // pbxProject.SetBuildProperty(target, "CLANG_ENABLE_MODULES", "YES");
            // // ==== Binary compiled with ARC and Stack smashing flag
            // pbxProject.AddBuildProperty(target, "OTHER_CFLAGS", "-fstack-protector-all");
            // pbxProject.AddBuildProperty(target, "OTHER_LDFLAGS", "-ObjC");

            // copy file Streaming asset to copy resource
            //pbxProject.AddFileToBuild(target, pbxProject.AddFile("Data/Raw/IHealthLabs/com_taggle_testihealths_ios.pem", projectPath+"/TaggleIHealthLabs/Plugins/iOS/IHealthLabs/Source/com_taggle_testihealths_ios.pem"));


            // PlistDocument plist = new PlistDocument();
            // plist.ReadFromString(File.ReadAllText(plistPath));
            // PlistElementDict rootDict = plist.root;
            // // set NSAppTransportSecurity
            // //PlistElementDict NSAppTransportSecurity = rootDict.CreateDict("NSAppTransportSecurity");
            // //NSAppTransportSecurity.SetBoolean("NSAllowsArbitraryLoads",false);
            // // set NSBluetoothAlwaysUsageDescription
            // rootDict.SetString("NSBluetoothAlwaysUsageDescription", BLUETOOTH_USAGE_DESCRIPTION);
            // // Debug.Log("Set tring NSBluetoothAlwaysUsageDescription");
            // IDictionary<string, PlistElement> rootDic = rootDict.values;
            // // remove NSAppTransportSecurity key info.plist
            // rootDic.Remove("NSAppTransportSecurity");
            // rootDic.Remove("NSAllowsArbitraryLoads");
            // // remove UIApplicationExitsOnSuspend key info.plist
            // rootDic.Remove("UIApplicationExitsOnSuspend");
            // File.WriteAllText(plistPath, plist.WriteToString());


            // write entitlements
            // string entitlementPath = pbxProject.GetBuildPropertyForConfig(target, "CODE_SIGN_ENTITLEMENTS");
            // ProjectCapabilityManager capabilityManager = new ProjectCapabilityManager(projectPath, entitlementPath, targetName);
            // capabilityManager.AddAssociatedDomains(new string[]
            // {
            //     "applinks:tagglehealthapp.page.link", "applinks:nhghealthapp.page.link" // "applinks:taggle.page.link"
            // });

            // 
            //capabilityManager.WriteToFile();

            // Copy and replace pod file for firebase
            //string podFilePath = "Assets/Pods/Podfile";
            //string destPath = Path.Combine(path, "Pods", Path.GetFileName(podFilePath));
            //if (File.Exists(destPath))
            //    FileUtil.ReplaceFile(podFilePath, destPath);
            //else
            //     File.Copy(podFilePath, destPath);
        }
    }
}
#endif