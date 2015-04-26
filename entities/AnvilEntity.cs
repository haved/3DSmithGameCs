using System;
using OpenTK;

namespace DSmithGameCs
{
	public class AnvilEntity : InteractiveEntity, EntityEventListener, View
	{
		private readonly Smith2DGame game;

		public AnvilEntity(Smith2DGame game, Mesh m, float x, float y, float z, float xSize, float ySize) : base(m, x, y, z, xSize, ySize){
			this.game = game;
			this.eventHandler = this;
		}

		public void InteractionPerformed(InteractiveEntity entity, object source)
		{
			game.SetView (this);
		}

		//Stuff related to the view
		private Vector3 prevEyePos, prevEyeTarget, prevEyeUp;
		private View prevView;
		private float transition;
		public void OnViewUsed(View prevView)
		{
			this.transition = 0;
			this.prevEyePos = prevView.GetEyePos ();
			this.prevEyeTarget= prevView.GetEyeTarget ();
			this.prevEyeUp = prevView.GetEyeUp ();
			this.prevView = prevView;
		}

		public bool ShouldUpdateScene ()
		{
			return false;
		}

		public void UpdateView (Scene s)
		{
			if (transition < 1) {
				transition += Time.delta ()*2;
				transition = Math.Min (1, transition);
			}

			if (Input.closeKeyPressed) {
				game.SetView (prevView);
			}
		}

		public bool ShouldRenderScene ()
		{
			return true;
		}

		public Vector3 GetEyePos()
		{
			return transition < 1 ? (pos + new Vector3 (0, 0, 10)) * transition + prevEyePos * (1 - transition) : pos + new Vector3 (0, 0, 10);
		}

		public Vector3 GetEyeTarget()
		{
			return transition < 1 ? pos*transition + prevEyeTarget*(1-transition) : pos;
		}

		public Vector3 GetEyeUp()
		{
			return transition < 1 ? Vector3.UnitY*transition+prevEyeUp*(1-transition) : Vector3.UnitY;
		}

		public void RenderView (Scene s)
		{
			
		}
	}
}

