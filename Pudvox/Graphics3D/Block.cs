using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace Pudvox.Graphics3D {
    class Block {
        private const float width = 1.0f;
        private const float length = 1.0f;
        private const float height = 1.0f;

        public void Render(int x, int y, int z, int shaderProgram) {
            GL.VertexAttrib3(0, x, y, z);
            GL.VertexAttrib3(2, width, length, height);
            GL.DrawArrays(PrimitiveType.Points, 0, 1);
        }

        public bool IsEmpty
        {
            get { return false;}
        }
    }
}
