using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using OpenTK;

namespace Pudvox {
    internal class Camera
    {
        private static Logger logger = LogManager.GetLogger("Camera");
        private Vector3 _eye = new Vector3(0.0f, 0.0f, 5);
        private Vector3 _target = new Vector3(0.0f, 0.0f, 0.0f);
        private Vector3 _up = new Vector3(0f, 1f, 0f);

        private float _upDownRotation = 0f, _leftRightRotation = 0f;
        private float minUpDownRot = -60f, maxUpDownRot = 60f;

        private Matrix4 _viewMatrix = Matrix4.Identity; // row-major matrix
        private Matrix4 _projectionMatrix = Matrix4.Identity;

        private Frustum frustum;

        // Note: OpenTK automatically uploads matrices to correct format to GLSL (OpenGL matrices are column-major)
        // Matrix multiplications on cpu must be done the OpenTK way: row major.
        // - Model * View * Projection * Vertex


        public Camera()
        {
            float fov = (float) (Math.PI/4f);
            float aspect = Settings.WindowWidth/Settings.WindowHeight;
            float nearD = 0.1f;
            float farD = 500f;
            _viewMatrix = Matrix4.LookAt(_eye, _target, _up);
            _projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(fov,
                Settings.WindowWidth/Settings.WindowHeight,nearD, farD);
            frustum = new Frustum(fov,aspect,nearD,farD);
        }

        public Vector3 Eye {
            get { return _eye; }
        }

        public Matrix4 ViewMatrix {
            get { return _viewMatrix; }
        }

        public Matrix4 ProjectionMatrix {
            get { return _projectionMatrix; }
        }

        public void Rotate(float leftRightRotate, float upDownRotate)
        {
            this._leftRightRotation -= leftRightRotate;
            this._leftRightRotation = _leftRightRotation%360;
            this._upDownRotation -= upDownRotate;
            this._upDownRotation = Clamp(_upDownRotation, minUpDownRot, maxUpDownRot);
            this.UpdateViewMatrix();
        }

        

        public void AddToPosition(Vector3 vectorToAdd)
        {
            Matrix4 camRotation = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(_upDownRotation))
                * Matrix4.CreateRotationY(MathHelper.DegreesToRadians(_leftRightRotation));
            Vector3 rotated = Vector3.Transform(vectorToAdd, camRotation);
            _eye += rotated;
            this.UpdateViewMatrix();
        }

        public void MoveOnUpwardsAxis(float camSpeed) {
            _eye = _eye + Vector3.UnitY * camSpeed;
            //UpdateViewMatrix();
        }

        public void Log()
        {
            logger.Info("camera at {0}", _eye);
            frustum.Log();
        }

        private void UpdateViewMatrix()
        {
            Matrix4 camRotation = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(_upDownRotation))
                * Matrix4.CreateRotationY(MathHelper.DegreesToRadians(_leftRightRotation));
            Vector3 camOriginalTarget = new Vector3(0, 0, -1);
            Vector3 camOriginalUp = Vector3.UnitY;
            Vector3 rotatedTarget = Vector3.Transform(camOriginalTarget, camRotation);
            _target = _eye + rotatedTarget;
            _up = Vector3.Transform(camOriginalUp, camRotation);
            _viewMatrix = Matrix4.LookAt(_eye, _target, _up);
            this.frustum.Update(this);
        }

        private static float Clamp(float toClamp, float min, float max) {
            return Math.Max(min, Math.Min(max, toClamp));
        }

        private class Frustum
        {
            private float fov, aspect, nearD, farD;
            private float nearHalfWidth, nearHalfHeight, farHalfWidth, farHalfHeight;
            private Vector3d nearTopLeft, nearTopRight, nearBottomLeft, nearBottomRight;
            private Vector3d farTopLeft, farTopRight, farBottomLeft, farBottomRight;
            private float tang;

            public Frustum(float fov, float aspect, float nearD, float farD)
            {
                this.fov = fov;
                this.aspect = aspect;
                this.nearD = nearD;
                this.farD = farD;
                this.tang = (float) Math.Tan(fov/2);
                this.farHalfHeight = farD * tang;
                this.farHalfWidth = aspect*farHalfHeight;
                this.nearHalfHeight = nearD * tang;
                this.nearHalfWidth = aspect * nearHalfHeight;
            }

            public void Update(Camera camera)
            {
                Vector3d camEye = (Vector3d) camera._eye;
                Vector3d camUp = (Vector3d) camera._up;
                Vector3d camDirection = (Vector3d) (camera._target - camera._eye);
                camDirection.Normalize();
                Vector3d perpendicular = Vector3d.Cross(camDirection, camUp);
                Vector3d nearv = camDirection*nearD;
                Vector3d nearup = camUp*nearHalfHeight;
                Vector3d nearRight = perpendicular*nearHalfWidth;
                nearTopLeft = (nearv + nearup - nearRight) + camEye;
                nearTopRight = (nearv + nearup + nearRight) + camEye;
                nearBottomLeft = (nearv - nearup - nearRight) + camEye;
                nearBottomRight = (nearv - nearup + nearRight) + camEye;
                Vector3d farv = camDirection * farD;
                Vector3d farup = camUp * farHalfHeight;
                Vector3d farRight = perpendicular * farHalfWidth;
                farTopLeft = (farv + farup - farRight) + camEye;
                farTopRight = (farv + farup + farRight) + camEye;
                farBottomLeft = (farv - farup - farRight) + camEye;
                farBottomRight = (farv - farup + farRight) + camEye;
            }

            public void Log()
            {
                object[] paramObjects = new object[] { fov, aspect, nearD, farD, nearTopLeft, nearTopRight, farTopLeft};
                logger.Info("Camera Frustum:\n"+
                    "\tfov {0}, aspect {1}, neard {2}, fard {3}\n"+
                    "near top left {4}\nnear top right {5}\n"+
                    "far top left {6}",paramObjects);
            }

            
        }
    }
}
