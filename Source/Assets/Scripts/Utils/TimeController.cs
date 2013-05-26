using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TimeController : MonoBehaviour
{
    public float SlomoTimeScale = 0.8f;
    private static TimeController _instance;

    private bool _isSlomo;

    private float _startFixedTime;

    public static TimeController Instance
    {
        get
        {
            Util.Assert(_instance != null);
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        _startFixedTime = Time.fixedDeltaTime;
    }

    private void ChangeTimeScale()
    {
        var triggerSpeed = 1.0f - Input.GetAxis("Slomo");

        Time.timeScale = triggerSpeed;
        Time.fixedDeltaTime = triggerSpeed*_startFixedTime;
    }

    private void Update()
    {
        ChangeTimeScale();
            
#if UNITY_EDITOR
        if (Input.GetButtonDown("Pause"))
        {
            EditorApplication.isPaused = !EditorApplication.isPaused;
        }
#endif
    }
}