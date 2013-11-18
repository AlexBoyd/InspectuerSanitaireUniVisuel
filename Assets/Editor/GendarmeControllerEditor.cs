using UnityEngine;
using UnityEditor;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[CustomEditor(typeof(GendarmeController))]
public class GendarmeControllerEditor : Editor
{
    public const string kDefaultGendarmeConsoleDebugDirectory = "\\gendarme\\gendarme\\console\\bin\\Debug\\";
    public const string kDefaultGendarmeConsoleReleaseDirectory = "\\gendarme\\gendarme\\console\\bin\\Release\\";
    public const string kDefaultAssemblyPath = "\\pandamonium\\Build\\Pandamonium_Data\\Managed\\Assembly-CSharp.dll";
    public const string kDefaultXmlFileName = "test.xml";

    private System.Diagnostics.Process mGendarmeProcess = new System.Diagnostics.Process();

    public override void OnInspectorGUI()
    {
        DirectoryInfo applicationPath = Directory.GetParent(Application.dataPath);

        GendarmeController myTarget = (GendarmeController)target;
        DrawDefaultInspector();

        if (myTarget.GendarmeConsoleDirectory == string.Empty)
        {
            if (myTarget.DebugBuild)
            {
                myTarget.GendarmeConsoleDirectory = applicationPath.FullName + kDefaultGendarmeConsoleDebugDirectory;
            }
            else
            {
                myTarget.GendarmeConsoleDirectory = applicationPath.FullName + kDefaultGendarmeConsoleReleaseDirectory;
            }
        }

        if (myTarget.XmlFileName == string.Empty)
        {
            myTarget.XmlFileName = kDefaultXmlFileName;
        }

        if (myTarget.AssemblyFilePath == string.Empty)
        {
            myTarget.AssemblyFilePath = applicationPath.FullName + kDefaultAssemblyPath;
        }

        if (GUILayout.Button("Run Gendarme"))
        {
            string arguments =
				"--xml " + myTarget.XmlFileName +
                " --severity " + myTarget.Severity.ToString() +
                " --confidence " + myTarget.Confidence.ToString() +
                " " + "\"" + myTarget.AssemblyFilePath + "\"";

            mGendarmeProcess.StartInfo.FileName = "gendarme.exe";
            mGendarmeProcess.StartInfo.Arguments = arguments;
            mGendarmeProcess.StartInfo.WorkingDirectory = myTarget.GendarmeConsoleDirectory;
            mGendarmeProcess.Start();
            mGendarmeProcess.WaitForExit();
        }
    }
}
