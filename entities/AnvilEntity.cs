using System;
using OpenTK;

namespace DSmithGameCs
{
	public class AnvilEntity : InteractiveEntity, EntityEventListener, View
	{
		private Vector3 prevEyePos, prevEyeTarget, prevEyeUp;
		private float transition;
		private Smith2DGame game;

		public AnvilEntity(Smith2DGame game, Mesh m, float x, float y, float z, float xSize, float ySize) : base(m, x, y, z, xSize, ySize){
			this.game = game;
			this.eventHandler = this;
		}

		public void InteractionPerformed(InteractiveEntity entity, object source)
		{
			this.transition = 0;
			this.prevEyePos = game.currentView.GetEyePos ();
			this.prevEyeTarget= game.currentView.GetEyeTarget ();
			this.prevEyeUp = game.currentView.GetEyeUp ();
			game.SetView (this);
		}

		public bool ShouldUpdateScene ()
		{
			return false;
		}

		public void UpdateView (Scene s)
		{
			if (transition < 1) {
				transition += Time.delta ();
				transition = Math.Min (1, transition);
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

