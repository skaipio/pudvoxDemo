using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace Pudvox.Graphics3D {
    class Chunk {
        public const int Size = 16;
        public const int FloatsPerBlock = 4;
        private Block[][][] _blocks; // TODO: Convert to 1D array to optimize
        private const float BlockSize = 0.5f;
        private readonly float[] _renderBuffer;
        private int _renderBufferOffset;

        public Chunk(int xOffset, int yOffset, int zOffset, float[] renderBuffer, int renderBufferOffset)
        {
            _renderBuffer = renderBuffer;
            _renderBufferOffset = renderBufferOffset;
            _blocks = Utils.InitializeArray3D(Size, Size, Size, () => new Block());
            int n = renderBufferOffset;
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    for (int k = 0; k < Size; k++)
                    {
                        _renderBuffer[n++] = xOffset + i;
                        _renderBuffer[n++] = yOffset + j;
                        _renderBuffer[n++] = zOffset + k;
                        _renderBuffer[n++] = BlockSize;
                    }
                }
            }
        }

        
        public void Render()
        {
            /*
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, _renderBufferSize, _renderBuffer, BufferUsageHint.DynamicDraw);
            GL.DrawArrays(PrimitiveType.Points, 0, Size * Size * Size);
             * */
        }
    }
}
