using DG.Tweening;
using DG.Tweening.Core;
using Exoa.Designer;
using Exoa.Events;
using Lean.Touch;
using System.Collections.Generic;
using UnityEngine;

namespace Exoa.Cameras
{
    public class CameraPerspective : CameraPerspBase
    {

        [Header("ROTATION")]
        public bool allowPitchRotation = true;
        public float PitchSensitivity = 0.25f;
        public bool PitchClamp = true;
        public Vector2 PitchMinMax = new Vector2(5.0f, 90.0f);
        private Vector2 initialRotation = new Vector2(35, 0);
        public bool allowYawRotation = true;
        public float YawSensitivity = 0.25f;
        [Header("TRANSLATION")]
        public float maxTranslationSpeed = 3f;



        override protected void Init()
        {
            base.Init();

            // Calculating the initial parameters based on camera's transform
            initialRotation = transform.rotation.eulerAngles;
            CalculateInitialRotation();
            initDistance = CalculateDistanceFromPositionAndRotation(transform.position, transform.rotation);
            initOffset = CalculateOffsetFromPosition(transform.position, transform.rotation, initDistance, groundHeight);

            currentPitch = Pitch = initialRotation.x;
            currentYaw = Yaw = initialRotation.y;

            finalOffset = initOffset;
            finalDistance = initDistance;
            finalRotation = Quaternion.Euler(currentPitch, currentYaw, 0.0f);
            finalPosition = CalculateNewPosition(finalOffset, finalRotation, finalDistance);


        }


        void Update()
        {
            if (disableMoves)
                return;

            List<LeanFinger> twoFingers = Inputs.TwoFingerFilter.UpdateAndGetFingers();
            List<LeanFinger> oneFinger = Inputs.OneFingerFilter.UpdateAndGetFingers();

            Vector2 screenCenter = cam.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, 0));

            worldPointCameraCenter = ClampPointsXZ(HeightScreenDepth.Convert(screenCenter));
            float pinchRatio = Inputs.pinchRatio;
            float scrollRatio = Inputs.GetScroll();

            if (isFocusingOrFollowing)
                HandleFocusAndFollow();

            if (IsInputMatching(InputMap.Translate))
            {
                worldPointFingersCenter = ClampPointsXZ(HeightScreenDepth.Convert(Inputs.screenPointAnyFingerCountCenter));

                worldPointFingersDelta = Vector3.ClampMagnitude(HeightScreenDepth.ConvertDelta(Inputs.lastScreenPointAnyFingerCountCenter, Inputs.screenPointAnyFingerCountCenter, gameObject), maxTranslationSpeed);

                if (disableTranslation)
                    worldPointFingersDelta = Vector3.zero;

                Vector3 vecFingersCenterToCamera = (finalPosition - worldPointFingersCenter);
                float vecFingersCenterToCameraDistance = vecFingersCenterToCamera.magnitude * pinchRatio;
                vecFingersCenterToCamera = vecFingersCenterToCamera.normalized * vecFingersCenterToCameraDistance;

                Vector3 targetPosition = worldPointFingersCenter + vecFingersCenterToCamera;

                twistRot = Quaternion.AngleAxis(allowYawRotation ? Inputs.twistDegrees : 0, Vector3.up);

                Vector3 offsetFromFingerCenter = worldPointFingersCenter - worldPointFingersDelta;
                //sphere.position = finalOffset;

                finalPosition = twistRot * (targetPosition - worldPointFingersCenter) + offsetFromFingerCenter;
                finalRotation = twistRot * finalRotation;

                currentPitch = Pitch = NormalizeAngle(finalRotation.eulerAngles.x);
                currentYaw = Yaw = (finalRotation.eulerAngles.y);

                Vector3 newWorldPointCameraCenter = CalculateNewCenter(finalPosition, finalRotation);
                Vector3 newWorldPointCameraCenterClamped = ClampPointsXZ(newWorldPointCameraCenter);

                finalOffset = newWorldPointCameraCenter;
                finalDistance = CalculateClampedDistance(finalPosition, newWorldPointCameraCenter, minMaxDistance);
                finalPosition = CalculateNewPosition(newWorldPointCameraCenterClamped, finalRotation, finalDistance);


                CalculateInertia();
            }
            else if (scrollRatio != 1)
            {
                finalOffset = worldPointCameraCenter;
                finalDistance = CalculateClampedDistance(finalPosition, worldPointCameraCenter, minMaxDistance, scrollRatio);
                finalPosition = CalculateNewPosition(worldPointCameraCenter, finalRotation, finalDistance);
            }
            else
            {
                if (IsInputMatching(InputMap.Rotate))
                {
                    Rotate(Inputs.oneFingerScaledPixelDelta);
                    CalculateInertia();
                }
                else ApplyInertia();

                //finalDistance = CalculateClampedDistance(finalPosition, worldPointCameraCenter, minMaxDistance);
                finalRotation = Quaternion.Euler(currentPitch, currentYaw, 0.0f);
                finalPosition = CalculateNewPosition(finalOffset, finalRotation, finalDistance);

            }




