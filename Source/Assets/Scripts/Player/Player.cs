using System;
using UnityEngine;
using System.Collections;
using XInputDotNetPure;
using Object = UnityEngine.Object;

public enum PlayerAnim
{
    Idle,
    Run,
	Push,
    PanicDeath,
    PullLever,
    DeathIdle,
    Count,
    None
}

public class Player : MonoBehaviour
{
    private static Player _gredel;
    private static Player _hansel;

    public AudioClip waterDeathSound;
    public float waterDeathSoundVolume;

    public AudioClip cratePushSound;
    public float cratePushVolume;
    private float cratePushCountdown;

    public GameObject lavaParticleEffect;
    public GameObject acidParticleEffect;
	public GameObject dustParticleEffect;
	public GameObject dustPushParticleEffect;
    public float bridgeRejectForce;
    public PlayerType playerType;
    public bool useKeyboard;
    public float blendSpeed;
    public float rotateSpeed;
    public float moveSpeed;
    public float moveDampening;
    public float gravity;
    public float maxSpeedForRunPx;
	public float pushPower = 1.0f;
    public float pushBlendOutSpeed = 1;
    public float _deadIdleBlendInTime;
	
    private float _runPx = 0;
    private Quaternion _desiredRotation = Quaternion.identity;
    private Vector2 _lateralVelocity;
    private CharacterController _charController;
    private float _verticalVelocity;
    private bool _isAlive = true;
	private bool _isPushing = false;
	private float _pushStarted = 0.0f;
	private float _lastPush = 0.0f;
	private float _lastDustPuff = 0.0f;
	private Object _dustPuffObj;

    private PlayerAnim _currentOnShotAnim = PlayerAnim.None;
    private float _oneShotElapsedTime = 0;
    private float[] _recordedBlendWeights = new float[(int)PlayerAnim.Count];

    private float _push = 0;

    // Constants
    static string[] PlayerAnimName = new string[(int)PlayerAnim.Count] 
    { 
        "Idle",
        "Run",
		"Push",
        "PanicDeath",
        "PullLever",
        "PanicDeathIdle",
    };

    public float _oneShotBlendInTime;
    public float _oneShotBlendOutTime;
    private float _oneShotTime;

    enum OneShotState
    {
        None,
        BlendIn,
        Playing,
        BlendOut
    }

    private OneShotState _oneShotState;
    private float _deadElapsedTime;
    private bool _deathAnimStarted = false;

    public static Player Gretel
    {
        get
        {
            Util.Assert(_gredel != null);
            return _gredel;
        }
    }

    public static Player Hansel
    {
        get
        {
            Util.Assert(_hansel != null);
            return _hansel;
        }
    }

    void Awake()
    {
        if (playerType == PlayerType.Gretel)
        {
            GameConfig.SetPlayer(PlayerIndex.One, this);

            Util.Assert(_gredel == null);
            _gredel = this;
        }
        else
        {
            GameConfig.SetPlayer(PlayerIndex.Two, this);

            Util.Assert(playerType == PlayerType.Hansel);
            Util.Assert(_hansel == null);
            _hansel = this;
        }

        _charController = GetComponent<CharacterController>();
    }

    void OnDestroy()
    {
        if (playerType == PlayerType.Gretel)
        {
            _gredel = null;
        }
        else
        {
            Util.Assert(playerType == PlayerType.Hansel);
            _hansel = this;
        }
    }

    void Start()
    {
        // All animations are 'on', the blend weights just vary
        for (var i = 0; i < (int)PlayerAnim.Count; i++)
        {
            var anim = animation[PlayerAnimName[i]];

            anim.blendMode = AnimationBlendMode.Blend;
            anim.layer = 1;
            anim.enabled = true;
            anim.weight = 0;
        }
    }

    void UpdateRotation(Vector3 moveDir, float moveAmount)
    {
        if (moveAmount > 0)
        {
            var angle = Mathf.Atan2(-moveDir.y, moveDir.x);
            _desiredRotation = Quaternion.AngleAxis(Mathf.Rad2Deg * angle, Vector3.up);
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.AngleAxis(90, Vector3.up) * _desiredRotation, Mathf.Min(1.0f, rotateSpeed * Time.deltaTime));
    }

