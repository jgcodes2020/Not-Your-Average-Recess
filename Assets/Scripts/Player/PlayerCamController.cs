using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamController : MonoBehaviour, IPauseable
{
    // parameters
    public Vector2 sensitivity;

    [Description("Minimum Camera Distance")]
    public float minCameraDist;

    [DefaultValue(50)] [Range(0, 90)] public float maxAngleBelow;
    [DefaultValue(50)] [Range(0, 90)] public float maxAngleAbove;

    [DefaultValue(10)] public float resetAngle;

    // these are technically parameters, but they're determined from the initial position of the camera
    private Vector3 _raycastDir;
    private float _raycastDist;

    public Transform facing;
    public Transform camFocus;

    public PlayerInput playerInput;
    private InputAction _actLookAround;
    private InputAction _actResetCam;

    private Vector2 _rotation;
    private bool _paused;
    private CursorLockMode? _prevLockMode = null;

    public float ForwardAngle
    {
        get => _rotation.x;
        set => _rotation.x = value;
    }

    public bool Paused
    {
        get => _paused;
        set => _paused = value;
    }

    private void Start()
    {
        _actLookAround = playerInput.currentActionMap["Look Around"];
        _actResetCam = playerInput.currentActionMap["Reset Camera"];
        if (playerInput.currentControlScheme == "KeyboardMouse")
        {
            _prevLockMode = Cursor.lockState;
            Cursor.lockState = CursorLockMode.Locked;
        }

        // setup
        var localPos = transform.localPosition;
        _raycastDist = localPos.magnitude;
        _raycastDir = localPos / _raycastDist;

        _actResetCam.performed += OnResetCam;
    }

    private void OnDestroy()
    {
        // if previous lock mode was set
        if (_prevLockMode is { } prevLockMode) 
            Cursor.lockState = prevLockMode;
    }

    private void OnResetCam(InputAction.CallbackContext context)
    {
        Debug.Log("reset");
        _rotation.y = -resetAngle;
    }

    private void Update()
    {
        // disable updates when paused
        if (Paused)
            return;

        var rawLookDelta = _actLookAround.ReadValue<Vector2>();
        var dTheta = rawLookDelta * sensitivity * Time.deltaTime;

        _rotation += dTheta;
        // since we invert the y direction later, this actually caps to 50 below and 70 up
        _rotation.y = Mathf.Clamp(_rotation.y, -maxAngleAbove, maxAngleBelow);
        _rotation.x = (_rotation.x + 180) % 360 - 180;

        // update parent rotation and model direction to reflect camera position
        camFocus.rotation = Quaternion.Euler(-_rotation.y, _rotation.x, 0);
        facing.rotation = Quaternion.Euler(0, _rotation.x, 0);

        // position the camera closer to the player if there's a wall, but never too close
        {
            // find direction to raycast (in world space)
            var worldRaycastDir = camFocus.TransformVector(_raycastDir);

            // raycast to determine where the camera should be located
            float realRaycastDist;
            if (Physics.Raycast(camFocus.position, worldRaycastDir, out var hitInfo, _raycastDist, ~Layers.Entity))
            {
                // if the camera would be too close to the player, let it clip into a wall
                realRaycastDist = Mathf.Max(hitInfo.distance, minCameraDist);
            }
            else
            {
                realRaycastDist = _raycastDist;
            }

            transform.localPosition = _raycastDir * realRaycastDist;
        }
    }
}