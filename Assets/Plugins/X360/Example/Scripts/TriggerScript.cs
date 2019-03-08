using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TriggerScript : MonoBehaviour
{

    [SerializeField]
    private Image fillImage;

    [SerializeField]
    private X360.Trigger trigger;


    private bool isVibrating = false;
    private const float maxFillAmount = 0.88f; // Because the sprite shouldn't be completely filled


    void FixedUpdate()
    {
        // Set a vibration when we hit the maximum trigger value
        if (!isVibrating && X360.GetTriggerValue(trigger) == 1f)
        {
            StartCoroutine(SetVibration(1f, 0.5f));
        }
    }

    void Update()
    {
        fillImage.fillAmount = X360.GetTriggerValue(trigger) * maxFillAmount;
    }

    /// <summary>
    /// Sets a vibration for the given <paramref name="duration"/>. <paramref name="vibration"/> should be between 0 and 1.
    /// </summary>
    IEnumerator SetVibration(float vibration, float duration)
    {
        isVibrating = true;

        float elapsedTime = 0f;
        WaitForEndOfFrame wait = new WaitForEndOfFrame();

        while (elapsedTime < duration)
        {
            if (trigger == X360.Trigger.Left)
            {
                X360.SetVibration(0, vibration, 0);
            }
            else if (trigger == X360.Trigger.Right)
            {
                X360.SetVibration(0, 0, vibration);
            }

            elapsedTime += Time.deltaTime;
            yield return wait;
        }

        X360.ResetVibration(0);

        isVibrating = false;
    }

    private void OnApplicationQuit()
    {
        X360.ResetVibration();
    }
}