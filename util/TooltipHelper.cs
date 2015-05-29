using System.Drawing;
using OpenTK;

namespace DSmithGameCs
{
	public class TooltipHelper
	{
		public readonly TextWriter Writer;

		const int tooltipWidth=150;
		const int tooltipHeight = 35;

		public TooltipHelper()
		{
			Writer = new TextWriter(new Font(FontFamily.GenericSansSerif, 18), tooltipWidth, tooltipHeight);
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
			Owner = null;
			currentItem = null;
		}

		Item currentItem;
		public void RenderItemTooltip(Item i, float x, float y)
		{
			if (i != currentItem) {
				Writer.Clear ();
				Writer.Resize ((int)Writer.GetLineWidth (i.GetTooltipName()), (int)Writer.GetLineHeight ());
				Writer.DrawString (i.GetTooltipName(), 0, 0, Util.GetColorFromVector(i.GetTooltipColor()));
			}
			OrthoRenderEngine.DrawExtendedColoredTexturedBox (TextureCollection.DialogBG, Util.White, x-16, y-16, Writer.Width+32, Writer.Height+32);
			OrthoRenderEngine.DrawTexturedBox (Writer.GetTextureID (), x, y, Writer.Width, Writer.Height);
		}

		public void RenderNormalDialog(float x, float y, Vector4 color)
		{
			OrthoRenderEngine.DrawExtendedColoredTexturedBox (TextureCollection.DialogBG, color, x-16, y-16, Writer.Width+32, Writer.Height+32);
			OrthoRenderEngine.DrawTexturedBox (Writer.GetTextureID (), x, y, Writer.Width, Writer.Height);
		}
	}
}

