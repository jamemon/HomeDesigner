using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Exoa.Cameras
{
    [CustomEditor(typeof(CameraIsometricOrtho))]
    public class CameraIsometricOrthoEditor : CameraBaseEditor
    {
        private bool debugFoldout;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            CameraIsometricOrtho c = target as CameraIsometricOrtho;
            List<string> dontIncludeMe = new List<string>() { "m_Script", };
            CameraModeSwitcher ms = c.gameObject.GetComponent<CameraModeSwitcher>();

            if (ms == null)
            {
                dontIncludeMe.Add("defaultMode");
            }
            if (!c.YawClamp)
            {
                dontIncludeMe.Add("YawMinMax");
            }
            if (!c.allowYawRotation)
            {
                dontIncludeMe.Add("YawClamp");
                dontIncludeMe.Add("YawSensitivity");
            }
            if (!c.allowPitchRotation)
            {
                dontIncludeMe.Add("PitchSensitivity");
                dontIncludeMe.Add("PitchClamp");
                dontIncludeMe.Add("PitchMinMax");
            }
            if (!c.enableTranslationInertia)
            {
                dontIncludeMe.Add("translationInertiaDuration");
                dontIncludeMe.Add("translationInertiaMultiplier");
            }
            if (!c.enableRotationInertia)
            {
                dontIncludeMe.Add("rotationInertiaDuration");
                dontIncludeMe.Add("rotationInertiaMultiplier");
            }

            DrawPropertiesExcluding(serializedObject, dontIncludeMe.ToArray());
            serializedObject.ApplyModifiedProperties();


            debugFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(debugFoldout, "Debug Info");
            if (debugFoldout)
            {
                EditorGUILayout.LabelField("Ground Height:" + c.groundHeight);
                EditorGUILayout.LabelField("Distance:" + c.FinalDistance);
                EditorGUILayout.LabelField("Offset:" + c.FinalOffset);
                EditorGUILayout.LabelField("Rotation:" + c.FinalRotation);
                EditorGUILayout.LabelField("Position:" + c.FinalPosition);
                EditorGUILayout.LabelField("PitchAndYaw:" + c.PitchAndYaw);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
    }
}
