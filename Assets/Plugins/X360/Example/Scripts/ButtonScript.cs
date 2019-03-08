using UnityEngine;
using UnityEngine.UI;

public class ButtonScript : MonoBehaviour
{

    [SerializeField]
    private X360.Button button;

    [SerializeField]
    private Image imgComponent;

    [SerializeField]
    private Sprite normalSprite;

    [SerializeField]
    private Sprite pressedSprite;


    void Update()
    {
        imgComponent.sprite = X360.IsButtonHold(button) ? pressedSprite : normalSprite;
    }
}