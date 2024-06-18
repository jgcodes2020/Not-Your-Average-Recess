using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

[RequireComponent(typeof(PlayerInputManager))]
public class GameStateManager : MonoBehaviour
{
    [Description("Player Prefab")] public GameObject playerPrefab;

    [Description("UI Prefab")] public GameObject uiPrefab;

    [Range(2f, 15f)] public double gameLengthMinutes;
    [Range(0.25f, 3f)] public double itLengthMinutes;
    [Range(0f, 100f)] public uint tagCooldownTicks;

    private TimeSpan _gameLength;
    private TimeSpan _itLength;

    // pre-start game state
    [ItemCanBeNull] private List<InputDevice[]> _playerInputList = new();

    // listeners
    private UnityAction<Scene, Scene> _onSceneChange;

    // game state
    private List<IPauseable> _pauseableState = new();
    private bool _paused = false;
    private bool _gameOver = false;

    private int _currentIt = -1;

    // timers
    [CanBeNull] private Stopwatch _gameTimer;
    [CanBeNull] private Stopwatch _itTimer;
    private uint _tagCooldown;

    // UI
    private GamePauseMenu _pauseMenu;


    public TimeSpan GameTimeLeft => _gameLength - _gameTimer!.Elapsed;
    public TimeSpan ItTimeLeft => _itLength - _itTimer!.Elapsed;
    public uint TagCooldown => _tagCooldown;

    public int PlayerCount
    {
        get => _playerInputList.Count;
        set
        {
            if (value < _playerInputList.Count)
            {
                _playerInputList.RemoveRange(value, _playerInputList.Count - value);
            }
            else if (value > _playerInputList.Count)
            {
                if (value > _playerInputList.Capacity)
                    _playerInputList.Capacity = value;
                _playerInputList.AddRange(Enumerable.Repeat<InputDevice[]>(null, value - _playerInputList.Count));
            }
        }
    }

    public InputDevice[] this[int index]
    {
        get => _playerInputList[index];
        set => _playerInputList[index] = value;
    }

    public int CurrentIt => _currentIt;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        _onSceneChange = OnSceneChange;
        SceneManager.activeSceneChanged += _onSceneChange;
    }

    private void Start()
    {
        _gameLength = TimeSpan.FromMinutes(gameLengthMinutes);
        _itLength = TimeSpan.FromMinutes(itLengthMinutes);
    }

    private void OnDestroy()
    {
        // SceneManager will keep notifying us about scene changes even after the object is
        // destroyed if we don't unregister ourselves. This also allows this object to be GC'ed.
        SceneManager.activeSceneChanged -= _onSceneChange;
    }

    private void Update()
    {
        if (_gameTimer == null || _itTimer == null)
            return;
        if (_gameOver)
            return;

        // if the game timer or it timer expires, game's over
        if (_gameTimer.Elapsed > _gameLength || _itTimer.Elapsed > _itLength)
        {
            StartCoroutine(EndGame());
        }
    }

    private void FixedUpdate()
    {
        if (_tagCooldown > 0)
            --_tagCooldown;
    }

    private IEnumerator EndGame()
    {
        _gameOver = true;
        foreach (var pauseable in _pauseableState)
        {
            pauseable.Paused = true;
        }
        // show animation??

        var endStateObject = new GameObject("End State");
        var endState = endStateObject.AddComponent<GameEndState>();

        endState.PassState(_playerInputList, _currentIt == 1 ? 0 : 1);

        yield return new WaitForSecondsRealtime(0.5f);
        yield return StartCoroutine(Scenes.ToEndScreen());
    }

    private static Transform[] LocateSpawnpoints()
    {
        var spawnpointsObj = GameObject.Find("Level").FindChild("Spawnpoints");

        // All children of the Spawnpoints object represent the positions of spawnpoints in the world.
        // Get each child, find its position, and merge them into a single array.
        return spawnpointsObj.transform
            .GetChildren()
            .ToArray();
    }

    private void GameStart()
    {
        // Ensure object gets destroyed when scene ends
        SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());

        // find spawn points for players
        var spawnpointList = LocateSpawnpoints();

        // set the "it" player
        _currentIt = Random.Range(0, PlayerCount);

        Debug.Log($"Found {_playerInputList.Count} players");

        // setup players
        foreach (var (index, inputDev) in _playerInputList.Select((x, i) => (i, x)))
        {
            var playerInput = PlayerInput.Instantiate(playerPrefab, index, pairWithDevices: inputDev);

            // Setup the player with necessary global state
            var player = playerInput.GetComponent<PlayerController>();
            player.GameState = this;
            player.PlayerIndex = index;

            // Add player and player camera to list of pauseable things
            var playerCam = playerInput.GetComponentInChildren<PlayerCamController>();
            _pauseableState.Add(player);
            _pauseableState.Add(playerCam);

            // detect pause inputs
            playerInput.currentActionMap["Pause"].performed += _ => OnInputPause(player);

            // move the player to a spawn point corresponding to their player index
            // offset the position slightly to avoid clipping issues with more than 2 players (future support?)
            var spawnID = index % spawnpointList.Length;
            var playerRb = playerInput.GetComponent<Rigidbody>();
            
            playerRb.transform.position = spawnpointList[spawnID].position;
            // Take the signed angle as the forward angle
            playerCam.ForwardAngle = Vector2.SignedAngle(Vector2.down, spawnpointList[spawnID].forward.Horizontal2D());
            Physics.SyncTransforms();
        }

        // start the timers
        _gameTimer = Stopwatch.StartNew();
        _itTimer = Stopwatch.StartNew();

        // Spawn the global UI
        GameObject ui = Instantiate(uiPrefab);
        ui.GetComponent<GlobalGameUI>().GameState = this;

        // Locate and initialize the pause menu
        GameObject pauseMenuObj = GameObject.Find("Pause Menu");
        _pauseMenu = pauseMenuObj.GetComponent<GamePauseMenu>();
        _pauseMenu.GameState = this;
    }

    private void OnSceneChange(Scene current, Scene next)
    {
        if (next.name.Contains("Level"))
        {
            GameStart();
        }
    }

    public void OnPlayerTagged(int playerIndex)
    {
        if (_tagCooldown > 0)
            return;

        _currentIt = playerIndex;
        _itTimer!.Restart();
        _tagCooldown = tagCooldownTicks;
    }

    private void OnInputPause(PlayerController _)
    {
        if (_gameOver)
            return;
        // toggle pause state
        _paused = !_paused;
        UpdatePauseState();
    }

    public void SetPauseState(bool value)
    {
        _paused = value;
        UpdatePauseState();
    }

    private void UpdatePauseState()
    {
        // set pause state for all pauseable objects
        foreach (var pauseable in _pauseableState)
        {
            pauseable.Paused = _paused;
        }

        // show or hide the pause menu
        _pauseMenu.Visible = _paused;

        if (_paused)
        {
            _gameTimer!.Stop();
            _itTimer!.Stop();
        }
        else
        {
            _gameTimer!.Start();
            _itTimer!.Start();
        }
    }
}