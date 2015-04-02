using System;

namespace DSmithGameCs
{
	public class InteractiveEntity : MeshEntity
	{
		protected EntityEventListener eventHandler;

		public InteractiveEntity (EntityEventListener eventHandler, Mesh m, float x, float y, float z, float xSize, float ySize) : base(m, x, y, z, 0, 0, 0, xSize, ySize)
		{
			this.eventHandler = eventHandler;
		}

		protected InteractiveEntity (Mesh m, float x, float y, float z, float xSize, float ySize) : base(m, x, y, z, 0, 0, 0, xSize, ySize){}

		public void Interact(object source)
		{
			eventHandler.InteractionPerformed (this, source);
		}
	}
}

