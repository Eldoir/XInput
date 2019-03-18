#if UNITY_EDITOR || UNITY_STANDALONE
using UnityEngine;
using XInputDotNetPure;
using System.Collections;
using System.Collections.Generic;

namespace XInputInternal
{
    public class XInputController
    {

        private bool playerIndexSet;
        private PlayerIndex playerIndex;

        private GamePadState state;
        private GamePadState prevState;

        private XInput.Direction dPadDirection;
        private XInput.Direction prevDPadDirection;

        private Dictionary<XInput.Trigger, Motor> motors;

        private const float detectDirectionThreshold = 0.1f;
        private static float deadzoneRadius = 0.25f;
        private static float triggerMinValueToConsiderPressedOrReleased = 0.95f;


        public XInputController(int playerIndex)
        {
            this.playerIndex = (PlayerIndex)playerIndex;

            motors = new Dictionary<XInput.Trigger, Motor>()
            {
                { XInput.Trigger.Left, new Motor() },
                { XInput.Trigger.Right, new Motor() }
            };
        }

        public void Update()
        {
            prevState = state;
            state = GamePad.GetState(playerIndex);
            prevDPadDirection = dPadDirection;
            dPadDirection = GetDPadDirection();
        }

        #region Controller methods
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
        #endregion

        #region Button methods
        public bool ButtonHold(XInput.Button button)
        {
            return IsButtonPressedOrReleased(button, ButtonState.Pressed, ButtonState.Pressed);
        }

        public bool ButtonPressed(XInput.Button button)
        {
            return IsButtonPressedOrReleased(button, ButtonState.Released, ButtonState.Pressed);
        }

        public bool ButtonReleased(XInput.Button button)
        {
            return IsButtonPressedOrReleased(button, ButtonState.Pressed, ButtonState.Released);
        }

        private bool IsButtonPressedOrReleased(XInput.Button button, ButtonState prevButtonState, ButtonState buttonState)
        {
            switch (button)
            {
                case XInput.Button.A:
                    return prevState.Buttons.A == prevButtonState && state.Buttons.A == buttonState;
                case XInput.Button.B:
                    return prevState.Buttons.B == prevButtonState && state.Buttons.B == buttonState;
                case XInput.Button.Back:
                    return prevState.Buttons.Back == prevButtonState && state.Buttons.Back == buttonState;
                case XInput.Button.Guide:
                    return prevState.Buttons.Guide == prevButtonState && state.Buttons.Guide == buttonState;
                case XInput.Button.LeftShoulder:
                    return prevState.Buttons.LeftShoulder == prevButtonState && state.Buttons.LeftShoulder == buttonState;
                case XInput.Button.LeftStick:
                    return prevState.Buttons.LeftStick == prevButtonState && state.Buttons.LeftStick == buttonState;
                case XInput.Button.RightShoulder:
                    return prevState.Buttons.RightShoulder == prevButtonState && state.Buttons.RightShoulder == buttonState;
                case XInput.Button.RightStick:
                    return prevState.Buttons.RightStick == prevButtonState && state.Buttons.RightStick == buttonState;
                case XInput.Button.Start:
                    return prevState.Buttons.Start == prevButtonState && state.Buttons.Start == buttonState;
                case XInput.Button.X:
                    return prevState.Buttons.X == prevButtonState && state.Buttons.X == buttonState;
                case XInput.Button.Y:
                    return prevState.Buttons.Y == prevButtonState && state.Buttons.Y == buttonState;
                case XInput.Button.DPadUp:
                    return prevState.DPad.Up == prevButtonState && state.DPad.Up == buttonState;
                case XInput.Button.DPadDown:
                    return prevState.DPad.Down == prevButtonState && state.DPad.Down == buttonState;
                case XInput.Button.DPadLeft:
                    return prevState.DPad.Left == prevButtonState && state.DPad.Left == buttonState;
                case XInput.Button.DPadRight:
                    return prevState.DPad.Right == prevButtonState && state.DPad.Right == buttonState;
                default:
                    return false;
            }
        }
        #endregion

