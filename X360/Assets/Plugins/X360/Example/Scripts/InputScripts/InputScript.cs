using UnityEngine;

public abstract class InputScript : MonoBehaviour
{

    protected bool isListeningToEvents;
    protected int playerIndex;

    protected abstract void InputUpdate();
    protected abstract void InputReset();


    void Update()
    {
        if (isListeningToEvents)
        {
            InputUpdate();
        }
    }

    public void ListenToEvents(bool listen)
    {
        isListeningToEvents = listen;

        if (!listen)
        {
            InputReset();
        }
    }

    public virtual void SetPlayerIndex(int playerIndex)
    {
        this.playerIndex = playerIndex;
    }
}