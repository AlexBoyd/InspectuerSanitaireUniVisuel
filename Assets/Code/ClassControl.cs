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
			ch.FlowParticles.particleSystem.startColor = new Color(SewageLevel * 0.05f, 1f - (SewageLevel * 0.05f), 0f);;
			ch.FlowParticles.particleSystem.startSize = SewageLevel;
			ch.FlowParticles.particleSystem.emissionRate = ch.DepedancyValue * 5f;
		}
			
	}
	
	private void Update()
	{
		InnerClassEmitter.emissionRate = SewageLevel / 2f;
		InnerClassEmitter.startColor = new Color(SewageLevel * 0.05f, 1f - (SewageLevel * 0.05f), 100f);	
		
		
		//rigidbody forces are used on each object to do a physical annealing based spatial sort.
		//All class nodes repeal each other, are attracted to their dependancies, and slowing gain drag 
		//and squeezing forces to cause them to settle stocastically close to their dependancies.
		
		foreach(ClassHookup cc in ClassDependancies)
		{
			rigidbody.AddForce((cc.AttachedClass.gameObject.transform.position - gameObject.transform.position).normalized 
				* Mathf.Pow(cc.DepedancyValue, 2f) * 10f);
		}
		
		rigidbody.AddForce((Vector3.zero - new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 0f)).normalized * ManhatanDist(Vector3.zero, gameObject.transform.position).sqrMagnitude/ 80000f );
		
		foreach(ClassControl cc in ClassGen.Classes)
		{
			if(cc != this)
			{
				Vector2 distance = gameObject.transform.position - cc.gameObject.transform.position;
				rigidbody.AddForce(distance.normalized * 20000f / (Mathf.Pow(distance.magnitude, 2f)));
			}	
		}
		//Push to Z = 0;
		rigidbody.AddForce(new Vector3(0,0, -(transform.position.z + (ClassDependancies.Count *100))) * Mathf.Pow(Time.realtimeSinceStartup, 2) / 500f);
		
		
		rigidbody.drag = Time.realtimeSinceStartup * 1f;
	}
	
	void LateUpdate() 
	{          
		//Manage all of the class to class particle streams. 
		foreach(ClassHookup ch in ClassDependancies)
		{
			ParticleSystem.Particle[] particles = new ParticleSystem.Particle[ch.FlowParticles.particleSystem.particleCount];
		    int length = ch.FlowParticles.particleSystem.GetParticles(particles); 
        	int i = 0;
        	//ch.FlowParticles.transform.LookAt(ch.AttachedClass.transform);	
		
			while (i < length) 
			{
            	//Target is a Transform object

	            Vector3 direction = ch.AttachedClass.transform.position - particles[i].position;
	
	            direction.Normalize();
	     
	            float variableSpeed = ch.FlowParticles.particleSystem.startSpeed / (particles[i].lifetime + 0.1f) + particles[i].startLifetime;
	            particles[i].position += direction * variableSpeed * Time.deltaTime;
	            if(Vector3.Distance(ch.AttachedClass.transform.position, particles[i].position) < 1.0f) 
				{
	                particles[i].lifetime = -0.1f; //Kill the particle
	            }
	
	            i++;
	        }
	
	        ch.FlowParticles.particleSystem.SetParticles(particles, length);
		}
    }
		
	private Vector3 ManhatanDist(Vector3 v1, Vector3 v2)
	{
		return new Vector3(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
	}
}
	
