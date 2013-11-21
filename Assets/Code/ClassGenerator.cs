using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ClassGenerator : MonoBehaviour
{
    #region Tunables
	public GameObject ClassPrefab;
	public int NumberOfClasses = 7;
	
	public int ParticleSize = 20f;
	public int SelectedParticleSize = 50f;
	
	public float DependancyFactor = 64;
	public float DependencyHubFactor = 2;
	
	[HideInInspector]
	public int MaxNumberOfDependancies = 0;
	
	public float DepthIncrements = 250f;
	public float DependencyAttractionFactor = 10f;
	public float ClassRepulsionFactor = 20000f;
	public float ManhattanCompressionDampingFactor = 80000f;
	public float ZAxisCompressionDampingFactor = 500f;
	
	public List<ClassControl> Classes;
    #endregion

    #region Singleton stuff
    private static ClassGenerator mInstance;
    public static ClassGenerator Instance
    {
        get
        {
            if (mInstance == null)
            {
                Debug.LogWarning(string.Format("No {0} singleton exists! Creating new one.", typeof(ClassGenerator).Name));
                GameObject owner = new GameObject("Classes");
                mInstance = owner.AddComponent<ClassGenerator>();
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

    private void Start()
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
				if(cd != cc && DependancyFactor < Random.value * 100 + DependencyHubFactor * cc.ClassDependancies.Count - Mathf.Pow(cc.ClassDependancies.Count, 1.5f))
				{
					cc.ClassDependancies.Add(new ClassControl.ClassHookup(cd, Random.Range(0.1f, 1f)));
				}
				MaxNumberOfDependancies = Mathf.Max(MaxNumberOfDependancies, cc.ClassDependancies.Count);
			}
		}
		
	}
	
	public IEnumerable<ClassControl> GetDepenendants	(ClassControl cc)
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
