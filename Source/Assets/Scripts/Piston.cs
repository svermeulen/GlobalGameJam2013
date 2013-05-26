using UnityEngine;
//using UnityEditor; //not allowed in package builds
using System.Collections;

public class Piston : MonoBehaviour 
{
	public enum PistonState
	{
		Waiting,
		Extending,
		Extended,
		Retracting
	};
	
	public float InitialOffset = 0.0f;
	public float IdleWaitTime = 0.5f;
	public float ExtendedWaitTime = 1.0f;
	
	public float RandomMin = 0.0f;
	public float RandomMax = 0.0f;
	
	public float ImpactForce = 10.0f;
	
	public Animation PistonAnimation;
		
	private float NextActivationDuration = 0.0f;
	private float RunningTime = 0.0f;
	private PistonState state = PistonState.Waiting;
	
	// Use this for initialization
	void Start () 
	{
		state = PistonState.Waiting;
		PistonAnimation.Play("Idle");
		
		NextActivationDuration = InitialOffset;
		if(RandomMin != RandomMax)
			NextActivationDuration += Random.Range(RandomMin, RandomMax);
	}
	
	// Update is called once per frame
	void Update () 
	{
		switch(state)
		{
		case PistonState.Waiting:
			if(!IsAnyAnimPlaying())
			{
				RunningTime += Time.deltaTime;
				if(RunningTime > NextActivationDuration)
				{
					PistonAnimation.Play("Hit");
					state = PistonState.Extending;
					NextActivationDuration = 0.0f;
					RunningTime = 0.0f;
				}
			}
			break;
			
		case PistonState.Extending:
			if(!IsAnyAnimPlaying())
			{
				RunningTime += Time.deltaTime;
				if(RunningTime > NextActivationDuration)
				{
					state = PistonState.Extended;
					
					NextActivationDuration = ExtendedWaitTime;
					if(RandomMin != RandomMax)
						NextActivationDuration += Random.Range(RandomMin, RandomMax);
					RunningTime = 0.0f;
				}
			}
			break;
			
		case PistonState.Extended:
			if(!IsAnyAnimPlaying())
			{
				RunningTime += Time.deltaTime;
				if(RunningTime > NextActivationDuration)
				{
					state = PistonState.Retracting;
					PistonAnimation.Play ("Retract");
					NextActivationDuration = 0.0f;
					RunningTime = 0.0f;
				}
			}
			break;
			
		case PistonState.Retracting:
			if(!IsAnyAnimPlaying())
			{
				RunningTime += Time.deltaTime;
				if(RunningTime >= NextActivationDuration)
				{
					state = PistonState.Waiting;
					PistonAnimation.Play ("Idle");
					
					NextActivationDuration = IdleWaitTime;
					if(RandomMin != RandomMax)
						NextActivationDuration += Random.Range(RandomMin, RandomMax);
					RunningTime = 0.0f;
				}
			}
			break;
		}
	}
	
	void OnTriggerStay(Collider collider)
	{
		if(collider.tag == "Player" && state != PistonState.Waiting && state != PistonState.Retracting)
		{
			Vector3 force = transform.forward * ImpactForce * Time.deltaTime;
			collider.transform.position += force;
		}	
	}
	
	private bool IsAnyAnimPlaying()
	{        
		/*bool isPlaying = false;
        foreach (AnimationClip clip in AnimationUtility.GetAnimationClips(PistonAnimation))
        {
            AnimationState animState = PistonAnimation[clip.name];

            if (animState.enabled)
            {
                if (animState.normalizedTime < 1)
                {
                    isPlaying = true;
                }
            }
        }
		return isPlaying;*/
		return PistonAnimation.isPlaying;
	}
}
