using UnityEngine;
using UnityEditor;

using System.Collections;

namespace DigitalRuby.AdvancedPolygonCollider
{
    [CustomEditor(typeof(AdvancedPolygonCollider))]
    public class AdvancedPolygonColliderEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            AdvancedPolygonCollider c = target as AdvancedPolygonCollider;
            if (c != null)
            {
                EditorGUILayout.LabelField("Vertices: " + c.VerticesCount);
            }
        }
    }
}