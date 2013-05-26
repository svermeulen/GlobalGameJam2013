using UnityEngine;
using System.Collections.Generic;

namespace TriggerSystem
{
	namespace Triggers
	{
		public class PhysicsTrigger : BaseTrigger
		{
            public List<string> WatchListTags = new List<string>();
			
			private uint TriggerCount = 0;
		
			void OnTriggerEnter(Collider other)
            {
//                Debug.Log("Trigger enter");
                if (WatchListTags.Contains(other.tag))
					++TriggerCount;
			}
			
			void OnTriggerExit(Collider other)
            {
//                Debug.Log("Trigger exit");
                if (WatchListTags.Contains(other.tag))
					--TriggerCount;
			}
				
			void Update()
			{
				if(TriggerCount > 0)
					Activate();
				else
					Deactivate();
			}
		}
	}// namespace Triggers
}// namespace TriggerSystem
