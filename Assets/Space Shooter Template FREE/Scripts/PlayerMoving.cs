using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script defines the borders of ‘Player’s’ movement. Depending on the chosen handling type, it moves the ‘Player’ together with the pointer.
/// </summary>

[System.Serializable]
public class Borders
{
    [Tooltip("offset from viewport borders for player's movement")]
    public float minXOffset = 1.5f, maxXOffset = 1.5f, minYOffset = 1.5f, maxYOffset = 1.5f;
    [HideInInspector] public float minX, maxX, minY, maxY;
}

public class PlayerMoving : MonoBehaviour {

    [Tooltip("offset from viewport borders for player's movement")]
    public Borders borders;
    Camera mainCamera;
    bool controlIsActive = true; 
    [Header("Desktop Keyboard Control")]
    [Tooltip("WASD/Arrow keys speed (units/sec) when mouse isn't pressed on desktop")]
    public float keyboardSpeed = 12f;
    [Header("Debug")]
    public bool verboseLog = true;
    private float _logTimer;
    private Vector3 _lastPos;

    public static PlayerMoving instance; //unique instance of the script for easy access to the script

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        mainCamera = Camera.main;
        ResizeBorders();                //setting 'Player's' moving borders deending on Viewport's size
        _lastPos = transform.position;
        if (verboseLog)
        {
            Debug.Log($"[PlayerMoving] Start borders x:[{borders.minX:F2},{borders.maxX:F2}] y:[{borders.minY:F2},{borders.maxY:F2}] camOrtho={(mainCamera!=null?mainCamera.orthographicSize:0)}");
        }
    }

    private void Update()
    {
        if (controlIsActive)
        {
#if UNITY_STANDALONE || UNITY_EDITOR    //if the current platform is not mobile, setting mouse handling 

            if (Input.GetMouseButton(0)) //if mouse button was pressed       
            {
                Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition); //calculating mouse position in the worldspace
                mousePosition.z = transform.position.z;
                transform.position = Vector3.MoveTowards(transform.position, mousePosition, 30 * Time.deltaTime);
                if (verboseLog)
                {
                    _logTimer -= Time.deltaTime;
                    if (_logTimer <= 0f)
                    {
                        _logTimer = 0.5f;
                        Debug.Log($"[PlayerMoving] MouseMove to={mousePosition} pos={transform.position}");
                    }
                }
            }
            else
            {
                // Keyboard fallback for desktop (WASD / Arrow keys)
                float ax = Input.GetAxisRaw("Horizontal");
                float ay = Input.GetAxisRaw("Vertical");

                // Legacy GetKey fallback if axes are not wired
                if (Mathf.Abs(ax) <= 0.01f && Mathf.Abs(ay) <= 0.01f)
                {
                    bool left = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
                    bool right = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
                    bool up = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
                    bool down = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
                    ax = (right ? 1f : 0f) - (left ? 1f : 0f);
                    ay = (up ? 1f : 0f) - (down ? 1f : 0f);
                    if (verboseLog)
                    {
                        _logTimer -= Time.deltaTime;
                        if (_logTimer <= 0f)
                        {
                            _logTimer = 0.5f;
                            Debug.Log($"[PlayerMoving] GetKey L={left} R={right} U={up} D={down} -> ax={ax} ay={ay}");
                        }
                    }
                }
                
#if ENABLE_INPUT_SYSTEM
                if (Mathf.Abs(ax) <= 0.01f && Mathf.Abs(ay) <= 0.01f)
                {
                    // Input System fallback when legacy axes are unavailable
                    var k = UnityEngine.InputSystem.Keyboard.current;
                    var g = UnityEngine.InputSystem.Gamepad.current;
                    if (k != null)
                    {
                        ax = (k.dKey.isPressed || k.rightArrowKey.isPressed ? 1f : 0f) - (k.aKey.isPressed || k.leftArrowKey.isPressed ? 1f : 0f);
                        ay = (k.wKey.isPressed || k.upArrowKey.isPressed ? 1f : 0f) - (k.sKey.isPressed || k.downArrowKey.isPressed ? 1f : 0f);
                    }
                    if (g != null)
                    {
                        var ls = g.leftStick.ReadValue();
                        ax += ls.x; ay += ls.y;
                    }
                }
#endif
                if (Mathf.Abs(ax) > 0.01f || Mathf.Abs(ay) > 0.01f)
                {
                    Vector3 delta = new Vector3(ax, ay, 0f).normalized * keyboardSpeed * Time.deltaTime;
                    transform.position += delta;
                    if (verboseLog)
                    {
                        _logTimer -= Time.deltaTime;
                        if (_logTimer <= 0f)
                        {
                            _logTimer = 0.5f;
                            Debug.Log($"[PlayerMoving] Keyboard ax={ax:F2} ay={ay:F2} pos={transform.position}");
                        }
                    }
                }
                else if (verboseLog)
                {
                    _logTimer -= Time.deltaTime;
                    if (_logTimer <= 0f)
                    {
                        _logTimer = 0.5f;
                        Debug.Log($"[PlayerMoving] NoInput ax=0 ay=0 pos={transform.position}");
                    }
                }
            }
#endif

#if UNITY_IOS || UNITY_ANDROID //if current platform is mobile, 

            if (Input.touchCount == 1) // if there is a touch
            {
                Touch touch = Input.touches[0];
                Vector3 touchPosition = mainCamera.ScreenToWorldPoint(touch.position);  //calculating touch position in the world space
                touchPosition.z = transform.position.z;
                transform.position = Vector3.MoveTowards(transform.position, touchPosition, 30 * Time.deltaTime);
            }
#endif
            transform.position = new Vector3    //if 'Player' crossed the movement borders, returning him back 
                (
                Mathf.Clamp(transform.position.x, borders.minX, borders.maxX),
                Mathf.Clamp(transform.position.y, borders.minY, borders.maxY),
                0
                );

            if (verboseLog)
            {
                if ((transform.position - _lastPos).sqrMagnitude > 0.0001f)
                {
                    Debug.Log($"[PlayerMoving] Clamped pos={transform.position}");
                    _lastPos = transform.position;
                }
            }
        }
    }

    //setting 'Player's' movement borders according to Viewport size and defined offset
    void ResizeBorders() 
    {
        borders.minX = mainCamera.ViewportToWorldPoint(Vector2.zero).x + borders.minXOffset;
        borders.minY = mainCamera.ViewportToWorldPoint(Vector2.zero).y + borders.minYOffset;
        borders.maxX = mainCamera.ViewportToWorldPoint(Vector2.right).x - borders.maxXOffset;
        borders.maxY = mainCamera.ViewportToWorldPoint(Vector2.up).y - borders.maxYOffset;
    }
}
