using System;
using OpenTK;

namespace DSmithGameCs
{
	public class CoalStripTable : InteractiveEntity, IEntityEventListener, IView
	{
		readonly Smith2DGame game;
		readonly PointLight light = new PointLight (new Vector3(1,0,0), Vector3.Zero, 8, 12, 0.1f, 0.3f, 1.2f);
		AnvilEntity anvil;
		public readonly Mesh Coal;
		Vector4 coalColor = new Vector4(1,1,1,1);
		readonly float height;
		public CoalStripTable (Smith2DGame game, Mesh table, Mesh coal, float x, float y, float z, float xSize, float ySize, float height) : base(table, x,y,z,xSize,ySize)
		{
			this.game = game;
			Coal = coal;
			EventHandler = this;
			this.height = height;
		}

		public override void DisposeEntity()
		{
			base.DisposeEntity ();
			Coal.Dispose ();
		}

		public override void OnAddedToScene(Scene s)
		{
			s.AddLight (light);
		}

		public override void OnRemovedFromScene(Scene s)
		{
			s.RemoveLight (light);
		}

		public override void UpdateModelspaceMatrix()
		{
			base.UpdateModelspaceMatrix ();
			light.Position = Pos+new Vector3(0, height, 0);
		}

		public override void Render(Scene s, Matrix4 VP, INormalShader shader)
		{
			shader.SetModelspaceMatrix (Modelspace);
			shader.SetMVP (Modelspace * VP);
			Mesh.Draw ();
		}

		public override void PostRender(Scene s, Matrix4 VP)
		{
			ColorShader.Instance.Bind ();
			ColorShader.Instance.SetMVP (Modelspace * VP);
			coalColor.X = coalColor.Y = coalColor.Z = 1.3f + (float)Math.Sin(Time.CurrentTime()*2)/6;
			ColorShader.Instance.SetColor (coalColor);
			Coal.Draw ();
		}

		#region IEntityEventListener implementation

		public void InteractionPerformed (InteractiveEntity entity, object source)
		{
			if (game.GameStats.PlayerInventory.HasSelectedItem ()) {
				var bladeItem = game.GameStats.PlayerInventory.GetSelectedItem () as BladeItem;
				if (bladeItem != null) {
					game.GameStats.PlayerInventory.Deselect ();
					OnTableUsed (new float[bladeItem.Type.Points.Length], bladeItem);
					parentView = game.CurrentView;
					game.SetView (this);
					return;
				}
			}

			game.ErrortipHelper.ShowError (Localization.GetLocalization ("ui.error.nobladeselected"), Input.OrthoMouseX, Input.OrthoMouseY, 1.4f, false);
		}

		#endregion

		#region View implementation

		Transition transition = new Transition();
		IView parentView;
		BladeItem blade;
		int diamond;
		float[] heat;
		float panAngle;

		public void OnViewUsed (IView prevView)
		{
			transition.SetStart (prevView);
			Input.InteractKeyPressed = false;
		}

		public bool ShouldUpdateScene ()
		{
			return false;
		}
			
		public void UpdateView (Scene s)
		{
			transition.UpdateTransition (Time.Delta () * 2f);
			if (Input.CloseKeyPressed) {
				OnTableNotUsed ();
				game.SetView (parentView);
				return;
			}
			if (Input.InteractKeyPressed | Input.MousePressed & Input.OrthoMouseX > OrthoRenderEngine.GetCanvasWidth()-300 & Input.OrthoMouseX < OrthoRenderEngine.GetCanvasWidth()-50 & Input.OrthoMouseY > 50 & Input.OrthoMouseY < 300) {
				game.SetView (anvil);
				anvil.OnAnvilUsed (parentView, blade, heat, diamond);
				OnTableNotUsed ();
				return;
			}

			if (Input.UpKeyPressed & diamond < blade.Type.Points.Length - 1)
				diamond++;
			else if (Input.DownKeyPressed & diamond >= 0)
				diamond--;
			if (Input.LeftKey)
				panAngle -= (panAngle + 0.4f) * Time.Delta () * 3;
			else if (Input.RightKey)
				panAngle -= (panAngle - 0.4f) * Time.Delta () * 3;
			else
				panAngle -= panAngle * Time.Delta () * 3;

			for (int i = 0; i < heat.Length; i++) {
				if (i != diamond & heat [i] > 25)
					heat [i] -= Time.Delta () * 200;
				else if (heat [i] < 1927)
					heat [i] += Time.Delta () * 100;
			}
		}

		public bool ShouldRenderScene ()
		{
			return true;
		}
			
		static readonly Vector3 eyeUp =	-Vector3.UnitX;
		public Vector3 GetEyePos ()
		{
			return transition.GetEyePos (Pos+new Vector3(0,(float)Math.Sin(panAngle)*10,(float)Math.Cos(panAngle)*10));
		}

		public Vector3 GetEyeTarget ()
		{
			return transition.GetEyeTarget (Pos);
		}

		public Vector3 GetEyeUp ()
		{
			return transition.GetEyeUp (eyeUp);
		}

		Matrix4 projection = Matrix4.CreateRotationX (-Util.PI / 2) * Matrix4.CreateRotationY (0.5f) * Matrix4.CreateTranslation (0, -4.2f, -7)
				*Matrix4.CreatePerspectiveFieldOfView(Util.PI / 180 * 60, 1, 0.1f, 100)*Matrix4.CreateTranslation(0.25f, 0.8f, 0);
		public void RenderView (Matrix4 VP, Scene s)
		{
			blade.RenderBlade (VP, Pos.X+   (diamond<0?-0.6f:blade.Type.Points[diamond]*blade.Type.MeshScale)    , Pos.Y, height, Util.PI, heat, GetEyePos());
			OrthoRenderEngine.DrawExtendedColoredTexturedBox (TextureCollection.DialogBG, Util.White, OrthoRenderEngine.GetCanvasWidth () - 300, 50, 250, 250);
			OrthoRenderEngine.DrawColoredMesh (anvil.Mesh, projection, Util.White, OrthoRenderEngine.GetCanvasWidth () - 300, 50, 250, 250);
		}

		#endregion

		public void SetAnvil(AnvilEntity anvil)
		{
			this.anvil = anvil;
		}

		public void OnTableUsed(float[] heat, BladeItem blade)
		{
			this.heat = heat;
			this.blade = blade;
		}

		public void OnTableNotUsed()
		{
			blade = null;
			panAngle = 0;
		}
	}
}

