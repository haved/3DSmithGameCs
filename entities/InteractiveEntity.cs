using System;
using OpenTK;

namespace DSmithGameCs
{
	public class InteractiveEntity : MeshEntity
	{
		protected IEntityEventListener EventHandler;

		public InteractiveEntity (IEntityEventListener eventHandler, Mesh m, float x, float y, float z, float xSize, float ySize) : base(m, x, y, z, 0, 0, 0, xSize, ySize)
		{
			EventHandler = eventHandler;
		}

		protected InteractiveEntity (Mesh m, float x, float y, float z, float xSize, float ySize) : base(m, x, y, z, 0, 0, 0, xSize, ySize){}

		public void Interact(object source)
		{
			if(EventHandler!=null)
				EventHandler.InteractionPerformed (this, source);
		}

		public bool IsInField(Vector2 point)
		{
			return ((point - Pos.Xy) * new Vector2 (1 / XSize, 1 / YSize)).Length < 1.4f;
		}
	}
}

