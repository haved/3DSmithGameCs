using OpenTK;
using System;

namespace DSmithGameCs
{
	public class Vertex
	{
		public const int VERTEX_FLOAT_AMOUNT = 9;
		public const int VERTEX_BYTE_AMOUNT = VERTEX_FLOAT_AMOUNT*sizeof(float);

		public float x, y, z, red, green, blue;
		public Vector3 normal;

		public Vertex(float x, float y, float z) : this(x,y,z,0,0,1,1,1,1){}

		public Vertex(float x, float y, float z, float red, float green, float blue) : this(x,y,z,0,0,1,red,green,blue){}

		public Vertex(float x, float y, float z, float nx, float ny, float nz, float red, float green, float blue) : this(x,y,z,new Vector3(nx,ny,nz),red,green,blue){}

		public Vertex(float x, float y, float z, Vector3 normal, float red, float green, float blue){
			this.x = x;
			this.y = y;
			this.z = z;
			this.normal = normal;
			this.red = red;
			this.green = green;
			this.blue = blue;
		}

		public static float[] VertexArrayToFloatArray(Vertex[] vertices)
		{
			float[] output = new float[vertices.Length*VERTEX_FLOAT_AMOUNT];

			for (int i = 0; i < vertices.Length; i++) {
				output [i * VERTEX_FLOAT_AMOUNT+0] = vertices [i].x;
				output [i * VERTEX_FLOAT_AMOUNT+1] = vertices [i].y;
				output [i * VERTEX_FLOAT_AMOUNT+2] = vertices [i].z;
				output [i * VERTEX_FLOAT_AMOUNT+3] = vertices [i].normal.X;
				output [i * VERTEX_FLOAT_AMOUNT+4] = vertices [i].normal.Y;
				output [i * VERTEX_FLOAT_AMOUNT+5] = vertices [i].normal.Z;
				output [i * VERTEX_FLOAT_AMOUNT+6] = vertices [i].red;
				output [i * VERTEX_FLOAT_AMOUNT+7] = vertices [i].green;
				output [i * VERTEX_FLOAT_AMOUNT+8] = vertices [i].blue;
			}

			return output;
		}
	}
}

