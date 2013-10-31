using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClassGenerator : MonoBehaviour {
	
	public GameObject ClassPrefab;
	public int NumberOfClasses = 7;
	public float DependancyFactor = 64;
	
	public List<ClassControl> Classes;
	
	private void Awake()
	{
		for(int i = 0; i < NumberOfClasses; i++)
		{
			Classes.Add((Instantiate(ClassPrefab, Random.insideUnitSphere * 250f, Quaternion.identity) as GameObject).GetComponent<ClassControl>());
			Classes[Classes.Count -1].ClassGen = this;
			Classes[Classes.Count -1].gameObject.transform.parent = transform;
		}
		
		foreach(ClassControl cc in Classes)
		{
			foreach(ClassControl cd in Classes)
			{
				if(cd != cc && DependancyFactor < Random.value * 100 + 1 * cc.ClassDependancies.Count - Mathf.Pow(cc.ClassDependancies.Count, 1.5f))
				{
					cc.ClassDependancies.Add(new ClassControl.ClassHookup(cd, Random.Range(0.1f, 1f)));
				}
			}
		}
	}
}
