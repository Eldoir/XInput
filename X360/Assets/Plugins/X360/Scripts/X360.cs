#if UNITY_EDITOR || UNITY_STANDALONE
using UnityEngine;
using XInputDotNetPure;

/// <summary>
/// <para>Use this class to communicate with the Xbox360 controllers.</para>
/// <para>All the methods can be passed a playerIndex, to communicate with a specific controller.</para>
/// <para>Player 1 has playerIndex 0, Player 2 has playerIndex 1, and so on.</para>
/// </summary>
public class X360 : MonoBehaviour
{
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

    public delegate void OnControllerConnected(int playerIndex);
    public delegate void OnControllerDisconnected(int playerIndex);
    public delegate void OnButtonPressed(Button button, int playerIndex);
    public delegate void OnButtonReleased(Button button, int playerIndex);
    public delegate void OnStickDirectionChanged(Stick stick, Direction direction, int playerIndex);

    private static X360Controller[] controllers;
    private const int maxControllers = 4;

    /// <summary>
    /// Use this to listen to the <see cref="OnControllerConnected"/> event.
    /// </summary>
    public static OnControllerConnected onControllerConnected;
    /// <summary>
    /// Use this to listen to the <see cref="OnControllerDisconnected"/> event.
    /// </summary>
    public static OnControllerDisconnected onControllerDisconnected;
    /// <summary>
    /// Use this to listen to the <see cref="OnButtonPressed"/> event.
    /// </summary>
    public static OnButtonPressed onButtonPressed;
    /// <summary>
    /// Use this to listen to the <see cref="OnButtonReleased"/> event.
    /// </summary>
    public static OnButtonReleased onButtonReleased;
    /// <summary>
    /// Use this to listen to the <see cref="OnStickDirectionChanged"/> event.
    /// </summary>
    public static OnStickDirectionChanged onStickDirectionChanged;

    private static X360 Instance;


    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError(string.Format("Only one X360 Script should exist in the scene. Destroying this one, attached to GameObject \"{0}\"", gameObject.name));
            Destroy(this);
            return;
        }

        Instance = this;

        InitControllers();
    }

    void InitControllers()
    {
        controllers = new X360Controller[maxControllers];

        for (int i = 0; i < maxControllers; i++)
        {
            controllers[i] = new X360Controller(i);
        }
    }

    void Update()
    {
        for (int i = 0; i < maxControllers; i++)
        {
            controllers[i].Update();

            if (controllers[i].JustConnected() && onControllerConnected != null)
            {
                onControllerConnected(i);
            }
            else if (controllers[i].JustDisconnected() && onControllerDisconnected != null)
            {
                onControllerDisconnected(i);
            }

            foreach (Button button in System.Enum.GetValues(typeof(Button)))
            {
                if (IsButtonPressed(button, i) && onButtonPressed != null)
                {
                    onButtonPressed(button, i);
                }
                else if (IsButtonReleased(button, i) && onButtonReleased != null)
                {
                    onButtonReleased(button, i);
                }
            }

            if (onStickDirectionChanged != null)
            {
                Direction dirLeftStick = StickDirectionChanged(Stick.Left, i);

                if (dirLeftStick != Direction.None)
                {
                    onStickDirectionChanged(Stick.Left, dirLeftStick, i);
                }

                Direction dirRightStick = StickDirectionChanged(Stick.Right);

                if (dirRightStick != Direction.None)
                {
                    onStickDirectionChanged(Stick.Right, dirRightStick, i);
                }
            }
        }
    }

    /// <summary>
    /// <paramref name="leftMotor"/> and <paramref name="rightMotor"/> should be between 0 and 1.
    /// </summary>
    public static void SetVibration(float leftMotor, float rightMotor, int playerIndex = 0)
    {
        GamePad.SetVibration((PlayerIndex)playerIndex, leftMotor, rightMotor);
    }

    /// <summary>
    /// Resets the vibration for all controllers.
    /// </summary>
    public static void ResetAllVibrations()
    {
        foreach (var playerIndex in System.Enum.GetValues(typeof(PlayerIndex)))
        {
            ResetVibration((PlayerIndex)playerIndex);
        }
    }

    /// <summary>
    /// Resets the vibration for the controller with the given <paramref name="playerIndex"/>.
    /// </summary>
    public static void ResetVibration(int playerIndex = 0)
    {
        ResetVibration((PlayerIndex)playerIndex);
    }

    /// <summary>
    /// Resets the vibration for the controller with the given <paramref name="playerIndex"/>.
    /// </summary>
    public static void ResetVibration(PlayerIndex playerIndex = PlayerIndex.One)
    {
        GamePad.SetVibration(playerIndex, 0, 0);
    }

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

    /// <summary>
    /// Returns a normalized direction vector for the given <paramref name="stick"/>.
    /// </summary>
    public static Vector2 GetStickDirection(Stick stick, int playerIndex = 0)
    {
        return controllers[playerIndex].GetStickDirection(stick);
    }

    /// <summary>
    /// Returns <see cref="Direction.None"/> if no DPad button is currently hold.
    /// </summary>
    public static Direction GetDpadDirection(int playerIndex = 0)
    {
        return controllers[playerIndex].GetDpadDirection();
    }

    /// <summary>
    /// Returns the value of the given <paramref name="trigger"/>, between 0 and 1.
    /// </summary>
    public static float GetTriggerValue(Trigger trigger, int playerIndex = 0)
    {
        return controllers[playerIndex].GetTriggerValue(trigger);
    }

    /// <summary>
    /// Returns <see cref="Direction.None"/> if the direction has not changed.
    /// </summary>
    public static Direction StickDirectionChanged(Stick stick, int playerIndex = 0)
    {
        return controllers[playerIndex].StickDirectionChanged(stick);
    }

    /// <summary>
    /// Returns whether the given <paramref name="stick"/> is in the dead zone.
    /// </summary>
    public static bool StickInDeadZone(Stick stick, int playerIndex = 0)
    {
        return controllers[playerIndex].StickInDeadZone(stick);
    }

    /// <summary>
    /// Returns true if the button is being hold this frame.
    /// </summary>
    public static bool IsButtonHold(Button button, int playerIndex = 0)
    {
        return controllers[playerIndex].IsButtonHold(button);
    }

    /// <summary>
    /// Returns true if the button was pressed this frame.
    /// </summary>
    public static bool IsButtonPressed(Button button, int playerIndex = 0)
    {
        return controllers[playerIndex].IsButtonPressed(button);
    }

    /// <summary>
    /// Returns true if the button was released this frame.
    /// </summary>
    public static bool IsButtonReleased(Button button, int playerIndex = 0)
    {
        return controllers[playerIndex].IsButtonReleased(button);
    }
}
#endif