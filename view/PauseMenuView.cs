﻿using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace DSmithGameCs
{
	public class PauseMenuView : View
	{
		static Vector4 gradiantColor = new Vector4(0, 0, 0, 0.7f);
		readonly Smith2DGame game;
		View prevView;

		public PauseMenuView(Smith2DGame game)
		{
			this.game = game;
		}

		#region View implementation

		public void OnViewUsed (View prevView)
		{
			this.prevView = prevView;
		}

		public bool ShouldUpdateScene ()
		{
			return false;
		}

		public void UpdateView (Scene s)
		{
			if (Input.CloseKeyPressed)
				game.SetView (prevView);
		}

		public bool ShouldRenderScene ()
		{
			return true;
		}

		public Vector3 GetEyePos ()
		{
			return prevView.GetEyePos ();
		}

		public Vector3 GetEyeTarget ()
		{
			return prevView.GetEyeTarget ();
		}

		public Vector3 GetEyeUp ()
		{
			return prevView.GetEyeUp ();
		}

		public void RenderView (Scene s)
		{
			prevView.RenderView (s);
			GL.Disable (EnableCap.DepthTest);
			OrthoRenderEngine.DrawColorOnEntireScreen (gradiantColor);
			GL.Enable (EnableCap.DepthTest);
		}

		#endregion
	}
}