    int GetPlayerIndex()
    {
        return playerType == PlayerType.Hansel ? 0 : 1;
    }

    Vector2 GetMoveDelta()
    {
	    GamePadState state;

        switch (GameConfig.GetControlType(GetPlayerIndex()))
        {
            case ControlType.Controller1LeftJoystick:

                state = GamePad.GetState(PlayerIndex.One);
                return new Vector2(-state.ThumbSticks.Left.X, -state.ThumbSticks.Left.Y);

            case ControlType.Controller2LeftJoystick:

                state = GamePad.GetState(PlayerIndex.Two);
                return new Vector2(-state.ThumbSticks.Left.X, -state.ThumbSticks.Left.Y);

            case ControlType.Controller1RightJoystick:

                state = GamePad.GetState(PlayerIndex.One);
                return new Vector2(-state.ThumbSticks.Right.X, -state.ThumbSticks.Right.Y);

            case ControlType.Controller2RightJoystick:

                state = GamePad.GetState(PlayerIndex.Two);
                return new Vector2(-state.ThumbSticks.Right.X, -state.ThumbSticks.Right.Y);

            case ControlType.KeyboardArrows:
                return new Vector2(-Input.GetAxis("KeyboardHorizontalArrow"), -Input.GetAxis("KeyboardVerticalArrow"));

            case ControlType.KeyboardWasd:
                return new Vector2(-Input.GetAxis("KeyboardHorizontal"), -Input.GetAxis("KeyboardVertical"));

            default:
                Util.Assert(false);
                break;
        }

        return Vector2.zero;
    }

    public void Update()
    {
        if (_oneShotState != OneShotState.None)
        {
            UpdateOneShotAnim();
        }
        else
        {
            if (_isAlive)
            {
                var desiredMoveDelta = GetMoveDelta();

                var moveAmount = desiredMoveDelta.magnitude;
                var moveDir = desiredMoveDelta / moveAmount;

                UpdateRotation(moveDir, moveAmount);
                UpdatePosition(moveDir, moveAmount);

                if (!_isPushing)
                {
                    var nonPush = 1.0f - _push;
                    SetBlendWeight(PlayerAnim.Idle, nonPush * (1.0f - _runPx));
                    SetBlendWeight(PlayerAnim.Run, nonPush * _runPx);
                    SetBlendWeight(PlayerAnim.Push, _push);

                    _push -= _push * Mathf.Min(1.0f, pushBlendOutSpeed * Time.deltaTime);
                }
                else
                {
                    float _deltaTime = (Time.time - _pushStarted);
                    _push = Mathf.Clamp(_deltaTime / 0.6f, 0.0f, 1.0f);
                    SetBlendWeight(PlayerAnim.Idle, 0.0f);
                    SetBlendWeight(PlayerAnim.Run, 1.0f - _push);
                    SetBlendWeight(PlayerAnim.Push, _push);

                    var stick = -1 * GetMoveDelta();
//                    GamePadState state = GamePad.GetState(playerIndex);
//                    Vector2 stick = new Vector2(state.ThumbSticks.Left.X, state.ThumbSticks.Left.Y);

                    if (Time.time - _lastPush > 0.1f || stick.sqrMagnitude < 0.01f)
					{
						if(_dustPuffObj != null)
							Destroy(_dustPuffObj);
                        _isPushing = false;
					}
                }

                CheckFear();
            }
            else
            {
                if (!_deathAnimStarted)
                {
                    _deathAnimStarted = true;
                    RecordBlendWeights();
                }

                _deadElapsedTime += Time.deltaTime;

                var px = _deadElapsedTime/_deadIdleBlendInTime;

                float sum = 0;
                // Fade out all other animations
                for (var i = 0; i < (int)PlayerAnim.Count; i++)
                {
                    if (i == (int)PlayerAnim.DeathIdle)
                        continue;

                    var anim = animation[PlayerAnimName[i]];

                    anim.weight = Mathf.Lerp(_recordedBlendWeights[i], 0, px);

                    sum += anim.weight;
                }

                animation[PlayerAnimName[(int) PlayerAnim.DeathIdle]].weight = 1.0f - sum;

                // fall down by gravity
                UpdatePosition(Vector3.zero, 0);
            }
        }

        if (_isPushing)
        {
            if (cratePushCountdown < 0.2)
            {
                cratePushCountdown = cratePushSound.length;
                Camera.main.audio.PlayOneShot(cratePushSound, cratePushVolume);      
            }
            cratePushCountdown -= Time.deltaTime;
        }
    }

