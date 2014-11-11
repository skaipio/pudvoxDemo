using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using OpenTK.Platform;
using Pudvox.Graphics3D;

namespace Pudvox {
    public class Pudvox : IDisposable {
        private static readonly Logger Logger = LogManager.GetLogger("Pudvox");
        private GameWindow _window;
        private Camera _camera;
        private CameraController _cameraController;
        private double fpsAverage = 0;
        private int fpsRecordCounter = 0;
        private double time = 0.0;

        private Pudvox() {
        }


        public static void Main() {
            var pudvox = new Pudvox();
            pudvox.Init();
            pudvox.Run();
            pudvox.Shutdown();
        }

        private void Run() {
            _window.Run(60);
        }

        private void Init() {
            Logger.Info("Initializing Pudvox Engine.");


            _window = new GameWindow((int)Settings.WindowWidth, (int)Settings.WindowHeight, new GraphicsMode(32, 24, 0, 4), "Pudvox Engine",
                GameWindowFlags.Default, DisplayDevice.Default,
                4, 3,
                GraphicsContextFlags.Debug);

            _window.MakeCurrent();
            _window.VSync = VSyncMode.On;
            _window.RenderFrame += Render;
            _window.KeyDown += WindowOnKeyDown;
            

            BlockManager.Init();

            GL.PointSize(10f);

            ErrorCode error = GL.GetError();
            if (error != ErrorCode.NoError) {
                Logger.Debug("GL Error: {0}", error);
            }

            Mouse.SetPosition(Settings.WindowWidth/2d, Settings.WindowHeight/2d);
            _camera = new Camera();
            _cameraController = new CameraController(_camera);
            _cameraController.Init();
            
        }



        private void Shutdown() {
            BlockManager.Shutdown();
            _window.Dispose();
        }

        private void Render(object sender, FrameEventArgs args) {
            _cameraController.Update(Convert.ToSingle(args.Time));
            if (time >= 1.0) {
                _camera.Log();
                fpsAverage /= fpsRecordCounter;
                Console.WriteLine(fpsAverage);
                fpsAverage = 0.0f;
                fpsRecordCounter = 0;
                time = 0.0;
            }
            GL.ClearColor(Color4.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            BlockManager.Render(_camera, Convert.ToSingle(args.Time));
            _window.SwapBuffers();
            fpsAverage += _window.RenderFrequency;
            fpsRecordCounter++;
            time += args.Time;
        }


        public void Dispose() {
            this.Shutdown();
        }

        private void WindowOnKeyDown(object sender, KeyboardKeyEventArgs keyboardKeyEventArgs) {
            switch (keyboardKeyEventArgs.Key) {
                case Key.Escape:
                    _window.Close();
                    break;
            }
        }
    }
}
