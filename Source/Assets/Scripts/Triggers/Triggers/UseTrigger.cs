using UnityEngine;
using System.Collections.Generic;
using XInputDotNetPure;

namespace TriggerSystem
{
	namespace Triggers
	{
		public class UseTrigger : BaseTrigger
		{
		    public Vector2 popupPos;
            public Texture2D popupTextureKeyboard;
            public Texture2D popupTextureController;
            public List<string> WatchListTags = new List<string>();
			public float Radius = 1.0f;
			
			private List<Player> TriggeredPlayers = new List<Player>();
            
            private void OnGUI()
            {
                if (TriggeredPlayers.Count > 0)
                {
                    var controlType = TriggeredPlayers[0].GetControlType();

                    Texture2D texture = null;
                    if (controlType == ControlType.KeyboardArrows || controlType == ControlType.KeyboardWasd)
                    {
                        texture = popupTextureKeyboard;   
                    }
                    else
                    {
                        texture = popupTextureController;
                    }
                    
                    if (texture != null)
                    {
                        var width = texture.width;
                        var height = texture.height;

                        var center = new Vector2(popupPos.x, Screen.height - popupPos.y);

                        GUI.DrawTexture(new Rect(center.x - width / 2.0f, center.y - height / 2.0f, width, height), texture);
                    }
                }
            }
			
			void OnTriggerEnter(Collider other)
			{
                if (WatchListTags.Contains(other.tag))
				{
					Player player = other.gameObject.GetComponent<Player>();
					if(player != null)
						TriggeredPlayers.Add(player);
				}
			}
			
			void OnTriggerExit(Collider other)
            {
                if (WatchListTags.Contains(other.tag))	
				{
					Player player = other.gameObject.GetComponent<Player>();
					if(player != null)
						TriggeredPlayers.Remove(player);
				}
			}
			
			void Update()
			{
				foreach(Player player in TriggeredPlayers)
				{
                    if (player.CheckButtonPress())
					{
					    player.OnPressedButton();
						Activate();
						return;
					}
				}
				Deactivate();
			}
		}
	}// namespace Triggers
}// namespace TriggerSystem
	

