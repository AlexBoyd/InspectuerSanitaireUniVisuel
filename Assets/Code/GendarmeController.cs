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
    public bool UseDefaultResultsDirectory = true;
    public string ResultsDirectory = string.Empty;
    public bool UseDefaultResultsXmlFileName = false;
    public string ResultsXmlFileName = string.Empty;

    public SeverityLevel Severity = SeverityLevel.All;
    public ConfidenceLevel Confidence = ConfidenceLevel.All;
    #endregion

    public string ResultsXmlFullPath
    {
        get { return ResultsDirectory + "\\" + ResultsXmlFileName; }
    }

    #region Singleton stuff
    private static GendarmeController mInstance;
    public static GendarmeController Instance
    {
        get
        {
            if (mInstance == null)
            {
                Debug.LogWarning(string.Format("No {0} singleton exists! Creating new one.", typeof(GendarmeController).Name));
                GameObject owner = new GameObject("GendarmeController");
                mInstance = owner.AddComponent<GendarmeController>();
            }
            return mInstance;
        }
    }
    #endregion

    #region Component Methods
    private void Awake()
    {
        if (mInstance != null && mInstance != this)
        {
            Destroy(gameObject);
        }

        mInstance = this;

        DontDestroyOnLoad(gameObject);
    }
    #endregion
}
