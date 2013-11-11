using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class GendarmeController : MonoBehaviour 
{
	public string GendarmeConsolePath = string.Empty;
    public string AssemblyFilePath = string.Empty;
//    public UnityEngine.Object AssemblyFile = null;
    public string XmlFileName = string.Empty;
    public string test = string.Empty;

    public SeverityLevel Severity = SeverityLevel.All;
    public ConfidenceLevel Confidence = ConfidenceLevel.All;

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
}
