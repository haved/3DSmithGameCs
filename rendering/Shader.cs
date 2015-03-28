using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace DSmithGameCs
{
	public class Shader
	{
		private int programID;
		private List<int> shaders = new List<int> ();

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

		public void Bind()
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


		private Dictionary<string, int> uniforms = new Dictionary<string, int>();
		protected void AddUniform(string name)
		{
			int loc = GL.GetUniformLocation (programID, name);
			uniforms.Add (name, loc);
		}

		public void SetFloat(string name, float value)
		{
			GL.Uniform1 (uniforms [name], value);
		}

		public void SetVector2(string name, Vector2 value)
		{
			GL.Uniform2 (uniforms[name], ref value);
		}

		public void SetVector3(string name, Vector3 value)
		{
			GL.Uniform3 (uniforms[name], ref value);
		}

		public void SetMatrix4(string name, Matrix4 matrix)
		{
			GL.UniformMatrix4 (uniforms [name], false, ref matrix);
		}


		public static void UseNoShader()
		{
			GL.UseProgram (0);
		}
	}
}
