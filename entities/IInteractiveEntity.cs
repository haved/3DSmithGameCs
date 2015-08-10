using System;
using OpenTK;

namespace DSmithGameCs
{
	public interface IInteractiveEntity
	{
		void InteractionPerformed(object source);

		bool IsInField(Vector2 point);
	}
}

