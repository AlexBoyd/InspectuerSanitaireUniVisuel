using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class ClassControl : MonoBehaviour 
{
	public ParticleSystem InnerClassEmitter;
	public int SewageLevel = 20;
	public List<ClassHookup> ClassDependancies; 
	public ClassGenerator ClassGen;
	public GameObject FlowParticlePrefab;
//	private ParticleSystem.Particle[] particles;
	
	[Serializable]
	public class ClassHookup
	{
		public ClassControl AttachedClass;
		public float DepedancyValue;
		public GameObject FlowParticles;
		
		public ClassHookup(ClassControl cc, float dv)
		{
			this.AttachedClass = cc;
			this.DepedancyValue = dv;
		}
	}
	
	private void Start()
	{
		this.SewageLevel = UnityEngine.Random.Range(0,20);	
		foreach(ClassHookup ch in ClassDependancies)
		{
			ch.FlowParticles = Instantiate(FlowParticlePrefab) as GameObject;
			ch.FlowParticles.transform.parent = transform;
			ch.FlowParticles.transform.localPosition = Vector3.zero;
			ch.FlowParticles.particleSystem.emissionRate = this.SewageLevel * ch.DepedancyValue;
		}
		
		
	}
	
	private void Update()
	{
		InnerClassEmitter.emissionRate = SewageLevel / 2f;
		InnerClassEmitter.startColor = new Color(SewageLevel * 0.05f, 1f - (SewageLevel * 0.05f), 0f);	
		
		foreach(ClassHookup cc in ClassDependancies)
		{
			rigidbody.AddForce((cc.AttachedClass.gameObject.transform.position - gameObject.transform.position).normalized * Mathf.Pow(cc.DepedancyValue, 5f) * 10f / (Time.realtimeSinceStartup + 1));
		}
		
		rigidbody.AddForce((Vector3.zero - new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 0f)).normalized * (Vector3.zero - gameObject.transform.position).sqrMagnitude * (Vector3.zero - gameObject.transform.position).sqrMagnitude / 100000000f);
		
		foreach(ClassControl cc in ClassGen.Classes)
		{
			if(cc != this)
			{
				Vector3 distance = gameObject.transform.position - cc.gameObject.transform.position;
				rigidbody.AddForce(distance.normalized * 1000f / (distance.sqrMagnitude * distance.magnitude));
			}	
		}
		//Push to Z = 0;
		rigidbody.AddForce(new Vector3(0,0, -transform.position.z) * Mathf.Pow(Time.realtimeSinceStartup, 3) / 500f);
		
		
		rigidbody.drag = Time.realtimeSinceStartup * 1f;
	}
	
	void LateUpdate() 
	{  
		
		foreach(ClassHookup ch in ClassDependancies)
		{
			ParticleSystem.Particle[] particles = new ParticleSystem.Particle[100];
        	int length = ch.FlowParticles.particleSystem.GetParticles(particles); 
         	int i = 0;

        	ch.FlowParticles.transform.LookAt(ch.AttachedClass.transform);

	        while (i < length) 
			{
	            if(Vector3.Distance(ch.AttachedClass.transform.position, particles[i].position) < 20f) 
				{
					Debug.Log("Killing the particles");
	                particles[i].velocity = new Vector3(0,0,100000);;
	            }
	
	            i++;
	        }
	
	      	ch.FlowParticles.particleSystem.SetParticles(particles, length); 
		}
    }
}
	
