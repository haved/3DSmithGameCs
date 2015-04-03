using System;
using OpenTK;

namespace DSmithGameCs
{
	public interface View
	{
		bool ShouldUpdateScene();

		void UpdateView(Scene s);

		bool ShouldRenderScene ();

		Matrix4 GetViewMatrix();

		void RenderView(Scene s);
	}
}

