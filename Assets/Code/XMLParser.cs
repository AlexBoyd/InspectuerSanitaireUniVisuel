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
        reader.ReadToFollowing(kResultsXmlElement);
        while (!reader.EOF)
        {
            //Parse the results element
            if (reader.IsStartElement())
            {
                //While we aren't at the end of the results element
                reader.ReadToFollowing(kRuleXmlElement);
                while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == kResultsXmlElement))
                {
                    //Parse any rule elements
                    if (reader.IsStartElement())
                    {
                        //While we aren't at the end of the rule element
                        reader.ReadToFollowing(kTargetXmlElement);
                        while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == kRuleXmlElement))
                        {
                            //Parse any target elements
                            if (reader.IsStartElement())
                            {
                                string name = reader.GetAttribute(kNameElement);

                                //While we aren't at the end of the target element
                                reader.ReadToFollowing(kDefectXmlElement);
                                while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == kTargetXmlElement))
                                {
                                    //Parse any defect elements
                                    if (reader.IsStartElement())
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

                                    //If there are no more defect elements, then exit loop
                                    if (!reader.ReadToNextSibling(kDefectXmlElement))
                                    {
                                        break;
                                    }
                                }
                            }

                            //If there are no more target elements, then exit loop
                            if (!reader.ReadToNextSibling(kTargetXmlElement))
                            {
                                break;
                            }
                        }
                    }

                    //If there are no more rule elements, then exit loop
                    if (!reader.ReadToNextSibling(kRuleXmlElement))
                    {
                        break;
                    }
                }
                reader.Close();
            }

            //If there are no more results elements, then exit loop
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
            reader.ReadToFollowing(kResultsXmlElement);
            if (reader.IsStartElement())
            {
                reader.ReadToFollowing(kDepRuleXmlElement);
                while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == kResultsXmlElement))
                {
                    if (reader.IsStartElement())
                    {
                        reader.ReadToFollowing(kTargetXmlElement);
                        while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == kDepRuleXmlElement))
                        {
                            if (reader.IsStartElement())
                            {
                                string targetName = reader.GetAttribute(kNameElement);

                                reader.ReadToFollowing(kDependencyXmlElement);
                                while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == kTargetXmlElement))
                                {
                                    if (reader.IsStartElement())
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
                                        break;
                                    }
                                }
                            }

                            if (!reader.ReadToNextSibling(kTargetXmlElement))
                            {
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
