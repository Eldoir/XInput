using UnityEngine;
using UnityEngine.UI;

public class GuideScript : InputScript
{

    #pragma warning disable 0649
    [SerializeField]
    private Image[] images;
    #pragma warning restore 0649


    protected override void InputReset()
    {
        // Do nothing.
    }

    protected override void InputUpdate()
    {
        // Do nothing.
    }

    public override void SetPlayerIndex(int playerIndex)
    {
        base.SetPlayerIndex(playerIndex);

        UpdatePlayer(playerIndex);
    }

    private void UpdatePlayer(int playerIndex)
    {
        for (int i = 0; i < images.Length; i++)
        {
            images[i].enabled = (i == playerIndex);
        }
    }
}