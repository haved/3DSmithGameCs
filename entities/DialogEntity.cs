using System;

namespace DSmithGameCs
{
	public class DialogEntity : InteractiveEntity, EntityEventListener
	{
		private readonly Dialog d;
		private readonly DialogController dc;

		public DialogEntity (Dialog d, DialogController dc, Mesh m, float x, float y, float z, float xSize, float ySize) : base(m, x, y, z, xSize, ySize)
		{
			this.eventHandler = this;
			this.d = d;
			this.dc = dc;
		}

		public void InteractionPerformed(InteractiveEntity entity, object source)
		{
			dc.OpenDialog (d);
		}
	}
}

