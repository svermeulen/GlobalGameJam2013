using UnityEngine;
using XInputDotNetPure;

public class PlayerKillVolume : MonoBehaviour 
{
	public string WatchTag = "Player";
	
	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == WatchTag)
		{
			//Player p = other.gameObject.GetComponent<Player>();
			//p.Kill();
		}
	}
}
