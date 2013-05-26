using UnityEngine;
using System;
using System.Collections.Generic;

namespace TriggerSystem
{
	namespace Actions
	{
		public class ColliderAction : BaseAction
		{
			public List<Collider> TargetObjects = new List<Collider>();
			public bool InitiallyEnabled = true;
			
			void Start()
			{
				foreach(Collider c in TargetObjects)
					c.enabled = InitiallyEnabled;
			}
			
			public override void OnActivate()
			{
				foreach(Collider c in TargetObjects)
					c.enabled = !InitiallyEnabled;
			}
			
			public override void OnDeactivate()
			{
				foreach(Collider c in TargetObjects)
					c.enabled = InitiallyEnabled;
			}
		}
	}// namespace Actions
}// namespace TriggerSystem

