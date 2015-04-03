using System;
using OpenTK;

namespace DSmithGameCs
{
	public class AnvilEntity : InteractiveEntity, EntityEventListener, View
	{
		private Smith2DGame game;

		public AnvilEntity(Smith2DGame game, Mesh m, float x, float y, float z, float xSize, float ySize) : base(m, x, y, z, xSize, ySize){
			this.game = game;
			this.eventHandler = this;
		}

		public void InteractionPerformed(InteractiveEntity entity, object source)
		{
			game.SetView (this);
		}

		public bool ShouldUpdateScene ()
		{
			return false;
		}

		public void UpdateView (Scene s)
		{
			
		}

		public bool ShouldRenderScene ()
		{
			return true;
		}

		public Matrix4 GetViewMatrix ()
		{
			return Matrix4.LookAt (pos + new Vector3 (0, 0, 10), pos, Vector3.UnitY);
		}

		public void RenderView (Scene s)
		{
			
		}
	}
}

