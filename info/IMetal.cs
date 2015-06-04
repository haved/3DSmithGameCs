using System;
using OpenTK;

namespace DSmithGameCs
{
	public interface IMetal
	{
		int GetMeltingPoint();

		Vector4 GetColor();

		string GetName();

		IMetal Clone();
	}
}

