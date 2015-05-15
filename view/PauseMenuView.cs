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

		public void UpdateView (Scene s)
		{
			if (Input.closeKeyPressed)
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

		Texture t = new Texture("../../res/textures/button.png");
		public void RenderView (Scene s)
		{
			OrthoRenderEngine.DrawColorOnEntireScreen (gradiantColor);
		}

		#endregion
	}
}

