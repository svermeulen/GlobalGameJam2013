using UnityEngine;
using System;
using System.Collections.Generic;

namespace TriggerSystem
{
	namespace Actions
	{
		public class ParticleStartAction : BaseAction
		{
			public List<ParticleSystem> TargetObjects = new List<ParticleSystem>();
			public bool StopOnDeactivate = false;
			
			public override void OnActivate()
			{
				foreach(ParticleSystem ps in TargetObjects)
					ps.Play();
			}
			
			public override void OnDeactivate()
			{
				if(StopOnDeactivate)
				{
					foreach(ParticleSystem ps in TargetObjects)
					{
						if(ps.isPlaying)
							ps.Stop();
					}
				}
			}
		}
	}
}

