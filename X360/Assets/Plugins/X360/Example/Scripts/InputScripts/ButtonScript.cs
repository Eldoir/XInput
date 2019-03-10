using UnityEngine;
using UnityEngine.UI;

public class ButtonScript : InputScript
{

    [SerializeField]
    private X360.Button button;

    [SerializeField]
    private Image imgComponent;

    [SerializeField]
    private Sprite normalSprite;

    [SerializeField]
    private Sprite pressedSprite;


    protected override void InputUpdate()
    {
        imgComponent.sprite = X360.IsButtonHold(button, playerIndex) ? pressedSprite : normalSprite;
    }

    protected override void InputReset()
    {
        imgComponent.sprite = normalSprite;
    }
}