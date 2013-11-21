using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.IO;
using System.Text;


public class XMLParser
{
	 //fields used
    private XmlReader reader = XmlReader.Create("test3.xml");
    private int low;
    private int med;
    private int high;
    private int crit;
    private List<Severity> listOfObject;

    static void Main()
    {
        XmlReader readers = XmlReader.Create("test3.xml");

        int low = 0;
        int med = 0;
        int high = 0;
        int crit = 0;
        while (readers.Read())
        {
            readers.ReadToFollowing("target");
            readers.MoveToFirstAttribute();
            string name = readers.Value;
            Console.WriteLine(name);

            readers.ReadToFollowing("defect");
            readers.MoveToFirstAttribute();
            string severity = readers.Value;
            if (severity == "Low")
                low += 1;
            if (severity == "Medium")
                med += 1;
            if (severity == "High")
                high += 1;
            if (severity == "Critical")
                crit += 1;
            Console.WriteLine(severity);
        }
        Console.WriteLine("Number of Low severities: " + low);
        Console.WriteLine("Number of Medium severities: " + med);
        Console.WriteLine("Number of High severities: " + high);
        Console.WriteLine("Number of Critical severities: " + crit);
        Console.WriteLine("Press any key to close...");
        Console.ReadLine();
       
    }

    public XMLParser()
    {
        low = 0;
        med = 0;
        high = 0;
        crit = 0;
        listOfObject = new List<Severity>();
    }

    // parses the xml file, gets the name of the target class and the severity of its defect
    // creates a severity object for each name and defect pair
    // puts each severity object in a list of severity objects
    public void createObjects()
    {
        while (reader.Read())
        {
            reader.ReadToFollowing("target");
            reader.MoveToFirstAttribute();
            string name = reader.Value;

            reader.ReadToFollowing("defect");
            reader.MoveToFirstAttribute();
            string severity = reader.Value;
            int severityLevel = 0;
            if (severity == "Low")
            {
                low += 1;
                severityLevel = 0;
            }
            if (severity == "Medium")
            {
                med += 1;
                severityLevel = 1;
            }
            if (severity == "High")
            {
                high += 1;
                severityLevel = 3;
            }
            if (severity == "Critical")
            {
                crit += 1;
                severityLevel = 9;
            }

            Severity severityObject = new Severity(name, severityLevel);
            listOfObject.Add(severityObject);

        }
    }

    // takes in an element name and which attribute value you want
    // returns a list of values for each attribute value for every element 
    public List<string> getListOfAttribute(string element, string attribute)
    {
        List<string> returnValue = new List<string>();

        while (reader.Read())
        {
            reader.ReadToFollowing(element);
            reader.MoveToAttribute(attribute);
            string genre = reader.Value;
            returnValue.Add(genre);

        }

        foreach (string str in returnValue)
            Console.WriteLine(str);
        return returnValue;
    }


    // takes in an element name of the value you want
    // returns a list of values for every element value 
    public List<string> getListOfElementValues(string element)
    {
        List<string> returnValue = new List<string>();
        while (reader.Read())
        {
            reader.ReadToFollowing(element);
            string genre = reader.ReadElementContentAsString();
            returnValue.Add(genre);

        }
        return returnValue;
    }

    //getter and setter functions for all global variables
    public void setLow(int num)
    {
        low = num;
    }

    public void setMed(int num)
    {
        med = num;
    }

    public void setHigh(int num)
    {
        high = num;
    }

    public void setCrit(int num)
    {
        crit = num;
    }

    public void setList(List<Severity> list)
    {
        listOfObject = list;
    }

    public int getLow()
    {
        return low;
    }

    public int getMed()
    {
        return med;
    }

    public int getHigh()
    {
        return high;
    }

    public int getCrit()
    {
        return crit;
    }

    public List<Severity> getList()
    {
        return listOfObject;
    }
}
