using UnityEngine;

public class ControllerLayout : MonoBehaviour
{
    public enum Corner { TopLeft, TopRight, BottomLeft, BottomRight }

    #pragma warning disable 0649
    [SerializeField]
    private GameObject controllerPrefab;
    #pragma warning restore 0649

    private GameObject[] controllers;
    private RectTransform[] rects;

    private int originalWidth;
    private Vector2 originalScale;


    void Awake()
    {
        originalWidth = Screen.width;
        originalScale = controllerPrefab.GetComponent<RectTransform>().localScale;

        CleanChildren();

        InstantiateControllers();

        UpdateControllersPosition();

        XInput.OnControllerConnected += OnControllerConnected;
        XInput.OnControllerDisconnected += OnControllerDisconnected;
    }

    private void Update()
    {
        UpdateControllersPosition();
        UpdateControllersScale();
    }

    void OnControllerConnected(int playerIndex)
    {
        controllers[playerIndex].GetComponent<ControllerUI>().SetConnected();
    }

    void OnControllerDisconnected(int playerIndex)
    {
        controllers[playerIndex].GetComponent<ControllerUI>().SetDisconnected();
    }

    private void InstantiateControllers()
    {
        controllers = new GameObject[4];
        rects = new RectTransform[4];

        for (int i = 0; i < 4; i++)
        {
            InstantiateController(i);
        }
    }

    private void InstantiateController(int i)
    {
        controllers[i] = Instantiate(controllerPrefab);
        controllers[i].transform.SetParent(transform);
        rects[i] = controllers[i].GetComponent<RectTransform>();
        rects[i].Reset();
        controllers[i].name = "Controller_" + (i + 1);
        controllers[i].GetComponent<ControllerUI>().SetIndex(i);
    }

    private void CleanChildren()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void UpdateControllersPosition()
    {
        for (int i = 0; i < controllers.Length; i++)
        {
            SetCorner(rects[i], (Corner)i);
        }
    }

    private void UpdateControllersScale()
    {
        for (int i = 0; i < controllers.Length; i++)
        {
            rects[i].localScale = originalScale * (Screen.width / (float)originalWidth);
        }
    }

    private void SetCorner(RectTransform rt, Corner corner)
    {
        switch (corner)
        {
            case Corner.TopLeft:
                rt.SetRight(Screen.width / 2);
                rt.SetBottom(Screen.height / 2);
                break;
            case Corner.TopRight:
                rt.SetLeft(Screen.width / 2);
                rt.SetBottom(Screen.height / 2);
                break;
            case Corner.BottomLeft:
                rt.SetRight(Screen.width / 2);
                rt.SetTop(Screen.height / 2);
                break;
            case Corner.BottomRight:
                rt.SetLeft(Screen.width / 2);
                rt.SetTop(Screen.height / 2);
                break;
            default:
                break;
        }
    }
}