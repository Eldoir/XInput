using UnityEngine;
using UnityEngine.UI;

public class ButtonScript : InputScript
{

    #pragma warning disable 0649
    [SerializeField]
    private XInput.Button button;

    [SerializeField]
    private Image imgComponent;

    [SerializeField]
    private Sprite normalSprite;

    [SerializeField]
    private Sprite pressedSprite;
    #pragma warning restore 0649


    protected override void InputStart()
    {
        base.InputStart();

        XInput.OnButtonPressed += OnButtonPressed; // We listen to the event
        XInput.OnButtonReleased += OnButtonReleased;
    }

    void OnButtonPressed(XInput.Button button, int playerIndex)
    {
        if (this.button == button && this.playerIndex == playerIndex)
        {
            imgComponent.sprite = pressedSprite;
        }
    }

    void OnButtonReleased(XInput.Button button, int playerIndex)
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
        XInput.OnButtonPressed -= OnButtonPressed; // We're about to be destroyed, we don't need to listen to the event anymore
        XInput.OnButtonReleased -= OnButtonReleased;
    }
}