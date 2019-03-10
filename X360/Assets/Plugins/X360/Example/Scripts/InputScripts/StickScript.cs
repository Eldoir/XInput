using UnityEngine;
using UnityEngine.UI;

public class StickScript : InputScript
{

    [SerializeField]
    private RectTransform rectTransform;

    [SerializeField]
    private X360.Stick stick;

    [SerializeField]
    private X360.Button button;

    [SerializeField]
    private Image pressedImg;


    private Vector2 originalPosition;
    private const float amplitude = 15f;


    void Awake()
    {
        originalPosition = rectTransform.anchoredPosition;
    }

    protected override void InputUpdate()
    {
        rectTransform.anchoredPosition = originalPosition + amplitude * X360.GetStickDirection(stick, playerIndex);

        pressedImg.enabled = X360.IsButtonHold(button, playerIndex);
    }

    protected override void InputReset()
    {
        rectTransform.anchoredPosition = originalPosition;

        pressedImg.enabled = false;
    }
}