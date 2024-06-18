using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class InputChooser : VisualElement
{
    public new class UxmlFactory : UxmlFactory<InputChooser, UxmlTraits>
    {
    }

    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        // no children
        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get
            {
                yield break;
            }
        }

        public UxmlIntAttributeDescription PlayerIndex = new() { name = "player-index" };

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            ((InputChooser)ve).PlayerIndex = PlayerIndex.GetValueFromBag(bag, cc);
        }
    }

    private Button _controlBtn;
    private Button ControlBtn => _controlBtn ??= this.Q<Button>("control-btn");

    private Label _playerLabel;
    private Label PlayerLabel => _playerLabel ??= this.Q<Label>("player-label");

    private Label _controlSchemeLabel;
    private Label ControlSchemeLabel => _controlSchemeLabel ??= this.Q<Label>("control-scheme");  
    
    private InputAction _actListenInput;
    private InputDevice[] _devices;
    private EventSystem _prevEventSystem;

    [CanBeNull] public event EventHandler<object> OnInputChanged;

    private string DeviceDescription
    {
        get
        {
            if (_devices == null)
                return "Click to\nset device";

            return _devices[0] switch
            {
                Keyboard or Mouse => "KB/Mouse",
                Gamepad gamepad => $"Gamepad:\n{gamepad.displayName}",
                _ => "Unknown Device"
            };
        }
    }

    private string ControlScheme
    {
        get
        {
            if (_devices == null)
                return "";
            
            return _devices[0] switch
            {
                Keyboard or Mouse => 
                    "WASD to move\n" +
                    "Mouse to look around\n" +
                    "Left Shift to sprint\n" +
                    "Space to jump\n" +
                    "LMB to tag",
                Gamepad gamepad => 
                    "Left Stick to move\n" +
                    "Right Stick to look around\n" +
                    $"{gamepad.leftTrigger.shortDisplayName} to sprint\n" +
                    $"{gamepad.buttonSouth.shortDisplayName} to jump\n" +
                    $"{gamepad.rightTrigger.shortDisplayName} to tag",
                _ => "Unknown Device"
            };
        }
    }

    private bool Listening
    {
        get => _actListenInput.enabled;
        set
        {
            if (value)
            {
                _actListenInput.Enable();
                ControlBtn.text = "Waiting for input...";
                
                _prevEventSystem = EventSystem.current;
                _prevEventSystem.enabled = false;
            }
            else
            {
                _actListenInput.Disable();
                ControlBtn.text = DeviceDescription;
                ControlSchemeLabel.text = ControlScheme;
                
                _prevEventSystem.enabled = true;
                _prevEventSystem = null;
                Blur();
            }
        }
    }

    public ReadOnlyMemory<InputDevice> Devices
    {
        get
        {
            if (_devices == null)
                return ReadOnlyMemory<InputDevice>.Empty;

            return _devices;
        }
    }

    public void SetDevices(ReadOnlySpan<InputDevice> devices)
    {
        _devices = devices.ToArray();
        _controlBtn.text = DeviceDescription;
        ControlSchemeLabel.text = ControlScheme;
    }

    private int _playerIndex;

    public int PlayerIndex
    {
        get => _playerIndex;
        set
        {
            _playerIndex = value;
            PlayerLabel.text = $"P{_playerIndex}";
        }
    }

    public InputChooser()
    {
        var asset = Resources.Load<VisualTreeAsset>("Components/InputChooser");
        asset.CloneTree(this);

        ControlBtn.clicked += OnControlBtnClicked;
        ControlBtn.text = DeviceDescription;
        PlayerIndex = 1;

        _actListenInput = new InputAction(binding: "*/<Button>");
        _actListenInput.performed += OnInputReceived;
        
    }

    private void OnControlBtnClicked()
    {
        Listening = !Listening;
        if (Listening)
        {
            ControlBtn.Blur();
        }
    }

    private void OnInputReceived(InputAction.CallbackContext context)
    {
        var device = context.control.device;

        switch (device)
        {
            case Keyboard:
                _devices = new InputDevice[] { Keyboard.current, Mouse.current };
                break;
            case Gamepad g:
                _devices = new InputDevice[] { g };
                break;
        }
        
        OnInputChanged?.Invoke(this, null);

        Listening = false;
    }

    public void ResetInput()
    {
        _devices = null;
        _controlBtn.text = DeviceDescription;
    }
}