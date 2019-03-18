using UnityEngine;
using UnityEngine.UI;

public class DPadScript : InputScript
{

    #pragma warning disable 0649
    [SerializeField]
    private DPadDirection[] dPadDirections;
    #pragma warning restore 0649


    protected override void InputUpdate()
    {
        XInput.Direction direction = XInput.GetDPadDirection(playerIndex);

        foreach (DPadDirection dPadDirection in dPadDirections)
        {
            // That way, we only enable the current direction's image
            dPadDirection.EnableImage(dPadDirection.IsConcerned(direction));
        }
    }

    protected override void InputReset()
    {
        foreach (DPadDirection dPadDirection in dPadDirections)
        {
            dPadDirection.EnableImage(false);
        }
    }


    [System.Serializable]
    public class DPadDirection
    {
        #pragma warning disable 0649
        [SerializeField]
        private XInput.Direction direction;

        [SerializeField]
        private Image imgComponent;
        #pragma warning restore 0649


        public bool IsConcerned(XInput.Direction direction)
        {
            return this.direction == direction;
        }

        public void EnableImage(bool enabled)
        {
            imgComponent.enabled = enabled;
        }
    }
}