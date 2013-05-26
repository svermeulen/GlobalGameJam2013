using UnityEngine;
using System;
using XInputDotNetPure;

public class PlayerProximityManager : MonoBehaviour
{
    private static PlayerProximityManager instance;

    public static PlayerProximityManager Instance
    {
        get
        {
            Util.Assert(instance != null);
            return instance;
        }
    }

    public HeartbeatPulser Pulser;
	public float MaxDistance;
    public float SafeDistance;

    public float maxFearSpeed;
    public float minFearSpeed;
	
	public float StrengthLow;
	public float StrengthHigh;
	
	public float FrequencyLow;
	public float FrequencyHigh;

	private Player Player1;
	private Player Player2;

    public float fearAcceleration;
    public float fear;
    public float fearForMaxPulse;

    void Awake()
    {
        Util.Assert(instance == null);
        instance = this;
    }

    void OnDestroy()
    {
        instance = null;
    }

    void UpdateFear()
    {
        var playerDistance = Vector3.Distance(Player1.gameObject.transform.position, Player2.gameObject.transform.position);

        var fearSpeed = 0.0f;

        if (playerDistance > SafeDistance)
        {
            var px = (playerDistance - SafeDistance)/(MaxDistance - SafeDistance);

            fearSpeed = px*maxFearSpeed;
        }
        else
        {
            var px = (SafeDistance - playerDistance)/SafeDistance;

            fearSpeed = px*minFearSpeed;
        }

        fear = fear + fearSpeed * Time.deltaTime;
        fear = Mathf.Clamp(fear, 0, 1);

        Debugging.Instance.ShowText("Dist: " + playerDistance.ToString("F2") + ", Fear: " + fear.ToString("F2") + ", Speed: " + fearSpeed.ToString("F2"));
    }
	
	void Update()
	{
		if(Player1 != null && Player2 != null)
		{
		    UpdateFear();

		    var pulseStrength = Mathf.Min( 1.0f, fear / fearForMaxPulse);

		    Pulser.Fear = fear;
            Pulser.FearStrength = Mathf.Lerp(StrengthLow, StrengthHigh, pulseStrength);
            Pulser.LulFrequency = Mathf.Lerp(FrequencyLow, FrequencyHigh, pulseStrength);
		}
		else
		{
			Player1 = GameConfig.GetPlayer(PlayerIndex.One);
			Player2 = GameConfig.GetPlayer(PlayerIndex.Two);
		}
	}
}

