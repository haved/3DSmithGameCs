using System;
using System.Collections.Generic;

namespace Smith2D
{
	public class Scene
	{
		private List<Entity> entities;

		public Scene ()
		{
			entities = new List<Entity>();
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

		public void Render()
		{
			foreach (Entity e in entities)
				e.Render (this);
		}

		public List<Entity> GetEntities()
		{
			return entities;
		}
	}
}

