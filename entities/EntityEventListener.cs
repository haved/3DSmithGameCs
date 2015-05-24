using System;

namespace DSmithGameCs
{
	public interface IEntityEventListener
	{
		void InteractionPerformed(InteractiveEntity entity, object source);
	}
}

