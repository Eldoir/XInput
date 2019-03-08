#if UNITY_EDITOR || UNITY_STANDALONE
using UnityEngine;
using XInputDotNetPure;

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

    public delegate void OnButtonPressed(Button button);
    public delegate void OnButtonReleased(Button button);
    public delegate void OnStickDirectionChanged(Stick stick, Direction direction);

    private bool playerIndexSet;
    private PlayerIndex playerIndex;
    private static GamePadState state;
    private static GamePadState prevState;
    private const float detectDirectionThreshold = 0.1f;
    private const float deadzone = 0.25f;

    private bool previousLeftStickInDeadZone;
    private bool previousRightStickInDeadZone;

    public static OnButtonPressed onButtonPressed;
    public static OnButtonReleased onButtonReleased;
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
    }

    void Update()
    {
        FindPlayerIndex();

        prevState = state;
        state = GamePad.GetState(playerIndex);

        foreach (Button button in System.Enum.GetValues(typeof(Button)))
        {
            if (IsButtonPressed(button) && onButtonPressed != null)
            {
                onButtonPressed(button);
            }
            else if (IsButtonReleased(button) && onButtonReleased != null)
            {
                onButtonReleased(button);
            }
        }

        if (onStickDirectionChanged != null)
        {
            Direction dirLeftStick = StickDirectionChanged(Stick.Left);

            if (dirLeftStick != Direction.None)
            {
                onStickDirectionChanged(Stick.Left, dirLeftStick);
            }

            Direction dirRightStick = StickDirectionChanged(Stick.Right);

            if (dirRightStick != Direction.None)
            {
                onStickDirectionChanged(Stick.Right, dirRightStick);
            }
        }
    }

    /// <summary>
    /// <paramref name="leftMotor"/> and <paramref name="rightMotor"/> should be between 0 and 1.
    /// </summary>
    public static void SetVibration(int playerIndex, float leftMotor, float rightMotor)
    {
        GamePad.SetVibration((PlayerIndex)playerIndex, leftMotor, rightMotor);
    }

    /// <summary>
    /// Resets the vibration for all controllers.
    /// </summary>
    public static void ResetVibration()
    {
        foreach (var playerIndex in System.Enum.GetValues(typeof(PlayerIndex)))
        {
            ResetVibration((PlayerIndex)playerIndex);
        }
    }

    /// <summary>
    /// Resets the vibration for the controller with the given <paramref name="playerIndex"/>.
    /// </summary>
    public static void ResetVibration(int playerIndex)
    {
        ResetVibration((PlayerIndex)playerIndex);
    }

    /// <summary>
    /// Resets the vibration for the controller with the given <paramref name="playerIndex"/>.
    /// </summary>
    public static void ResetVibration(PlayerIndex playerIndex)
    {
        GamePad.SetVibration(playerIndex, 0, 0);
    }

    /// <summary>
    /// Returns a normalized direction vector for the given <paramref name="stick"/>.
    /// </summary>
    public static Vector2 GetStickDirection(Stick stick)
    {
        if (stick == Stick.Left)
        {
            return new Vector2(state.ThumbSticks.Left.X, state.ThumbSticks.Left.Y);
        }
        else if (stick == Stick.Right)
        {
            return new Vector2(state.ThumbSticks.Right.X, state.ThumbSticks.Right.Y);
        }

        return Vector2.zero;
    }

    /// <summary>
    /// Returns <see cref="Direction.None"/> if no dpad button is currently hold.
    /// </summary>
    public static Direction GetDpadDirection()
    {
        if (IsButtonHold(Button.DPadUp))
        {
            if (IsButtonHold(Button.DPadLeft)) return Direction.UpLeft;
            else if (IsButtonHold(Button.DPadRight)) return Direction.UpRight;
            else return Direction.Up;
        }
        else if (IsButtonHold(Button.DPadDown))
        {
            if (IsButtonHold(Button.DPadLeft)) return Direction.DownLeft;
            else if (IsButtonHold(Button.DPadRight)) return Direction.DownRight;
            else return Direction.Down;
        }
        else if (IsButtonHold(Button.DPadLeft))
        {
            return Direction.Left;
        }
        else if (IsButtonHold(Button.DPadRight))
        {
            return Direction.Right;
        }

        return Direction.None;
    }

    /// <summary>
    /// Returns the value of the given <paramref name="trigger"/>, between 0 and 1.
    /// </summary>
    public static float GetTriggerValue(Trigger trigger)
    {
        if (trigger == Trigger.Left)
        {
            return state.Triggers.Left;
        }
        else if (trigger == Trigger.Right)
        {
            return state.Triggers.Right;
        }

        return 0f;
    }

    /// <summary>
    /// Returns <see cref="Direction.None"/> if the direction has not changed.
    /// </summary>
    public static Direction StickDirectionChanged(Stick stick)
    {
        if (StickInDeadZone(prevState, stick) && !StickInDeadZone(state, stick))
        {
            float deltaX = 0, deltaY = 0;

            if (stick == Stick.Left)
            {
                deltaX = state.ThumbSticks.Left.X - prevState.ThumbSticks.Left.X;
                deltaY = state.ThumbSticks.Left.Y - prevState.ThumbSticks.Left.Y;
            }
            else if (stick == Stick.Right)
            {
                deltaX = state.ThumbSticks.Right.X - prevState.ThumbSticks.Right.X;
                deltaY = state.ThumbSticks.Right.Y - prevState.ThumbSticks.Right.Y;
            }

            if (Mathf.Abs(deltaX) < detectDirectionThreshold && Mathf.Abs(deltaY) > detectDirectionThreshold) // Vertical
            {
                return deltaY > 0 ? Direction.Up : Direction.Down;
            }
            else if (Mathf.Abs(deltaX) > detectDirectionThreshold && Mathf.Abs(deltaY) < detectDirectionThreshold) // Horizontal
            {
                return deltaX > 0 ? Direction.Right : Direction.Left;
            }
            else if (deltaX > detectDirectionThreshold)
            {
                if (deltaY > detectDirectionThreshold)
                {
                    return Direction.UpRight;
                }
                else if (deltaY < detectDirectionThreshold)
                {
                    return Direction.DownRight;
                }
            }
            else if (deltaX < detectDirectionThreshold)
            {
                if (deltaY > detectDirectionThreshold)
                {
                    return Direction.UpLeft;
                }
                else if (deltaY < detectDirectionThreshold)
                {
                    return Direction.DownLeft;
                }
            }
        }

        return Direction.None;
    }

    private static bool StickInDeadZone(GamePadState state, Stick stick)
    {
        Vector2 stickDir = Vector2.zero;

        if (stick == Stick.Left)
        {
            stickDir = new Vector2(state.ThumbSticks.Left.X, state.ThumbSticks.Left.Y);
        }
        else if (stick == Stick.Right)
        {
            stickDir = new Vector2(state.ThumbSticks.Right.X, state.ThumbSticks.Right.Y);
        }

        return stickDir.magnitude < deadzone;
    }

    /// <summary>
    /// Returns true if the button is being hold this frame.
    /// </summary>
    public static bool IsButtonHold(Button button)
    {
        return IsButtonPressedOrReleased(button, ButtonState.Pressed, ButtonState.Pressed);
    }

    /// <summary>
    /// Returns true if the button was pressed this frame.
    /// </summary>
    public static bool IsButtonPressed(Button button)
    {
        return IsButtonPressedOrReleased(button, ButtonState.Released, ButtonState.Pressed);
    }

    /// <summary>
    /// Returns true if the button was released this frame.
    /// </summary>
    public static bool IsButtonReleased(Button button)
    {
        return IsButtonPressedOrReleased(button, ButtonState.Pressed, ButtonState.Released);
    }

    private static bool IsButtonPressedOrReleased(Button button, ButtonState prevButtonState, ButtonState buttonState)
    {
        switch (button)
        {
            case Button.A:
                return prevState.Buttons.A == prevButtonState && state.Buttons.A == buttonState;
            case Button.B:
                return prevState.Buttons.B == prevButtonState && state.Buttons.B == buttonState;
            case Button.Back:
                return prevState.Buttons.Back == prevButtonState && state.Buttons.Back == buttonState;
            case Button.Guide:
                return prevState.Buttons.Guide == prevButtonState && state.Buttons.Guide == buttonState;
            case Button.LeftShoulder:
                return prevState.Buttons.LeftShoulder == prevButtonState && state.Buttons.LeftShoulder == buttonState;
            case Button.LeftStick:
                return prevState.Buttons.LeftStick == prevButtonState && state.Buttons.LeftStick == buttonState;
            case Button.RightShoulder:
                return prevState.Buttons.RightShoulder == prevButtonState && state.Buttons.RightShoulder == buttonState;
            case Button.RightStick:
                return prevState.Buttons.RightStick == prevButtonState && state.Buttons.RightStick == buttonState;
            case Button.Start:
                return prevState.Buttons.Start == prevButtonState && state.Buttons.Start == buttonState;
            case Button.X:
                return prevState.Buttons.X == prevButtonState && state.Buttons.X == buttonState;
            case Button.Y:
                return prevState.Buttons.Y == prevButtonState && state.Buttons.Y == buttonState;
            case Button.DPadUp:
                return prevState.DPad.Up == prevButtonState && state.DPad.Up == buttonState;
            case Button.DPadDown:
                return prevState.DPad.Down == prevButtonState && state.DPad.Down == buttonState;
            case Button.DPadLeft:
                return prevState.DPad.Left == prevButtonState && state.DPad.Left == buttonState;
            case Button.DPadRight:
                return prevState.DPad.Right == prevButtonState && state.DPad.Right == buttonState;
            default:
                return false;
        }
    }

    private void FindPlayerIndex()
    {
        // Find a PlayerIndex, for a single player game
        // Will find the first controller that is connected and use it
        if (!playerIndexSet || !prevState.IsConnected)
        {
            for (int i = 0; i < 4; ++i)
            {
                PlayerIndex testPlayerIndex = (PlayerIndex)i;
                GamePadState testState = GamePad.GetState(testPlayerIndex);
                if (testState.IsConnected)
                {
                    Debug.Log(string.Format("GamePad found {0}", testPlayerIndex));
                    playerIndex = testPlayerIndex;
                    playerIndexSet = true;
                }
            }
        }

    }
}
#endif