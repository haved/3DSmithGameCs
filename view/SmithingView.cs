using System;
using OpenTK;

namespace DSmithGameCs
{
	public class SmithingView : View
	{
		private Vector3 prevEyePos, prevEyeTarget, prevEyeUp;
		private float transition;
		private readonly Smith2DGame game;

		public SmithingView(Smith2DGame game)
		{
			this.game = game;
			transition = 1;
		}

		public void OnViewUsed(View prevView)
		{
			prevEyePos = prevView.GetEyePos ();
			prevEyeTarget= prevView.GetEyeTarget ();
			prevEyeUp = prevView.GetEyeUp ();
			transition = 0;
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
			var cW = OrthoRenderEngine.GetCanvasWidth ();
			var cH = OrthoRenderEngine.GetCanvasHeight ();
			//OrthoRenderEngine.DrawTexturedBox (TextureCollection.Button, cW-iconSize-20, cH-iconSize*Inventory.SIZE-20, iconSize, iconSize*Inventory.SIZE, 0, 0, 1, 4);

			uint l=0;
			for (int i = 0; i < game.GameStats.PlayerInventory.GetItemAmount(); i++) {
				Item item = game.GameStats.PlayerInventory.GetItem(i);

				OrthoRenderEngine.DrawTexturedBox (TextureCollection.Button, cW-iconSize-20, cH-iconSize*(l+0.5f)-20, iconSize, iconSize/2, 0, 0, 1, 0.5f);
				OrthoRenderEngine.DrawTexturedBox (TextureCollection.Numbers, cW - iconSize - 20 + 2, cH - iconSize * l - 20 - 12, 10, 10, l / 4f, 0, 0.25f, 1);
				uint oldL = l;
				for (l++; l < oldL + item.GetSize (); l++) {
					OrthoRenderEngine.DrawTexturedBox (TextureCollection.Button, cW-iconSize-20, cH-iconSize*l-20, iconSize, iconSize/2, 0, 0.25f, 1, 0.5f);
					OrthoRenderEngine.DrawTexturedBox (TextureCollection.Button, cW-iconSize-20, cH-iconSize*(l+0.5f)-20, iconSize, iconSize/2, 0, 0.25f, 1, 0.5f);
				}
				OrthoRenderEngine.DrawTexturedBox (TextureCollection.Button, cW-iconSize-20, cH-iconSize*l-20, iconSize, iconSize/2, 0, 0.5f, 1, 0.5f);
				item.Render (cW-iconSize-20, cH-iconSize*l-20, iconSize, iconSize*item.GetSize());
			}

			for (uint i = l; i < Inventory.SIZE; i++) {
				OrthoRenderEngine.DrawTexturedBox (TextureCollection.Button, cW-iconSize-20, cH-iconSize*(i+1)-20, iconSize, iconSize, 0, 0, 1, 1);
				OrthoRenderEngine.DrawTexturedBox (TextureCollection.Numbers, cW - iconSize - 20 + 2, cH - iconSize * i - 20 - 12, 10, 10, i / 4f, 0, 0.25f, 1);
			}
		}
	}
}

