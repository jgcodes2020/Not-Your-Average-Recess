using System;
using UnityEngine;
using UnityEngine.UIElements;

public class CreditsScreen : MonoBehaviour
{
    private ScreenManager _screenManager;
    private UIDocument _document;
    
    private Button _backButton;

    private void Start()
    {
        _document = GetComponent<UIDocument>() ??
                    throw new ApplicationException("Can't find UI document");
        _screenManager = transform.parent.gameObject.GetComponent<ScreenManager>();

        var body = _document.rootVisualElement;
        _backButton = body.Q<Button>("back-button");
        
        _backButton.clicked += OnBackButtonClicked;
    }

    private void OnBackButtonClicked()
    {
        _screenManager.SwitchScreen("Title");
    }
}