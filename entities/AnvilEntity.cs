using System;
using OpenTK;

namespace DSmithGameCs
{
	public class AnvilEntity : InteractiveEntity, IEntityEventListener, View
	{
		private readonly Smith2DGame game;

		public AnvilEntity(Smith2DGame game, Mesh m, float x, float y, float z, float xSize, float ySize) : base(m, x, y, z, xSize, ySize){
			this.game = game;
			this.EventHandler = this;
		}

		public void InteractionPerformed(InteractiveEntity entity, object source)
		{
			game.SetView (this);
		}

		//Stuff related to the view
		Transition transition = new Transition();
		View prevView;
		public void OnViewUsed(View prevView)
		{
			transition.SetStart (prevView);
			this.prevView = prevView;
		}

		public bool ShouldUpdateScene ()
		{
			return false;
		}

		public void UpdateView (Scene s)
		{
			transition.UpdateTransition (Time.Delta()*2);

			if (Input.CloseKeyPressed) {
				game.SetView (prevView);
			}
		}

		public bool ShouldRenderScene ()
		{
			return true;
		}

		static Vector3 eyeOffset = new Vector3(0, 0, 10);
		public Vector3 GetEyePos()
		{
			return transition.GetEyePos (pos+eyeOffset);
		}

		public Vector3 GetEyeTarget()
		{
			return transition.GetEyeTarget (pos);
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

