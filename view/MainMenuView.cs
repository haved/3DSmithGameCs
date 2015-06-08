using System.Drawing;
using OpenTK;

namespace DSmithGameCs
{
	public class MainMenuView : View
	{
		readonly Smith2DGame game;

		public MainMenuView (Smith2DGame game)
		{
			this.game = game;
		}

		#region View implementation

		const string newGame = "New game";
		public void OnViewUsed (View prevView)
		{
			game.TooltipHelper.Writer.Resize (200, 30);
			game.TooltipHelper.Writer.Clear ();
			game.TooltipHelper.Writer.DrawString (newGame, (game.TooltipHelper.Writer.Width - game.TooltipHelper.Writer.GetLineWidth (newGame)) / 2, 0, Color.White);
		}

		public bool ShouldUpdateScene ()
		{
			return false;
		}

		bool hoveringNewGame = false;
		public void UpdateView (Scene s)
		{
			if(Input.MousePressed & hoveringNewGame)
				game.NewGame();
		}

		public bool ShouldRenderScene ()
		{
			return false;
		}

		public Vector3 GetEyePos ()
		{
			return Vector3.Zero;
		}

		public Vector3 GetEyeTarget ()
		{
			return Vector3.Zero;
		}

		public Vector3 GetEyeUp ()
		{
			return Vector3.Zero;
		}

		public void RenderView (Scene s)
		{
			OrthoRenderEngine.DrawColoredBox (Vector4.UnitW, 100, 0, 200, OrthoRenderEngine.GetCanvasHeight());
			float maxY = OrthoRenderEngine.GetCanvasHeight () * 0.55f;
			float x = Input.OrthoMouseX - 100;
			float y = Input.OrthoMouseY - maxY;
			hoveringNewGame = x > 0 & x < game.TooltipHelper.Writer.Width & y > 0 & y < game.TooltipHelper.Writer.Height;
			OrthoRenderEngine.DrawColoredTexturedBox (hoveringNewGame ? Util.White : Util.White60, game.TooltipHelper.Writer.GetTextureID (), 100, maxY, game.TooltipHelper.Writer.Width, game.TooltipHelper.Writer.Height);
		}

		#endregion
	}
}

