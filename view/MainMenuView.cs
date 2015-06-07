using System.Drawing;
using OpenTK;

namespace DSmithGameCs
{
	public class MainMenuView : View
	{
		Smith2DGame game;

		public MainMenuView (Smith2DGame game)
		{
			this.game = game;
		}

		#region View implementation

		const string newGame = "New game";
		public void OnViewUsed (View prevView)
		{
			game.TooltipHelper.Writer.Resize (600, 50);
			game.TooltipHelper.Writer.Clear ();
			game.TooltipHelper.Writer.DrawString (newGame, (game.TooltipHelper.Writer.Width - game.TooltipHelper.Writer.GetLineWidth (newGame)) / 2, 10, Color.White);
		}

		public bool ShouldUpdateScene ()
		{
			return false;
		}

		public void UpdateView (Scene s)
		{
			if (Input.MousePressed) {
				float x = Input.OrthoMouseX - OrthoRenderEngine.GetCanvasWidth () / 2;
				float y = Input.OrthoMouseY - OrthoRenderEngine.GetCanvasHeight () / 2 + 70;
				float width = game.TooltipHelper.Writer.Width / 2;
				float height = game.TooltipHelper.Writer.Height / 2;

				if (x > -width & x < width & y > -height & y < height) {
					game.NewGame ();
				}
			}
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
			game.TooltipHelper.RenderNormalDialog ((OrthoRenderEngine.GetCanvasWidth()-game.TooltipHelper.Writer.Width)/2, OrthoRenderEngine.GetCanvasHeight()/2-25-70, Util.White);
		}

		#endregion
	}
}

