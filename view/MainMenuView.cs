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

		public void OnViewUsed (View prevView)
		{
			string newGame = Localization.GetLocalization ("ui.button.newgame");
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
			return true;
		}

		static readonly Vector3 eyePos = new Vector3(1, -3, 6);
		static readonly Vector3 eyeTarget = new Vector3(1, 7, 0);
		static readonly Vector3 eyeUp = Vector3.UnitY;
		public Vector3 GetEyePos ()
		{
			return eyePos;
		}

		public Vector3 GetEyeTarget ()
		{
			return eyeTarget;
		}

		public Vector3 GetEyeUp ()
		{
			return eyeUp;
		}

		static readonly Vector4 diffuse = new Vector4(1,1,1,0.4f);
		public void RenderView (Scene s)
		{
			OrthoRenderEngine.DrawColorOnEntireScreen (diffuse);
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

