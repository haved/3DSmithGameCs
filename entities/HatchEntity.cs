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
			Matrix4 MVP = modelspace * VP;
			BasicShader.GetInstance ().SetModelspaceMatrix (modelspace);
			BasicShader.GetInstance ().SetMVP (MVP);
			hatchHole.Draw ();
			Matrix4 hatchTransform = Matrix4.CreateRotationY (hatchRotation) * hatchTranslation;
			BasicShader.GetInstance ().SetModelspaceMatrix (hatchTransform*modelspace);
			BasicShader.GetInstance ().SetMVP (hatchTransform*MVP);
			hatch.Draw ();
		}

		public void InteractionPerformed(InteractiveEntity entity, object source)
		{
			game.GameStats.PlayerInventory.Deselect ();
			game.SetView (this);
			if (game.Player.GetSolidX1 () < GetSolidX2 () & game.Player.GetSolidX2 () > GetSolidX1 () & game.Player.GetSolidY1 () < GetSolidY2 () & game.Player.GetSolidY2 () > GetSolidY1 ()) {
				game.Player.LookAt (pos.X, pos.Y);
				game.Player.pos.X = pos.X - (float)Math.Cos (game.Player.rot.Z) * 5;
				game.Player.pos.Y = pos.Y - (float)Math.Sin (game.Player.rot.Z) * 5;
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
		}

		public bool ShouldRenderScene ()
		{
			return true;
		}

		static Vector3 eyeOffset = new Vector3 (0, 0, 10);
		public Vector3 GetEyePos()
		{
			return transition.GetEyePos (pos+eyeOffset);
		}

		public Vector3 GetEyeTarget()
		{
			return transition.GetEyeTarget (pos);
		}

		public Vector3 GetEyeUp()
		{
			return transition.GetEyeUp (Vector3.UnitY);
		}

		const float iconSize = 64;
		const int iconsPerDir = 4;
		const int iconsPerRow = iconsPerDir * 2;

		public void RenderView(Scene s)
		{
			Inventory playerInv = game.GameStats.PlayerInventory;
			HatchInventory hatchInv = game.GameStats.HatchInv;
			playerInv.Render ();

			if (!transition.IsDone ())
				return;
			
			float x = OrthoRenderEngine.GetCanvasWidth () / 2 - iconSize * iconsPerDir;
			float y = OrthoRenderEngine.GetCanvasHeight () / 2 - iconSize * iconsPerDir;

			OrthoRenderEngine.DrawTexturedBox (TextureCollection.Button, x, y, iconSize * iconsPerRow, iconSize * iconsPerRow, 0, 0, iconsPerRow, iconsPerRow);

			float itemX = 0;
			float itemY = 0;

			for (int i = 0; i < hatchInv.GetItemAmount (); i++) {
				Item item = hatchInv.GetItem (i);
				item.RenderItem (x + itemX * iconSize, y + itemY * iconSize, iconSize, iconSize*item.GetSize());

				itemY += item.GetSize ();
				if (itemY >= iconsPerRow) {
					itemY = 0;
					itemX++;
					if (itemX >= iconsPerRow) {
					}//TODO add multiple pages or something 
				}
			}
				
			if (Input.OrthoMouseX > x & Input.OrthoMouseY > y & Input.OrthoMouseX < x + iconSize * iconsPerRow & Input.OrthoMouseY < y + iconSize * iconsPerRow) {
				int xIndex = (int)(Input.OrthoMouseX - x) / (int)iconSize;
				int yIndex = (int)(Input.OrthoMouseY - y) / (int)iconSize;

				int iconIndex = yIndex + xIndex * iconsPerRow;
				uint box = 0;
				int itemIndex = 0;

				for (; itemIndex < hatchInv.GetItemAmount (); itemIndex++) {
					box += hatchInv.GetItem (itemIndex).GetSize ();
					if (box > iconIndex)
						break;
				}

				if (itemIndex >= hatchInv.GetItemAmount ())
					return;

				Item item = hatchInv.GetItem (itemIndex);

				Inventory.RenderToolTip (item, Input.OrthoMouseX+30, Input.OrthoMouseY);

				if(Input.MousePressed)
					if (playerInv.CanFitItem (item)) {
						playerInv.AddItem (item);
						hatchInv.RemoveItem (itemIndex);
					} else
						playerInv.InventoryTooFull (item);
			}
		}
	}
}

