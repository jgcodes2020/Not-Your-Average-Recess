using System;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class GamePauseMenu : MonoBehaviour
{
    private UIDocument _uiDocument;
    private Button _resumeButton;
    private Button _quitButton;
    
    private CursorLockMode? _prevLockMode;
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

    private void Start()
    {
        _uiDocument = GetComponent<UIDocument>();
        var body = _uiDocument.rootVisualElement;
        
        body.style.display = DisplayStyle.None;

        _resumeButton = body.Q<Button>("resume-btn");
        _quitButton = body.Q<Button>("quit-btn");
        
        _resumeButton.clicked += OnResumeClicked;
        _quitButton.clicked += OnQuitClicked;
    }

    private void OnResumeClicked()
    {
        // unpause the game
        _gameState.SetPauseState(false);
    }

    private void OnQuitClicked()
    {
        // go back to the title screen
        StartCoroutine(Scenes.ToTitleScreen());
    }

    private void Show()
    {
        _uiDocument.rootVisualElement.style.display = DisplayStyle.Flex;
        _prevLockMode = Cursor.lockState;
        Cursor.lockState = CursorLockMode.None;
    }

    private void Hide()
    {
        _uiDocument.rootVisualElement.style.display = DisplayStyle.None;
        if (_prevLockMode is { } prevLockMode)
        {
            Cursor.lockState = prevLockMode;
            _prevLockMode = null;
        }
    }

    public bool Visible
    {
        set
        {
            if (value)
                Show();
            else
                Hide();
        }
    }
}