//

// This controller map was made so that I could use someone's 3rd-party Switch controller during development. It also requires a patched version of the
// Unity Input System to work properly; this is a tracked issue on Unity's end and I have made a PR to fix it.
#if UNITY_STANDALONE_LINUX
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

/// <summary>
/// Data object holding input from a PDP Faceoff Deluxe+ Audio Wired Controller for Nintendo Switch. Used by
/// the Unity 
/// </summary>
[StructLayout(LayoutKind.Explicit, Size = 28)]
public class PdpFaceoffDeluxeAudioPlusWiredSwitchProControllerState : IInputStateTypeInfo
{
    public FourCC format => new('L', 'J', 'O', 'Y');

    [InputControl(name = "buttonNorth", layout = "Button", displayName = "X", format = "BIT", bit = 3)]
    [InputControl(name = "buttonEast", layout = "Button", displayName = "A", format = "BIT", bit = 2)]
    [InputControl(name = "buttonSouth", layout = "Button", displayName = "B", format = "BIT", bit = 1)]
    [InputControl(name = "buttonWest", layout = "Button", displayName = "Y", format = "BIT", bit = 0)]
    [InputControl(name = "leftShoulder", layout = "Button", displayName = "L", format = "BIT", bit = 4)]
    [InputControl(name = "rightShoulder", layout = "Button", displayName = "R", format = "BIT", bit = 5)]
    [InputControl(name = "leftTrigger", layout = "Button", displayName = "ZL",format = "BIT",  bit = 6)]
    [InputControl(name = "rightTrigger", layout = "Button", displayName = "ZR",format = "BIT",  bit = 7)]
    [InputControl(name = "select", displayName = "Select", format = "BIT", offset = 1, bit = 0)]
    [InputControl(name = "start", displayName = "Start", format = "BIT", offset = 1, bit = 1)]
    [InputControl(name = "home", layout = "Button", displayName = "Home", format = "BIT", offset = 1, bit = 4)]
    [InputControl(name = "screenshot", layout = "Button", displayName = "Screenshot", format = "BIT", offset = 1, bit = 5)]
    [InputControl(name = "leftStickPress", layout = "Button", offset = 1, bit = 2)]
    [InputControl(name = "rightStickPress", layout = "Button", offset = 1, bit = 3)]
    [FieldOffset(0)]
    public byte buttons0;
    [FieldOffset(1)]
    public byte buttons1;

    [InputControl(name = "leftStick", layout = "Stick", format = "BIT", sizeInBits = 64)]
    [InputControl(name = "leftStick/x", offset = 0, format = "BYTE",
        parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
    [InputControl(name = "leftStick/left", offset = 0, format = "BYTE",
        parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp,clampMin=0,clampMax=0.5,invert")]
    [InputControl(name = "leftStick/right", offset = 0, format = "BYTE",
        parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp,clampMin=0.5,clampMax=1")]
    [InputControl(name = "leftStick/y", offset = 4, format = "BYTE",
        parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,invert")]
    [InputControl(name = "leftStick/up", offset = 4, format = "BYTE",
        parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp,clampMin=0.5,clampMax=1")]
    [InputControl(name = "leftStick/down", offset = 4, format = "BYTE",
        parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp,clampMin=0,clampMax=0.5,invert")]
    [FieldOffset(4)]
    public ulong leftStickState;

    [InputControl(name = "rightStick", layout = "Stick", format = "BIT", sizeInBits = 64)]
    [InputControl(name = "rightStick/x", offset = 0, format = "BYTE",
        parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
    [InputControl(name = "rightStick/left", offset = 0, format = "BYTE",
        parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp,clampMin=0,clampMax=0.5,invert")]
    [InputControl(name = "rightStick/right", offset = 0, format = "BYTE",
        parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp,clampMin=0.5,clampMax=1")]
    [InputControl(name = "rightStick/y", offset = 4, format = "BYTE",
        parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
    [InputControl(name = "rightStick/up", offset = 4, format = "BYTE",
        parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp,clampMin=0,clampMax=0.5,invert")]
    [InputControl(name = "rightStick/down", offset = 4, format = "BYTE",
        parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp,clampMin=0.5,clampMax=1")]
    [FieldOffset(12)]
    public ulong rightStickState;

    [InputControl(name = "dpad", layout = "Dpad", format = "BIT", sizeInBits = 64)]
    [InputControl(name = "dpad/left", format = "UINT", offset = 0, bit = 0, layout = "DiscreteButton",
        parameters = "minValue=-1,maxValue=-1")]
    [InputControl(name = "dpad/right", format = "UINT", offset = 0, bit = 0, layout = "DiscreteButton",
        parameters = "minValue=1,maxValue=1")]
    [InputControl(name = "dpad/up", format = "UINT", offset = 4, bit = 0, layout = "DiscreteButton",
        parameters = "minValue=-1,maxValue=-1")]
    [InputControl(name = "dpad/down", format = "UINT", offset = 4, bit = 0, layout = "DiscreteButton",
        parameters = "minValue=1,maxValue=1")]
    [FieldOffset(20)]
    public int dpadX; 

    [FieldOffset(24)] public int dpadY;
}

/// <summary>
/// Custom controller mapping for the PDP Faceoff Deluxe+ Audio Wired Controller for Nintendo Switch on Linux.
/// </summary>
[InputControlLayout(stateType = typeof(PdpFaceoffDeluxeAudioPlusWiredSwitchProControllerState))]
#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class PDPFaceoffDeluxeAudioWiredControllerForNintendoSwitch : Gamepad
{
    private static readonly InputDeviceMatcher controlMatcher = new InputDeviceMatcher()
        .WithInterface("Linux")
        .WithManufacturer("Logic3", false)
        .WithProduct("PDP Faceoff Deluxe+ Audio Wired Controller for Nintendo Switch", false);
    
    static PDPFaceoffDeluxeAudioWiredControllerForNintendoSwitch()
    {
        
        InputSystem.RegisterLayout<PDPFaceoffDeluxeAudioWiredControllerForNintendoSwitch>(
            name: "PDP Faceoff Deluxe+ Audio Wired Controller for Nintendo Switch",
            matches: controlMatcher
        );
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init() {}
}
#endif