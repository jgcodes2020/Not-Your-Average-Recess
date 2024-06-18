using System;
using System.Collections;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class InputConfigScreen : MonoBehaviour
{
    public GameObject gameManagerPrefab;

    private ScreenManager _screenManager;
    private UIDocument _document;

    private Button _startButton;
    private Button _backButton;
    private InputChooser[] _playerInputs;

    private void Start()
    {
        _document = GetComponent<UIDocument>() ??
                    throw new ApplicationException("Can't find UI document");
        _screenManager = transform.parent.gameObject.GetComponent<ScreenManager>();

        var body = _document.rootVisualElement;
        _startButton = body.Q<Button>("start-button");
        _backButton = body.Q<Button>("back-button");

        var devChooserBin = body.Q<VisualElement>("dev-chooser");
        _playerInputs = devChooserBin.Children().Select(x =>
        {
            var res = (InputChooser) x;
            res.OnInputChanged += OnInputChanged;
            return res;
        }).ToArray();
        DetectEndScreenState();

        _startButton.clicked += StartButtonClicked;
        _backButton.clicked += BackButtonClicked;
    }

    private void DetectEndScreenState()
    {
        var endStateObj = GameObject.Find("End State");
        if (endStateObj == null)
            return;
        
        Debug.Log("found end state");

        var endScreenState = endStateObj.GetComponent<GameEndState>();
        foreach ((var input, InputDevice[] devices) in _playerInputs.Zip(
                     endScreenState.PlayerInputList, (chooser, devices) => (chooser, devices)))
        {
            input.SetDevices(devices);
        }
        
        Destroy(endStateObj);

        StartCoroutine(SwitchToInputConfig());
        return;

        IEnumerator SwitchToInputConfig()
        {
            yield return null;
            _screenManager.SwitchScreen("Input Config");
        }
    }

    private void StartButtonClicked()
    {
        var gameManagerObj = Instantiate(gameManagerPrefab);
        var gameManager = gameManagerObj.GetComponent<GameStateManager>();

        if (!_playerInputs.All(chooser => chooser.Devices.Length > 0))
        {
            // invalid devices... need to show error message
            return;
        }

        // attach all player devices
        gameManager.PlayerCount = _playerInputs.Length;
        for (int i = 0; i < _playerInputs.Length; i++)
        {
            gameManager[i] = _playerInputs[i].Devices.ToArray();
        }

        // Load and switch to game.
        StartCoroutine(Scenes.ToGameLevel());
    }

    private void BackButtonClicked()
    {
        _screenManager.SwitchScreen("Title");
    }

    private void OnInputChanged(object inputObj, object args)
    {
        var currentPlayer = (InputChooser) inputObj;

        foreach (var player in _playerInputs)
        {
            if (player == currentPlayer)
                continue;

            // if this player's input is the same as the one we just changed, unset it
            var srcDevices = MemoryMarshal.ToEnumerable(player.Devices).Select(dev => dev.deviceId);
            var dstDevices = MemoryMarshal.ToEnumerable(currentPlayer.Devices).Select(dev => dev.deviceId);

            if (srcDevices.SequenceEqual(dstDevices))
            {
                player.ResetInput();
            }
        }
    }
}