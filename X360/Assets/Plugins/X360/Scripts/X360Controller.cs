#if UNITY_EDITOR || UNITY_STANDALONE
using UnityEngine;
using XInputDotNetPure;

namespace X360Internal
{
    public class X360Controller
    {

        private bool playerIndexSet;
        private PlayerIndex playerIndex;
        private GamePadState state;
        private GamePadState prevState;

        private const float detectDirectionThreshold = 0.1f;
        private const float deadzone = 0.25f;


        public X360Controller(int playerIndex)
        {
            this.playerIndex = (PlayerIndex)playerIndex;
        }

        public void Update()
        {
            prevState = state;
            state = GamePad.GetState(playerIndex);
        }

        public bool IsConnected()
        {
            return state.IsConnected;
        }

        public bool JustConnected()
        {
            return state.IsConnected && !prevState.IsConnected;
        }

        public bool JustDisconnected()
        {
            return !state.IsConnected && prevState.IsConnected;
        }

        public Vector2 GetStickDirection(X360.Stick stick)
        {
            if (stick == X360.Stick.Left)
            {
                return new Vector2(state.ThumbSticks.Left.X, state.ThumbSticks.Left.Y);
            }
            else if (stick == X360.Stick.Right)
            {
                return new Vector2(state.ThumbSticks.Right.X, state.ThumbSticks.Right.Y);
            }

            return Vector2.zero;
        }

        public X360.Direction GetDpadDirection()
        {
            if (IsButtonHold(X360.Button.DPadUp))
            {
                if (IsButtonHold(X360.Button.DPadLeft)) return X360.Direction.UpLeft;
                else if (IsButtonHold(X360.Button.DPadRight)) return X360.Direction.UpRight;
                else return X360.Direction.Up;
            }
            else if (IsButtonHold(X360.Button.DPadDown))
            {
                if (IsButtonHold(X360.Button.DPadLeft)) return X360.Direction.DownLeft;
                else if (IsButtonHold(X360.Button.DPadRight)) return X360.Direction.DownRight;
                else return X360.Direction.Down;
            }
            else if (IsButtonHold(X360.Button.DPadLeft))
            {
                return X360.Direction.Left;
            }
            else if (IsButtonHold(X360.Button.DPadRight))
            {
                return X360.Direction.Right;
            }

            return X360.Direction.None;
        }

        public float GetTriggerValue(X360.Trigger trigger)
        {
            if (trigger == X360.Trigger.Left)
            {
                return state.Triggers.Left;
            }
            else if (trigger == X360.Trigger.Right)
            {
                return state.Triggers.Right;
            }

            return 0f;
        }

        public X360.Direction StickDirectionChanged(X360.Stick stick)
        {
            if (StickInDeadZone(prevState, stick) && !StickInDeadZone(state, stick))
            {
                float deltaX = 0, deltaY = 0;

                if (stick == X360.Stick.Left)
                {
                    deltaX = state.ThumbSticks.Left.X - prevState.ThumbSticks.Left.X;
                    deltaY = state.ThumbSticks.Left.Y - prevState.ThumbSticks.Left.Y;
                }
                else if (stick == X360.Stick.Right)
                {
                    deltaX = state.ThumbSticks.Right.X - prevState.ThumbSticks.Right.X;
                    deltaY = state.ThumbSticks.Right.Y - prevState.ThumbSticks.Right.Y;
                }

                if (Mathf.Abs(deltaX) < detectDirectionThreshold && Mathf.Abs(deltaY) > detectDirectionThreshold) // Vertical
                {
                    return deltaY > 0 ? X360.Direction.Up : X360.Direction.Down;
                }
                else if (Mathf.Abs(deltaX) > detectDirectionThreshold && Mathf.Abs(deltaY) < detectDirectionThreshold) // Horizontal
                {
                    return deltaX > 0 ? X360.Direction.Right : X360.Direction.Left;
                }
                else if (deltaX > detectDirectionThreshold)
                {
                    if (deltaY > detectDirectionThreshold)
                    {
                        return X360.Direction.UpRight;
                    }
                    else if (deltaY < detectDirectionThreshold)
                    {
                        return X360.Direction.DownRight;
                    }
                }
                else if (deltaX < detectDirectionThreshold)
                {
                    if (deltaY > detectDirectionThreshold)
                    {
                        return X360.Direction.UpLeft;
                    }
                    else if (deltaY < detectDirectionThreshold)
                    {
                        return X360.Direction.DownLeft;
                    }
                }
            }

            return X360.Direction.None;
        }

