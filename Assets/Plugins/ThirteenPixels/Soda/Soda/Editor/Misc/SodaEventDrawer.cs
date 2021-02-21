// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda.Editor
{
    using UnityEngine;
    using UnityEditor;

    /// <summary>
    /// Draws listeners registered to a SodaEvent.
    /// </summary>
    internal static class SodaEventDrawer
    {
        private const int maxListenerDisplayCount = 50;
        private static readonly object[] listenerBuffer = new object[maxListenerDisplayCount];
        private static readonly object[] scopedVariableListenerBuffer = new object[maxListenerDisplayCount];

        /// <summary>
        /// Displays the listeners of the provided SodaEvent.
        /// </summary>
        /// <param name="sodaEvent">The SodaEvent to display the listeners of.</param>
        /// <param name="except">An optional object that will not be displayed. Intended for the editor instance that draws the listeners.</param>
        public static void DisplayListeners(SodaEventBase sodaEvent, Object except = null)
        {
            try
            {
                var count = sodaEvent.GetListeners(listenerBuffer);
                DisplayListenersWithLayout(listenerBuffer, count, except);
            }
            catch
            {
                EditorGUILayout.HelpBox("An error occurred while drawing the listeners.", MessageType.Error);
                return;
            }
        }

        private static void DisplayListenersWithLayout(object[] listeners, int count, object except = null)
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(20);
               
                
                GUILayout.BeginVertical(GUILayout.Width(Screen.width - 100));

                DisplayListeners(listeners, count, except, out var displayedAtLeastOne);

                if (count == listeners.Length)
                {
                    EditorGUILayout.HelpBox("There might be more listeners after this.", MessageType.Info);
                }

                if (!displayedAtLeastOne)
                {
                    GUILayout.Label("None");
                }

                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
        }

        private static void DisplayListeners(object[] listeners, int count, object except, out bool displayedAtLeastOne)
        {
            displayedAtLeastOne = false;

            for (var i = 0; i < count; i++)
            {
                var listener = listeners[i];
                if (listener != except)
                {
                    GUILayout.BeginHorizontal();
                    if (listener is Object)
                    {
                        var listenerObject = (Object)listener;
                        if (GUILayout.Button("Select", GUILayout.Width(100)))
                        {
                            Selection.activeObject = listenerObject;
                        }
                        GUILayout.Label(listenerObject.name + " (" + listenerObject.GetType().Name + ")");
                        displayedAtLeastOne = true;
                    }
                    else if (listener is ScopedVariableBase)
                    {
                        var scopedVariableEvent = ((ScopedVariableBase)listener).GetOnChangeValueEvent();
                        var innerCount = scopedVariableEvent.GetListeners(scopedVariableListenerBuffer);
                        DisplayListeners(scopedVariableListenerBuffer, innerCount, except, out var scopedVariableDisplayedAtLeastOne);
                        displayedAtLeastOne = displayedAtLeastOne || scopedVariableDisplayedAtLeastOne;
                    }
                    else
                    {
                        GUILayout.Space(100);
                        GUILayout.Label(listener.GetType().Name);
                        displayedAtLeastOne = true;
                    }
                    GUILayout.EndHorizontal();
                }
            }
        }
    }
}
