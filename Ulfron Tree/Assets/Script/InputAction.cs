using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InputAction : MonoBehaviour
{
    CanvasScaler scaler;
    DefaultInputActions utInput;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        scaler = GetComponent<CanvasScaler>();
        utInput = new DefaultInputActions();
    }

    // Update is called once per frame
    void Update()
    {
        scaler.scaleFactor += (utInput.UI.ScrollWheel.ReadValue<Vector2>().y / 10f);

        Vector2 translation = -utInput.UI.Navigate.ReadValue<Vector2>();
        foreach (RectTransform rt in GetComponentsInChildren<RectTransform>())
        {
            if (rt.CompareTag("Case") || rt.CompareTag("Barre"))
                rt.Translate(translation);
        }
    }

    private void OnEnable()
    {
        utInput.UI.ScrollWheel.Enable();
        utInput.UI.Navigate.Enable();
    }

    private void OnDisable()
    {
        utInput.UI.ScrollWheel.Disable();
        utInput.UI.Navigate.Disable();
    }
}