    private void UpdateOneShotAnim()
    {
        var animClip = animation[PlayerAnimName[(int) _currentOnShotAnim]];

        switch (_oneShotState)
        {
            case OneShotState.BlendIn:
            {
                var sum = 0.0f;
                _oneShotElapsedTime += Time.deltaTime;

                var px = Mathf.Clamp(_oneShotElapsedTime / _oneShotBlendInTime, 0, 1);

                // Fade out all other animations
                for (var i = 0; i < (int)PlayerAnim.Count; i++)
                {
                    if ((int)_currentOnShotAnim == i)
                        continue;

                    var anim = animation[PlayerAnimName[i]];

                    anim.weight = (1.0f - px) * _recordedBlendWeights[i];

                    sum += anim.weight;
                }

                Util.Assert(sum >= 0 && sum <= 1.0f);

                animClip.weight = 1.0f - sum;

                if (px >= 1)
                {
                    animClip.speed = 1.0f;
                    _oneShotState = OneShotState.Playing;
                    _oneShotElapsedTime = 0;
                }
                break;
            }
            case OneShotState.Playing:
            {
                _oneShotElapsedTime += Time.deltaTime;

                if (_oneShotElapsedTime > animClip.length)
                {
                    _oneShotState = OneShotState.None;
                }

                break;
            }
            default:
            {
                Util.Assert(false);
                break;
            }
        }
    }

    private void PlayOneShot(PlayerAnim oneShotAnim)
    {
        if (_oneShotState != OneShotState.None)
        {
            return;
        }

        _oneShotState = OneShotState.BlendIn;
        _currentOnShotAnim = oneShotAnim;

        var animClip = animation[PlayerAnimName[(int)oneShotAnim]];

        animClip.normalizedTime = 0;
        animClip.speed = 0;

        RecordBlendWeights();
    }

    private void RecordBlendWeights()
    {
        for (var i = 0; i < (int)PlayerAnim.Count; i++)
        {
            var anim = animation[PlayerAnimName[i]];
            _recordedBlendWeights[i] = anim.weight;
        }
    }

    private void CheckFear()
    {
        var fear = PlayerProximityManager.Instance.fear;

        if (fear >= 1)
        {
            _isAlive = false;
            PlayOneShot(PlayerAnim.PanicDeath);
            GameEventMgr.Instance.Trigger(GameEvents.PlayerHitMaxFear);
        }
    }

    private void UpdatePosition(Vector2 moveDir, float moveAmount)
    {
        if (moveAmount > 0)
        {
            _lateralVelocity += moveDir * moveSpeed * moveAmount * Time.deltaTime;
        }

        if (_charController.isGrounded)
        {
            _verticalVelocity = 0;
        }
        else
        {
            _verticalVelocity += Physics.gravity.y*Time.deltaTime;
        }

        _lateralVelocity += -_lateralVelocity * Mathf.Min(1.0f, moveDampening * Time.deltaTime);

        var desiredRunPx = Mathf.Clamp(_lateralVelocity.magnitude / maxSpeedForRunPx, 0, 1);
        _runPx = Mathf.Lerp(_runPx, desiredRunPx, Mathf.Min(1.0f, blendSpeed * Time.deltaTime));

//        Debugging.Instance.ShowText("Speed: " + _lateralVelocity.magnitude + ", " + desiredRunPx + ", " + _runPx);   

        var moveDelta = new Vector3(_lateralVelocity.x, _verticalVelocity, _lateralVelocity.y) * Time.deltaTime;

        _charController.Move(moveDelta);
    }

    private void SetBlendWeight(PlayerAnim anim, float px)
    {
        animation[PlayerAnimName[(int)anim]].weight = px;
    }
 
