using System;
using OpenTK;

namespace DSmithGameCs
{
	public static class DialogRenderer
	{
		public static Texture DialogBG {
			get { return TextureCollection.DialogBG; }
		}

		static Vector4 color = new Vector4(1, 1, 1, 0.7f);
		public static void DrawDialogBox(float x, float y, float width, float height)
		{
			OrthoRenderEngine.DrawExtendedColoredTexturedBox (DialogBG, color, x, y, width, height);
		}
	}
}

