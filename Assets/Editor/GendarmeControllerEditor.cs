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
    public const string kDefaultRuleSetsFileName = "rules.xml";
    public const string kDefaultRuleSets = "default";
    public const string kDefaultAssemblyPath = "\\pandamonium\\Build\\Pandamonium_Data\\Managed\\Assembly-CSharp.dll";
    public const string kDefaultResultsXmlFileName = "test.xml";

    public override void OnInspectorGUI()
    {
        DirectoryInfo applicationPath = Directory.GetParent(Application.dataPath);

        GendarmeController myTarget = (GendarmeController)target;
        DrawDefaultInspector();

        if (myTarget.GendarmeConsoleDirectory == string.Empty || myTarget.UseDefaultGendarmeConsoleDirectory)
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

        if (myTarget.ResultsDirectory == string.Empty || myTarget.UseDefaultResultsDirectory)
        {
            myTarget.ResultsDirectory = applicationPath.FullName;
        }

        if (myTarget.ResultsXmlFileName == string.Empty || myTarget.UseDefaultResultsXmlFileName)
        {
            myTarget.ResultsXmlFileName = kDefaultResultsXmlFileName;
        }

        if (string.IsNullOrEmpty(Path.GetExtension(myTarget.ResultsXmlFileName)))
        {
          myTarget.ResultsXmlFileName += ".xml";
        }

        if (myTarget.RuleSetsFileName == string.Empty || myTarget.UseDefaultRuleSetsFileName)
        {
            myTarget.RuleSetsFileName = kDefaultRuleSetsFileName;
        }

        if (myTarget.RuleSetsToRun == string.Empty || myTarget.UseDefaultRuleSets)
        {
            myTarget.RuleSetsToRun = kDefaultRuleSets;
        }

        if (myTarget.AssemblyFilePath == string.Empty || myTarget.UseDefaultAssemblyFilePath)
        {
            myTarget.AssemblyFilePath = applicationPath.FullName + kDefaultAssemblyPath;
        }

        if (GUILayout.Button("Run Gendarme"))
        {
            string arguments =
                " --config " + myTarget.RuleSetsFileName +
                " --set " + myTarget.RuleSetsToRun +
				" --xml " + "\"" + myTarget.ResultsXmlFullPath + "\"" +
                " --severity " + myTarget.Severity.ToString() +
                " --confidence " + myTarget.Confidence.ToString() +
                " " + "\"" + myTarget.AssemblyFilePath + "\"";

            System.Diagnostics.Process mGendarmeProcess = new System.Diagnostics.Process();
            mGendarmeProcess.StartInfo.FileName = "gendarme.exe";
            mGendarmeProcess.StartInfo.Arguments = arguments;
            mGendarmeProcess.StartInfo.WorkingDirectory = myTarget.GendarmeConsoleDirectory;
            mGendarmeProcess.Start();
            mGendarmeProcess.WaitForExit();
        }

        //Browse to the XML results file if it exists (otherwise browse to the gendarme console directory location)
        if (GUILayout.Button("Explore to XML results file"))
        {
            if (Event.current.button == 0)
            {
                if (File.Exists(myTarget.ResultsXmlFullPath))
                {
                    System.Diagnostics.Process.Start("explorer.exe", string.Format("/select, {0}", myTarget.ResultsXmlFullPath));
                }
                else
                {
                    System.Diagnostics.Process.Start("explorer.exe", string.Format("/select, {0}", myTarget.ResultsDirectory));
                }
            }
        }

        //Reparse the XML file
        if (GUILayout.Button("Reparse XML file"))
        {
            if (Event.current.button == 0)
            {
                XMLParser parser = myTarget.GetComponent<XMLParser>();
                if (parser != null && File.Exists(myTarget.ResultsXmlFullPath))
                {
                    parser.PopulateDefectList(myTarget.ResultsXmlFullPath);
                }
                else
                {
                    Debug.LogError("Missing XMLParser component!");
                }
            }
        }
    }
}
