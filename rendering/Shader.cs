using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace DSmithGameCs
{
	public class Shader
	{
		int programID;
		List<int> shaders = new List<int> ();

		public Shader ()
		{
			CreateProgram ();
		}

		~Shader()
		{
			if (GraphicsContext.CurrentContext != null) {
				foreach(int shader in shaders)
					GL.DeleteShader (shader);
				GL.DeleteProgram (programID);
			}
		}

		public void CreateProgram()
		{
			programID = GL.CreateProgram();
			if (programID == 0) {
				Console.Error.WriteLine ("Cold not allocate program space for shader!");
			}
		}

		public int GetProgramID()
		{
			return programID;
		}

		public void Compile()
		{
			GL.LinkProgram( programID );
			GL.ValidateProgram (programID);
		}

		public virtual void Bind()
		{
			GL.UseProgram (programID);
		}

		public void AddVertexShader(string vertexShader)
		{
			int shader = GL.CreateShader( ShaderType.VertexShader );
			GL.ShaderSource( shader, vertexShader);
			GL.CompileShader( shader );

			string info;
			int status_code = -1;

			GL.GetShaderInfoLog(shader, out info);
			GL.GetShader(shader, ShaderParameter.CompileStatus, out status_code);

			if (status_code != 1) {
				Console.WriteLine ("Failed to Compile Vertex Shader Source." +
					Environment.NewLine + info + Environment.NewLine + "Status Code: " + status_code.ToString ());
			}

			GL.AttachShader( programID, shader );
			shaders.Add (shader);
		}

		public void AddFragmentShader(string fragmentShader)
		{
			int shader = GL.CreateShader( ShaderType.FragmentShader );
			GL.ShaderSource( shader, fragmentShader);
			GL.CompileShader( shader );

			string info;
			int status_code = -1;

			GL.GetShaderInfoLog(shader, out info);
			GL.GetShader(shader, ShaderParameter.CompileStatus, out status_code);

			if (status_code != 1) {
				Console.WriteLine ("Failed to Compile Fragment Shader Source." +
					Environment.NewLine + info + Environment.NewLine + "Status Code: " + status_code.ToString ());
			}

			GL.AttachShader( programID, shader );
			shaders.Add (shader);
		}

		public void AddVertexShaderFromFile(string vertexShaderFile)
		{
			AddVertexShader (System.IO.File.ReadAllText(vertexShaderFile));
		}

		public void AddFragmentShaderFromFile(string fragmentShaderFile)
		{
			AddFragmentShader (System.IO.File.ReadAllText(fragmentShaderFile));
		}


		protected int AddUniform(string name)
		{
			return GL.GetUniformLocation (programID, name);
		}

		protected void SetInteger(int id, int value)
		{
			GL.Uniform1 (id, value);
		}

		protected void SetFloat(int id, float value)
		{
			GL.Uniform1 (id, value);
		}

		protected void SetVector2(int id, Vector2 value)
		{
			GL.Uniform2 (id, ref value);
		}

		protected void SetVector2(int id, float v0, float v1)
		{
			GL.Uniform2 (id, v0, v1);
		}

		protected void SetVector3(int id, Vector3 value)
		{
			GL.Uniform3 (id, ref value);
		}

		protected void SetVector4(int id, Vector4 value)
		{
			GL.Uniform4 (id, ref value);
		}

		protected void SetMatrix4(int id, Matrix4 matrix)
		{
			GL.UniformMatrix4 (id, false, ref matrix);
		}

		public static void UseNoShader()
		{
			GL.UseProgram (0);
		}
	}
}
