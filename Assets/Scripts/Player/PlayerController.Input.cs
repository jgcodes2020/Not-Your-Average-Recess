using UnityEngine;
using UnityEngine.InputSystem;

public partial class PlayerController
{
    
    // inputs
    private Vector2 _moveVector = Vector2.zero;
    private uint _jumpBuffer = 0;
    private bool _hasJumped = false;
    private bool _isSprinting = false;
    private bool _isTagging = false;
    
    
    // actions
    private InputAction _actMove;
    private InputAction _actJump;
    private InputAction _actSprint;
    private InputAction _actTag;
    private void OnJump(InputAction.CallbackContext context)
    {
        _jumpBuffer = jumpBufferTicks + 1;
    }

    private void PollInput()
    {
        _moveVector = _actMove.ReadValue<Vector2>();
        _isSprinting = _actSprint.IsPressed();
        _isTagging = _actTag.IsPressed();
    }
}