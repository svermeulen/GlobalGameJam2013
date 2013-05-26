using UnityEngine;
using System.Collections.Generic;
using XInputDotNetPure;

public class Climbable : MonoBehaviour 
{
	public GameObject ClimableObject;
	public float RelativeTranslation;
	public float BoxHeight;
	public float ClimbDuration;
	
	private float _climbStarted = 0.0f;
	private bool _isClimbing = false;
	
	private List<Player> _touchingPlayers = new List<Player>();
	private Player _climbingPlayer;
	
	void Update()
	{
		if(!_isClimbing)
		{
//			foreach(Player player in _touchingPlayers)
//			{
//				GamePadState state = GamePad.GetState(player.playerIndex);
//				if(state.Buttons.A == ButtonState.Pressed)
//				{
//					_climbStarted = Time.time;
//					_isClimbing = true;	
//					_touchingPlayers.Remove(player);
//					_climbingPlayer = player;
//				}
//			}
		}
		else if(Time.time - _climbStarted > ClimbDuration)
		{
			Vector3 playerPos = _climbingPlayer.gameObject.transform.position;
			Vector3 boxPos = ClimableObject.transform.position;
			boxPos.y += BoxHeight;
			Vector3 toBox = playerPos - boxPos;
			toBox.y = 0.0f;
			toBox.Normalize();
			_climbingPlayer.gameObject.transform.position = boxPos + RelativeTranslation * toBox;
			
			_climbingPlayer = null;
			_isClimbing = false;
		}
	}
	
	void OnTriggerEnter(Collider collider)
	{
		if(collider.gameObject.tag != "Player")
			return;
		
		Player player = collider.gameObject.GetComponent<Player>();
		if(player != null)
			_touchingPlayers.Add(player);
	}
	
	void OnTriggerExit(Collider collider)
	{
		if(collider.gameObject.tag != "Player")
			return;
		
		Player player = collider.gameObject.GetComponent<Player>();
		if(player != null)
			_touchingPlayers.Remove(player);
	}
}
