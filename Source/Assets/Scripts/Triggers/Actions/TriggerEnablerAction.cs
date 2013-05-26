using System;
using System.Collections.Generic;

namespace TriggerSystem
{
	namespace Actions
	{
		public class TriggerEnablerAction : BaseAction
		{
			public List<Triggers.BaseTrigger> TargetObjects = new List<Triggers.BaseTrigger>();
			
			public TriggerEnablerAction ()
			{
			}
			
			public override void OnActivate()
			{
				foreach(Triggers.BaseTrigger trigger in TargetObjects)
					trigger.IsEnabled = true;
			}
			
			public override void OnDeactivate()
			{
				foreach(Triggers.BaseTrigger trigger in TargetObjects)
					trigger.IsEnabled = false;
			}
		}
	}// namespace Actions
}// namespace TriggerSystem

