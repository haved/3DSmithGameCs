using System;
using System.Drawing;
using System.IO;

namespace DSmithGameCs
{
	public abstract class Item
	{
		public abstract uint GetSize ();

		public abstract void DrawTooltip (TextWriter writer);

		public void DrawStandardTooltip(TextWriter writer, string[] lines, Color[] colors, string[] values)
		{
			int lineCount = Math.Max (lines.Length, values.Length);

			var widths = new float[lineCount, 2];
			float width = 0;
			float height = writer.GetLineHeight () * lineCount;

			for (int i = 0; i < lineCount; i++) {
				if (i < lines.Length && lines[i]!=null)
					widths [i,0] = writer.GetLineWidth (lines[i]);
				if (i < values.Length && values [i] != null)
					widths [i,1] = writer.GetLineWidth (values [i]);
				if (widths [i,0] + widths [i,1] > width)
					width = widths [i,0] + widths [i,1];
			}

			writer.Clear ();
			writer.Resize ((int)width, (int)height);

			Color current = Color.Black;
			for (int i = 0; i < lineCount; i++) {
				if (i < colors.Length && colors [i] != default(Color))
					current = colors [i];
				if (widths [i,0] > 0)
					writer.DrawString (lines [i], 0, i * writer.GetLineHeight (), current);
				if (widths[i,1]>0)
					writer.DrawString (values [i], width-widths[i,1], i * writer.GetLineHeight (), Color.White);
			}
		}

		public abstract void RenderItem (float x, float y, float width, float height);

		public abstract void LoadInfoFromFile(Stream reader);

		public abstract void SaveInfoToFile(Stream writer);
	}
}

