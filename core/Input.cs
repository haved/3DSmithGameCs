﻿using System;
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

		public static float mouseX, mouseY;
		public static bool mouseDown, leftKey, rightKey, upKey, downKey, interactKey;

		public static void AddToWindow(GameWindow window)
		{
			window.KeyDown += (sender, e) => 
			{
				if(e.IsRepeat)
					return;
				if(e.Key == Key.Escape)
					window.Close();
				else if(e.Key == LEFT_KEY)
					leftKey = true;
				else if(e.Key == RIGHT_KEY)
					rightKey = true;
				else if(e.Key == UP_KEY)
					upKey = true;
				else if(e.Key == DOWN_KEY)
					downKey = true;
				else if(e.Key == INTERACT_KEY)
					interactKey = true;
			};

			window.Keyboard.KeyUp += (sender, e) => 
			{
				if(e.Key == Key.A)
					leftKey = false;
				else if(e.Key == Key.D)
					rightKey = false;
				else if(e.Key == Key.W)
					upKey = false;
				else if(e.Key == Key.S)
					downKey = false;
			};

			window.Mouse.ButtonDown += (sender, e) => {
				if(e.Button == MouseButton.Left)
					mouseDown = true;
			};

			window.Mouse.ButtonUp += (sender, e) => {
				if(e.Button == MouseButton.Left)
					mouseDown = false;
			};

			window.Mouse.Move += (sender, e) => {
				mouseX = e.X*2f/window.Width-1;
				mouseY = e.Y*2f/window.Height-1;
			};
		}

		static bool prevTickInteractKeyDown = false;
		public static void Update()
		{
			if (prevTickInteractKeyDown)
				interactKey = false;
			prevTickInteractKeyDown = interactKey;
		}

		public static void SetMousePos(float mousePosX, float mousePosY)
		{
			Input.mouseX = mousePosX;
			Input.mouseY = mousePosY;
		}
	}
}

