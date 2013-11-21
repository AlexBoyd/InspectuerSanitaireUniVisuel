using System;

using UnityEngine;

[Serializable]
public class DefectInfo
{
	#region Private Members
    private GendarmeController.SeverityLevel mSeverity;
    private string mName;
	#endregion

	#region Constructor
    public DefectInfo(string name, GendarmeController.SeverityLevel severity)
    {
        mName = name;
        mSeverity = severity;
    }
	#endregion

	#region Public Properties
	public GendarmeController.SeverityLevel Severity
	{
		get { return mSeverity; }
        set { mSeverity = value; }
	}

    public string Name
    {
        get { return mName; }
        set { mName = value; }
    }
	#endregion
}
