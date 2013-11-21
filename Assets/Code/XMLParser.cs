using UnityEngine;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.IO;
using System.Text;

public class XMLParser : MonoBehaviour
{
    [SerializeField]private GendarmeController m_GendarmeController = null;

	#region Private Members
    private List<DefectInfo> mListOfDefects = new List<DefectInfo>();

    private const string kTargetXmlElement = "target";
    private const string kDefectXmlAttribute = "defect";
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

    #region Component Methods
    private void Start()
    {
        if (m_GendarmeController == null)
        {
            m_GendarmeController = GetComponent<GendarmeController>();
        }

        if (m_GendarmeController != null)
        {
            PopulateDefectList(m_GendarmeController.ResultsXmlFullPath);
        }
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
        }
        else
        {
            Debug.LogError(string.Format("Unable to read file {0}", filename), this);
        }
    }
    #endregion

    #region Helper Methods
    private GendarmeController.SeverityLevel StringToSeverityLevel(string str)
    {
        switch(str)
        {
            case "Low":
                return GendarmeController.SeverityLevel.Low;
            case "Medium":
                return GendarmeController.SeverityLevel.Medium;
            case "High":
                return GendarmeController.SeverityLevel.High;
            case "Critical":
                return GendarmeController.SeverityLevel.Critical;
            default:
                Debug.Log(string.Format("Can't convert {0} string to a SeverityLevel. Returning GendarmeController.SeverityLevel.All", str), this);
                return GendarmeController.SeverityLevel.All;
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
