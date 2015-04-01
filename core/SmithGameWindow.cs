using System;
using System.Drawing;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace DSmithGameCs
{
	public class SmithGameWindow : GameWindow
	{
		readonly Smith2DGame game;
		public SmithGameWindow (Smith2DGame game) : base(1100, 750, new GraphicsMode(32, 24, 0, 8), "Smith")
		{
			this.game = game;
			game.Init ();
			GL.Enable (EnableCap.CullFace);
			GL.CullFace (CullFaceMode.Back);
			GL.Enable (EnableCap.DepthTest);
			GL.BlendFunc (BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

			Input.AddToWindow (this);
		}

		protected override void OnRenderFrame(FrameEventArgs e)
		{
			if(e.Time < 0.1f)
				Time.SetDelta ((float)e.Time);
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

