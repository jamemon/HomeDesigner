using UnityEngine;
using UnityEditor;

namespace Exoa.Cameras
{
    public abstract class CameraBaseEditor : Editor
    {
        protected string[] _dontIncludeMe = new string[] { "m_Script" };

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawPropertiesExcluding(serializedObject, _dontIncludeMe);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
