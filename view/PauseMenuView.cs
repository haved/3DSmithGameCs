using OpenTK;
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

		bool hoveringSaveGame, hoveringLoadGame, hoveringExitToMain;
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
			OrthoRenderEngine.DrawColorOnEntireScreen (gradiantColor);
			OrthoRenderEngine.DrawColoredBox (Vector4.UnitW, 100, 0, 200, OrthoRenderEngine.GetCanvasHeight());
			float maxY = OrthoRenderEngine.GetCanvasHeight () * 0.55f;
			float x = Input.OrthoMouseX - 100;
			float y = Input.OrthoMouseY - maxY;
			hoveringSaveGame = x > 0 & x < game.TooltipHelper.Writer.Width & y > 0 & y < game.TooltipHelper.Writer.Height;
			OrthoRenderEngine.DrawColoredTexturedBox (hoveringSaveGame ? Util.White : Util.White60, game.TooltipHelper.Writer.GetTextureID (), 100, maxY, game.TooltipHelper.Writer.Width, game.TooltipHelper.Writer.Height);
		}

		#endregion
	}
}

