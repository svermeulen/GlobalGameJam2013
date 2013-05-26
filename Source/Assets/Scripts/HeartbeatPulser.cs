using UnityEngine;
using System.Collections.Generic;
using XInputDotNetPure;

public class HeartbeatPulser : MonoBehaviour
{
    public bool enableVibrate = false;

    public AudioClip heartBeatClip;

    [System.Serializable]
    public class Visual
    {
        public Texture2D healthBarBackground;
        public Vector2 pos;
        public float minScale;
        public float maxScaleNormalRate;
        public float maxScaleMaxRate;
        public float blendSpeed;
    };

    public Visual visual;

	[System.Serializable]
	public class BeatPattern
	{
		public float Duration;
		public float Strength;
	};
	
	public List<PlayerIndex> ActiveControllers;
    public float Fear = 1.0f;
	public float FearStrength = 1.0f;
	public float BeatFrequency = 1.0f;
	public float LulFrequency = 1.0f;
	public List<BeatPattern> Durations = new List<BeatPattern>();
	
	private bool IsActive = true;
	private float LastBeat = 0.0f;
	private int CurrentBeat = 0;
    private float _strength = 0;
    private float _visualHeartScale = 1;
	
	// Use this for initialization
	void Start () 
	{
		CurrentBeat = 0;
		LastBeat = Time.time;
	}
	
	void OnDestroy()
	{
		foreach(PlayerIndex index in ActiveControllers)
			GamePad.SetVibration(index, 0.0f, 0.0f);
	}

    private void OnGUI()
    {
        var maxScale = Mathf.Lerp(visual.maxScaleNormalRate, visual.maxScaleMaxRate, Fear);
        var desiredScale = Mathf.Lerp(visual.minScale, maxScale, _strength);

        _visualHeartScale = Mathf.Lerp(_visualHeartScale, desiredScale, Mathf.Min(1.0f, visual.blendSpeed*Time.deltaTime));

        var width = _visualHeartScale * visual.healthBarBackground.width;
        var height = _visualHeartScale * visual.healthBarBackground.height;

        var center = new Vector2(Screen.width - visual.pos.x, Screen.height - visual.pos.y);

        // Draw background of health bar
        GUI.DrawTexture(new Rect(center.x - width / 2.0f, center.y - height / 2.0f, width, height), visual.healthBarBackground);
    }

	// Update is called once per frame
	void Update () 
	{
		if(IsActive)
		{
			float actualFrequency = (Durations[CurrentBeat].Strength > 0.001f ? BeatFrequency : LulFrequency);
			if(Time.time - LastBeat > Durations[CurrentBeat].Duration * actualFrequency)
			{
				++CurrentBeat;

				if(CurrentBeat >= Durations.Count)
					CurrentBeat = 0;

                if (Durations[CurrentBeat].Strength <= 0)
                {
                    Camera.main.audio.PlayOneShot(heartBeatClip, 1);
                }

				LastBeat = Time.time;
				
				_strength = Durations[CurrentBeat].Strength * FearStrength;

//			    Debugging.Instance.ShowText("Strength: " + _strength);

                if (enableVibrate)
                {
				    foreach(PlayerIndex index in ActiveControllers)
                        GamePad.SetVibration(index, _strength, _strength);
                }
			}
		}
	}
}
