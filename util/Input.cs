using System;
using OpenTK;
using OpenTK.Input;

namespace DSmithGameCs
{
	public static class Input
	{
		public const Key UPKEY = Key.W;
		public const Key DOWNKEY = Key.S;
		public const Key LEFTKEY = Key.A;
		public const Key RIGHTKEY = Key.D;
		public const Key INTERACTKEY = Key.E;
		public const Key POURKEY = Key.R;
		public const Key CLOSEKEY = Key.Escape;
		public static Key[] ITEMKEYS = {Key.Number1, Key.Number2, Key.Number3, Key.Number4};

		public static float RelativeMouseX, RelativeMouseY;
		public static float OrthoMouseX, OrthoMouseY;
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
				case CLOSEKEY:
					closeKeyBuffered = true;
					break;
				case LEFTKEY:
					LeftKey = true;
					break;
				case RIGHTKEY:
					RightKey = true;
					break;
				case UPKEY:
					UpKey = true;
					break;
				case DOWNKEY:
					DownKey = true;
					break;
				case INTERACTKEY:
					interactKeyBuffered = true;
					break;
				default:
					break;
				}
					
				for(int i = 0; i < ITEMKEYS.Length; i++)
					if(e.Key == ITEMKEYS[i])
					{
						bufferedItemKey = i;
						break;
					}
			};

			window.Keyboard.KeyUp += (sender, e) => {
				switch (e.Key) {
				case LEFTKEY:
					LeftKey = false;
					break;
				case RIGHTKEY:
					RightKey = false;
					break;
				case UPKEY:
					UpKey = false;
					break;
				case DOWNKEY:
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
				else if(e.Button == MouseButton.Right)
					interactKeyBuffered = true;
			};

			window.Mouse.ButtonUp += (sender, e) => {
				if (e.Button == MouseButton.Left)
					MouseDown = false;
			};

			window.Mouse.Move += (sender, e) => {
				RelativeMouseX = e.X * 2f / window.Width - 1;
				RelativeMouseY = e.Y * 2f / window.Height - 1;
				OrthoMouseX = (float)e.X / window.Width * OrthoRenderEngine.GetCanvasWidth();
				OrthoMouseY = (1-(float)e.Y / window.Height) * OrthoRenderEngine.GetCanvasHeight();
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
			Input.RelativeMouseX = mousePosX;
			Input.RelativeMouseY = mousePosY;
		}

		public static string GetKeyName(Key key)
		{
			return key.GetType ().GetEnumName (key);
		}
	}
}

