using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace DSmithGameCs
{
	public class TooltipHelper
	{
		public readonly TextWriter Writer;

		const int tooltipWidth=150;
		const int tooltipHeight = 35;

		public TooltipHelper()
		{
			Writer = new TextWriter(new Font(FontFamily.GenericSansSerif, 18*2), tooltipWidth, tooltipHeight);
		}

		object Owner;

		public bool CanClaimButHasNot(object o)
		{
			return Owner == null;
		}

		/*
		 * 
		 * @return bool: If the tooltip was just claimed
		 * */
		public bool ClaimIfPossible(object o)
		{
			if(CanClaimButHasNot(o)) {
				Owner = o;
				Console.Out.WriteLine ("Claimed by: " + o);
				return true;
			}
			return false;
		}

		public object GetOwner()
		{
			return Owner;
		}

		public void UnClaim()
		{
			Console.Out.WriteLine ("UnClaimed from: " + Owner);
			Owner = null;
			currentItem = null;
		}

		Item currentItem;
		public void RenderItemTooltip(Item i, float x, float y)
		{
			if (i != currentItem) {
				i.DrawTooltip (Writer);
			}
			x -= Writer.Width;
			GL.DepthFunc (DepthFunction.Always);
			OrthoRenderEngine.DrawExtendedColoredTexturedBox (TextureCollection.DialogBG, Util.White60, x-16, y-16, Writer.Width+32, Writer.Height+32);
			OrthoRenderEngine.DrawTexturedBox (Writer.GetTextureID (), x, y, Writer.Width, Writer.Height);
			GL.DepthFunc (DepthFunction.Lequal);
			currentItem = i;
		}

		public void RenderNormalDialog(float x, float y, Vector4 color)
		{
			GL.DepthFunc (DepthFunction.Always);
			OrthoRenderEngine.DrawExtendedColoredTexturedBox (TextureCollection.DialogBG, color, x-16, y-16, Writer.Width+32, Writer.Height+32);
			OrthoRenderEngine.DrawTexturedBox (Writer.GetTextureID (), x, y, Writer.Width, Writer.Height);
			GL.DepthFunc (DepthFunction.Lequal);
		}
	}
}