        #region Stick methods
        public Vector2 GetStickDirection(XInput.Stick stick)
        {
            if (stick == XInput.Stick.Left)
            {
                return new Vector2(state.ThumbSticks.Left.X, state.ThumbSticks.Left.Y);
            }
            else if (stick == XInput.Stick.Right)
            {
                return new Vector2(state.ThumbSticks.Right.X, state.ThumbSticks.Right.Y);
            }

            return Vector2.zero;
        }

        public XInput.Direction StickDirectionChanged(XInput.Stick stick)
        {
            if (StickInDeadZone(prevState, stick) && !StickInDeadZone(state, stick))
            {
                float deltaX = 0, deltaY = 0;

                if (stick == XInput.Stick.Left)
                {
                    deltaX = state.ThumbSticks.Left.X - prevState.ThumbSticks.Left.X;
                    deltaY = state.ThumbSticks.Left.Y - prevState.ThumbSticks.Left.Y;
                }
                else if (stick == XInput.Stick.Right)
                {
                    deltaX = state.ThumbSticks.Right.X - prevState.ThumbSticks.Right.X;
                    deltaY = state.ThumbSticks.Right.Y - prevState.ThumbSticks.Right.Y;
                }

                if (Mathf.Abs(deltaX) < detectDirectionThreshold && Mathf.Abs(deltaY) > detectDirectionThreshold) // Vertical
                {
                    return deltaY > 0 ? XInput.Direction.Up : XInput.Direction.Down;
                }
                else if (Mathf.Abs(deltaX) > detectDirectionThreshold && Mathf.Abs(deltaY) < detectDirectionThreshold) // Horizontal
                {
                    return deltaX > 0 ? XInput.Direction.Right : XInput.Direction.Left;
                }
                else if (deltaX > detectDirectionThreshold)
                {
                    if (deltaY > detectDirectionThreshold)
                    {
                        return XInput.Direction.UpRight;
                    }
                    else if (deltaY < detectDirectionThreshold)
                    {
                        return XInput.Direction.DownRight;
                    }
                }
                else if (deltaX < detectDirectionThreshold)
                {
                    if (deltaY > detectDirectionThreshold)
                    {
                        return XInput.Direction.UpLeft;
                    }
                    else if (deltaY < detectDirectionThreshold)
                    {
                        return XInput.Direction.DownLeft;
                    }
                }
            }

            return XInput.Direction.None;
        }

        public bool StickReleased(XInput.Stick stick)
        {
            return !StickInDeadZone(prevState, stick) && StickInDeadZone(state, stick);
        }

        public bool StickInDeadZone(XInput.Stick stick)
        {
            return StickInDeadZone(state, stick);
        }

        private bool StickInDeadZone(GamePadState state, XInput.Stick stick)
        {
            Vector2 stickDir = Vector2.zero;

            if (stick == XInput.Stick.Left)
            {
                stickDir = new Vector2(state.ThumbSticks.Left.X, state.ThumbSticks.Left.Y);
            }
            else if (stick == XInput.Stick.Right)
            {
                stickDir = new Vector2(state.ThumbSticks.Right.X, state.ThumbSticks.Right.Y);
            }

            return stickDir.magnitude < deadzoneRadius;
        }

        public static void SetDeadZoneRadius(float newRadius)
        {
            newRadius = Mathf.Clamp(newRadius, 0f, 1f);
            deadzoneRadius = newRadius;

            if (deadzoneRadius == 0f)
            {
                Debug.LogWarning("Please be aware that setting the deadzone radius to 0 will make it no longer detecting whether a stick has changed direction.");
            }
        }
        #endregion

        #region Trigger methods
        public float GetTriggerValue(XInput.Trigger trigger)
        {
            if (trigger == XInput.Trigger.Left)
            {
                return state.Triggers.Left;
            }
            else if (trigger == XInput.Trigger.Right)
            {
                return state.Triggers.Right;
            }

            return 0f;
        }

