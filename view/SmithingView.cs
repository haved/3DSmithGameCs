using System;
using OpenTK;

namespace DSmithGameCs
{
	public class SmithingView : View
	{
		Transition transition;
		private readonly Smith2DGame game;

		public SmithingView(Smith2DGame game)
		{
			this.game = game;
			transition = new Transition ();
		}

		public void OnViewUsed(View prevView)
		{
			transition.SetStart (prevView.GetEyePos(), prevView.GetEyeTarget(), prevView.GetEyeUp());
		}

		public bool ShouldUpdateScene ()
		{
			return true;
		}

		public void UpdateView (Scene s)
		{
			transition.UpdateTransition (Time.Delta () * 2);

			if (Input.CloseKeyPressed) {
				game.SetView (new PauseMenuView (game));
			}

			game.GameStats.PlayerInventory.HandleInput ();
		}

		public bool ShouldRenderScene ()
		{
			return true;
		}

		public Vector3 GetEyePos()
		{
			return transition.GetEyePos (new Vector3 (game.Player.pos.X, game.Player.pos.Y - 10, 40));
		}

		public Vector3 GetEyeTarget()
		{
			return transition.GetEyeTarget (new Vector3 (game.Player.pos.X, game.Player.pos.Y, 1.73f));
		}

		public Vector3 GetEyeUp()
		{
			return transition.GetEyeUp (Vector3.UnitY);
		}

		public void RenderView (Scene s)
		{
			game.GameStats.PlayerInventory.Render (game);
		}
	}
}

