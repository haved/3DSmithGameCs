using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace DSmithGameCs
{
	public class CoalStripTable : InteractiveEntity, IEntityEventListener, IView
	{
		readonly Smith2DGame game;
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

		public override void Render(Scene s, Matrix4 VP)
		{
			Matrix4 MVP = Modelspace * VP;
			BasicShader.Instance.Bind ();
			BasicShader.Instance.SetModelspaceMatrix (Modelspace);
			BasicShader.Instance.SetMVP (MVP);
			Mesh.Draw ();
			ColorShader.Instance.Bind ();
			ColorShader.Instance.SetMVP (MVP);
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
					OnTableUsed (-1, 25, bladeItem);
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
		int hotspot;
		float temperature;
		float panAngle;

		public void OnViewUsed (IView prevView)
		{
			transition.SetStart (prevView);
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
			if (Input.MousePressed & Input.OrthoMouseX > OrthoRenderEngine.GetCanvasWidth()-300 & Input.OrthoMouseX < OrthoRenderEngine.GetCanvasWidth()-50 & Input.OrthoMouseY > 50 & Input.OrthoMouseY < 300) {
				game.SetView (anvil);
				anvil.OnAnvilUsed (parentView, blade, hotspot, temperature);
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

			if (hotspot != diamond & temperature > 25)
				temperature -= Time.Delta () * 200;
			if (temperature <= 25)
				hotspot = diamond;
			if (hotspot == diamond & hotspot >= 0 & temperature < 1927) //Temperature of coal
				temperature += Time.Delta () * 100;
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
			blade.RenderBlade (VP, Pos.X+   (diamond<0?-0.6f:blade.Type.Points[diamond]*blade.Type.MeshScale)    , Pos.Y, height, Util.PI, hotspot, temperature);
			OrthoRenderEngine.DrawExtendedColoredTexturedBox (TextureCollection.DialogBG, Util.White, OrthoRenderEngine.GetCanvasWidth () - 300, 50, 250, 250);
			OrthoRenderEngine.DrawColoredMesh (anvil.Mesh, projection, Util.White, OrthoRenderEngine.GetCanvasWidth () - 300, 50, 250, 250);
		}

		#endregion

		public void SetAnvil(AnvilEntity anvil)
		{
			this.anvil = anvil;
		}

		public void OnTableUsed(int hotspot, float temperature, BladeItem blade)
		{
			this.hotspot = hotspot;
			diamond = hotspot;
			this.temperature = temperature;
			this.blade = blade;
		}

		public void OnTableNotUsed()
		{
			blade = null;
			panAngle = 0;
		}
	}
}

