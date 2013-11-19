using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class GendarmeController : MonoBehaviour 
{
    #region Enums
    public enum SeverityLevel
    {
        All,
        Critical,
        High,
        Medium,
        Low,
        Audit,
    }

    public enum ConfidenceLevel
    {
        All,
        Total,
        High,
        Normal,
        Low,
    }
    #endregion

    #region Member Variables
    public bool DebugBuild = true;
    public bool UseDefaultGendarmeConsoleDirectory = true;
	public string GendarmeConsoleDirectory = string.Empty;
    public bool UseDefaultRuleSetsFileName = true;
    public string RuleSetsFileName = string.Empty;
    public bool UseDefaultRuleSets = true;
    public string RuleSetsToRun = string.Empty;
    public bool UseDefaultAssemblyFilePath = true;
    public string AssemblyFilePath = string.Empty;
    public string XmlFileName = string.Empty;

    public SeverityLevel Severity = SeverityLevel.All;
    public ConfidenceLevel Confidence = ConfidenceLevel.All;
    #endregion
}
