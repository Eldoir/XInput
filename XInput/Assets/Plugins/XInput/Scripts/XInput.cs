#if UNITY_EDITOR || UNITY_STANDALONE
using System;
using UnityEngine;
using XInputInternal;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// <para>Use this class to communicate with Xbox 360 and Xbox One controllers.</para>
/// <para>All the methods can be passed a playerIndex, to communicate with a specific controller.</para>
/// <para>Player 1 has playerIndex 0, Player 2 has playerIndex 1, and so on.</para>
/// </summary>
public class XInput : MonoBehaviour
{
    #region Enums
    public enum Button
    {
        Start, Back,
        LeftStick, RightStick,
        LeftShoulder, RightShoulder,
        Guide, A, B, X, Y,
        DPadUp, DPadDown, DPadLeft, DPadRight
    }

    public enum Stick
    {
        Left, Right
    }

    public enum Trigger
    {
        Left, Right
    }

    public enum Direction
    {
        None,
        Up, Down,
        Left, Right,
        UpLeft, UpRight,
        DownLeft, DownRight
    }
    #endregion

    #region Events

    #region Controller Events
    /// <summary>
    /// <see cref="int"/> is the playerIndex.
    /// </summary>
    public static Action<int> OnControllerConnected;
    /// <summary>
    /// <see cref="int"/> is the playerIndex.
    /// </summary>
    public static Action<int> OnControllerDisconnected;
    #endregion

    #region Button Events
    /// <summary>
    /// <see cref="Button"/> is the button that was pressed, <see cref="int"/> is the playerIndex.
    /// </summary>
    public static Action<Button, int> OnButtonPressed;
    /// <summary>
    /// <see cref="Button"/> is the button that was released, <see cref="int"/> is the playerIndex.
    /// </summary>
    public static Action<Button, int> OnButtonReleased;
    #endregion

    #region Stick Events
    /// <summary>
    /// <see cref="Stick"/> is the concerned stick, <see cref="Direction"/> is the new direction of the stick, <see cref="int"/> is the playerIndex.
    /// </summary>
    public static Action<Stick, Direction, int> OnStickDirectionChanged;
    /// <summary>
    /// <see cref="Stick"/> is the concerned stick, <see cref="int"/> is the playerIndex.
    /// </summary>
    public static Action<Stick, int> OnStickReleased;
    #endregion

    #region Trigger Events
    /// <summary>
    /// <see cref="Trigger"/> is the trigger that was pressed, <see cref="int"/> is the playerIndex.
    /// </summary>
    public static Action<Trigger, int> OnTriggerPressed;
    /// <summary>
    /// <see cref="Trigger"/> is the trigger that was released, <see cref="int"/> is the playerIndex.
    /// </summary>
    public static Action<Trigger, int> OnTriggerReleased;
    #endregion

    #region DPad Events
    /// <summary>
    /// <see cref="Direction"/> is the new direction of the DPad, <see cref="int"/> is the playerIndex.
    /// </summary>
    public static Action<Direction, int> OnDPadDirectionChanged;
    /// <summary>
    /// <see cref="int"/> is the playerIndex.
    /// </summary>
    public static Action<int> OnDPadReleased;
    #endregion

    #endregion

    #region Private variables
    private static XInputController[] controllers;
    private const int maxControllers = 4;
    private static XInput Instance;
    #endregion


    #if UNITY_EDITOR
    [MenuItem("GameObject/Input/XInput")]