            // Apply Edge Boundaries
            if (IsUsingCameraEdgeBoundaries())
            {
                finalPosition = ClampCameraCorners(finalPosition, out bool clampApplied, true, currentPitch > 60);
                finalOffset = CalculateOffsetFromPosition(finalPosition, finalRotation, finalDistance, groundHeight);
                if (clampApplied) CalculateInertia();
            }

            if (!initDataSaved)
            {
                //SetResetValues(finalOffset, FinalRotation, finalDistance);
            }


            ApplyToCamera();




        }

        override protected Quaternion CalculateInitialRotation()
        {
            initRotation = Quaternion.Euler(initialRotation.x, initialRotation.y, 0);
            return initRotation;
        }

        override public void SetResetValues(Vector3 offset, Quaternion rotation, float distance)
        {
            initOffset = offset;
            initDistance = distance;
            initialRotation = rotation.eulerAngles;
            CalculateInitialRotation();
            initDataSaved = true;
            //print("SetResetValues initOffset:" + initOffset);
        }

        override public void ResetCamera()
        {
            StopFollow();

            DOTween.To(() => finalDistance, x => finalDistance = x, initDistance, focusTweenDuration).SetEase(focusEase);
            Quaternion currentRot = finalRotation;
            float currentDist = finalDistance;
            Vector3 currentOffset = finalOffset;
            disableMoves = true;
            float lerp = 0;
            Tween t = DOTween.To(() => lerp, x => lerp = x, 1, focusTweenDuration).SetEase(focusEase);
            t.OnUpdate(() =>
            {
                finalOffset = Vector3.Lerp(currentOffset, initOffset, lerp);
                finalRotation = Quaternion.Lerp(currentRot, initRotation, lerp);
                finalDistance = Mathf.Lerp(currentDist, initDistance, lerp);
                finalPosition = CalculateNewPosition(finalOffset, finalRotation, finalDistance);
                ApplyToCamera();
            })
            .OnComplete(() =>
            {
                currentPitch = Pitch = initialRotation.x;
                currentYaw = Yaw = initialRotation.y;

                disableMoves = false;
            });
        }


        #region EVENTS


        override protected void OnBeforeSwitchPerspective(bool orthoMode)
        {
            if (!orthoMode)
            {
                currentPitch = Pitch = initialRotation.x;
                currentYaw = Yaw = initialRotation.y;
                finalRotation = Quaternion.Euler(currentPitch, currentYaw, 0);
                finalPosition = CalculateNewPosition(finalOffset, finalRotation, finalDistance);
            }
        }

        #endregion

        #region UTILS





        public void Rotate(Vector2 delta)
        {
            var sensitivity = GetRotationSensitivity();

            if (allowYawRotation)
            {
                deltaYaw = delta.x * YawSensitivity * sensitivity;
                Yaw += deltaYaw;
            }
            if (allowPitchRotation)
            {
                deltaPitch = -delta.y * PitchSensitivity * sensitivity;
                Pitch += deltaPitch;
            }
            currentPitch = Pitch;
            currentYaw = Yaw;
            ClampRotation();
        }

        override protected void ClampRotation()
        {
            if (PitchClamp)
                currentPitch = Mathf.Clamp(NormalizeAngle(currentPitch), PitchMinMax.x, PitchMinMax.y);
        }



        public static float ClampAngle(float current, float min = -90f, float max = 90f)
        {
            float dtAngle = Mathf.Abs(((min - max) + 180) % 360 - 180);
            float hdtAngle = dtAngle * 0.5f;
            float midAngle = min + hdtAngle;

            float offset = Mathf.Abs(Mathf.DeltaAngle(current, midAngle)) - hdtAngle;
            if (offset > 0)
                current = Mathf.MoveTowardsAngle(current, midAngle, offset);
            return current;
        }







        #endregion



        #region FOCUS


        override public void FocusCameraOnGameObject(GameObject go, bool allowYOffsetFromGround = false)
        {
            Bounds b = go.GetBoundsRecursive();

            if (b.size == Vector3.zero && b.center == Vector3.zero)
                return;

            // offseting the bounding box
            if (!allowYOffsetFromGround)
            {
                float yOffset = b.center.y;
                b.extents = b.extents.SetY(b.extents.y + yOffset);
                b.center = b.center.SetY(groundHeight);
            }
            Vector3 max = b.size;
            // Get the radius of a sphere circumscribing the bounds
            float radius = max.magnitude * focusRadiusMultiplier;
            //Debug.Log("FocusCameraOnGameObject targetGo position:" + go.transform.position + " b.center:" + b.center);


            float aspect = cam.aspect;
            float horizontalFOV = 2f * Mathf.Atan(Mathf.Tan(fov * Mathf.Deg2Rad / 2f) * aspect) * Mathf.Rad2Deg;
            // Use the smaller FOV as it limits what would get cut off by the frustum        
            float fovMin = Mathf.Min(fov, horizontalFOV);
            float dist = radius / (Mathf.Sin(fovMin * Mathf.Deg2Rad / 2f));


            Vector3 targetOffset = b.center;
            float targetDistance = Mathf.Clamp((dist * focusDistanceMultiplier), minMaxDistance.x, minMaxDistance.y);


            // Disable follow mode
            StopFollow();



            if (targetOffset != finalOffset || finalDistance != targetDistance)
            {
                disableMoves = true;

                Quaternion currentRot = finalRotation;
                float currentDist = finalDistance;
                Vector3 currentOffset = finalOffset;
                float lerp = 0;

                Tween t = DOTween.To(() => lerp, x => lerp = x, 1, focusTweenDuration).SetEase(focusEase);
                t.OnUpdate(() =>
                {
                    b = go.GetBoundsRecursive();
                    targetOffset = b.center.SetY(groundHeight);

                    finalOffset = Vector3.Lerp(currentOffset, targetOffset, lerp);
                    finalDistance = Mathf.Lerp(currentDist, targetDistance, lerp);
                    finalPosition = CalculateNewPosition(finalOffset, finalRotation, finalDistance);
                    ApplyToCamera();

                }).OnComplete(() =>
                {
                    disableMoves = false;
                    CameraEvents.OnFocusComplete?.Invoke(go);
                });

            }
        }
        #endregion

        public void MoveCameraTo(Vector3 targetOffset, float targetDistance, float lerp = .1f)
        {
            finalOffset = Vector3.Lerp(finalOffset, targetOffset, lerp);
            finalDistance = Mathf.Lerp(finalDistance, targetDistance, lerp);
            finalPosition = CalculateNewPosition(finalOffset, finalRotation, finalDistance);
            ApplyToCamera();
        }
        private void HandleFocusAndFollow()
        {
            if (!isFocusingOrFollowing)
                return;

            Bounds b = targetGo.GetBoundsRecursive();

            if (b.size == Vector3.zero && b.center == Vector3.zero)
                return;

            // offseting the bounding box
            float yOffset = b.center.y;
            b.extents = b.extents.SetY(b.extents.y + yOffset);
            b.center = b.center.SetY(groundHeight);
            //Debug.Log("HandleFocusAndFollow targetGo position:" + targetGo.transform.position + " b.center:" + b.center);

            Vector3 max = b.size;
            // Get the radius of a sphere circumscribing the bounds
            float radius = max.magnitude * followRadiusMultiplier;

            float aspect = cam.aspect;
            float horizontalFOV = 2f * Mathf.Atan(Mathf.Tan(fov * Mathf.Deg2Rad / 2f) * aspect) * Mathf.Rad2Deg;
            // Use the smaller FOV as it limits what would get cut off by the frustum        
            float fovMin = Mathf.Min(fov, horizontalFOV);
            float dist = radius / (Mathf.Sin(fovMin * Mathf.Deg2Rad / 2f));

            Vector3 targetOffset = b.center;
            float targetDistance = Mathf.Clamp((dist * followDistanceMultiplier), minMaxDistance.x, minMaxDistance.y);

            if (enableFocusOnGameObject)
            {
                finalDistance = Mathf.Lerp(finalDistance, targetDistance, focusLerp);
            }

            finalOffset = worldPointCameraCenter = Vector3.Lerp(finalOffset, targetOffset, followLerp);
            finalPosition = Vector3.Lerp(finalPosition, CalculateNewPosition(finalOffset, finalRotation, finalDistance), followLerp);

            ApplyToCamera();

        }
    }
}
