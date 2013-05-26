using UnityEngine;
using System;
using System.Collections.Generic;

namespace TriggerSystem
{
    namespace Actions
    {
        public class AudioAction : BaseAction
        {
            public AudioClip audio;
            public float volume = 1.0f;

            public int repeats = 0;
            public float repeatTime;

            private int playsLeft = 0;
            private float countdown = 0;

            public override void OnActivate()
            {
                Camera.main.audio.PlayOneShot(audio, volume);
                countdown = audio.length;
                playsLeft = repeats;
            }

            public override void OnDeactivate()
            {
            }

            public void Update()
            {
                if (playsLeft > 0)
                {
                    countdown -= Time.deltaTime;

                    if (countdown < repeatTime)
                    {
                        playsLeft--;
                        Camera.main.audio.PlayOneShot(audio, volume);
                        countdown = audio.length;
                    }
                }
            }
        }
    }
}

