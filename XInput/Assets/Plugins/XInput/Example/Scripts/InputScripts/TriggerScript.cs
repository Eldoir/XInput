using UnityEngine;
using UnityEngine.UI;

public class TriggerScript : InputScript
{

    #pragma warning disable 0649
    [SerializeField]
    private Image fillImage;

    [SerializeField]
    private XInput.Trigger trigger;
    #pragma warning restore 0649


    private const float maxFillAmount = 0.88f; // Because the sprite shouldn't be completely filled


    protected override void InputUpdate()
    {
        float triggerValue = XInput.GetTriggerValue(trigger, playerIndex);

        if (triggerValue == 1f) // Set a vibration when we hit the maximum trigger value
        {
            XInput.SetVibration(trigger, 0.5f, 0.5f, playerIndex);
        }

        fillImage.fillAmount = triggerValue * maxFillAmount;
    }

    protected override void InputReset()
    {
        fillImage.fillAmount = 0;
    }

    private void OnApplicationQuit()
    {
        XInput.StopAllVibrations();
    }
}