        public bool TriggerPressed(XInput.Trigger trigger)
        {
            if (trigger == XInput.Trigger.Left)
            {
                return state.Triggers.Left >= triggerMinValueToConsiderPressedOrReleased && prevState.Triggers.Left < triggerMinValueToConsiderPressedOrReleased;
            }
            else if (trigger == XInput.Trigger.Right)
            {
                return state.Triggers.Right >= triggerMinValueToConsiderPressedOrReleased && prevState.Triggers.Right < triggerMinValueToConsiderPressedOrReleased;
            }

            return false;
        }

        public bool TriggerReleased(XInput.Trigger trigger)
        {
            if (trigger == XInput.Trigger.Left)
            {
                return state.Triggers.Left < triggerMinValueToConsiderPressedOrReleased && prevState.Triggers.Left >= triggerMinValueToConsiderPressedOrReleased;
            }
            else if (trigger == XInput.Trigger.Right)
            {
                return state.Triggers.Right < triggerMinValueToConsiderPressedOrReleased && prevState.Triggers.Right >= triggerMinValueToConsiderPressedOrReleased;
            }

            return false;
        }

        public static void SetTriggerMinValueToConsiderPressedOrReleased(float newValue)
        {
            newValue = Mathf.Clamp(newValue, 0f, 1f);
            triggerMinValueToConsiderPressedOrReleased = newValue;
        }
        #endregion

        #region DPad methods
        public XInput.Direction GetDPadDirection()
        {
            if (ButtonHold(XInput.Button.DPadUp))
            {
                if (ButtonHold(XInput.Button.DPadLeft)) return XInput.Direction.UpLeft;
                else if (ButtonHold(XInput.Button.DPadRight)) return XInput.Direction.UpRight;
                else return XInput.Direction.Up;
            }
            else if (ButtonHold(XInput.Button.DPadDown))
            {
                if (ButtonHold(XInput.Button.DPadLeft)) return XInput.Direction.DownLeft;
                else if (ButtonHold(XInput.Button.DPadRight)) return XInput.Direction.DownRight;
                else return XInput.Direction.Down;
            }
            else if (ButtonHold(XInput.Button.DPadLeft))
            {
                return XInput.Direction.Left;
            }
            else if (ButtonHold(XInput.Button.DPadRight))
            {
                return XInput.Direction.Right;
            }

            return XInput.Direction.None;
        }

        public XInput.Direction DPadDirectionChanged()
        {
            if (prevDPadDirection != dPadDirection)
            {
                return dPadDirection;
            }

            return XInput.Direction.None;
        }

        public bool DPadReleased()
        {
            return prevDPadDirection != XInput.Direction.None && dPadDirection == XInput.Direction.None;
        }
        #endregion

        #region Vibration methods
        public bool CanVibrate(XInput.Trigger trigger)
        {
            return motors[trigger].CanVibrate;
        }

        public IEnumerator SetVibrationCorout(XInput.Trigger trigger, float value, float duration)
        {
            SetVibration(trigger, value);

            yield return new WaitForSeconds(duration);

            motors[trigger].FreeCorout();

            if (motors[trigger].HasNoCorout)
            {
                StopVibration(trigger);
            }
        }

        public void StopVibration(XInput.Trigger trigger)
        {
            SetVibration(trigger, 0f);
        }

        public void SetVibration(XInput.Trigger trigger, float value)
        {
            motors[trigger].SetVibration(value);
            GamePad.SetVibration(playerIndex, motors[XInput.Trigger.Left].Value, motors[XInput.Trigger.Right].Value);
        }
        #endregion
    }

    public class Motor
    {
        private int nbCoroutVibrating;
        private bool isVibrating;
        private float value;


        public bool HasNoCorout { get { return nbCoroutVibrating == 0; } }
        public bool CanVibrate { get { return !isVibrating; } }
        public float Value { get { return value; } }


        public void SetVibration(float value)
        {
            this.value = value;

            isVibrating = value != 0;

            if (value != 0)
            {
                nbCoroutVibrating++;
            }
        }

        public void FreeCorout()
        {
            nbCoroutVibrating--;
        }
    }
}
#endif