    static void XInputInstance()
    {
        GameObject go = new GameObject("XInput");
        go.AddComponent<XInput>();
        Undo.RegisterCreatedObjectUndo(go, "Create XInput");
    }
    #endif

    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError(string.Format("Only one XInput Script should exist in the scene. Destroying this one, attached to GameObject \"{0}\"", gameObject.name));
            Destroy(this);
            return;
        }

        Instance = this;

        InitControllers();
    }

    void InitControllers()
    {
        controllers = new XInputController[maxControllers];

        for (int i = 0; i < maxControllers; i++)
        {
            controllers[i] = new XInputController(i);
        }
    }

    void Update()
    {
        for (int i = 0; i < maxControllers; i++)
        {
            controllers[i].Update();

            #region Handling Controller Events
            if (OnControllerConnected != null && controllers[i].JustConnected())
            {
                OnControllerConnected(i);
            }
            else if (OnControllerDisconnected != null && controllers[i].JustDisconnected())
            {
                OnControllerDisconnected(i);
            }
            #endregion

            if (!controllers[i].IsConnected())
            {
                continue; // The controller isn't connected, so we don't need to go further to fire input events
            }

            #region Handling Button Events
            foreach (Button button in Enum.GetValues(typeof(Button)))
            {
                if (OnButtonPressed != null && ButtonPressed(button, i))
                {
                    OnButtonPressed(button, i);
                }
                else if (OnButtonReleased != null && ButtonReleased(button, i))
                {
                    OnButtonReleased(button, i);
                }
            }
            #endregion

            #region Handling Stick Events
            if (OnStickDirectionChanged != null)
            {
                Direction dirLeftStick = StickDirectionChanged(Stick.Left, i);

                if (dirLeftStick != Direction.None)
                {
                    OnStickDirectionChanged(Stick.Left, dirLeftStick, i);
                }

                Direction dirRightStick = StickDirectionChanged(Stick.Right);

                if (dirRightStick != Direction.None)
                {
                    OnStickDirectionChanged(Stick.Right, dirRightStick, i);
                }
            }

            if (OnStickReleased != null)
            {
                if (StickReleased(Stick.Left, i))
                {
                    OnStickReleased(Stick.Left, i);
                }

                if (StickReleased(Stick.Right, i))
                {
                    OnStickReleased(Stick.Right, i);
                }
            }
            #endregion

            #region Handling Trigger Events
            if (OnTriggerPressed != null)
            {
                if (TriggerPressed(Trigger.Left, i))
                {
                    OnTriggerPressed(Trigger.Left, i);
                }

                if (TriggerPressed(Trigger.Right, i))
                {
                    OnTriggerPressed(Trigger.Right, i);
                }
            }

            if (OnTriggerReleased != null)
            {
                if (TriggerReleased(Trigger.Left, i))
                {
                    OnTriggerReleased(Trigger.Left, i);
                }

                if (TriggerReleased(Trigger.Right, i))
                {
                    OnTriggerReleased(Trigger.Right, i);
                }
            }
            #endregion

            #region Handling DPad Events
            if (OnDPadDirectionChanged != null)
            {
                Direction dirDPad = DPadDirectionChanged(i);

                if (dirDPad != Direction.None)
                {
                    OnDPadDirectionChanged(dirDPad, i);
                }
            }

            if (OnDPadReleased != null && DPadReleased(i))
            {
                OnDPadReleased(i);
            }
            #endregion
        }
    }

    #region Controller methods
    /// <summary>
    /// Returns true if the controller is currently connected.
    /// </summary>
    public static bool IsControllerConnected(int playerIndex = 0)
    {
        return controllers[playerIndex].IsConnected();
    }

    /// <summary>
    /// Returns true if the controller was connected this frame.
    /// </summary>
    public static bool ControllerConnected(int playerIndex = 0)
    {
        return controllers[playerIndex].JustConnected();
    }

    /// <summary>
    /// Returns true if the controller was disconnected this frame.
    /// </summary>
    public static bool ControllerDisconnected(int playerIndex = 0)
    {
        return controllers[playerIndex].JustDisconnected();
    }
    #endregion

    #region Button methods
    /// <summary>
    /// Returns true if the button is being hold this frame.
    /// </summary>
    public static bool ButtonHold(Button button, int playerIndex = 0)
    {
        return controllers[playerIndex].ButtonHold(button);
    }

    /// <summary>
    /// Returns true if the button was pressed this frame.
    /// </summary>
    public static bool ButtonPressed(Button button, int playerIndex = 0)
    {
        return controllers[playerIndex].ButtonPressed(button);
    }

    /// <summary>
    /// Returns true if the button was released this frame.
    /// </summary>
    public static bool ButtonReleased(Button button, int playerIndex = 0)
    {
        return controllers[playerIndex].ButtonReleased(button);
    }
    #endregion

    #region Stick methods
    /// <summary>
    /// Returns a normalized direction vector for the given <paramref name="stick"/>.
    /// </summary>
    public static Vector2 GetStickDirection(Stick stick, int playerIndex = 0)
    {
        return controllers[playerIndex].GetStickDirection(stick);
    }

    /// <summary>
    /// Returns <see cref="Direction.None"/> if the direction has not changed.
    /// </summary>
    public static Direction StickDirectionChanged(Stick stick, int playerIndex = 0)
    {
        return controllers[playerIndex].StickDirectionChanged(stick);
    }

    /// <summary>
    /// Returns true if the given <paramref name="stick"/> was just released, that is if it was outside of the dead zone at last frame and it's inside at this frame.
    /// </summary>
    public static bool StickReleased(Stick stick, int playerIndex = 0)
    {
        return controllers[playerIndex].StickReleased(stick);
    }

    /// <summary>
    /// Returns whether the given <paramref name="stick"/> is in the dead zone.
    /// </summary>
    public static bool StickInDeadZone(Stick stick, int playerIndex = 0)
    {
        return controllers[playerIndex].StickInDeadZone(stick);
    }

    /// <summary>
    /// The param <paramref name="newRadius"/> must be between [0-1]. If it's out of bounds, it will be clamped anyway.
    /// </summary>
    public static void SetDeadZoneRadius(float newRadius)
    {
        XInputController.SetDeadZoneRadius(newRadius);
    }
    #endregion

    #region Trigger methods
    /// <summary>
    /// Returns the value of the given <paramref name="trigger"/>, between 0 and 1.
    /// </summary>
    public static float GetTriggerValue(Trigger trigger, int playerIndex = 0)
    {
        return controllers[playerIndex].GetTriggerValue(trigger);
    }

    /// <summary>
    /// <para>Returns true if the trigger was just pressed this frame.</para>
    /// <para>Uses a default specific value to determine if the trigger was pressed.</para>
    /// <para>You can call <see cref="SetTriggerMinValueToConsiderPressedOrReleased(float)"/> if you want to redefine that value.</para>
    /// </summary>
    public static bool TriggerPressed(Trigger trigger, int playerIndex = 0)
    {
        return controllers[playerIndex].TriggerPressed(trigger);
    }

    /// <summary>
    /// <para>Returns true if the trigger was just released this frame.</para>
    /// <para>Uses a default specific value to determine if the trigger was released.</para>
    /// <para>You can call <see cref="SetTriggerMinValueToConsiderPressedOrReleased(float)"/> if you want to redefine that value.</para>
    /// </summary>
    public static bool TriggerReleased(Trigger trigger, int playerIndex = 0)
    {
        return controllers[playerIndex].TriggerReleased(trigger);
    }

    /// <summary>
    /// The param <paramref name="newValue"/> must be between [0-1]. If it's out of bounds, it will be clamped anyway.
    /// </summary>
    public static void SetTriggerMinValueToConsiderPressedOrReleased(float newValue)
    {
        XInputController.SetTriggerMinValueToConsiderPressedOrReleased(newValue);
    }
    #endregion

    #region DPad methods
    /// <summary>
    /// Returns <see cref="Direction.None"/> if no DPad button is currently hold.
    /// </summary>
    public static Direction GetDPadDirection(int playerIndex = 0)
    {
        return controllers[playerIndex].GetDPadDirection();
    }

    /// <summary>
    /// Returns <see cref="Direction.None"/> if the direction has not changed.
    /// </summary>
    public static Direction DPadDirectionChanged(int playerIndex = 0)
    {
        return controllers[playerIndex].DPadDirectionChanged();
    }

    /// <summary>
    /// Returns true if the DPad was just released, that is if it had a certain direction at last frame and it's not the case at this frame anymore.
    /// </summary>
    public static bool DPadReleased(int playerIndex = 0)
    {
        return controllers[playerIndex].DPadReleased();
    }
    #endregion

    #region Vibration methods
    /// <summary>
    /// Sets a vibration on both triggers for the given <paramref name="duration"/>.
    /// </summary>
    /// <param name="power">Should be between 0 and 1. If it's out of bounds, it will be clamped anyway.</param>
    public static void SetVibration(float power, float duration, int playerIndex = 0)
    {
        SetVibration(Trigger.Left, power, duration, playerIndex);
        SetVibration(Trigger.Right, power, duration, playerIndex);
    }

    /// <summary>
    /// Sets a vibration on the given <paramref name="trigger"/> for the given <paramref name="duration"/>.
    /// </summary>
    /// <param name="power">Should be between 0 and 1. If it's out of bounds, it will be clamped anyway.</param>
    public static void SetVibration(Trigger trigger, float power, float duration, int playerIndex = 0)
    {
        if (controllers[playerIndex].CanVibrate(trigger))
        {
            Instance.StartCoroutine(controllers[playerIndex].SetVibrationCorout(trigger, power, duration));
        }
    }

    /// <summary>
    /// Stops the vibration for all controllers.
    /// </summary>
    public static void StopAllVibrations()
    {
        for (int i = 0; i < controllers.Length; i++)
        {
            StopVibration(i);
        }
    }

    /// <summary>
    /// Stops the vibration for both triggers for the controller with the given <paramref name="playerIndex"/>.
    /// </summary>
    public static void StopVibration(int playerIndex = 0)
    {
        StopVibration(Trigger.Left, playerIndex);
        StopVibration(Trigger.Right, playerIndex);
    }

    /// <summary>
    /// Stops the vibration for the given <paramref name="trigger"/> for the controller with the given <paramref name="playerIndex"/>.
    /// </summary>
    public static void StopVibration(Trigger trigger, int playerIndex = 0)
    {
        controllers[playerIndex].StopVibration(trigger);
    }
    #endregion
}
#endif
