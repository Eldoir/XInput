using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class ControllerUI : MonoBehaviour
{

    #pragma warning disable 0649
    [SerializeField]
    private Image indexImg;

    [SerializeField]
    private Text indexTxt;

    [SerializeField]
    private Color connectedColor;

    [SerializeField]
    private Color disconnectedColor;
    #pragma warning restore 0649


    private int index;


    void Start()
    {
        SetDisconnected();
    }

    public void SetIndex(int playerIndex)
    {
        indexTxt.text = (playerIndex + 1).ToString(); // Index starts at 0 but we want the UI to start at 1
        SetIndexToInputScripts(playerIndex);
    }

    public void SetConnected()
    {
        SetIndexImgColor(connectedColor);
        ListenToEvents(true);
        SetCanvasGroupAlpha(1f);
    }

    public void SetDisconnected()
    {
        SetIndexImgColor(disconnectedColor);
        ListenToEvents(false);
        SetCanvasGroupAlpha(0.3f);
    }

    private void SetIndexImgColor(Color color)
    {
        indexImg.color = color;
    }

    private void SetIndexToInputScripts(int playerIndex)
    {
        InputScript[] inputScripts = GetInputScripts();

        foreach (InputScript inputScript in inputScripts)
        {
            inputScript.SetPlayerIndex(playerIndex);
        }
    }

    private void ListenToEvents(bool listen)
    {
        InputScript[] inputScripts = GetInputScripts();

        foreach (InputScript inputScript in inputScripts)
        {
            inputScript.ListenToEvents(listen);
        }
    }

    private InputScript[] GetInputScripts()
    {
        return GetComponentsInChildren<InputScript>();
    }

    private void SetCanvasGroupAlpha(float alpha)
    {
        GetComponent<CanvasGroup>().alpha = alpha;
    }
}