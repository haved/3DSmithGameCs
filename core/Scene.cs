using System;
using System.Collections.Generic;
using OpenTK;

namespace DSmithGameCs
{
	public class Scene : IDisposable
	{
		static uint instances;
		static uint undisposed;

		readonly List<Entity> entities;

		bool disposed;

		public Scene ()
		{
			entities = new List<Entity>();
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
				e.Render (this, VP);
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

