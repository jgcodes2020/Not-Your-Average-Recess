using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[RequireComponent(typeof(PlayerInput))]
public partial class PlayerController : MonoBehaviour, IPauseable
{
    public float walkSpeed;
    public float sprintSpeed;
    public float maxSpeedCap;

    [Tooltip("Speed multiplier for players who are it.")]
    public float itSpeedBoost;

    public float groundDrag;
    public float airDrag;

    [Tooltip("Amount of force that the player jumps with.")]
    public float jumpImpulse;

    [FormerlySerializedAs("jumpBufferFrames")]
    [Range(0, 3)]
    [Tooltip("Number of physics ticks to allow jumps to be buffered.")]
    public uint jumpBufferTicks;

    [Range(0.7f, 3f)] [Tooltip("Distance to allow players to tag.")]
    public float maxTagDist;

    [Range(0f, 1.5f)] [Tooltip("Radius of the tag hitbox.")]
    public float tagHitRadius;

    [Range(0, 1)] [Tooltip("Controls how much the player can influence their trajectory in the air.")]
    public float airStrafeStrength;

    [Range(0, 90)] public double maxFloorSlope;
    private float _minFloorYNormal;

    // Linked GameObjects/Components
    public Transform facing;
    public Transform camFocus;

    // global game state
    private GameStateManager _gameState = null;

    public GameStateManager GameState
    {
        get => _gameState;
        set
        {
            // Once a player is bound to a game state, it should not be bound to another
            if (_gameState != null)
                throw new InvalidOperationException("Game state manager is already set!");

            _gameState = value ?? throw new ArgumentNullException(nameof(value));
        }
    }

    private int _playerIndex = -1;

    public int PlayerIndex
    {
        get
        {
            if (_playerIndex < 0)
                throw new InvalidOperationException("Player index isn't set!");
            return _playerIndex;
        }
        set
        {
            if (value < 0)
                throw new ArgumentException("Index must be positive", nameof(value));
            // Once a player has an index, it should not change its index
            if (_playerIndex >= 0)
                throw new InvalidOperationException("Player index is already set!");

            _playerIndex = value;
        }
    }

    public bool IsIt => _gameState.CurrentIt == _playerIndex;

    public bool Paused
    {
        get => _paused;
        set
        {
            Debug.Log($"paused = {value}");
            _paused = value;
            // disable physics and animation
            _rb.constraints = value ? RigidbodyConstraints.FreezeAll : RigidbodyConstraints.FreezeRotation;
            animator.speed = value ? 0.0f : 1.0f;
        }
    }

    // collision and terrain
    private Rigidbody _rb;
    private HashSet<Collider> _activeFloors;
    private bool _paused;

    private PlayerInput _input;

    private bool Grounded => _activeFloors.Count > 0;

    private void Start()
    {
        _activeFloors = new HashSet<Collider>();
        _rb = GetComponent<Rigidbody>();

        _minFloorYNormal = (float) Math.Cos(maxFloorSlope * (Math.PI / 180));

        // actions config
        var playerInput = GetComponent<PlayerInput>();

        _actMove = playerInput.currentActionMap["Move"];
        _actJump = playerInput.currentActionMap["Jump"];
        _actSprint = playerInput.currentActionMap["Sprint"];
        _actTag = playerInput.currentActionMap["Tag"];
        _actJump.performed += OnJump;

        // input states
        _jumpBuffer = 0;
        _isSprinting = false;
    }

    private void Update()
    {
        if (Paused)
            return;

        PollInput();
        UpdateAnimator();
    }


    private void FixedUpdate()
    {
        if (Paused)
            return;

        // apply movement
        DoMovementTick();
        // attempt tagging
        #if UNITY_EDITOR
        if (_gameState != null && IsIt && _isTagging && _gameState.TagCooldown == 0)
        #else
        if (IsIt && _isTagging && _gameState.TagCooldown == 0)
        #endif
        {
            if (TryTag())
            {
                PlayTagSound();
            }
        }

        // decrement jump timer
        if (_jumpBuffer > 0)
            --_jumpBuffer;
    }
}