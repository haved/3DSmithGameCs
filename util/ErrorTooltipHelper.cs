using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace DSmithGameCs
{
	public class ErrorTooltipHelper
	{
		public readonly TextWriter Writer;

		const int tooltipWidth=150;
		const int tooltipHeight = 35;

		public ErrorTooltipHelper()
		{
			Writer = new TextWriter(new Font(FontFamily.GenericSansSerif, 18*2), tooltipWidth, tooltipHeight);
		}

		float x, y, time;

		public void ShowError(string textIn, float x, float y, float time, bool printLeft)
		{
			string[] lines = textIn.Split ('\n');

			float width = 0;
			foreach (string line in lines)
				width = Math.Max (width, Writer.GetLineWidth(line));

			Writer.Resize ((int)width, (int)(Writer.GetLineHeight()*lines.Length));

			for(int i = 0; i < lines.Length; i++)
				Writer.DrawString (lines [i], 0, Writer.GetLineHeight () * i, Color.White);

			this.x = printLeft ? x-Writer.Width : x;
			this.y = y;
			this.time = Time.CurrentTime() + time;
		}

		public void UnClaim()
		{
			time = 0;
		}

		public void Render()
		{
			if (Time.CurrentTime () > time)
				return;
			
			GL.DepthFunc (DepthFunction.Always);
			OrthoRenderEngine.DrawExtendedColoredTexturedBox (TextureCollection.DialogBG, Util.LightRed60, x-16, y-16, Writer.Width+32, Writer.Height+32);
			OrthoRenderEngine.DrawTexturedBox (Writer.GetTextureID (), x, y, Writer.Width, Writer.Height);
			GL.DepthFunc (DepthFunction.Lequal);
		}
	}
}

