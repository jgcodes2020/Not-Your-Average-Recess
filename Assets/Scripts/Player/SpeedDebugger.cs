using UnityEngine;
using UnityEngine.UIElements;

public class SpeedDebugger : MonoBehaviour
{
    public UIDocument uiDocument;
    public string labelQuery;

    private Rigidbody _rb;
    private Label _uiLabel;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();

        _uiLabel = uiDocument.rootVisualElement.Q<Label>(labelQuery);
        if (_uiLabel == null)
        {
            Debug.LogError("No UI element!");
        }
    }

    private void Update()
    {
        float hSpeed = new Vector2(_rb.velocity.x, _rb.velocity.z).magnitude;
        float ySpeed = _rb.velocity.y;

        _uiLabel.text =
            $"HSPD {hSpeed}\n" +
            $"YSPD {ySpeed}";
    }
}
