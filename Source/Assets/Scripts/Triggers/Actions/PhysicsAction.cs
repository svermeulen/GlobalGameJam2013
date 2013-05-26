using UnityEngine;
using System;
using System.Collections.Generic;

namespace TriggerSystem
{
	namespace Actions
	{
		public class PhysicsAction : BaseAction
		{
			public List<Rigidbody> TargetObjects = new List<Rigidbody>();
		    public RigidbodyConstraints[] activateConstraints;
		    public Vector3 forceVector;

            void Start()
            {
                foreach(Rigidbody body in TargetObjects)
                    body.constraints = RigidbodyConstraints.FreezeAll;
            }

			public override void OnActivate()
			{
				foreach(Rigidbody body in TargetObjects)
				{
				    RigidbodyConstraints constraints = 0;
                    foreach (var constraint in activateConstraints)
                    {
                        constraints = constraints | constraint;
                    }

                    body.constraints = constraints;
					body.WakeUp();
                    body.AddForce(forceVector);
				}
			}
			
			public override void OnDeactivate()
			{
				foreach(Rigidbody body in TargetObjects)
				{
					body.WakeUp();
				}
			}
		}
	}
}

