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
		private static Key[] ITEM_KEYS = {Key.Number1, Key.Number2, Key.Number3, Key.Number4};

		public static float MouseX, MouseY;
		public static bool MouseDown, LeftKey, RightKey, UpKey, DownKey;
		public static bool InteractKeyPressed, CloseKeyPressed;
		public static bool[] ItemKeys = new bool[ITEM_KEYS.Length];
		private static bool closeKeyHasBeenPressed, interactKeyHasBeenPressed;
		private static bool[] pressedItemKeys = new bool[ITEM_KEYS.Length];


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
					LeftKey = true;
					break;
				case RIGHT_KEY:
					RightKey = true;
					break;
				case UP_KEY:
					UpKey = true;
					break;
				case DOWN_KEY:
					DownKey = true;
					break;
				case INTERACT_KEY:
					interactKeyHasBeenPressed = true;
					break;
				default:
					break;
				}
					
				for(uint i = 0; i < ITEM_KEYS.Length; i++)
					if(e.Key == ITEM_KEYS[i])
					{
						pressedItemKeys[i] = true;
						break;
					}
			};

			window.Keyboard.KeyUp += (sender, e) => {
				switch (e.Key) {
				case LEFT_KEY:
					LeftKey = false;
					break;
				case RIGHT_KEY:
					RightKey = false;
					break;
				case UP_KEY:
					UpKey = false;
					break;
				case DOWN_KEY:
					DownKey = false;
					break;
				default:
					break;
				}
			};

			window.Mouse.ButtonDown += (sender, e) => {
				if (e.Button == MouseButton.Left)
					MouseDown = true;
			};

			window.Mouse.ButtonUp += (sender, e) => {
				if (e.Button == MouseButton.Left)
					MouseDown = false;
			};

			window.Mouse.Move += (sender, e) => {
				MouseX = e.X * 2f / window.Width - 1;
				MouseY = e.Y * 2f / window.Height - 1;
			};
		}

		public static void Update ()
		{
			closeKeyHasBeenPressed &= !CloseKeyPressed; //If close key is pressed, set closeKeyhasBeenPressed to false
			interactKeyHasBeenPressed &= !InteractKeyPressed;
			CloseKeyPressed = closeKeyHasBeenPressed;
			InteractKeyPressed = interactKeyHasBeenPressed;
			for (uint i = 0; i < pressedItemKeys.Length; i++) {
				pressedItemKeys[i] &= !ItemKeys [i];
				ItemKeys [i] = pressedItemKeys [i];
			}
		}

		public static void SetMousePos (float mousePosX, float mousePosY)
		{
			Input.MouseX = mousePosX;
			Input.MouseY = mousePosY;
		}
	}
}

