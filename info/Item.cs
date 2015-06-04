using System.IO;
using OpenTK;

namespace DSmithGameCs
{
	public abstract class Item
	{
		public abstract uint GetSize ();

		public abstract string GetTooltipName ();

		public abstract Vector4 GetTooltipColor ();

		public abstract void RenderItem (float x, float y, float width, float height);

		public abstract void LoadFromFile(StreamReader reader);

		public abstract void SaveToFile(StreamWriter writer);
	}
}

