using System;
using OpenTK;

namespace DSmithGameCs
{
	public class CoalStripTable : InteractiveEntity, IEntityEventListener, IView
	{
		readonly Smith2DGame game;
		AnvilEntity anvil;
		readonly Mesh table, coal;
		Vector4 coalColor = new Vector4(1,1,1,1);
		readonly float height;
		public CoalStripTable (Smith2DGame game, Mesh table, Mesh coal, float x, float y, float z, float xSize, float ySize, float height) : base(null, x,y,z,xSize,ySize)
		{
			this.game = game;
			this.table = table;
			this.coal = coal;
			EventHandler = this;
			eyeTarget = new Vector3 (x, y, 0);
			this.height = height;
		}

		public override void Render(Scene s, Matrix4 VP)
		{
			Matrix4 MVP = Modelspace * VP;
			BasicShader.GetInstance ().Bind ();
			BasicShader.GetInstance ().SetModelspaceMatrix (Modelspace);
			BasicShader.GetInstance ().SetMVP (MVP);
			table.Draw ();
			ColorShader.GetInstance ().Bind ();
			ColorShader.GetInstance ().SetMVP (MVP);
			coalColor.X = coalColor.Y = coalColor.Z = 1.3f + (float)Math.Sin(Time.CurrentTime()*2)/6;
			ColorShader.GetInstance ().SetColor (coalColor);
			coal.Draw ();
		}

		#region IEntityEventListener implementation

		public void InteractionPerformed (InteractiveEntity entity, object source)
		{
			if (game.GameStats.PlayerInventory.HasSelectedItem ()) {
				var bladeItem = game.GameStats.PlayerInventory.GetSelectedItem () as BladeItem;
				if (bladeItem != null) {
					game.GameStats.PlayerInventory.Deselect ();
					blade = bladeItem;
					parentView = game.CurrentView;
					anvil.SetParentView (parentView);
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
		int diamond = -1;
		float panAngle = 0;

		public void OnViewUsed (IView prevView)
		{
			transition.SetStart (prevView);
			diamond = -1;
		}

		public bool ShouldUpdateScene ()
		{
			return false;
		}
			
		public void UpdateView (Scene s)
		{
			transition.UpdateTransition (Time.Delta () * 2f);
			if (Input.MousePressed)
				game.SetView (anvil);
			else if (Input.CloseKeyPressed) {
				game.SetView (parentView);
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
		}

		public bool ShouldRenderScene ()
		{
			return true;
		}

		readonly Vector3 eyeTarget;
		static readonly Vector3 eyeUp =	-Vector3.UnitX;
		public Vector3 GetEyePos ()
		{
			return transition.GetEyePos (eyeTarget+new Vector3(0,(float)Math.Sin(panAngle)*10,(float)Math.Cos(panAngle)*10));
		}

		public Vector3 GetEyeTarget ()
		{
			return transition.GetEyeTarget (eyeTarget);
		}

		public Vector3 GetEyeUp ()
		{
			return transition.GetEyeUp (eyeUp);
		}

		public void RenderView (Matrix4 VP, Scene s)
		{
			blade.RenderBlade (VP, Pos.X+   (diamond<0?-0.6f:blade.Type.Points[diamond]*blade.Type.MeshScale)    , Pos.Y, height, Util.PI);
		}

		#endregion

		public void SetAnvil(AnvilEntity anvil)
		{
			this.anvil = anvil;
		}
	}
}

