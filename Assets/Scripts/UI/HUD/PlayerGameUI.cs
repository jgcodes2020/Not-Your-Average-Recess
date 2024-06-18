using UnityEngine;
using UnityEngine.UIElements;

public class PlayerGameUI : MonoBehaviour
{
    public PlayerController Player;
    public Camera PlayerCam;
    
    private UIDocument _uiDocument;
    private VisualElement _uiBody;
    private Label _playerIndex;
    
    private void Start()
    {
        _uiDocument = GetComponent<UIDocument>();
        _uiBody = _uiDocument.rootVisualElement;
        _playerIndex = _uiBody.Q<Label>("player-index");
    }

    private void Update()
    {
        // match UI root's rectangle to camera rectangle
        var camRect = PlayerCam.rect;
        
        var top = Length.Percent(camRect.yMin * 100);
        var left = Length.Percent(camRect.xMin * 100);
        var bottom = Length.Percent((1 - camRect.yMax) * 100);
        var right = Length.Percent((1 - camRect.xMax) * 100);

        _uiBody.style.top = top;
        _uiBody.style.left = left;
        _uiBody.style.bottom = bottom;
        _uiBody.style.right = right;
        
        // update text

        _playerIndex.text = $"P{Player.PlayerIndex + 1}" + (Player.IsIt ? " (IT)" : "");
    }
}