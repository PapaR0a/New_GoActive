using System.IO;
using UnityEditor.Android;
using UnityEngine;

public class TaggleViatomAndroidPostBuildProcessor : IPostGenerateGradleAndroidProject
{
    public int callbackOrder => 999;

    public void OnPostGenerateGradleAndroidProject(string path)
    {
        Debug.Log("OnPostGenerateGradleAndroidProject Bulid path : " + path);
        string gradlePropertiesFile = path + "/gradle.properties";
        if (File.Exists(gradlePropertiesFile))
        {
            File.Delete(gradlePropertiesFile);
        }
        StreamWriter writer = File.CreateText(gradlePropertiesFile);
        writer.WriteLine("org.gradle.parallel=true");
        writer.WriteLine("android.useAndroidX=true");
        writer.WriteLine("android.enableJetifier=true");
        writer.Flush();
        writer.Close();
    }
}
