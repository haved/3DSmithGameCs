using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Smith2D
{
	public class SmithGameWindow : GameWindow
	{
		public static SmithGameWindow instance;

		Smith2DGame game;
		public SmithGameWindow () : base(1600, 900, new GraphicsMode(32, 24, 0, 8), "Smith")
		{
			game = new Smith2DGame ();
			GL.Enable (EnableCap.CullFace);
			GL.CullFace (CullFaceMode.Back);
			GL.Enable (EnableCap.DepthTest);
		}

		protected override void OnRenderFrame(FrameEventArgs e)
		{
			Time.SetDelta ((float)e.Time);
			Input.SetMousePos (Mouse.X*2f/Width-1, Mouse.Y*2f/Height-1);
			game.Update ();
			GL.ClearColor (Color.Aqua);
			GL.Clear (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			game.Render ();
			SwapBuffers ();
		}

		protected override void OnResize(EventArgs e)
		{
			game.OnResize (Width, Height);
		}

		public static void Main (string[] args)
		{
			using (instance = new SmithGameWindow ())
				instance.Run (60.00);
		}
	}
}

