using UnityEngine;

namespace Exoa.Cameras
{
    public class CameraPerspBase : CameraBase
    {
        [Header("DISTANCE")]
        public Vector2 minMaxDistance = new Vector2(3, 30);
        protected float initDistance = 10f;
        protected float fov = 55.0f;


        public float Fov
        {
            get
            {
                return fov;
            }
        }
        public float GetDistance()
        {
            return finalDistance;
        }
        override protected void Init()
        {
            fov = cam.fieldOfView;
            finalDistance = initDistance;
            finalOffset = transform.position.SetY(groundHeight);
            //print("Init finalOffset:" + finalOffset);

            base.Init();
        }

        override public Matrix4x4 GetMatrix()
        {
            float aspect = cam.aspect;
            float near = cam.nearClipPlane, far = cam.farClipPlane;
            return Matrix4x4.Perspective(fov, aspect, near, far);
        }


        public void SetPositionByDistance(float v)
        {
            finalDistance = Mathf.Clamp(v, minMaxDistance.x, minMaxDistance.y);
            finalPosition = CalculateNewPosition(finalOffset, finalRotation, finalDistance);
        }




        #region FOLLOW
        [Header("FOLLOW")]
        public float followDistanceMultiplier = 1f;
        #endregion


        virtual public void SetResetValues(Vector3 offset, Quaternion rotation, float distance)
        {
            initOffset = offset;
            initDistance = distance;
            initRotation = rotation;
            initDataSaved = true;
        }
    }
}
