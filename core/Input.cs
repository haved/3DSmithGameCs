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
		public static bool InteractKeyPressed, CloseKeyPressed, MousePressed;
		public static int PressedItemKey = -1;
		private static bool closeKeyBuffered, interactKeyBuffered, mousePressBuffered;
		private static int bufferedItemKey = -1;


		public static void AddToWindow (GameWindow window)
		{
			window.KeyDown += (sender, e) => {
				if (e.IsRepeat)
					return;
				
				switch (e.Key) {
				case CLOSE_KEY:
					closeKeyBuffered = true;
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
					interactKeyBuffered = true;
					break;
				default:
					break;
				}
					
				for(int i = 0; i < ITEM_KEYS.Length; i++)
					if(e.Key == ITEM_KEYS[i])
					{
						bufferedItemKey = i;
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
				{
					MouseDown = true;
					mousePressBuffered = true;
				}
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
			closeKeyBuffered &= !CloseKeyPressed; //If close key is pressed from the previos tick, set closeKeyBuffered to false
			CloseKeyPressed = closeKeyBuffered;

			interactKeyBuffered &= !InteractKeyPressed; //Same as the above
			InteractKeyPressed = interactKeyBuffered;

			mousePressBuffered &= !MousePressed;
			MousePressed = mousePressBuffered;
			if (PressedItemKey != -1)
				bufferedItemKey = -1;
			PressedItemKey = bufferedItemKey;
		}

		public static void SetMousePos (float mousePosX, float mousePosY)
		{
			Input.MouseX = mousePosX;
			Input.MouseY = mousePosY;
		}
	}
}

