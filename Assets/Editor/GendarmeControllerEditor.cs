using UnityEngine;
using UnityEditor;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[CustomEditor(typeof(GendarmeController))]
public class GendarmeControllerEditor : Editor
{
    public const string kDefaultGendarmeConsolePath = "D:\\My Documents\\Dropbox\\CPSC 410\\Project\\InspectuerSanitaireUniVisuel\\gendarme\\gendarme\\console\\bin\\Debug\\";

    public const string kDefaultAssemblyPath = "\\pandamonium\\Build\\Pandamonium_Data\\Managed\\Assembly-CSharp.dll";
    public const string kDefaultAssemblyAbsolutePath = "D:\\My Documents\\Dropbox\\CPSC 410\\Project\\InspectuerSanitaireUniVisuel\\pandamonium\\Build\\Pandamonium_Data\\Managed\\Assembly-CSharp.dll";
    public const string kDefaultXmlFileName = "test.xml";

    private System.Diagnostics.Process mGendarmeProcess = new System.Diagnostics.Process();

    public override void OnInspectorGUI()
    {
        GendarmeController myTarget = (GendarmeController)target;
        DrawDefaultInspector();

        if (myTarget.GendarmeConsolePath == string.Empty)
        {
            myTarget.GendarmeConsolePath = kDefaultGendarmeConsolePath;
        }

        if (myTarget.XmlFileName == string.Empty)
        {
            myTarget.XmlFileName = kDefaultXmlFileName;
        }

        if (GUILayout.Button("Run Gendarme"))
        {
            string assemblyFilePath = myTarget.AssemblyFilePath;
//            if (myTarget.AssemblyFile != null)
//            {
//                assemblyFilePath = Application.dataPath;
//            }

            string strCmdText = "chdir gendarme\\gendarme\\console\\bin\\Debug\\";
            string arguments = "--xml " + kDefaultXmlFileName + " --severity " + myTarget.Severity.ToString() + " --confidence " + myTarget.Confidence.ToString() + " " + assemblyFilePath;
            string strCmdText2 = "gendarme";

            mGendarmeProcess.StartInfo.FileName = "gendarme.exe";
            mGendarmeProcess.StartInfo.Arguments = arguments;
            mGendarmeProcess.StartInfo.WorkingDirectory = kDefaultGendarmeConsolePath;
//            mGendarmeProcess.StartInfo.UseShellExecute = false;
//            mGendarmeProcess.StartInfo.RedirectStandardInput = true;
//            mGendarmeProcess.StartInfo.RedirectStandardOutput = true;
//            mGendarmeProcess.StartInfo.RedirectStandardError = true;
//            mGendarmeProcess.StartInfo.CreateNoWindow = false;
            mGendarmeProcess.Start();
//            mGendarmeProcess.StandardInput.WriteLine(strCmdText);
//            mGendarmeProcess.StandardInput.WriteLine(strCmdText2 + " " + arguments);
//            StreamWriter myStreamWriter = mGendarmeProcess.StandardInput;
//            myStreamWriter.WriteLine(strCmdText);
//            string line = mGendarmeProcess.StandardOutput.ReadLine();
//            while (line != null)
//            {
//                Debug.Log(line);
//                line = mGendarmeProcess.StandardOutput.ReadLine();
//            }
//
//            string error = mGendarmeProcess.StandardError.ReadLine();
//            while (error != null)
//            {
//                Debug.Log(error);
//                error = mGendarmeProcess.StandardError.ReadLine();
//            }
            mGendarmeProcess.WaitForExit();
//            mGendarmeProcess.Close();
        }
    }
}
