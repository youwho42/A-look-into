using UnityEditor;
using UnityEngine;

//[CustomPropertyDrawer(typeof(ConditionalAttribute))]
public class ConditionalDrawer : PropertyDrawer
{
    //public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    //{
    //    ConditionalAttribute conditionalAttribute = (ConditionalAttribute)attribute;
    //    string conditionalBoolName = conditionalAttribute.ConditionalBoolName;

    //    SerializedProperty showPropertiesProperty = property.serializedObject.FindProperty(conditionalBoolName);

    //    if (showPropertiesProperty != null && showPropertiesProperty.propertyType == SerializedPropertyType.Boolean)
    //    {
    //        bool showProperties = showPropertiesProperty.boolValue;

    //        if (showProperties)
    //        {
    //            EditorGUI.PropertyField(position, property, label, true);
    //        }
    //    }
    //    else
    //    {
    //        EditorGUI.HelpBox(position, "Boolean property not found", MessageType.Error);
    //    }
    //}

    //public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    //{
    //    ConditionalAttribute conditionalAttribute = (ConditionalAttribute)attribute;
    //    string conditionalBoolName = conditionalAttribute.ConditionalBoolName;

    //    SerializedProperty showPropertiesProperty = property.serializedObject.FindProperty(conditionalBoolName);

    //    if (showPropertiesProperty != null && showPropertiesProperty.propertyType == SerializedPropertyType.Boolean)
    //    {
    //        bool showProperties = showPropertiesProperty.boolValue;

    //        if (showProperties)
    //        {
    //            return EditorGUI.GetPropertyHeight(property, label, true);
    //        }
    //    }

    //    return 0;
    //}
}