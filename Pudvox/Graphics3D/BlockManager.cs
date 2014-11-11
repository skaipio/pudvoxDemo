using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using NLog;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace Pudvox.Graphics3D {
    static class BlockManager {
        private static readonly Logger Logger = LogManager.GetLogger("BlockManager");
        private static Chunk[][][] _chunks; // TODO: Convert to 1D array to optimize
        private const int ChunksPerSide = 1;
        private const int BlocksInTotal = ChunksPerSide*ChunksPerSide*ChunksPerSide*
                                          Chunk.Size*Chunk.Size*Chunk.Size;
        private static int _vertexArrayObject = 0;  // NEED THIS, even for drawing a simple point
        private static int _vboPositions = 0;
        private static int _shaderProgram = 0;
        private static float[] _blockPositions;
        private static int _floatsPerBlock;
        private static int _floatsPerChunk;
        private static int _bytesPerBlock;
        private static float _totalTime;
        

        public static void Init() {
            Logger.Info("Initializing Block Manager.");

            _vertexArrayObject = GL.GenVertexArray();
            _vboPositions = GL.GenBuffer();

            _floatsPerBlock = Chunk.FloatsPerBlock;
            _floatsPerChunk = Chunk.Size * Chunk.Size * Chunk.Size * _floatsPerBlock;
            _bytesPerBlock = _floatsPerBlock * sizeof(float);

            _blockPositions = new float[BlocksInTotal*_bytesPerBlock];

            _chunks = new Chunk[ChunksPerSide][][];
            int offset = 0;
            for (int i = 0; i < ChunksPerSide; i++)
            {
                _chunks[i] = new Chunk[ChunksPerSide][];
                for (int j = 0; j < ChunksPerSide; j++)
                {
                    _chunks[i][j] = new Chunk[ChunksPerSide];
                    for (int k = 0; k < ChunksPerSide; k++)
                    {
                        _chunks[i][j][k] = new Chunk(i * Chunk.Size, j * Chunk.Size, k * Chunk.Size,
                            _blockPositions, offset);
                        offset += _floatsPerChunk;
                    }
                }
            }

            int voxelsByteSize = _blockPositions.Length*sizeof(float);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vboPositions);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)voxelsByteSize, _blockPositions, BufferUsageHint.StaticDraw);
            
            _shaderProgram = ShaderManager.LoadShaderProgram(
                "../Assets/Shaders/vs_rotating_cubes.glsl",
                "../Assets/Shaders/fs_rotating_cubes.glsl",
                "../Assets/Shaders/gs_rotating_cubes.glsl");

            if (_shaderProgram == 0)
                Logger.Error("Failed to load voxel shader program.");

            Console.WriteLine(GL.GetError());
        }

        public static void Shutdown() {
            Logger.Info("Shutting down Block Manager.");
            _chunks = null;
            GL.DeleteVertexArray(_vertexArrayObject);
            _vertexArrayObject = 0;
            GL.DeleteBuffer(_vboPositions);
            _vboPositions = 0;
            GL.DeleteProgram(_shaderProgram);
            _shaderProgram = 0;
        }

        public static void Render(Camera camera, float delta) {
            bool valid = CheckShaderProgram();
            if (_shaderProgram != 0 && valid) {
                GL.UseProgram(_shaderProgram);
                FillUniforms(camera);
                GL.Enable(EnableCap.DepthTest);

                GL.BindBuffer(BufferTarget.ArrayBuffer, _vboPositions);
                GL.BindVertexArray(_vertexArrayObject);
                PrepareVertexArray();
                
                GL.DrawArrays(PrimitiveType.Points, 0, BlocksInTotal);

                DismantleVertexArray();

                _totalTime += delta;
            }
        }

        private static void DismantleVertexArray()
        {
            int positionAttrib = GL.GetAttribLocation(_shaderProgram, "position");
            GL.DisableVertexAttribArray(positionAttrib);
        }

        private static void FillUniforms(Camera camera)
        {
            int viewUniform = GL.GetUniformLocation(_shaderProgram, "viewMatrix");
            int projUniform = GL.GetUniformLocation(_shaderProgram, "projectionMatrix");
            int viewpointUniform = GL.GetUniformLocation(_shaderProgram, "viewpoint");
            int timeUniform = GL.GetUniformLocation(_shaderProgram, "time");

            Matrix4 viewMatrix = camera.ViewMatrix;
            Matrix4 projectionMatrix = camera.ProjectionMatrix;
            Vector3 eye = camera.Eye;

            GL.UniformMatrix4(viewUniform, false, ref viewMatrix);
            GL.UniformMatrix4(projUniform, false, ref projectionMatrix);
            GL.Uniform3(viewpointUniform, ref eye);
            GL.Uniform1(timeUniform, _totalTime);
        }

        private static void PrepareVertexArray()
        {
            int positionAttrib = GL.GetAttribLocation(_shaderProgram, "position");
            int sizeAttrib = GL.GetAttribLocation(_shaderProgram, "size");
            GL.VertexAttribPointer(positionAttrib,
                3, VertexAttribPointerType.Float,
                false, 4*sizeof(float), 0);
            GL.VertexAttribPointer(sizeAttrib,
                1, VertexAttribPointerType.Float,
                false, 4 * sizeof(float), 3*sizeof(float));
            GL.EnableVertexAttribArray(positionAttrib);
            GL.EnableVertexAttribArray(sizeAttrib);
        }

        private static bool CheckShaderProgram() {
            int status = 0;
            GL.ValidateProgram(_shaderProgram);
            GL.GetProgram(_shaderProgram, GetProgramParameterName.ValidateStatus, out status);
            return status != 0;
        }
    }
}
