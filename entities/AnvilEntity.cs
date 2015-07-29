using System;
using OpenTK;

namespace DSmithGameCs
{
	public class AnvilEntity : InteractiveEntity, IEntityEventListener, IView
	{
		readonly Smith2DGame game;
		readonly float height;
		CoalStripTable table;

		public AnvilEntity(Smith2DGame game, Mesh m, float x, float y, float z, float xSize, float ySize, float height) : base(m, x, y, z, xSize, ySize){
			this.game = game;
			this.height = height;
			EventHandler = this;
		}

		public void InteractionPerformed(InteractiveEntity entity, object source)
		{
			table.InteractionPerformed (entity, source);
		}

		//Stuff related to the view
		Transition transition = new Transition();
		IView parentView;
		BladeItem blade;
		float[] heat;
		int diamond;
		float panAngle;
		public void OnViewUsed(IView prevView)
		{
			transition.SetStart (prevView);
		}

		public bool ShouldUpdateScene ()
		{
			return false;
		}

		float hammerMove;
		float hammerX;
		float hammerY;
		bool hammerDown = true;
		public void UpdateView (Scene s)
		{
			transition.UpdateTransition (Time.Delta()*2);

			if (Input.CloseKeyPressed) {
				OnAnvilNotUsed ();
				game.SetView (parentView);
				return;
			}
			if (Input.InteractKeyPressed | Input.MousePressed & Input.OrthoMouseX > 50 & Input.OrthoMouseX < 300 & Input.OrthoMouseY > 50 & Input.OrthoMouseY < 300) {
				game.SetView (table);
				table.OnTableUsed (heat, blade);
				OnAnvilNotUsed ();
				return;
			}

			for (int i = 0; i < heat.Length; i++) {
				if(heat[i]>25)
					heat [i] -= Time.Delta () * (i==diamond? 100 : 200);
			}

			if (!hammerDown && Input.MousePressed & hammerMove < 0.3f & Input.OrthoMouseX > OrthoRenderEngine.GetCanvasWidth () / 2 - 100 & Input.OrthoMouseX < OrthoRenderEngine.GetCanvasWidth () / 2 + 100 &
			   Input.OrthoMouseY > OrthoRenderEngine.GetCanvasHeight () / 2 - 70 & Input.OrthoMouseY < OrthoRenderEngine.GetCanvasHeight () / 2 + 70) {
				hammerDown = true;
				hammerX = Util.NextFloat () - 0.5f;
				hammerY = Util.NextFloat () - 0.5f;
			}

			if (hammerDown) {
				if (hammerMove < 1) {
					hammerMove += Time.Delta () * 10;
					if (hammerMove >= 1) {
						hammerDown = false;
						hammerMove = 1;
						if (diamond >= 0) {
							blade.Sharpness [diamond] = Math.Min (1, blade.Sharpness [diamond] + KnownMetal.GetRedEmmission (blade.Metal, heat[diamond]) * 0.01f * (1 - blade.Sharpness [diamond]));
							blade.UpdateSharpnessMap ();
						}
					}
				}
			} else if (hammerMove > 0)
				hammerMove = Math.Max(0, hammerMove-Time.Delta () * 4);

			if (Input.LeftKey) {
				if (!Input.RightKey)
					panAngle -= (panAngle + 0.4f) * Time.Delta () * 3;
			}
			else if (Input.RightKey)
				panAngle -= (panAngle - 0.4f) * Time.Delta () * 3;
			else
				panAngle -= panAngle * Time.Delta () * 3;
		}

		public bool ShouldRenderScene ()
		{
			return true;
		}
			
		public Vector3 GetEyePos()
		{
			return transition.GetEyePos (Pos+new Vector3((float)Math.Sin(panAngle)*10,0,(float)Math.Cos(panAngle)*10));
		}

		public Vector3 GetEyeTarget()
		{
			return transition.GetEyeTarget (Pos);
		}

		public Vector3 GetEyeUp()
		{
			return transition.GetEyeUp (Vector3.UnitY);
		}

		Matrix4 projection = Matrix4.CreateRotationX (-Util.PI / 2) * Matrix4.CreateRotationY (0.9f) * Matrix4.CreateTranslation (0, -4.2f, -7)
			*Matrix4.CreatePerspectiveFieldOfView(Util.PI / 180 * 62, 1, 0.1f, 100)*Matrix4.CreateTranslation(-0.1f, 0.8f, 0);
		public void RenderView (Matrix4 VP, Scene s)
		{
			blade.RenderBlade (VP, Pos.X, Pos.Y- (diamond<0?-0.6f:blade.Type.Points[diamond]*blade.Type.MeshScale), height, Util.PI/2, heat);

			BasicShader.Instance.Bind ();
			Matrix4 modelspace = Matrix4.CreateTranslation(0, 1, 0) * Matrix4.CreateRotationX(-hammerMove) * Matrix4.CreateTranslation(0, 2.5f+hammerY*hammerMove/4, 0) * Matrix4.CreateRotationX(0.9f-hammerMove) *
				Matrix4.CreateRotationZ(0.7f+hammerX*hammerMove/4) * Matrix4.CreateTranslation (1.9f, -2.4f, height+1) * Modelspace;
			BasicShader.Instance.SetMVP (modelspace*VP);
			BasicShader.Instance.SetModelspaceMatrix (modelspace);
			BasicShader.Instance.ResetColor ();
			MeshCollection.Hammer.Draw ();

			OrthoRenderEngine.DrawExtendedColoredTexturedBox (TextureCollection.DialogBG, Util.White, 50, 50, 250, 250);
			OrthoRenderEngine.DrawColoredMesh (table.Mesh, projection, Util.White, 50, 50, 250, 250);
			OrthoRenderEngine.DrawColoredMesh (table.Coal, projection, Util.White, 50, 50, 250, 250);
		}

		public void SetCoalStripTable(CoalStripTable table)
		{
			this.table = table;
		}

		public void OnAnvilUsed(IView parentView, BladeItem blade, float[] heat, int diamond)
		{
			this.parentView = parentView;
			this.blade = blade;
			this.heat = heat;
			this.diamond = diamond;
		}

		public void OnAnvilNotUsed()
		{
			blade = null;
			panAngle = 0;
		}
	}
}

