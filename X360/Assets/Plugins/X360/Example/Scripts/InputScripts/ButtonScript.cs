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


    protected override void InputStart()
    {
        base.InputStart();

        X360.onButtonPressed += OnButtonPressed; // We listen to the event
        X360.onButtonReleased += OnButtonReleased;
    }

    void OnButtonPressed(X360.Button button, int playerIndex)
    {
        if (this.button == button && this.playerIndex == playerIndex)
        {
            imgComponent.sprite = pressedSprite;
        }
    }

    void OnButtonReleased(X360.Button button, int playerIndex)
    {
        if (this.button == button && this.playerIndex == playerIndex)
        {
            imgComponent.sprite = normalSprite;
        }
    }

    protected override void InputReset()
    {
        imgComponent.sprite = normalSprite;
    }

    private void OnDestroy()
    {
        X360.onButtonPressed -= OnButtonPressed; // We're about to be destroyed, we don't need to listen the event anymore
        X360.onButtonReleased -= OnButtonReleased;
    }
}