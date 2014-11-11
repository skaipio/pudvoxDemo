using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using NLog;
using OpenTK.Graphics.OpenGL4;

namespace Pudvox {
    class ShaderManager
    {
        private static readonly Logger Logger = LogManager.GetLogger("ShaderManager");
        public static byte[] LoadShaderSource(string filePath)
        {
            byte[] shaderSrc = null;
            try
            {
                using (var stream = new FileStream(filePath, FileMode.Open)) {
                    int len = (int)stream.Length;
                    shaderSrc = new byte[len];
                    stream.Read(shaderSrc, 0, len);
                }
            }
            catch (Exception)
            {
                Logger.Error("Failed to load shader from path {0}", filePath);
            }
            return shaderSrc;
        }

        public static int LoadShaderProgram(string vertexShaderPath, string fragmentShaderPath = null, string geometryShaderPath = null)
        {
            int program = GL.CreateProgram();
            bool status = true;
            if (vertexShaderPath != null) status = AttachShader(vertexShaderPath, ShaderType.VertexShader, program);
            if (fragmentShaderPath != null) status = AttachShader(fragmentShaderPath, ShaderType.FragmentShader, program);
            if (geometryShaderPath != null) status = AttachShader(geometryShaderPath, ShaderType.GeometryShader, program);
            if (status)
            {
                int n = 0;
                GL.GetProgram(program, GetProgramParameterName.AttachedShaders, out n);
                Logger.Debug("Number of shaders attached: {0}", n);
                Logger.Info("Linking shader program.");               
                GL.LinkProgram(program);
                GL.GetProgram(program, GetProgramParameterName.ActiveAttributes, out n);
                Logger.Debug("Number of active attributes in program: {0}", n);
                GL.GetProgram(program, GetProgramParameterName.ActiveUniforms, out n);
                Logger.Debug("Number of active uniforms in program: {0}", n);
            }

            status = CheckShaderProgramStatus(program);
            if(!status) GL.DeleteProgram(program);

            return (status ? program : 0);
        }

        private static bool AttachShader(string shaderPath, ShaderType shaderType, int shaderProgram)
        {
            bool success = true;

            byte[] shaderBytes = LoadShaderSource(shaderPath);
            if (shaderBytes == null)
            {
                success = false;
            }
            else
            {
                int shader = GL.CreateShader(shaderType);
                string src = System.Text.Encoding.Default.GetString(shaderBytes);

                GL.ShaderSource(shader, src);
                Logger.Info("Compiling shader source from {0}", shaderPath);
                GL.CompileShader(shader);
                if (CheckShaderStatus(shader))
                {
                    Logger.Debug("Attaching shader.");
                    GL.AttachShader(shaderProgram, shader);
                }
                else
                {
                    success = false;
                }
                GL.DeleteShader(shader);
            }

            return success;
        }

        private static bool CheckShaderProgramStatus(int program)
        {
            int status = 0;
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out status);
            if (status == 0)
            {
                Logger.Error("Failed to link shader program.");
                Logger.Error(GL.GetProgramInfoLog(0));
            }

            return status != 0;
        }

        private static bool CheckShaderStatus(int shader)
        {
            int status = 0;
            GL.GetShader(shader, ShaderParameter.CompileStatus, out status);
            if (status == 0)
            {
                Logger.Error("Shader compilation failed.");
                string infoLog = GL.GetShaderInfoLog(shader);
                Logger.Error(infoLog);
            }

            return status != 0;
        }
    }
}
