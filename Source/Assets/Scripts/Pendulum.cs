using UnityEngine;
using System.Collections;

public class Pendulum : MonoBehaviour 
{
	public float ImpactForce = 10.0f;
	
	void OnTriggerStay(Collider hit)
	{
		if(collider.tag == "Player")
		{
			Vector3 playerCenter = hit.transform.position;
			Vector3 pendulumCenter = gameObject.collider.transform.position;
			Vector3 direction = playerCenter - pendulumCenter;
			
			Vector3 force = direction.normalized * ImpactForce * Time.deltaTime;
			hit.transform.position += force;
		}	
	}
}
