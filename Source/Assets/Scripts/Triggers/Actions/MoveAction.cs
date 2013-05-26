using UnityEngine;
using System;
using System.Collections.Generic;

namespace TriggerSystem
{
	namespace Actions
	{
		public class MoveAction : BaseAction
		{
			private enum TransitionState
			{
				Waiting,
				InTransitForward,
				InTransitReverse
			}
			
			public GameObject TargetObject;
			public Vector3 TranslationAmount;
			public Vector3 RotationAmount;
			public float TransitionTime = 1.0f;
			
			private Vector3 OriginalLocation;
			private Vector3 ActualEndLocation;
			private Quaternion OriginalOrientation;
			private Quaternion ActualEndRotation;
			
			private float StartTime = 0.0f;
			private float CurrentProgress = 0.0f;
			private TransitionState State = TransitionState.Waiting;
			
			void Start()
			{
				OriginalLocation = TargetObject.transform.position;
				ActualEndLocation = OriginalLocation + TranslationAmount;
				
				OriginalOrientation = TargetObject.transform.rotation;
				ActualEndRotation = OriginalOrientation * Quaternion.Euler(RotationAmount);
			}
			
			void Update()
			{
				if(State == TransitionState.InTransitForward || State == TransitionState.InTransitReverse)
				{
					CurrentProgress = (Time.time - StartTime) / TransitionTime;
					CurrentProgress = Mathf.Clamp(CurrentProgress, 0.0f, 1.0f);
						
					if(State == TransitionState.InTransitForward)
					{
						TargetObject.transform.position = Vector3.Lerp(OriginalLocation, ActualEndLocation, CurrentProgress);
						TargetObject.transform.rotation = Quaternion.Lerp(OriginalOrientation, ActualEndRotation, CurrentProgress);
					}
					else if(State == TransitionState.InTransitReverse)
					{
						TargetObject.transform.position = Vector3.Lerp(ActualEndLocation, OriginalLocation, CurrentProgress);
						TargetObject.transform.rotation = Quaternion.Lerp(ActualEndRotation, OriginalOrientation, CurrentProgress);
					}
					
					if(Time.time - StartTime > TransitionTime)
						State = TransitionState.Waiting;
				}
			}
			
			public override void OnActivate()
			{
				if(State == TransitionState.InTransitForward || State == TransitionState.InTransitReverse)
					StartTime = Time.time - (TransitionTime * CurrentProgress);
				else
					StartTime = Time.time;
				
				State = TransitionState.InTransitForward;
			}
			
			public override void OnDeactivate()
			{
				if(State == TransitionState.InTransitForward || State == TransitionState.InTransitReverse)
					StartTime = Time.time - (TransitionTime * CurrentProgress);
				else
					StartTime = Time.time;
				
				State = TransitionState.InTransitReverse;
			}
		}
	}
}

