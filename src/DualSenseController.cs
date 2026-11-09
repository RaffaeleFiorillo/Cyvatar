using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cyvatar.src
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    public class DualSenseController
    {
        // Dead zone for analog sticks and triggers
        private readonly float deadZone = 0.25f;

        // We always use PlayerIndex.One for the first controller
        private PlayerIndex playerIndex = PlayerIndex.One;

        public DualSenseController()
        {
            // Check if a controller is connected
            if (GamePad.GetState(playerIndex).IsConnected)
                System.Diagnostics.Debug.WriteLine("Controller connected!");
            else
                System.Diagnostics.Debug.WriteLine("No controller detected.");
        }

        /// <summary>
        /// Call this every frame.
        /// Returns a list of currently pressed buttons (as strings).
        /// </summary>
        public List<string> GetPressed()
        {
            var pressed = new List<string>();
            GamePadState state = GamePad.GetState(playerIndex);

            if (!state.IsConnected)
                return pressed;

            // === Face buttons ===
            if (state.Buttons.A == ButtonState.Pressed) pressed.Add("cross");       // ✕
            if (state.Buttons.B == ButtonState.Pressed) pressed.Add("circle");      // ○
            if (state.Buttons.X == ButtonState.Pressed) pressed.Add("square");      // □
            if (state.Buttons.Y == ButtonState.Pressed) pressed.Add("triangle");    // △

            // === Shoulder buttons ===
            if (state.Buttons.LeftShoulder == ButtonState.Pressed) pressed.Add("l1");
            if (state.Buttons.RightShoulder == ButtonState.Pressed) pressed.Add("r1");

            // === Stick clicks ===
            if (state.Buttons.LeftStick == ButtonState.Pressed) pressed.Add("l3");
            if (state.Buttons.RightStick == ButtonState.Pressed) pressed.Add("r3");

            // === Menu buttons ===
            if (state.Buttons.Back == ButtonState.Pressed) pressed.Add("share");     // Create
            if (state.Buttons.Start == ButtonState.Pressed) pressed.Add("options");

            // === D-Pad ===
            if (state.DPad.Up == ButtonState.Pressed) pressed.Add("up");
            if (state.DPad.Down == ButtonState.Pressed) pressed.Add("down");
            if (state.DPad.Left == ButtonState.Pressed) pressed.Add("left");
            if (state.DPad.Right == ButtonState.Pressed) pressed.Add("right");

            // === Triggers as digital buttons (L2 / R2) ===
            if (state.Triggers.Left > deadZone) pressed.Add("l2");
            if (state.Triggers.Right > deadZone) pressed.Add("r2");

            // === Left Stick as digital directions (useful for Battle Network style) ===
            Vector2 leftStick = state.ThumbSticks.Left;

            if (leftStick.X < -deadZone) pressed.Add("left_stick_left");
            else if (leftStick.X > deadZone) pressed.Add("left_stick_right");

            if (leftStick.Y > deadZone) pressed.Add("left_stick_up");      // Note: Y is inverted in MonoGame
            else if (leftStick.Y < -deadZone) pressed.Add("left_stick_down");

            return pressed;
        }

        // Optional: check if a specific button is pressed
        public bool IsPressed(string buttonName)
        {
            return GetPressed().Contains(buttonName);
        }
    }
}
