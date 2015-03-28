using System;
using System.Drawing;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Smith2D
{
	public class SmithGameWindow : GameWindow
	{
		readonly Smith2DGame game;
		public SmithGameWindow (Smith2DGame game) : base(1600, 900, new GraphicsMode(32, 24, 0, 8), "Smith")
		{
			this.game = game;
			game.Init ();
			GL.Enable (EnableCap.CullFace);
			GL.CullFace (CullFaceMode.Back);
			GL.Enable (EnableCap.DepthTest);

			Keyboard.KeyDown += (sender, e) => 
			{
				if(e.Key == Key.Escape)
					Close();
			};
		}

		protected override void OnRenderFrame(FrameEventArgs e)
		{
			
			if(e.Time < 0.1f)
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
	}
}

