using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class PlayerController
{
    #region Collision handlers

    private void OnCollisionEnter(Collision other)
    {
        bool prevAirborne = _activeFloors.Count == 0;
        // track active floors
        if (other.gameObject.CompareTag("Terrain"))
        {
            // if floor is flat enough to stand on, it's an active floor
            if (ContactNormals(other).Any(n => n.y > _minFloorYNormal))
                _activeFloors.Add(other.collider);
        }
        
        // when landing, zero speed if not inputting any movement
        if (prevAirborne && Grounded && _moveVector.magnitude < 0.1f)
        {
            _rb.velocity = Vector3.zero;
        }
    }

    private void OnCollisionStay(Collision other)
    {
        // track active floors
        if (other.gameObject.CompareTag("Terrain"))
        {
            // if 
            if (ContactNormals(other).Any(n => n.y > _minFloorYNormal))
                _activeFloors.Add(other.collider);
            else
                _activeFloors.Remove(other.collider);
        }
    }

    private void OnCollisionExit(Collision other)
    {
        // track active floors
        if (other.gameObject.CompareTag("Terrain"))
        {
            _activeFloors.Remove(other.collider);
        }
    }

    #endregion
    
    private void DoMovementTick()
    {
        float moveSpeed = _isSprinting && _moveVector.y > 0 ? sprintSpeed : walkSpeed;
        #if UNITY_EDITOR
        if (_gameState != null && IsIt)
            moveSpeed *= itSpeedBoost;
        #else
        if (IsIt)
            moveSpeed *= itSpeedBoost;
        #endif
        
        if (Grounded)
        {
            GroundStep(moveSpeed);
            if (HandleJump()) return;
        }
        else
        {
            AirStep(moveSpeed);
        }
        
        CapSpeed(maxSpeedCap);

        // Make motion more snappy by letting the player stop quicker
        if (_rb.velocity.magnitude < 0.5f)
        {
            _rb.velocity = Vector3.zero;
        }
    }

    #region Action utilities
    /// <summary>
    /// Applies ground movement and drag.
    /// </summary>
    /// <param name="moveSpeed">The player's current movement speed.</param>
    private void GroundStep(float moveSpeed)
    {
        // set drag
        _rb.drag = groundDrag;
        

        // handle horizontal movement. Downward force is added to ensure player maintains
        // contact with ground.
        Vector3 moveForce = _moveVector.x * facing.right + _moveVector.y * facing.forward;
        moveForce = moveForce * (moveSpeed * 1.3f * groundDrag) + Vector3.up * -5f;

        _rb.AddForce(moveForce, ForceMode.Force);
    }

    /// <summary>
    /// Applies air straining and drag.
    /// </summary>
    /// <param name="moveSpeed">The player's current movement speed.</param>
    private void AirStep(float moveSpeed)
    {
        // set drag
        _rb.drag = airDrag;
        
        // handle horizontal movement
        Vector3 moveForce = _moveVector.x * facing.right + _moveVector.y * facing.forward;
        // Prevent the player from increasing their speed too much by strafing
        // If the player is at max speed and they are strafing in the direction
        // of motion, reduce power
        Vector3 rbHVelocity = _rb.velocity.Horizontal();
        float strafePower = moveSpeed * 10f;
        if (rbHVelocity.magnitude > moveSpeed && Vector3.Dot(rbHVelocity, moveForce) > 0)
        {
            strafePower *= airStrafeStrength;
        }
        moveForce *= strafePower;

        _rb.AddForce(moveForce, ForceMode.Force);
    }
    
    /// <summary>
    /// Checks and handles a jump input.
    /// </summary>
    /// <returns>Whether the player jumped</returns>
    private bool HandleJump()
    {
        if (_jumpBuffer == 0) return false;
        _jumpBuffer = 0;
        _rb.AddForce(facing.up * jumpImpulse, ForceMode.Impulse);

        // we will be in the air on this frame, set air drag
        _rb.drag = airDrag;

        _hasJumped = true;
        
        return true;
    }
    
    private bool TryTag()
    {
        // Raycast to locate a taggable player
        if (!Physics.SphereCast(camFocus.position, tagHitRadius, facing.forward, out var hitInfo, maxTagDist)) 
            return false;
        
        // if the hit object is a player, tag them
        if (hitInfo.rigidbody is { } rb)
        {
            
            if (rb.GetComponent<PlayerController>() is { } player)
            {
                _gameState.OnPlayerTagged(player.PlayerIndex);
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Caps the player's speed in the direction of travel.
    /// </summary>
    /// <param name="moveSpeed">The speed cap.</param>
    private void CapSpeed(float moveSpeed)
    {
        var hVelocity = _rb.velocity.Horizontal();
        var hSpeed = hVelocity.magnitude;
        if (hSpeed > moveSpeed)
        {
            var cappedVelocity = hVelocity.normalized * moveSpeed;
            _rb.velocity = new Vector3(cappedVelocity.x, _rb.velocity.y, cappedVelocity.z);
        }
    }


    #endregion

    private static IEnumerable<Vector3> ContactNormals(Collision other) => Enumerable
        .Range(0, other.contactCount)
        .Select(i => other.GetContact(i).normal);

}