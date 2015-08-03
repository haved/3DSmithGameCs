using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace DSmithGameCs
{
	public class Scene : IDisposable
	{
		static uint instances;
		static uint undisposed;

		readonly List<Entity> entities;
		public Vector4 AmbientLight=new Vector4(0.5f,0.5f,0.5f,1f);
		readonly List<ILight> lights;

		bool disposed;

		public Scene ()
		{
			entities = new List<Entity>();
			lights = new List<ILight> ();
			instances++;
			undisposed++;
			Console.WriteLine ("Scene constructed. New total instances: " + instances + "  Total undisposed scenes: " + undisposed);
		}

		~Scene()
		{
			Dispose ();
			instances--;
			Console.WriteLine ("Scene destructed. New total instances: " + instances);
		}

		public void AddEntity(Entity e)
		{
			entities.Add (e);
			e.OnAddedToScene (this);
		}

		public void AddLight(ILight light)
		{
			lights.Add (light);
		}

		public void RemoveLight(ILight light)
		{
			lights.Remove (light);
		}

		public void Update()
		{
			for (int i = 0; i < entities.Count; i++) {
				entities [i].Update (this);
				if (entities [i].IsDead ()) {
					entities [i].OnRemovedFromScene (this);
					entities.RemoveAt (i);
					i--;
				}
			}
		}

		public void Render(Matrix4 VP, Vector3 eyePos)
		{
			foreach (Entity e in entities)
				e.PreRender (this, VP);
			
			ForAmbientShader.Instance.Bind ();
			ForAmbientShader.Instance.SetAmbientLight (AmbientLight);
			foreach (Entity e in entities)
				e.Render (this, VP, ForAmbientShader.Instance);

			GL.DepthFunc (DepthFunction.Equal);
			GL.DepthMask (false);
			GL.BlendFunc (BlendingFactorSrc.One, BlendingFactorDest.One);

			foreach (ILight light in lights)
			{
				INormalShader shader = light.GetUseShader(this, eyePos);
				foreach (Entity e in entities)
					e.Render (this, VP, shader);
			}

			GL.DepthFunc (DepthFunction.Lequal);
			GL.DepthMask (true);
			GL.BlendFunc (BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

			foreach (Entity e in entities)
				e.PostRender (this, VP);
		}

		public void RenderWithShader(Matrix4 VP, INormalShader shader)
		{
			foreach (Entity e in entities)
				e.Render (this, VP, shader);
		}

		public List<Entity> GetEntities()
		{
			return entities;
		}

		public void Dispose()
		{
			if (!disposed) {
				foreach (Entity e in entities)
					e.Dispose ();
				foreach (ILight light in lights)
					light.Dispose ();
				disposed = true;
				undisposed--;
				Console.Out.WriteLine ("Scene disposed. New total undisposed: " + undisposed);
			}
		}
	}
}

