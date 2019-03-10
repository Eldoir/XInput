using UnityEngine;

public class ControllerLayout : MonoBehaviour
{
    public enum Corner { TopLeft, TopRight, BottomLeft, BottomRight }

    [SerializeField]
    private GameObject controllerPrefab;

    private GameObject[] controllers;


    void Awake()
    {
        CleanChildren();

        InstantiateControllers();

        UpdateControllersPosition();

        X360.onControllerConnected += OnControllerConnected;
        X360.onControllerDisconnected += OnControllerDisconnected;
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

        for (int i = 0; i < 4; i++)
        {
            InstantiateController(i);
        }
    }

    private void InstantiateController(int i)
    {
        controllers[i] = Instantiate(controllerPrefab);
        controllers[i].transform.SetParent(transform);
        controllers[i].GetComponent<RectTransform>().Reset();
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
            SetCorner(controllers[i].GetComponent<RectTransform>(), (Corner)i);
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