using UnityEngine;

public partial class PlayerController
{
    // animation parameters
    public Animator animator;
    private static readonly int A_walkSpeed = Animator.StringToHash("Walk Speed");
    private static readonly int A_backpedalSpeed = Animator.StringToHash("Backpedal Speed");
    private static readonly int A_runBlend = Animator.StringToHash("Run Blend");
    private static readonly int A_grounded = Animator.StringToHash("Grounded");
    private static readonly int A_jump = Animator.StringToHash("Jump");
    private static readonly int A_falling = Animator.StringToHash("Falling");
    private static readonly int A_tag = Animator.StringToHash("Tag");
    
    
    
    private void UpdateAnimator()
    {
        float fwdVel = Vector3.Dot(_rb.velocity, facing.forward);
        float hSpeed = _rb.velocity.Horizontal().magnitude;

        {
            // relative speed of walking animation
            float animWalkSpeed;
            if (hSpeed > walkSpeed)
                animWalkSpeed = 1.0f;
            else
                animWalkSpeed = hSpeed / walkSpeed;
        
            // check if we're actually walking sideways or backwards
            float animBackpedalSpeed = 0.0f;
            if (fwdVel < 0.1f)
            {
                // backpedal instead of walking
                animBackpedalSpeed = animWalkSpeed;
                animWalkSpeed = 0.0f;
            }
            animator.SetFloat(A_walkSpeed, animWalkSpeed);
            animator.SetFloat(A_backpedalSpeed, animBackpedalSpeed);
        }

        {
            // blend between walk and run
            float runBlend;
            if (hSpeed <= walkSpeed)
                runBlend = 0.0f;
            else if (hSpeed >= sprintSpeed)
                runBlend = 1.0f;
            else
                runBlend = (hSpeed - walkSpeed) / (sprintSpeed - walkSpeed);
            animator.SetFloat(A_runBlend, runBlend);
        }

        if (_hasJumped)
        {
            animator.SetTrigger(A_jump);
            _hasJumped = false;
        }
        
        // check if grounded
        animator.SetBool(A_grounded, Grounded);
        // check if falling
        animator.SetBool(A_falling, _rb.velocity.y < 0);
        // check if tagging
        animator.SetBool(A_tag, _isTagging && _gameState.TagCooldown == 0);
    }
}