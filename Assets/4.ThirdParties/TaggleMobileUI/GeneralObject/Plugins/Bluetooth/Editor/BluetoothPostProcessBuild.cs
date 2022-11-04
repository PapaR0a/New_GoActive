#if UNITY_IOS
using UnityEditor.Callbacks;
using UnityEditor;
using UnityEditor.iOS.Xcode;
using System.IO;

public class BluetoothPostProcessBuild
{
    [PostProcessBuild]
    public static void ChangeXcodePlist(BuildTarget buildTarget, string pathToBuiltProject)
    {
        if (buildTarget == BuildTarget.iOS)
        {
            // Get plist
            string plistPath = pathToBuiltProject + "/Info.plist";
            PlistDocument plist = new PlistDocument();
            plist.ReadFromString(File.ReadAllText(plistPath));

            // Get root
            PlistElementDict rootDict = plist.root;

            rootDict.SetString("NSBluetoothPeripheralUsageDescription", "Uses BLE to communicate with devices.");

            // background modes
            PlistElementArray bgModes = rootDict.CreateArray("Required background modes");
            bgModes.AddString("App communicates using CoreBluetooth");
            bgModes.AddString("App shares data using CoreBluetooth");
            bgModes.AddString("App communicates with an accessory");


            // Write to file
            File.WriteAllText(plistPath, plist.WriteToString());
        }
    }

    [PostProcessBuild(10)]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
    {
        if (buildTarget == BuildTarget.iOS)
        {
            string projectPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";
            string plistPath = Path.Combine(path, "Info.plist");

            PBXProject pbxProject = new PBXProject();
            pbxProject.ReadFromFile(projectPath);

            string targetName = PBXProject.GetUnityTargetName();
            string target = pbxProject.TargetGuidByName(targetName);

            // copy file Streaming asset to copy resource
            pbxProject.AddFileToBuild(target, pbxProject.AddFile("Data/Raw/IHealthLabs/com_taggle_healthapp_ios.pem", projectPath+ "/TaggleIHealthLabs/Plugins/iOS/IHealthLabs/Source/com_taggle_healthapp_ios.pem"));
            
            pbxProject.WriteToFile(projectPath);
            
        }
    }

}
#endif