using System;

namespace DSmithGameCs
{
	public static class Input
	{
		private static float mouseX, mouseY;

		public static void SetMousePos(float mousePosX, float mousePosY)
		{
			Input.mouseX = mousePosX;
			Input.mouseY = mousePosY;
		}

		public static float GetMouseX()
		{
			return mouseX;
		}

		public static float GetMouseY()
		{
			return mouseY;
		}
	}
}

