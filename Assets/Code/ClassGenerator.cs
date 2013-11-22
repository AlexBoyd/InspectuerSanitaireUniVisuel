using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ClassGenerator : Singleton<ClassGenerator>
{
    #region Tunables
	public GameObject ClassPrefab;
	
	public bool MockValues = false;
	public int NumberOfClasses = 7;
	public float DependancyFactor = 64;
	
	[HideInInspector]
	public int MaxNumberOfDependancies = 0;
	
	public float DepthIncrements = 250f;
	public float DependencyAttractionFactor = 10f;
	public float ClassRepulsionFactor = 20000f;
	public float ManhattanCompressionDampingFactor = 80000f;
	public float ZAxisCompressionDampingFactor = 500f;
	
	public List<ClassControl> Classes;
    #endregion

    #region Component Methods
	private void Awake()
	{
        XMLParser.Instance.ParseComplete += GenerateClasses;
    }

    private void GenerateClasses(Dictionary<string, Dictionary<string, int>> dependencyScores, Dictionary<string, int> defectScores)
    {
		if (MockValues)
		{
			for (int i = 0; i < NumberOfClasses; i++)
			{
				ClassControl cc = (Instantiate(ClassPrefab, Random.insideUnitSphere * 250f, Quaternion.identity) as GameObject).GetComponent<ClassControl>();
				Classes.Add(cc);
				cc.ClassGen = this;
				cc.gameObject.transform.parent = transform;
			    cc.ClassName = i.ToString();
			    cc.gameObject.name = "Class #" + i.ToString();
			}
			
			foreach (ClassControl cc in Classes)
			{
				foreach (ClassControl cd in Classes)
				{
					if(cd != cc && DependancyFactor < Random.value * 100 + 4 * cc.ClassDependancies.Count - Mathf.Pow(cc.ClassDependancies.Count, 1.5f))
					{
						cc.ClassDependancies.Add(new ClassControl.ClassHookup(cd, Random.Range(0.1f, 1f)));
					}
					MaxNumberOfDependancies = Mathf.Max(MaxNumberOfDependancies, cc.ClassDependancies.Count);
				}
			}
		}
		else
		{
			foreach (string className in dependencyScores.Keys)
			{
				ClassControl cc = (Instantiate(ClassPrefab, Random.insideUnitSphere * 250f, Quaternion.identity) as GameObject).GetComponent<ClassControl>();
				Classes.Add(cc);
				cc.ClassGen = this;
				cc.gameObject.transform.parent = transform;
			    cc.ClassName = className;
			    cc.gameObject.name = className;
                if (defectScores.Keys.Contains(className))
                {
				    cc.SewageLevel = defectScores[className];
			    }
                else
                {
                    cc.SewageLevel = 0;
                }
            }
		}
	}

	public IEnumerable<ClassControl> GetDependents(ClassControl cc)
	{
		
		foreach(ClassControl otherCC in Classes)
		{
			if(otherCC.ClassDependancies.Exists((obj) => obj.AttachedClass == cc))
			{
				yield return otherCC;
			}
		}
	}
    #endregion
}
