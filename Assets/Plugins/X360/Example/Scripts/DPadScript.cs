using UnityEngine;
using UnityEngine.UI;

public class DPadScript : MonoBehaviour
{

    [SerializeField]
    private DPadDirection[] dPadDirections;


    void Update()
    {
        X360.Direction direction = X360.GetDpadDirection();

        foreach (DPadDirection dPadDirection in dPadDirections)
        {
            // That way, we only enable the current direction's image
            dPadDirection.EnableImage(dPadDirection.IsConcerned(direction));
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