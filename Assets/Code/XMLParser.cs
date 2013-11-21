using UnityEngine;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.IO;
using System.Text;

public class XMLParser : MonoBehaviour
{
    public Action<List<DefectInfo>> DefectListPopulated;
	public int lowSevValue = 1;
	public int medSevValue = 2;
	public int highSevValue = 3;
	public int critSevValue = 4;

	#region Private Members
    private List<DefectInfo> mListOfDefects = new List<DefectInfo>();
	private Dictionary<String, Dictionary<String, int>> dependencyDict = new Dictionary<String, Dictionary<String, int>>();

    private const string kTargetXmlElement = "target";
    private const string kDefectXmlAttribute = "defect";
	private const string kDepRuleXmlElement = "dependency_rule";
	private const string kDependencyXmlElement = "dependency";
	private const string kDependencyTargetXmlAttrib = "DependencyTarget";
	private const string kSeverityXmlAttrib = "Severity";
	
    #endregion

    #region Public Properties
    public IEnumerable<DefectInfo> LowSeverityDefects
    {
        get
        {
            foreach (DefectInfo def in mListOfDefects)
            {
                if (def.Severity == GendarmeController.SeverityLevel.Low)
                {
                    yield return def;
                }
            }
        }
    }

    public IEnumerable<DefectInfo> MediumSeverityDefects
    {
        get
        {
            foreach (DefectInfo def in mListOfDefects)
            {
                if (def.Severity == GendarmeController.SeverityLevel.Medium)
                {
                    yield return def;
                }
            }
        }
    }

    public IEnumerable<DefectInfo> HighSeverityDefects
    {
        get
        {
            foreach (DefectInfo def in mListOfDefects)
            {
                if (def.Severity == GendarmeController.SeverityLevel.High)
                {
                    yield return def;
                }
            }
        }
    }

    public IEnumerable<DefectInfo> CriticalSeverityDefects
    {
        get
        {
            foreach (DefectInfo def in mListOfDefects)
            {
                if (def.Severity == GendarmeController.SeverityLevel.Critical)
                {
                    yield return def;
                }
            }
        }
    }

    private List<DefectInfo> ListOfDefects
    {
        get { return mListOfDefects; }
    }
    #endregion

    #region Singleton stuff
    private static XMLParser mInstance;
    public static XMLParser Instance
    {
        get
        {
            if (mInstance == null)
            {
                Debug.LogWarning(string.Format("No {0} singleton exists! Creating new one.", typeof(XMLParser).Name));
                GameObject owner = new GameObject("XMLParser");
                mInstance = owner.AddComponent<XMLParser>();
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

    private void OnEnable()
    {
        PopulateDefectList(GendarmeController.Instance.ResultsXmlFullPath);
    }
    #endregion

    #region Public Methods
    // parses the given xml file
    // gets the name of the target class and the severity of the defect
    // creates a DefectInfo object for each name and severity pair
    // puts each DefectInfo object in the list of DefectInfo objects
    public void PopulateDefectList(string filename)
    {
        XmlReader reader = XmlReader.Create(filename);
        if (reader != null)
        {
            while (reader.ReadToFollowing(kTargetXmlElement))
            {
                reader.MoveToFirstAttribute();
                string name = reader.Value;

                reader.ReadToFollowing(kDefectXmlAttribute);
                reader.MoveToFirstAttribute();
                GendarmeController.SeverityLevel severity = StringToSeverityLevel(reader.Value);

                mListOfDefects.Add(new DefectInfo(name, severity));
            }
            reader.Close();

            if (DefectListPopulated != null)
            {
                DefectListPopulated(mListOfDefects);
            }
        }
        else
        {
            Debug.LogError(string.Format("Unable to read file {0}", filename), this);
        }
    }
	
	public void PopulateDependencyList(string filename)
	{
	XmlReader reader = XmlReader.Create(filename);
		if (reader != null)
		{
			reader.ReadToFollowing(kDepRuleXmlElement);
			while (reader.ReadToFollowing(kTargetXmlElement))	
			{
				reader.MoveToFirstAttribute();
                string targetName = reader.Value;
				
				while(reader.ReadToFollowing(kDependencyXmlElement))
				{
					reader.MoveToAttribute(kDependencyTargetXmlAttrib);
					string dependencyTargetName = reader.Value;
					if (dependencyTargetName.Substring(0,4) != "Unity" && dependencyTargetName.Substring(0,5) != "System") {
						reader.MoveToAttribute(kSeverityXmlAttrib);
						string severity = reader.Value;
						
						int severityInt = StringToSeverityLevel(severity);
						if (!dependencyDict.ContainsKey(targetName))
						{
							dependencyDict.Add(targetName, new Dictionary<String, int>());
						}
						
						if (dependencyDict[targetName].ContainsKey(dependencyTargetName))
						{
							dependencyDict[targetName][dependencyTargetName] += severityInt;
						}
						else
						{
							dependencyDict[targetName].Add(dependencyTargetName, severityInt);
						}
					}
				}
			}
			reader.Close();
		}
		else
        {
            Debug.LogError(string.Format("Unable to read file {0}", filename), this);
        }
	}
	
    #endregion

    #region Helper Methods
    private int StringToSeverityLevel(string str)
    {
        switch(str)
        {
            case "Low":
                return lowSevValue;
            case "Medium":
                return medSevValue;
            case "High":
                return highSevValue;
            case "Critical":
                return critSevValue;
			case "Audit":
				return 0;		
            default:
                Debug.Log(string.Format("Can't convert {0} string to a SeverityLevel. Returning GendarmeController.SeverityLevel.All", str), this);
                return 0;
        }
    }

    // takes in an element name and which attribute value you want
    // returns a list of values for each attribute value for every element 
    private List<string> getListOfAttribute(string filename, string element, string attribute)
    {
        List<string> returnValue = new List<string>();
        XmlReader reader = XmlReader.Create(filename);

        if (reader != null)
        {

            while (reader.Read())
            {
                reader.ReadToFollowing(element);
                reader.MoveToAttribute(attribute);
                string genre = reader.Value;
                returnValue.Add(genre);
            }

            foreach (string str in returnValue)
            {
                Console.WriteLine(str);
            }
        }
        return returnValue;
    }

    // takes in an element name of the value you want
    // returns a list of values for every element value 
    private List<string> getListOfElementValues(string filename, string element)
    {
        List<string> returnValue = new List<string>();
        XmlReader reader = XmlReader.Create(filename);

        if (reader != null)
        {
            while (reader.Read())
            {
                reader.ReadToFollowing(element);
                string genre = reader.ReadElementContentAsString();
                returnValue.Add(genre);
            }
        }
        return returnValue;
    }
    #endregion
}
