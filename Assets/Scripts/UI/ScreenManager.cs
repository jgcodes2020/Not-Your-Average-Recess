using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ScreenManager : MonoBehaviour
{
    public UIDocument initialScreen;

    private UIDocument currentScreen;
    private Dictionary<string, UIDocument> screenLookup = new();
    
    private void Awake()
    {
        if (initialScreen.gameObject.transform.parent.gameObject != gameObject)
        {
            throw new ApplicationException("Initial UI document should be a child of this UI manager");
        }
        currentScreen = initialScreen;
        foreach (var document in gameObject.GetComponentsInChildren<UIDocument>())
        {
            // Make each screen invisible except for the initial one
            document.rootVisualElement.style.display = 
                document == initialScreen ? DisplayStyle.Flex : DisplayStyle.None;

            string documentName = document.gameObject.name;
            screenLookup.Add(documentName, document);
        }
    }

    /// <summary>
    /// Switches to a different screen. Screen names are derived from the names
    /// of the GameObjects they are associated with.
    /// </summary>
    /// <param name="screen">The screen to switch to.</param>
    public void SwitchScreen(string screen)
    {
        var nextScreen = screenLookup[screen];
        
        // hide the current screen, then show the next one
        currentScreen.rootVisualElement.style.display = DisplayStyle.None;
        nextScreen.rootVisualElement.style.display = DisplayStyle.Flex;
        
        currentScreen = nextScreen;
    }
}