using UnityEngine;
using System.Collections.Generic;

namespace TriggerSystem
{
	namespace Triggers
	{
		public class BaseTrigger : MonoBehaviour 
		{
			public List<ReactionHandler> Reactors = new List<ReactionHandler>();
			public bool ActivatesOnce = false;
			public bool IsEnabled = true;
			
			protected bool IsActivated = false;
		
			// Use this for initialization
			void Start () 
			{
				IsActivated = false;

                foreach (var reactor in Reactors)
                {
                    reactor.AddTrigger(this);
                }
			}
			
			public virtual void Activate()
			{
				if(!IsEnabled)
					return;
				
				if(!IsActivated)
				{
					IsActivated = true;
					foreach(ReactionHandler reactor in Reactors)
						reactor.ReceiveTrigger(this, TriggerState.Activated);
				}
			}
			
			public virtual void Deactivate()
			{
				if(!IsEnabled)
					return;
				
				if((IsActivated && !ActivatesOnce) || !IsActivated)
				{
					IsActivated = false;
					foreach(ReactionHandler reactor in Reactors)
						reactor.ReceiveTrigger(this, TriggerState.Deactivated);
				}
			}
		}
	}// namespace Triggers
}// namespace TriggerSystem
