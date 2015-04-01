using System;

namespace DSmithGameCs
{
	public interface EntityEvent
	{
		void OnInteract(InteractiveEntity entity, object source);
	}
}

