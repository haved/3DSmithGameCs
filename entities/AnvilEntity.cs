using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

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
		int hotspot = -1;
		float temperature = 25;
		float panAngle = 0;
		public void OnViewUsed(IView prevView)
		{
			transition.SetStart (prevView);
		}

		public bool ShouldUpdateScene ()
		{
			return false;
		}

		public void UpdateView (Scene s)
		{
			transition.UpdateTransition (Time.Delta()*2);

			if (Input.CloseKeyPressed) {
				game.SetView (parentView);
				return;
			}
			if (Input.MousePressed & Input.OrthoMouseX > 50 & Input.OrthoMouseX < 300 & Input.OrthoMouseY > 50 & Input.OrthoMouseY < 300) {
				game.SetView (table);
				table.OnTableUsed (hotspot, temperature);
				return;
			}



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
			blade.RenderBlade (VP, Pos.X, Pos.Y- (hotspot<0?-0.6f:blade.Type.Points[hotspot]*blade.Type.MeshScale), height, Util.PI/2, hotspot, temperature);
			GL.Enable (EnableCap.DepthTest);
			OrthoRenderEngine.DrawExtendedColoredTexturedBox (TextureCollection.DialogBG, Util.White, 50, 50, 250, 250);
			OrthoRenderEngine.DrawColoredMesh (table.Mesh, projection, Util.White, 50, 50, 250, 250);
			OrthoRenderEngine.DrawColoredMesh (table.Coal, projection, Util.White, 50, 50, 250, 250);
		}

		public void SetCoalStripTable(CoalStripTable table)
		{
			this.table = table;
		}

		public void OnAnvilUsed(IView parentView, BladeItem blade, int hotspot, float temperature)
		{
			this.parentView = parentView;
			this.blade = blade;
			this.hotspot = hotspot;
			this.temperature = temperature;
			panAngle = 0;
		}
	}
}

