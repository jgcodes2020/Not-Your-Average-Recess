using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class TitleScreen : MonoBehaviour
{
    private bool _scenesLoaded;
    
    private ScreenManager _screenManager;
    private UIDocument _document;

    private Button _startButton;
    private Button _creditsButton;
    private Button _quitButton;

    private void Start()
    {
        _document = GetComponent<UIDocument>() ??
                    throw new ApplicationException("Can't find UI document");
        _screenManager = transform.parent.gameObject.GetComponent<ScreenManager>();

        var body = _document.rootVisualElement;
        _startButton = body.Q<Button>("start-button");
        _creditsButton = body.Q<Button>("credits-button");
        _quitButton = body.Q<Button>("quit-button");

        if (InputSystem.devices.Any(x => x is Gamepad))
        {
            _startButton.Focus();
        }

        _startButton.clicked += StartButtonClicked;
        _creditsButton.clicked += CreditsButtonClicked;
        _quitButton.clicked += QuitButtonClicked;
    }

    private void StartButtonClicked()
    {
        _screenManager.SwitchScreen("Input Config");
    }

    private void CreditsButtonClicked()
    {
        
        _screenManager.SwitchScreen("Credits");
    }

    private void QuitButtonClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}