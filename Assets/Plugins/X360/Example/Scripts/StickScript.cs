using UnityEngine;
using UnityEngine.UI;

public class StickScript : MonoBehaviour
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


    void Start()
    {
        originalPosition = rectTransform.anchoredPosition;
    }

    void Update()
    {
        rectTransform.anchoredPosition = originalPosition + amplitude * X360.GetStickDirection(stick);

        pressedImg.enabled = X360.IsButtonHold(button);
    }
}