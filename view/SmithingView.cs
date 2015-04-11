using System;
using OpenTK;

namespace DSmithGameCs
{
	public class SmithingView : View
	{
		private Smith2DGame game;

		public SmithingView(Smith2DGame game)
		{
			this.game = game;
		}

		public bool ShouldUpdateScene ()
		{
			return true;
		}

		public void UpdateView (Scene s)
		{
			
		}

		public bool ShouldRenderScene ()
		{
			return true;
		}

		public Vector3 GetEyePos()
		{
			return new Vector3 (game.player.pos.X, game.player.pos.Y - 10, 40);
		}

		public Vector3 GetEyeTarget()
		{
			return new Vector3 (game.player.pos.X, game.player.pos.Y, 1.73f);
		}

		public Vector3 GetEyeUp()
		{
			return Vector3.UnitY;
		}

		public void RenderView (Scene s)
		{

		}
	}
}

