using TriggerSystem.Triggers;
using UnityEngine;
using System.Collections.Generic;

namespace TriggerSystem
{
	public class ReactionHandler : MonoBehaviour
    {
        public enum RequirementsType
        {
            AnyTrigger,
            AllTriggers,
        }

        public RequirementsType requirementsType = RequirementsType.AllTriggers;
		public List<BaseAction> Actions = new List<BaseAction>();
		public bool ActivatesOnce = false;
        protected bool IsActivated = false;

        private List<BaseTrigger> RequiredTriggers = new List<BaseTrigger>();
        private List<BaseTrigger> ActivatedTriggers = new List<BaseTrigger>();
		
		// Use this for initialization
		void Start () {
			IsActivated = false;
		}

        public void ReceiveTrigger(BaseTrigger trigger, TriggerState state)
		{
			if(ActivatesOnce && IsActivated)
				return;
			
			switch(state)
			{
			case TriggerState.Activated:
                if (!ActivatedTriggers.Contains(trigger))
                    ActivatedTriggers.Add(trigger);
				break;
			case TriggerState.Deactivated:
                if (ActivatedTriggers.Contains(trigger))
                    ActivatedTriggers.Remove(trigger);
				break;
			default:
				return;
			}
			
			if(CheckRequirements())
			{
				IsActivated = true;
				DispatchResult(true);
			}
			else if(!ActivatesOnce && IsActivated)
			{
				IsActivated = false;
				DispatchResult(false);
			}
		}
		
		private bool CheckRequirements()
		{
			if(RequiredTriggers.Count == 0)
				return false;

            if (requirementsType == RequirementsType.AllTriggers)
            {
			    foreach(var trigger in RequiredTriggers)
			    {
				    if(!ActivatedTriggers.Contains(trigger))	
					    return false;
			    }

                return true;
            }

            Util.Assert(requirementsType == RequirementsType.AnyTrigger);

		    foreach (var trigger in RequiredTriggers)
		    {
		        if (ActivatedTriggers.Contains(trigger))
		            return true;
		    }

		    return false;
		}
		
		private void DispatchResult(bool activate)
		{
			foreach(BaseAction action in Actions)
			{
				if(activate)
					action.OnActivate();
				else
					action.OnDeactivate();
			}
		}

        public void AddTrigger(BaseTrigger trigger)
	    {
            RequiredTriggers.Add(trigger);
	    }
	}
}// namespace TriggerSystem