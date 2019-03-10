using UnityEngine;
using UnityEngine.UI;

public class DPadScript : InputScript
{

    [SerializeField]
    private DPadDirection[] dPadDirections;


    protected override void InputUpdate()
    {
        X360.Direction direction = X360.GetDpadDirection(playerIndex);

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
        [SerializeField]
        private X360.Direction direction;

        [SerializeField]
        private Image imgComponent;


        public bool IsConcerned(X360.Direction direction)
        {
            return this.direction == direction;
        }

        public void EnableImage(bool enabled)
        {
            imgComponent.enabled = enabled;
        }
    }
}