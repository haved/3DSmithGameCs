using System;
using System.Drawing;
using System.IO;

namespace DSmithGameCs
{
	public abstract class Item : IDisposable
	{
		~Item()
		{
			Dispose ();
		}

		bool disposed;
		public void Dispose()
		{
			if (!disposed)
				DisposeItem ();
			disposed = true;
		}

		public virtual void DisposeItem () {}

		public abstract uint GetSize ();

		public abstract void DrawTooltip (TextWriter writer);

		public abstract void RenderItem (float x, float y, float width, float height);

		public abstract void LoadInfoFromFile(Stream reader);

		public abstract void SaveInfoToFile(Stream writer);
	}
}

