using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace DSmithGameCs
{
	public class HatchEntity : InteractiveEntity, EntityEventListener, View
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
				hatchSpeed += Time.delta () * 8;
				hatchRotation += hatchSpeed * Time.delta ();
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
			game.SetView (this);
			if (game.Player.GetSolidX1 () < GetSolidX2 () & game.Player.GetSolidX2 () > GetSolidX1 () & game.Player.GetSolidY1 () < GetSolidY2 () & game.Player.GetSolidY2 () > GetSolidY1 ()) {
				game.Player.LookAt (pos.X, pos.Y);
				game.Player.pos.X = pos.X - (float)Math.Cos (game.Player.rot.Z) * 5;
				game.Player.pos.Y = pos.Y - (float)Math.Sin (game.Player.rot.Z) * 5;
				game.Player.UpdateModelspaceMatrix ();
			}
		}

		//Stuff related to the view
		private Vector3 prevEyePos, prevEyeTarget, prevEyeUp;
		private View prevView;
		private float transition;
		public void OnViewUsed(View prevView)
		{
			transition = 0;
			hatchSpeed = 0;
			this.prevEyePos = prevView.GetEyePos ();
			this.prevEyeTarget= prevView.GetEyeTarget ();
			this.prevEyeUp = prevView.GetEyeUp ();
			this.prevView = prevView;
		}

		public bool ShouldUpdateScene(){
			return false;
		}

		public void UpdateView (Scene s)
		{
			if (transition < 1) {
				transition += Time.delta ()*2;
				transition = Math.Min (1, transition);
			}

			if (hatchRotation > -1.5f)
				hatchSpeed -= Time.delta () * 8;
			else 
				hatchSpeed -= 4*hatchSpeed*Time.delta ();
			hatchRotation += hatchSpeed * Time.delta ();

			if (Input.CloseKeyPressed) {
				game.SetView (prevView);
			}
		}

		public bool ShouldRenderScene ()
		{
			return true;
		}

		public Vector3 GetEyePos()
		{
			return transition < 1 ? (pos + new Vector3 (0, 0, 10)) * transition + prevEyePos * (1 - transition) : pos + new Vector3 (0, 0, 10);
		}

		public Vector3 GetEyeTarget()
		{
			return transition < 1 ? pos*transition + prevEyeTarget*(1-transition) : pos;
		}

		public Vector3 GetEyeUp()
		{
			return transition < 1 ? Vector3.UnitY*transition+prevEyeUp*(1-transition) : Vector3.UnitY;
		}


		public void RenderView(Scene s){}
	}
}

