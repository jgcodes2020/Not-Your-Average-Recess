using System;
using UnityEngine;
using UnityEngine.UIElements;

public class GlobalGameUI : MonoBehaviour
{
    private GameStateManager _gameState;
    
    private UIDocument _uiDocument;
    private Label _gameTime;
    private Label _itTime;

    public GameStateManager GameState
    {
        set
        {
            if (_gameState != null)
                throw new InvalidOperationException("UI is already associated with a game state!");
            _gameState = value;
        }
    }

    private void Start()
    {
        _uiDocument = GetComponent<UIDocument>();
        var body = _uiDocument.rootVisualElement;

        _gameTime = body.Q<Label>("match-timer");
        _itTime = body.Q<Label>("it-timer");
    }

    private void Update()
    {
        _gameTime.text = $"{_gameState.GameTimeLeft:mm\\:ss}";
        _itTime.text = $"{_gameState.ItTimeLeft:mm\\:ss} to tag";
    }
}