using UnityEngine;
using UnityEngine.UI;

public class StickScript : InputScript
{

    #pragma warning disable 0649
    [SerializeField]
    private RectTransform rectTransform;

    [SerializeField]
    private XInput.Stick stick;

    [SerializeField]
    private XInput.Button button;

    [SerializeField]
    private Image pressedImg;
    #pragma warning restore 0649


    private Vector2 originalPosition;
    private const float amplitude = 15f;


    void Awake()
    {
        originalPosition = rectTransform.anchoredPosition;
    }

    protected override void InputUpdate()
    {
        rectTransform.anchoredPosition = originalPosition + amplitude * XInput.GetStickDirection(stick, playerIndex);

        pressedImg.enabled = XInput.ButtonHold(button, playerIndex);
    }

    protected override void InputReset()
    {
        rectTransform.anchoredPosition = originalPosition;

        pressedImg.enabled = false;
    }
}