using UnityEngine;
using System;

namespace TriggerSystem
{
	public abstract class BaseAction : MonoBehaviour
	{
		public abstract void OnActivate();
		public abstract void OnDeactivate();
	}
}

