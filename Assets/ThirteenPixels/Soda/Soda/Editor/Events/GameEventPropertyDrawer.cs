// Copyright © Sascha Graeff/13Pixels.
#if UNITY_2019_3_OR_NEWER
#define UNITY_NEW_SKIN
#endif

namespace ThirteenPixels.Soda.Editor
{
    using UnityEngine;
    using UnityEditor;
    
    [CustomPropertyDrawer(typeof(GameEventBase), true)]
    public class GameEventPropertyDrawer : PropertyDrawer
    {
        private SerializedObject serializedTargetObject;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, label);

            var originalIndentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

#if UNITY_NEW_SKIN
            const int buttonWidth = 60;
#else
            const int buttonWidth = 40;
#endif
            const int padding = 4;
            
            var valueRect = new Rect(position.x,
                                     position.y,
                                     position.width - buttonWidth - padding,
                                     EditorGUIUtility.singleLineHeight);
            var buttonRect = new Rect(position.xMax - buttonWidth,
                                      position.y,
                                      buttonWidth,
                                      EditorGUIUtility.singleLineHeight);

            var targetObject = property.objectReferenceValue as GameEvent;
            if (targetObject)
            {
                EditorGUI.PropertyField(valueRect, property, GUIContent.none);

                GUI.enabled = EditorApplication.isPlaying;
                if (GUI.Button(buttonRect, "Raise", EditorStyles.miniButton))
                {
                    targetObject.Raise();
                }
                GUI.enabled = true;
            }
            else
            {
                position.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(position, property, GUIContent.none);
            }

            EditorGUI.indentLevel = originalIndentLevel;
            
            property.serializedObject.ApplyModifiedProperties();
            EditorGUI.EndProperty();
        }
    }
}
