using UnityEngine;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.IO;
using System.Text;

public class XMLParser : Singleton<XMLParser>
{
	public Action<Dictionary<string, Dictionary<string, int>>, Dictionary<string, int>> ParseComplete;

    [SerializeField]private GendarmeController m_GendarmeController = null;

	public int lowSevValue = 1;
	public int medSevValue = 2;
	public int highSevValue = 3;
	public int critSevValue = 4;

	#region Private Members
    private Dictionary<String, int> defectDict = new Dictionary<String, int>();
	private Dictionary<String, Dictionary<String, int>> dependencyDict = new Dictionary<String, Dictionary<String, int>>();

    private const string kTargetXmlElement = "target";
	private const string kNameElement = "Name";
    private const string kDefectXmlElement = "defect";
	private const string kRuleXmlElement = "rule";
	private const string kDepRuleXmlElement = "dependency_rule";
	private const string kDependencyXmlElement = "dependency";
	private const string kDependencyTargetXmlAttrib = "DependencyTarget";
	private const string kSeverityXmlAttrib = "Severity";
	
    #endregion

    #region Component Methods
    private void Start()
    {
        if (m_GendarmeController != null)
        {
            Parse(m_GendarmeController.ResultsXmlFullPath);
        }
        else
        {
            Debug.LogError("Missing GendarmeController reference on XMLParser!", this);
        }
    }
    #endregion

    #region Public Methods
    // parses the given xml file
    // gets the name of the target class and the severity of the defects 
    // 
    public void Parse(string filename)
    {
        XmlReader reader = XmlReader.Create(filename);
        if (reader != null)
        {
			while (reader.ReadToFollowing(kRuleXmlElement))
			{
				bool isDone = false;
				reader.ReadToDescendant(kTargetXmlElement);
	            while (!isDone)
	            {
					Debug.Log("D");	
	                reader.MoveToAttribute(kNameElement);
					
	                string name = reader.Value;
	
	                reader.ReadToFollowing(kDefectXmlElement);
	                reader.MoveToAttribute(kSeverityXmlAttrib);
	                if(defectDict.ContainsKey(name))
					{
						defectDict[name] += StringToSeverityLevel(reader.Value);
					}
					else
					{
						defectDict.Add(name, StringToSeverityLevel(reader.Value));
					}
					isDone = !reader.ReadToNextSibling(kTargetXmlElement);
	            }
			}
			reader.Close();
		}
	    reader = XmlReader.Create(filename);
        if (reader != null)
        {
			reader.ReadToFollowing(kDepRuleXmlElement);
			while (reader.ReadToNextSibling(kDepRuleXmlElement))
			{
				reader.ReadToFollowing(kTargetXmlElement);
				while(reader.ReadToNextSibling(kTargetXmlElement))
				{
					reader.MoveToAttribute(kNameElement);
                	string targetName = reader.Value;
					reader.ReadToFollowing(kDepRuleXmlElement);
					while(reader.ReadToNextSibling(kDependencyXmlElement))
					{
						
						reader.MoveToAttribute(kDependencyTargetXmlAttrib);
						string dependencyTargetName = reader.Value;
						if (!dependencyTargetName.Contains("UnityEngine.") && !dependencyTargetName.Contains("System."))
						{
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
			}
            reader.Close();
			ParseComplete(dependencyDict, defectDict);
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
