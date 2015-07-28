using OpenTK;

namespace DSmithGameCs
{
	public interface ISimpleShader
	{
		void Bind();
		void SetModelspaceMatrix(Matrix4 modelspace);
		void SetMVP(Matrix4 MVP);
		void SetColor(Vector4 color);
	}
}

