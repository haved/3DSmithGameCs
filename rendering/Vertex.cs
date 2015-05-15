using OpenTK;
using System;

namespace DSmithGameCs
{
	public class Vertex
	{
		public const int VERTEXFLOATAMOUNT = 9;
		public const int VERTEXBYTEAMOUNT = VERTEXFLOATAMOUNT*sizeof(float);

		public float X, y, z, red, green, blue;
		public Vector3 Normal;

		public Vertex(float x, float y, float z) : this(x,y,z,1,1,1,1,1,1){}

		public Vertex(float x, float y, float z, float red, float green, float blue) : this(x,y,z,0,0,1,red,green,blue){}

		public Vertex(float x, float y, float z, float nx, float ny, float nz, float red, float green, float blue) : this(x,y,z,new Vector3(nx,ny,nz),red,green,blue){}

		public Vertex(float x, float y, float z, Vector3 normal, float red, float green, float blue){
			this.X = x;
			this.y = y;
			this.z = z;
			this.Normal = normal;
			this.red = red;
			this.green = green;
			this.blue = blue;
		}

		public static float[] VertexArrayToFloatArray(Vertex[] vertices)
		{
			float[] output = new float[vertices.Length*VERTEXFLOATAMOUNT];

			for (int i = 0; i < vertices.Length; i++) {
				output [i * VERTEXFLOATAMOUNT+0] = vertices [i].X;
				output [i * VERTEXFLOATAMOUNT+1] = vertices [i].y;
				output [i * VERTEXFLOATAMOUNT+2] = vertices [i].z;
				output [i * VERTEXFLOATAMOUNT+3] = vertices [i].Normal.X;
				output [i * VERTEXFLOATAMOUNT+4] = vertices [i].Normal.Y;
				output [i * VERTEXFLOATAMOUNT+5] = vertices [i].Normal.Z;
				output [i * VERTEXFLOATAMOUNT+6] = vertices [i].red;
				output [i * VERTEXFLOATAMOUNT+7] = vertices [i].green;
				output [i * VERTEXFLOATAMOUNT+8] = vertices [i].blue;
			}

			return output;
		}
	}
}

