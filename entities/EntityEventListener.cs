using System;

namespace DSmithGameCs
{
	public interface EntityEventListener
	{
		void InteractionPerformed(InteractiveEntity entity, object source);
	}
}

