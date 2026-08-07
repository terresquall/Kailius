using Terresquall;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(PersistentObject.SaveData), true)]
public class SaveDataPropertyDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty(position, label, property);
        Rect foldoutRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label, true);

        if (property.isExpanded) {
            EditorGUI.indentLevel++;
            float y = position.y + EditorGUIUtility.singleLineHeight;
            var iterator = property.Copy();
            var end = iterator.GetEndProperty();
            iterator.NextVisible(true);

            while (!SerializedProperty.EqualContents(iterator, end)) {
                if(iterator.name != "saveID") {
                    float h = EditorGUI.GetPropertyHeight(iterator, true);
                    EditorGUI.PropertyField(new Rect(position.x, y, position.width, h), iterator, true);
                    y += h;
                }
                iterator.NextVisible(false);
            }
            EditorGUI.indentLevel--;
        }
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        float height = EditorGUIUtility.singleLineHeight;
        if (property.isExpanded) {
            var iterator = property.Copy();
            var end = iterator.GetEndProperty();
            iterator.NextVisible(true);
            while (!SerializedProperty.EqualContents(iterator, end)) {
                height += EditorGUI.GetPropertyHeight(iterator, true);
                iterator.NextVisible(false);
            }
        }
        return height;
    }
}