        public bool StickInDeadZone(X360.Stick stick)
        {
            return StickInDeadZone(state, stick);
        }

        private bool StickInDeadZone(GamePadState state, X360.Stick stick)
        {
            Vector2 stickDir = Vector2.zero;

            if (stick == X360.Stick.Left)
            {
                stickDir = new Vector2(state.ThumbSticks.Left.X, state.ThumbSticks.Left.Y);
            }
            else if (stick == X360.Stick.Right)
            {
                stickDir = new Vector2(state.ThumbSticks.Right.X, state.ThumbSticks.Right.Y);
            }

            return stickDir.magnitude < deadzone;
        }

        public bool IsButtonHold(X360.Button button)
        {
            return IsButtonPressedOrReleased(button, ButtonState.Pressed, ButtonState.Pressed);
        }

        public bool IsButtonPressed(X360.Button button)
        {
            return IsButtonPressedOrReleased(button, ButtonState.Released, ButtonState.Pressed);
        }

        public bool IsButtonReleased(X360.Button button)
        {
            return IsButtonPressedOrReleased(button, ButtonState.Pressed, ButtonState.Released);
        }

        private bool IsButtonPressedOrReleased(X360.Button button, ButtonState prevButtonState, ButtonState buttonState)
        {
            switch (button)
            {
                case X360.Button.A:
                    return prevState.Buttons.A == prevButtonState && state.Buttons.A == buttonState;
                case X360.Button.B:
                    return prevState.Buttons.B == prevButtonState && state.Buttons.B == buttonState;
                case X360.Button.Back:
                    return prevState.Buttons.Back == prevButtonState && state.Buttons.Back == buttonState;
                case X360.Button.Guide:
                    return prevState.Buttons.Guide == prevButtonState && state.Buttons.Guide == buttonState;
                case X360.Button.LeftShoulder:
                    return prevState.Buttons.LeftShoulder == prevButtonState && state.Buttons.LeftShoulder == buttonState;
                case X360.Button.LeftStick:
                    return prevState.Buttons.LeftStick == prevButtonState && state.Buttons.LeftStick == buttonState;
                case X360.Button.RightShoulder:
                    return prevState.Buttons.RightShoulder == prevButtonState && state.Buttons.RightShoulder == buttonState;
                case X360.Button.RightStick:
                    return prevState.Buttons.RightStick == prevButtonState && state.Buttons.RightStick == buttonState;
                case X360.Button.Start:
                    return prevState.Buttons.Start == prevButtonState && state.Buttons.Start == buttonState;
                case X360.Button.X:
                    return prevState.Buttons.X == prevButtonState && state.Buttons.X == buttonState;
                case X360.Button.Y:
                    return prevState.Buttons.Y == prevButtonState && state.Buttons.Y == buttonState;
                case X360.Button.DPadUp:
                    return prevState.DPad.Up == prevButtonState && state.DPad.Up == buttonState;
                case X360.Button.DPadDown:
                    return prevState.DPad.Down == prevButtonState && state.DPad.Down == buttonState;
                case X360.Button.DPadLeft:
                    return prevState.DPad.Left == prevButtonState && state.DPad.Left == buttonState;
                case X360.Button.DPadRight:
                    return prevState.DPad.Right == prevButtonState && state.DPad.Right == buttonState;
                default:
                    return false;
            }
        }
    }
}
#endif