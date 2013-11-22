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

    private const string kResultsXmlElement = "results";
    private const string kTargetXmlElement = "target";
	private const string kNameElement = "Name";
    private const string kDefectXmlElement = "defect";
	private const string kRuleXmlElement = "rule";
	private const string kDepRuleXmlElement = "dependency_rule";
	private const string kDependencyXmlElement = "dependency";
	private const string kDependencyTargetXmlAttrib = "DependancyTarget";
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

        //Check if Xml file is empty
        reader.MoveToContent();
        if (reader.IsEmptyElement)
        {
            reader.Read();
            return;
        }

        //Parse the defect rules
        reader.Read();
        while (!reader.EOF)
        {
            if (reader.ReadToFollowing(kResultsXmlElement) && reader.IsStartElement())
            {
                while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == kResultsXmlElement))
                {
                    if (reader.ReadToFollowing(kRuleXmlElement) && reader.IsStartElement())
                    {
                        while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == kRuleXmlElement))
                        {
                            if (reader.ReadToFollowing(kTargetXmlElement) && reader.IsStartElement())
                            {
                                string name = reader.GetAttribute(kNameElement);

                                while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == kTargetXmlElement))
                                {
                                    if (reader.ReadToFollowing(kDefectXmlElement) && reader.IsStartElement())
                                    {
                                        string severity = reader.GetAttribute(kSeverityXmlAttrib);
                                        int severityNum = StringToSeverityLevel(severity);
                                        if (defectDict.ContainsKey(name))
                                        {
                                            defectDict[name] += severityNum;
                                        }
                                        else
                                        {
                                            defectDict.Add(name, severityNum);
                                        }
                                    }

                                    if (!reader.ReadToNextSibling(kDefectXmlElement))
                                    {
                                        reader.ReadToFollowing(kTargetXmlElement);
                                        break;
                                    }
                                }
                            }

                            if (!reader.ReadToNextSibling(kTargetXmlElement))
                            {
                                reader.ReadToFollowing(kRuleXmlElement);
                                break;
                            }
                        }
                    }

                    if (!reader.ReadToNextSibling(kRuleXmlElement))
                    {
                        break;
                    }
                }
                reader.Close();
            }

            if (!reader.ReadToNextSibling(kResultsXmlElement))
            {
                break;
            }
        }

        //Parse the dependency rules
        reader = XmlReader.Create(filename);
        reader.Read();
        while (!reader.EOF)
        {
            if (reader.ReadToFollowing(kResultsXmlElement) && reader.IsStartElement())
            {
                while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == kResultsXmlElement))
                {
                    if (reader.ReadToFollowing(kDepRuleXmlElement) && reader.IsStartElement())
                    {
                        while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == kDepRuleXmlElement))
                        {
                            if (reader.ReadToFollowing(kTargetXmlElement) && reader.IsStartElement())
                            {
                                string targetName = reader.GetAttribute(kNameElement);

                                while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == kTargetXmlElement))
                                {
                                    if (reader.ReadToFollowing(kDependencyXmlElement) && reader.IsStartElement())
                                    {
                                        string dependencyTargetName = reader.GetAttribute(kDependencyTargetXmlAttrib);
                                        if (!dependencyTargetName.Contains("UnityEngine.") && !dependencyTargetName.Contains("System."))
                                        {
                                            string severity = reader.GetAttribute(kSeverityXmlAttrib);
                                            int severityNum = StringToSeverityLevel(severity);

                                            if (!dependencyDict.ContainsKey(targetName))
                                            {
                                                dependencyDict.Add(targetName, new Dictionary<String, int>());
                                            }

                                            if (dependencyDict[targetName].ContainsKey(dependencyTargetName))
                                            {
                                                dependencyDict[targetName][dependencyTargetName] += severityNum;
                                            }
                                            else
                                            {
                                                dependencyDict[targetName].Add(dependencyTargetName, severityNum);
                                            }
                                        }
                                    }

                                    if (!reader.ReadToNextSibling(kDependencyXmlElement))
                                    {
                                        reader.ReadToFollowing(kTargetXmlElement);
                                        break;
                                    }
                                }
                            }

                            if (!reader.ReadToNextSibling(kTargetXmlElement))
                            {
                                reader.ReadToFollowing(kDepRuleXmlElement);
                                break;
                            }
                        }
                    }

                    if (!reader.ReadToNextSibling(kDepRuleXmlElement))
                    {
                        break;
                    }
                }
                reader.Close();
            }

            if (!reader.ReadToNextSibling(kResultsXmlElement))
            {
                break;
            }
        }

        Debug.Log(defectDict.Count);
        Debug.Log(defectDict.Keys.Count);
        Debug.Log (dependencyDict.Count);
        Debug.Log (dependencyDict.Keys.Count);
        ParseComplete(dependencyDict, defectDict);
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
    #endregion
}
