using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InputAction : MonoBehaviour
{
    CanvasScaler scaler;
    DefaultInputActions utInput;
    [SerializeField] float zoomSpeed = 100f;
    [SerializeField] float moveSpeed = 500f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        scaler = GetComponent<CanvasScaler>();
        utInput = new DefaultInputActions();
    }

    // Update is called once per frame
    void Update()
    {
        scaler.referenceResolution += Vector2.one * -utInput.UI.ScrollWheel.ReadValue<Vector2>().y * zoomSpeed;

        Vector2 translation = -utInput.UI.Navigate.ReadValue<Vector2>() * moveSpeed * Time.deltaTime;
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
