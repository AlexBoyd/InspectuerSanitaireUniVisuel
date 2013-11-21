using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClassGenerator : MonoBehaviour {
	
	public GameObject ClassPrefab;
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
	
	private void Awake()
	{
		for(int i = 0; i < NumberOfClasses; i++)
		{
			Classes.Add((Instantiate(ClassPrefab, Random.insideUnitSphere * 250f, Quaternion.identity) as GameObject).GetComponent<ClassControl>());
			Classes[Classes.Count -1].ClassGen = this;
			Classes[Classes.Count -1].gameObject.transform.parent = transform;
            Classes[Classes.Count -1].ClassName = i.ToString();
            Classes[Classes.Count -1].gameObject.name = "Class #" + i.ToString();
		}
		
		foreach(ClassControl cc in Classes)
		{
			foreach(ClassControl cd in Classes)
			{
				if(cd != cc && DependancyFactor < Random.value * 100 + 4 * cc.ClassDependancies.Count - Mathf.Pow(cc.ClassDependancies.Count, 1.5f))
				{
					cc.ClassDependancies.Add(new ClassControl.ClassHookup(cd, Random.Range(0.1f, 1f)));
				}
				MaxNumberOfDependancies = Mathf.Max(MaxNumberOfDependancies, cc.ClassDependancies.Count);
			}
		}
	}
}
