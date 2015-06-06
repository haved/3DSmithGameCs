using OpenTK;
using System;

namespace DSmithGameCs
{
	public struct Vertex
	{
		public const int VERTEXFLOATAMOUNT = 9;
		public const int VERTEXBYTEAMOUNT = VERTEXFLOATAMOUNT*sizeof(float);

		public float X, Y, Z, Red, Green, Blue;
		public Vector3 Normal;

		public Vertex(float x, float y, float z) : this(x,y,z,0,0,1,1,1,1){}

		public Vertex(float x, float y, float z, float red, float green, float blue) : this(x,y,z,0,0,1,red,green,blue){}

		public Vertex(float x, float y, float z, float nx, float ny, float nz, float red, float green, float blue) : this(x,y,z,new Vector3(nx,ny,nz),red,green,blue){}

		public Vertex(float x, float y, float z, Vector3 normal, float red, float green, float blue){
			this.X = x;
			this.Y = y;
			this.Z = z;
			this.Normal = normal;
			this.Red = red;
			this.Green = green;
			this.Blue = blue;
		}

		public static float[] VertexArrayToFloatArray(Vertex[] vertices)
		{
			var output = new float[vertices.Length*VERTEXFLOATAMOUNT];

			for (int i = 0; i < vertices.Length; i++) {
				output [i * VERTEXFLOATAMOUNT+0] = vertices [i].X;
				output [i * VERTEXFLOATAMOUNT+1] = vertices [i].Y;
				output [i * VERTEXFLOATAMOUNT+2] = vertices [i].Z;
				output [i * VERTEXFLOATAMOUNT+3] = vertices [i].Normal.X;
				output [i * VERTEXFLOATAMOUNT+4] = vertices [i].Normal.Y;
				output [i * VERTEXFLOATAMOUNT+5] = vertices [i].Normal.Z;
				output [i * VERTEXFLOATAMOUNT+6] = vertices [i].Red;
				output [i * VERTEXFLOATAMOUNT+7] = vertices [i].Green;
				output [i * VERTEXFLOATAMOUNT+8] = vertices [i].Blue;
			}

			return output;
		}
	}
}