	void OnControllerColliderHit (ControllerColliderHit hit)
    {
        if (hit.collider.tag == "DrawBridge")
        {
            var anim = hit.collider.gameObject.animation;

            bool isPlaying = false;
            foreach (AnimationState animState in anim)
            {
                if (animState.enabled)
                {
                    if (animState.normalizedTime < 1)
                    {
                        isPlaying = true;
                    }
                }
            }

            if (isPlaying)
            {
                _lateralVelocity += new Vector2(hit.normal.x, hit.normal.z) * bridgeRejectForce * Time.deltaTime;
//                _verticalVelocity += hit.normal.y * bridgeRejectForce * Time.deltaTime;   
            }
            return;
        }

        if (hit.collider.tag == "Crate")
		{
         	Rigidbody body = hit.collider.attachedRigidbody;

            var stick = -1 * GetMoveDelta();		
//			GamePadState state = GamePad.GetState(playerIndex);
//		    Vector2 stick = new Vector2(state.ThumbSticks.Left.X, state.ThumbSticks.Left.Y);

			if(stick.sqrMagnitude < 0.01f)
				return;
			
		    // no rigidbody
		    if (body == null || body.isKinematic) { return; }
		 
		    // We dont want to push objects below us
		    if (hit.moveDirection.y < -0.3) { return; }
		 
		    // Calculate push direction from move direction,
		    // we only push objects to the sides never up and down
			Vector3 pushDir;
			if(Mathf.Abs(hit.moveDirection.x) > Mathf.Abs(hit.moveDirection.z))
		    	pushDir = new Vector3(hit.moveDirection.x, 0, 0);
			else
				pushDir = new Vector3(0, 0, hit.moveDirection.z);
		 
		    // If you know how fast your character is trying to move,
		    // then you can also multiply the push velocity by that.
		 
		    // Apply the push
		    body.velocity = pushDir * pushPower;
			
			
			if(!_isPushing)
			{
				if(dustPushParticleEffect != null)
					_dustPuffObj = Instantiate(dustPushParticleEffect, transform.position, transform.rotation);
				_pushStarted = Time.time;
				_isPushing = true;
			}
			_lastPush = Time.time;
			
			return;
		}
	}
	
    void OnTriggerEnter(Collider other)
    {
        if (_isAlive)
        {
            if (other.tag == "LevelEnd")
            {
                _isAlive = false;

                Debug.Log("Player " + playerType + " reached end of level");
                animation.Stop();
                GameEventMgr.Instance.Trigger(GameEvents.PlayerFinishedLevel, playerType);
            }
            else if (other.tag == "KillZone" || other.tag == "KillZoneLava" || other.tag == "KillZoneAcid" || other.tag == "KillZoneImpact")
            {
                _isAlive = false;

                if (other.tag == "KillZoneLava" && lavaParticleEffect != null)
                {
                    Camera.main.audio.PlayOneShot(waterDeathSound, waterDeathSoundVolume);   
                    Instantiate(lavaParticleEffect, transform.position, Quaternion.identity);
                }

                if (other.tag == "KillZoneAcid" && acidParticleEffect != null)
                {
                    Camera.main.audio.PlayOneShot(waterDeathSound, waterDeathSoundVolume);   
                    Instantiate(acidParticleEffect, transform.position, Quaternion.identity);
                }
				
				if (other.tag == "KillZoneImpact" && dustParticleEffect != null)
                {
                    Instantiate(dustParticleEffect, transform.position, Quaternion.identity);
			    	PlayOneShot(PlayerAnim.PanicDeath);
                }

                GameEventMgr.Instance.Trigger(GameEvents.PlayerDied, playerType);
            }
        }
    }

    public void OnPressedButton()
    {
        PlayOneShot(PlayerAnim.PullLever);
    }

    public bool CheckButtonPress()
    {
        GamePadState state;

        switch (GameConfig.GetControlType(GetPlayerIndex()))
        {
            case ControlType.Controller1LeftJoystick:
            case ControlType.Controller1RightJoystick:
                state = GamePad.GetState(PlayerIndex.One);
                return state.Buttons.A == ButtonState.Pressed;

            case ControlType.Controller2LeftJoystick:
            case ControlType.Controller2RightJoystick:
                state = GamePad.GetState(PlayerIndex.Two);
                return state.Buttons.A == ButtonState.Pressed;

            case ControlType.KeyboardArrows:
            case ControlType.KeyboardWasd:
                return Input.GetButton("LeverPress");

            default:
                Util.Assert(false);
                break;
        }

        return false;
    }

    public ControlType GetControlType()
    {
        return GameConfig.GetControlType(GetPlayerIndex());
    }
}
