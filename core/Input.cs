using System;
using OpenTK;
using OpenTK.Input;

namespace DSmithGameCs
{
	public static class Input
	{
		private const Key UP_KEY = Key.W;
		private const Key DOWN_KEY = Key.S;
		private const Key LEFT_KEY = Key.A;
		private const Key RIGHT_KEY = Key.D;
		private const Key INTERACT_KEY = Key.E;
		private const Key CLOSE_KEY = Key.Escape;

		public static float mouseX, mouseY;
		public static bool mouseDown, leftKey, rightKey, upKey, downKey;
		public static bool interactKeyPressed, closeKeyPressed;
		private static bool closeKeyHasBeenPressed, interactKeyHasBeenPressed;

		public static void AddToWindow (GameWindow window)
		{
			window.KeyDown += (sender, e) => {
				if (e.IsRepeat)
					return;
				
				switch (e.Key) {
				case CLOSE_KEY:
					closeKeyHasBeenPressed = true;
					break;
				case LEFT_KEY:
					leftKey = true;
					break;
				case RIGHT_KEY:
					rightKey = true;
					break;
				case UP_KEY:
					upKey = true;
					break;
				case DOWN_KEY:
					downKey = true;
					break;
				case INTERACT_KEY:
					interactKeyHasBeenPressed = true;
					break;
				default:
					break;
				}
			};

			window.Keyboard.KeyUp += (sender, e) => {
				switch (e.Key) {
				case LEFT_KEY:
					leftKey = false;
					break;
				case RIGHT_KEY:
					rightKey = false;
					break;
				case UP_KEY:
					upKey = false;
					break;
				case DOWN_KEY:
					downKey = false;
					break;
				default:
					break;
				}
			};

			window.Mouse.ButtonDown += (sender, e) => {
				if (e.Button == MouseButton.Left)
					mouseDown = true;
			};

			window.Mouse.ButtonUp += (sender, e) => {
				if (e.Button == MouseButton.Left)
					mouseDown = false;
			};

			window.Mouse.Move += (sender, e) => {
				mouseX = e.X * 2f / window.Width - 1;
				mouseY = e.Y * 2f / window.Height - 1;
			};
		}

		public static void Update ()
		{
			closeKeyHasBeenPressed &= !closeKeyPressed; //If close key is pressed, set closeKeyhasBeenPressed to false
			interactKeyHasBeenPressed &= !interactKeyPressed;
			closeKeyPressed = closeKeyHasBeenPressed;
			interactKeyPressed = interactKeyHasBeenPressed;
		}

		public static void SetMousePos (float mousePosX, float mousePosY)
		{
			Input.mouseX = mousePosX;
			Input.mouseY = mousePosY;
		}
	}
}

