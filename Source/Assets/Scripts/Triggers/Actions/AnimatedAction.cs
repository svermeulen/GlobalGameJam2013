using UnityEngine;
using System;
using System.Collections.Generic;

namespace TriggerSystem
{
	namespace Actions
	{
		public class AnimatedAction : BaseAction
		{
			public List<Animation> TargetObjects = new List<Animation>();

            public AudioClip activateAudio;
		    public float audioVolume = 1;

            public float activateAnimSpeed = 1;
            public float deactivateAnimSpeed = 1;

		    public string ActivationAnimation;
			public string DeactivationAnimation;

		    private float countDown = 0;
		    private bool currentState = false;
            private bool desiredState = false;

			public override void OnActivate()
			{
                if (currentState)
                    return;

				foreach(Animation anim in TargetObjects)
                {
                    desiredState = true;

                    if (countDown <= 0)
                    {
                        currentState = true;
                        countDown = anim[ActivationAnimation].length / activateAnimSpeed;
                        anim[ActivationAnimation].speed = activateAnimSpeed;
                        anim.Play(ActivationAnimation);

                        if (activateAudio != null)
                        {
                            Camera.main.audio.PlayOneShot(activateAudio, audioVolume);   
                        }
                    }
                }
			}

            public override void OnDeactivate()
            {
                if (!currentState)
                    return;

                foreach (Animation anim in TargetObjects)
                {
                    desiredState = false;

                    if (countDown <= 0)
                    {
                        currentState = false;
                        countDown = anim[DeactivationAnimation].length / deactivateAnimSpeed;
                        anim[ActivationAnimation].speed = deactivateAnimSpeed;
                        anim.Play(DeactivationAnimation);
                    }
                }
            }

            public void Update()
            {
                if (countDown > 0)
                {
                    countDown -= Time.deltaTime;

                    if (countDown <= 0)
                    {
                        if (currentState != desiredState)
                        {
                            if (desiredState)
                            {
                                OnActivate();
                            }
                            else
                            {
                                OnDeactivate();
                            }
                        }
                    }
                }
            }
		}
	}
}

