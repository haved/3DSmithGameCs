using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using OpenTK;

namespace DSmithGameCs
{
	public class Inventory
	{
		public const int SIZE = 4;
		readonly List<Item> items;
		uint freeSpace = SIZE;

		public Inventory ()
		{
			items = new List<Item> (SIZE);
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
			selectedItem = -1;
		}

		public bool CanFitItem(Item i)
		{
			return freeSpace >= i.GetSize();
		}

		int selectedItem = -1;
		int hoveredItem = -1;
		public void HandleInput()
		{
			int pressedBox = -1;

			if (Input.OrthoMouseX < OrthoRenderEngine.GetCanvasWidth () - overscan & Input.OrthoMouseX > OrthoRenderEngine.GetCanvasWidth () - iconSize - overscan &
			   Input.OrthoMouseY < OrthoRenderEngine.GetCanvasHeight () - overscan & Input.OrthoMouseY > OrthoRenderEngine.GetCanvasHeight () - iconSize * SIZE - overscan)
				hoveredItem = (int)((OrthoRenderEngine.GetCanvasHeight () - Input.OrthoMouseY - overscan) / iconSize);
			else
				hoveredItem = -1;

			if (Input.PressedItemKey != -1)
				pressedBox = Input.PressedItemKey;
			else if (Input.MousePressed && hoveredItem > -1)
				pressedBox = hoveredItem;

			if (pressedBox == -1)
				return;

			uint box = 0;
			int index = 0;
			for (; index < GetItemAmount (); index++) {
				box += GetItem (index).GetSize();
				if (box > pressedBox)
					break;
			}

			selectedItem = (selectedItem != index & index < GetItemAmount ()) ? index : -1;
		}

		public bool HasSelectedItem()
		{
			return selectedItem != -1;
		}

		public int GetSelectedItemIndex()
		{
			return selectedItem;
		}

		public Item GetSelectedItem()
		{
			return items [selectedItem];
		}

		public void Deselect()
		{
			selectedItem = -1;
		}

		float tooFull = 0;

		const uint iconSize = 64;
		const uint overscan = 20;
		static Vector4 selected = new Vector4 (205 / 255f, 165 / 255f, 68 / 255f, 1);
		static Vector4 unSelected = Util.White;
		public void Render (Smith2DGame game)
		{
			var cW = OrthoRenderEngine.GetCanvasWidth ();
			var cH = OrthoRenderEngine.GetCanvasHeight ();

			uint l=0;
			for (int i = 0; i < GetItemAmount(); i++) {
				Item item = GetItem(i);

				Vector4 boxColor = i == selectedItem ? selected : Util.White;

				OrthoRenderEngine.DrawColoredTexturedBox (boxColor, TextureCollection.Button, cW-iconSize-overscan, cH-iconSize*(l+0.5f)-overscan, iconSize, iconSize/2, 0, 0, 1, 0.5f);
				OrthoRenderEngine.DrawColoredTexturedBox (boxColor, TextureCollection.Numbers, cW - iconSize - overscan + 2, cH - iconSize * l - overscan - 12, 10, 10, l / 4f, 0, 0.25f, 1);
				uint oldL = l;
				for (l++; l < oldL + item.GetSize (); l++) {
					OrthoRenderEngine.DrawColoredTexturedBox (boxColor, TextureCollection.Button, cW-iconSize-overscan, cH-iconSize*l-overscan, iconSize, iconSize/2, 0, 0.25f, 1, 0.5f);
					OrthoRenderEngine.DrawColoredTexturedBox (boxColor, TextureCollection.Button, cW-iconSize-overscan, cH-iconSize*(l+0.5f)-overscan, iconSize, iconSize/2, 0, 0.25f, 1, 0.5f);
				}
				OrthoRenderEngine.DrawColoredTexturedBox (boxColor, TextureCollection.Button, cW-iconSize-overscan, cH-iconSize*l-overscan, iconSize, iconSize/2, 0, 0.5f, 1, 0.5f);
				float bob = i == selectedItem ? (float) Math.Sin(Time.CurrentTime()*5)*4+2 : 0;
				item.RenderItem (cW-iconSize-overscan-bob, cH - iconSize * l - overscan-bob, iconSize+bob*2, iconSize*item.GetSize()+bob*2);
			}

			for (uint i = l; i < Inventory.SIZE; i++) {
				OrthoRenderEngine.DrawTexturedBox (TextureCollection.Button, cW-iconSize-20, cH-iconSize*(i+1)-20, iconSize, iconSize, 0, 0, 1, 1);
				OrthoRenderEngine.DrawTexturedBox (TextureCollection.Numbers, cW - iconSize - 20 + 2, cH - iconSize * i - 20 - 12, 10, 10, i / 4f, 0, 0.25f, 1);
			}

			if (hoveredItem > -1 & hoveredItem < items.Count && items [hoveredItem] != null){
				game.TooltipHelper.ClaimIfPossible (this);
				if (game.TooltipHelper.GetOwner()==this)
					game.TooltipHelper.RenderItemTooltip (items [hoveredItem], OrthoRenderEngine.GetCanvasWidth () - overscan - iconSize - 32, Input.OrthoMouseY);
			}
			else if (game.TooltipHelper.GetOwner () == this)
					game.TooltipHelper.UnClaim ();

			if (tooFull > 0) {
				if (game.ErrortipHelper.ClaimIfPossible(this)) {
					game.ErrortipHelper.Writer.Resize (300, 30);
					game.ErrortipHelper.Writer.Clear ();
					game.ErrortipHelper.Writer.DrawString ("The inventory is too full!", 0, 0, Color.White);
				}
				if (game.ErrortipHelper.GetOwner () == this)
					game.ErrortipHelper.RenderNormalDialog (OrthoRenderEngine.GetCanvasWidth()-overscan-game.ErrortipHelper.Writer.Width, OrthoRenderEngine.GetCanvasHeight()-overscan-iconSize*SIZE-32-30, Util.LightRed60);
				else
					tooFull = 0;
				tooFull -= Time.Delta ();
			}
			else if(game.ErrortipHelper.GetOwner()==this)
				game.ErrortipHelper.UnClaim();
		}

		public void InventoryTooFull(Item item)
		{
			tooFull = 1;
		}

		public void SaveToFile(Stream writer)
		{
			writer.WriteByte ((byte)items.Count);

			foreach (Item i in items) {
				StreamIO.SaveItem (i, writer);
			}
		}

		public void LoadFromFile(Stream reader)
		{
			int itemCount = reader.ReadByte();
			items.Clear ();

			for (int i = 0; i < itemCount; i++) {
				items.Add(StreamIO.LoadItem(reader));
			}
		}
	}
}

