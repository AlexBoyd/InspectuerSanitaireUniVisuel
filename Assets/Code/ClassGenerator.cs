using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClassGenerator : Singleton<ClassControl>
{
    #region Tunables
	public GameObject ClassPrefab;
	public int NumberOfClasses = 7;
	public float DependancyFactor = 64;
	
	public List<ClassControl> Classes;
    #endregion

    #region Component Methods
	private void Awake()
	{
        XMLParser.Instance.DefectListPopulated += GenerateClasses;
    }

    private void GenerateClasses(List<DefectInfo> defectList)
    {
		for (int i = 0; i < NumberOfClasses; i++)
		{
			Classes.Add((Instantiate(ClassPrefab, Random.insideUnitSphere * 250f, Quaternion.identity) as GameObject).GetComponent<ClassControl>());
			Classes[Classes.Count -1].ClassGen = this;
			Classes[Classes.Count -1].gameObject.transform.parent = transform;
            Classes[Classes.Count -1].ClassName = i.ToString();
            Classes[Classes.Count -1].gameObject.name = "Class #" + i.ToString();
		}

		foreach (ClassControl cc in Classes)
		{
			foreach (ClassControl cd in Classes)
			{
				if(cd != cc && DependancyFactor < Random.value * 100 + 1 * cc.ClassDependancies.Count - Mathf.Pow(cc.ClassDependancies.Count, 1.5f))
				{
					cc.ClassDependancies.Add(new ClassControl.ClassHookup(cd, Random.Range(0.1f, 1f)));
				}
			}
		}
	}
    #endregion
}
