using UnityEditor;
using UnityEngine;

namespace Exoa.Designer
{
    [CustomEditor(typeof(HomeLoadByName))]
    public class BuildingLoadByNameEditor : Editor
    {
        // Start is called before the first frame update
        public override void OnInspectorGUI()
        {
            HomeLoadByName obj = target as HomeLoadByName;

            DrawDefaultInspector();

            if (GUILayout.Button("Pre bake"))
            {
                obj.LoadFile(obj.fileName);
                obj.buildAtStart = false;
            }
            if (GUILayout.Button("Clear"))
            {
                obj.Clear();
            }
        }

    }
}
