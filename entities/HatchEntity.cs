using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace DSmithGameCs
{
	public class HatchEntity : InteractiveEntity, IEntityEventListener, View
	{
		private readonly Smith2DGame game;
		readonly Mesh hatchHole;
		readonly Mesh hatch;
		readonly Matrix4 hatchTranslation;
		float hatchRotation = 0;
		float hatchSpeed = 0;
		public HatchEntity (Smith2DGame game, Mesh hatchHole, Mesh hatch, Vector3 hatchTranslation, float x, float y, float z, float xSize, float ySize) : base(null, hatchHole, x, y, z, xSize, ySize)
		{
			this.game = game;
			this.hatchHole = hatchHole;
			this.hatch = hatch;
			this.hatchTranslation = Matrix4.CreateTranslation(hatchTranslation);
			this.solid = false;
			this.EventHandler = this;
		}

		public override void Update(Scene s)
		{
			if (hatchRotation < 0) {
				hatchSpeed += Time.Delta () * 8;
				hatchRotation += hatchSpeed * Time.Delta ();
				if (hatchRotation > 0) {
					hatchRotation = 0;
					hatchSpeed = 0;
				}
			}
		}

		public override void Render(Scene s, Matrix4 VP)
		{
			Matrix4 MVP = Modelspace * VP;
			BasicShader.GetInstance ().Bind ();
			BasicShader.GetInstance ().ResetColor ();
			BasicShader.GetInstance ().SetModelspaceMatrix (Modelspace);
			BasicShader.GetInstance ().SetMVP (MVP);
			hatchHole.Draw ();
			Matrix4 hatchTransform = Matrix4.CreateRotationY (hatchRotation) * hatchTranslation;
			BasicShader.GetInstance ().SetModelspaceMatrix (hatchTransform*Modelspace);
			BasicShader.GetInstance ().SetMVP (hatchTransform*MVP);
			hatch.Draw ();
		}

		public void InteractionPerformed(InteractiveEntity entity, object source)
		{
			game.GameStats.PlayerInventory.Deselect ();
			game.SetView (this);
			if (game.Player.GetSolidX1 () < GetSolidX2 () & game.Player.GetSolidX2 () > GetSolidX1 () & game.Player.GetSolidY1 () < GetSolidY2 () & game.Player.GetSolidY2 () > GetSolidY1 ()) {
				game.Player.LookAt (Pos.X, Pos.Y);
				game.Player.Pos.X = Pos.X - (float)Math.Cos (game.Player.Rot.Z) * 5;
				game.Player.Pos.Y = Pos.Y - (float)Math.Sin (game.Player.Rot.Z) * 5;
				game.Player.UpdateModelspaceMatrix ();
			}
		}

		//Stuff related to the view
		Transition transition = new Transition();
		View prevView;
		public void OnViewUsed(View prevView)
		{
			hatchSpeed = 0;
			transition.SetStart (prevView);
			this.prevView = prevView;
		}

		public bool ShouldUpdateScene(){
			return false;
		}

		int hoveredItem = -1;
		public void UpdateView (Scene s)
		{
			if (Input.CloseKeyPressed) {
				game.SetView (prevView);
				return;
			}

			transition.UpdateTransition (Time.Delta()*2);

			if (hatchRotation > -1.5f)
				hatchSpeed -= Time.Delta () * 8;
			else 
				hatchSpeed -= 4*hatchSpeed*Time.Delta ();
			hatchRotation += hatchSpeed * Time.Delta ();

			game.GameStats.PlayerInventory.HandleInput ();
			if (game.GameStats.PlayerInventory.HasSelectedItem ()) {
				game.GameStats.HatchInv.AddItem (game.GameStats.PlayerInventory.GetSelectedItem());
				game.GameStats.PlayerInventory.RemoveItem (game.GameStats.PlayerInventory.GetSelectedItemIndex ());
			}

			Inventory playerInv = game.GameStats.PlayerInventory;
			HatchInventory hatchInv = game.GameStats.HatchInv;
			float x = inventoryXPos;
			float y = inventoryYPos;

			if (Input.OrthoMouseX > x & Input.OrthoMouseY > y & Input.OrthoMouseX < x + iconSize * iconsPerRow & Input.OrthoMouseY < y + iconSize * iconsPerRow) {
				int xIndex = (int)(Input.OrthoMouseX - x) / (int)iconSize;
				int yIndex = (int)(Input.OrthoMouseY - y) / (int)iconSize;

				int iconIndex = yIndex + xIndex * iconsPerRow;
				uint box = 0;
				uint yPos=0;
				uint xPos=0;
				for (hoveredItem = 0; hoveredItem < hatchInv.GetItemAmount (); hoveredItem++) {
					box += hatchInv.GetItem (hoveredItem).GetSize ();
					yPos += hatchInv.GetItem (hoveredItem).GetSize ();

					if (yPos > iconsPerRow) {
						yPos = 0;
						xPos++;
						box = xPos * iconsPerRow+hatchInv.GetItem (hoveredItem).GetSize ();
					}
					if (iconIndex < box-hatchInv.GetItem(hoveredItem).GetSize()) {
						hoveredItem = -1;
						break;
					}
					if (box > iconIndex)
						break;
				}

				if (hoveredItem < hatchInv.GetItemAmount () & Input.MousePressed) {
					Item item = hatchInv.GetItem (hoveredItem);
					if (playerInv.CanFitItem (item)) {
						playerInv.AddItem (item);
						hatchInv.RemoveItem (hoveredItem);
					} else
						playerInv.InventoryTooFull (item);
				}
			} else
				hoveredItem = -1;
		}

		public bool ShouldRenderScene ()
		{
			return true;
		}

		static Vector3 eyeOffset = new Vector3 (0, 0, 10);
		public Vector3 GetEyePos()
		{
			return transition.GetEyePos (Pos+eyeOffset);
		}

		public Vector3 GetEyeTarget()
		{
			return transition.GetEyeTarget (Pos);
		}

		public Vector3 GetEyeUp()
		{
			return transition.GetEyeUp (Vector3.UnitY);
		}

		const float iconSize = 64;
		const int iconsPerDir = 4;
		const int iconsPerRow = iconsPerDir * 2;

		static float inventoryXPos {get { return OrthoRenderEngine.GetCanvasWidth () / 2 - iconSize * iconsPerDir; }}
		static float inventoryYPos {get { return OrthoRenderEngine.GetCanvasHeight () / 2 - iconSize * iconsPerDir; }}
		public void RenderView(Scene s)
		{
			Inventory playerInv = game.GameStats.PlayerInventory;
			playerInv.Render (game);

			if (!transition.IsDone ())
				return;

			HatchInventory hatchInv = game.GameStats.HatchInv;
			float x = inventoryXPos;
			float y = inventoryYPos;

			OrthoRenderEngine.DrawTexturedBox (TextureCollection.Button, x, y, iconSize * iconsPerRow, iconSize * iconsPerRow, 0, 0, iconsPerRow, iconsPerRow);

			float itemX = 0;
			float itemY = 0;

			for (int i = 0; i < hatchInv.GetItemAmount (); i++) {
				Item item = hatchInv.GetItem (i);
				if (itemY + item.GetSize () > iconsPerRow) {
					itemY = 0;
					itemX++;
					if (itemX >= iconsPerRow) {
					}//TODO add multiple pages or something
				}
				item.RenderItem (x + itemX * iconSize, y + itemY * iconSize, iconSize, iconSize * item.GetSize ());

				itemY += item.GetSize ();
			}

			if (hoveredItem>=0 & hoveredItem < hatchInv.GetItemAmount ()) {
				Item item = hatchInv.GetItem (hoveredItem);

				game.TooltipHelper.ClaimIfPossible (this);

				if (game.TooltipHelper.GetOwner () == this)
					game.TooltipHelper.RenderItemTooltip (item, Input.OrthoMouseX - 30, Input.OrthoMouseY);
			} else if (game.TooltipHelper.GetOwner () == this)
				game.TooltipHelper.UnClaim ();
		}
	}
}

