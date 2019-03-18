using UnityEngine;

public abstract class InputScript : MonoBehaviour
{

    protected bool isListeningToEvents;
    protected int playerIndex;

    protected abstract void InputReset();


    private void Start()
    {
        InputStart();
    }

    protected virtual void InputStart()
    {
        // Do nothing by default
    }

    void Update()
    {
        if (isListeningToEvents)
        {
            InputUpdate();
        }
    }

    protected virtual void InputUpdate()
    {
        // Do nothing by default
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