using UnityEngine;
using UnityEngine.UIElements;

public class EndScreen : MonoBehaviour
{
    private UIDocument _uiDocument;
    private Label _winMessage;
    private Button _rematchButton;
    private Button _quitButton;

    private GameEndState _endState;
    
    private void Start()
    {
        _uiDocument = GetComponent<UIDocument>();
        

        var body = _uiDocument.rootVisualElement;
        _winMessage = body.Q<Label>("win-message");
        _rematchButton = body.Q<Button>("rematch-button");
        _quitButton = body.Q<Button>("quit-button");
        
        _rematchButton.clicked += OnRematchClicked;
        _quitButton.clicked += OnQuitClicked;
        
        _endState = GameObject.Find("End State").GetComponent<GameEndState>();
        _winMessage.text = $"P{_endState.WinningPlayer + 1} won!";
    }

    private void OnRematchClicked()
    {
        StartCoroutine(Scenes.ToTitleScreen());
    }

    private void OnQuitClicked()
    {
        Destroy(_endState.gameObject);
        StartCoroutine(Scenes.ToTitleScreen());
    }

    private void Update()
    {
        
    }
}