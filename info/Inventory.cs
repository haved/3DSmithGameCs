using System;
using System.Collections.Generic;

namespace DSmithGameCs
{
	public class Inventory
	{
		public const uint SIZE = 4;
		readonly List<Item> items;
		uint freeSpace = SIZE;

		public Inventory ()
		{
			items = new List<Item> ((int)SIZE);
		}

		public bool AddItem(Item i)
		{
			if (!CanFitItem (i))
				return false;
			freeSpace -= i.GetSize();
			items.Add (i);
			return true;
		}

		public int GetItemAmount()
		{
			return items.Count;
		}

		public Item GetItem(int index)
		{
			return items [index];
		}

		public void RemoveItem(int index)
		{
			freeSpace += items [index].GetSize ();
			items.RemoveAt (index);
		}

		public bool CanFitItem(Item i)
		{
			return freeSpace >= i.GetSize();
		}

		const uint iconSize = 64;
		const uint overscan = 20;
		public void Render ()
		{
			var cW = OrthoRenderEngine.GetCanvasWidth ();
			var cH = OrthoRenderEngine.GetCanvasHeight ();

			uint l=0;
			for (int i = 0; i < GetItemAmount(); i++) {
				Item item = GetItem(i);

				OrthoRenderEngine.DrawTexturedBox (TextureCollection.Button, cW-iconSize-overscan, cH-iconSize*(l+0.5f)-overscan, iconSize, iconSize/2, 0, 0, 1, 0.5f);
				OrthoRenderEngine.DrawTexturedBox (TextureCollection.Numbers, cW - iconSize - overscan + 2, cH - iconSize * l - overscan - 12, 10, 10, l / 4f, 0, 0.25f, 1);
				uint oldL = l;
				for (l++; l < oldL + item.GetSize (); l++) {
					OrthoRenderEngine.DrawTexturedBox (TextureCollection.Button, cW-iconSize-overscan, cH-iconSize*l-overscan, iconSize, iconSize/2, 0, 0.25f, 1, 0.5f);
					OrthoRenderEngine.DrawTexturedBox (TextureCollection.Button, cW-iconSize-overscan, cH-iconSize*(l+0.5f)-overscan, iconSize, iconSize/2, 0, 0.25f, 1, 0.5f);
				}
				OrthoRenderEngine.DrawTexturedBox (TextureCollection.Button, cW-iconSize-overscan, cH-iconSize*l-overscan, iconSize, iconSize/2, 0, 0.5f, 1, 0.5f);
				item.Render (cW-iconSize-overscan, cH - iconSize * l - overscan, iconSize, iconSize*item.GetSize());
			}

			for (uint i = l; i < Inventory.SIZE; i++) {
				OrthoRenderEngine.DrawTexturedBox (TextureCollection.Button, cW-iconSize-20, cH-iconSize*(i+1)-20, iconSize, iconSize, 0, 0, 1, 1);
				OrthoRenderEngine.DrawTexturedBox (TextureCollection.Numbers, cW - iconSize - 20 + 2, cH - iconSize * i - 20 - 12, 10, 10, i / 4f, 0, 0.25f, 1);
			}
		}
	}
}

