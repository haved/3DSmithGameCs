﻿using System;
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
		List<ILight> lights;

		bool disposed;

		public Scene ()
		{
			entities = new List<Entity>();
			lights = new List<ILight> ();
			lights.Add (new DirectionalLight(new Vector3(1,1,1), 1f, Vector3.UnitZ));
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
		}

		public void Update()
		{
			for (int i = 0; i < entities.Count; i++) {
				entities [i].Update (this);
				if (entities [i].IsDead ()) {
					entities.RemoveAt (i);
					i--;
				}
			}
		}

		public void Render(Matrix4 VP)
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
				INormalShader shader = light.GetUseShader();
				foreach (Entity e in entities)
					e.Render (this, VP, shader);
			}

			GL.DepthFunc (DepthFunction.Lequal);
			GL.DepthMask (true);
			GL.BlendFunc (BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

			foreach (Entity e in entities)
				e.PostRender (this, VP);
		}

		public List<Entity> GetEntities()
		{
			return entities;
		}

		public void Dispose()
		{
			if (!disposed) {
				foreach (Entity e in entities) {
					e.Dispose ();
				}
				disposed = true;
				undisposed--;
				Console.Out.WriteLine ("Scene disposed. New total undisposed: " + undisposed);
			}
		}
	}
}

