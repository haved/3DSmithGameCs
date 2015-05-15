using System;
using OpenTK;

namespace DSmithGameCs
{
	public class SmithingView : View
	{
		private Vector3 prevEyePos, prevEyeTarget, prevEyeUp;
		private float transition;
		private Smith2DGame game;

		public SmithingView(Smith2DGame game)
		{
			this.game = game;
			transition = 1;
		}

		public void OnViewUsed(View prevView)
		{
			this.prevEyePos = prevView.GetEyePos ();
			this.prevEyeTarget= prevView.GetEyeTarget ();
			this.prevEyeUp = prevView.GetEyeUp ();
			this.transition = 0;
		}

		public bool ShouldUpdateScene ()
		{
			return true;
		}

		public void UpdateView (Scene s)
		{
			if (transition < 1) {
				transition += Time.delta ()*2;
				transition = Math.Min (1, transition);
			}

			if (Input.closeKeyPressed) {
				game.SetView (new PauseMenuView (game));
			}
		}

		public bool ShouldRenderScene ()
		{
			return true;
		}

		public Vector3 GetEyePos()
		{
			return transition < 1 ? new Vector3 (game.Player.pos.X, game.Player.pos.Y - 10, 40) * transition + prevEyePos*(1-transition) : new Vector3 (game.Player.pos.X, game.Player.pos.Y - 10, 40);
		}

		public Vector3 GetEyeTarget()
		{
			return transition < 1 ? new Vector3 (game.Player.pos.X, game.Player.pos.Y, 1.73f) * transition + prevEyeTarget*(1-transition) : new Vector3 (game.Player.pos.X, game.Player.pos.Y, 1.73f);
		}

		public Vector3 GetEyeUp()
		{
			return transition < 1 ? Vector3.UnitY * transition + prevEyeUp * (1 - transition) : Vector3.UnitY;
		}

		const uint iconSize = 64;
		public void RenderView (Scene s)
		{
			OrthoRenderEngine.DrawTexturedBox (TextureCollection.Button, OrthoRenderEngine.GetCanvasWidth()-iconSize-20, 20, iconSize, iconSize*Inventory.SIZE, 0, 0, 1, 4);
			for (int i = 0; i < Inventory.SIZE; i++)
				OrthoRenderEngine.DrawTexturedBox (TextureCollection.Numbers, OrthoRenderEngine.GetCanvasWidth () - iconSize - 20 + 2, 20 + iconSize * i + 2, 10, 10, i / 4f, 0, 0.25f, 1);
			uint l=0;
			for (int i = 0; i < game.GameStats.PlayerInventory.GetItemAmount (); i++) {
				Item item = game.GameStats.PlayerInventory.GetItem(i);
				item.Render (OrthoRenderEngine.GetCanvasWidth()-iconSize-20, iconSize*l+20, iconSize, iconSize*item.GetSize());
				l += item.GetSize ();
			}
		}
	}
}

