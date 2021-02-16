// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda.Editor
{
    using UnityEngine;
    using UnityEditor;

    [CustomEditor(typeof(GameEventListener))]
    public class GameEventListenerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var listener = (GameEventListener)target;

            GUILayout.Space(6);
            
            var gameEventProperty = serializedObject.FindProperty("_gameEvent");

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(gameEventProperty, new GUIContent("Game Event"));
            if(EditorGUI.EndChangeCheck() && Application.isPlaying)
            {
                listener.gameEvent = (GameEvent)gameEventProperty.objectReferenceValue;
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("_response"));
            
            var buttonRect = EditorGUILayout.GetControlRect(GUILayout.Height(0), GUILayout.Width(120));
            buttonRect.height = 15;
            buttonRect.y -= 16;
            buttonRect.x += 1;
            if (GUI.Button(buttonRect, "Invoke Response", EditorStyles.miniButton))
            {
                listener.OnEventRaised();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
