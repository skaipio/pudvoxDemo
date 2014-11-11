using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Input;

namespace Pudvox.Graphics3D {
    class CameraController
    {
        private Camera _camera;
        private MouseState _previousMouseState;
        private Vector3 moveVector = Vector3.Zero;
        private float movementSpeed = 3.0f;

        
        //private float minLeftRightRot, maxLeftRightRot;

        public CameraController(Camera camera)
        {
            this._camera = camera;
            RotationSpeed = 0.2f;
            this.SensitivityX = 0.5f;
            this.SensitivityY = 0.5f;
        }

        public float RotationSpeed { get; set; }

        public float SensitivityX { get; set; }

        public float SensitivityY { get; set; }

        public void Init()
        {
            _previousMouseState = Mouse.GetState();
        }
        public void Update(float delta)
        {
            if (Mouse.GetState() != _previousMouseState)
                this.Rotate();

            this.Move(delta);

            this._previousMouseState = Mouse.GetState();
        }

        private void Rotate() {
            var x = Mouse.GetState().X;
            float xDifference = x - _previousMouseState.X;
            var xRotation = xDifference * RotationSpeed;
            var y = Mouse.GetState().Y;
            float yDifference = y - _previousMouseState.Y;
            var yRotation = yDifference * RotationSpeed;
            _camera.Rotate(xRotation, yRotation);
        }

        private void Move(float delta)
        {
            moveVector.X = 0;
            moveVector.Y = 0;
            moveVector.Z = 0;
            KeyboardState ks = Keyboard.GetState();
            if (ks.IsKeyDown(Key.W))
            {
                this.moveVector.Z = -movementSpeed*delta;
            } else if (ks.IsKeyDown(Key.S))
            {
                this.moveVector.Z = movementSpeed*delta;
            }
            if (ks.IsKeyDown(Key.A)) {
                this.moveVector.X = -movementSpeed * delta;
            } else if (ks.IsKeyDown(Key.D)) {
                this.moveVector.X = movementSpeed * delta;
            }
            if (ks.IsKeyDown(Key.C)) {
                this.moveVector.Y = -movementSpeed * delta;
            } else if (ks.IsKeyDown(Key.V)) {
                this.moveVector.Y = movementSpeed * delta;
            }
            if (moveVector != Vector3.Zero)
                _camera.AddToPosition(moveVector);
        }
    }
}
