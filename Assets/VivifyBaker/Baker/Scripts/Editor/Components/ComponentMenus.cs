using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using VivifyBaker.Baker.Scripts.Components.Bakers.MaterialBaker;
using VivifyBaker.Baker.Scripts.Bakers.MaterialBaker;

namespace VivifyBaker.Baker.Scripts.Editor.Components
{
    [CustomEditor(typeof(MaterialBakerComponent))]
    public class MatComponentEditor : UnityEditor.Editor
    {
        private bool _foldout = false;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            serializedObject.Update();

            SerializedProperty properties = serializedObject.FindProperty("properties");
            
            _foldout = EditorGUILayout.Foldout(_foldout, $"Properties to bake", true);

            if (_foldout)
            {
                EditorGUI.indentLevel++;

                for (int i = 0; i < properties.arraySize; i++)
                {
                    SerializedProperty element = properties.GetArrayElementAtIndex(i);
                    SerializedProperty enabledProp = element.FindPropertyRelative("enabled");
                    SerializedProperty nameProp = element.FindPropertyRelative("name");

                    EditorGUILayout.BeginHorizontal();
                    
                    enabledProp.boolValue = EditorGUILayout.Toggle(enabledProp.boolValue, GUILayout.Width(40));
                    
                    EditorGUILayout.LabelField(nameProp.stringValue);

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUI.indentLevel--;
            }
            
            // add bake button
            if (GUILayout.Button("Bake"))
            {
                // generate bake settings
                MaterialBakeSettings settings = new MaterialBakeSettings();
                settings.Clip = serializedObject.FindProperty("clip").objectReferenceValue as AnimationClip;

                settings.MaterialName =
                    AssetDatabase.GetAssetPath(serializedObject.FindProperty("mat").objectReferenceValue as Material).ToLower();

                List<string> property_names = new List<string>();
                for (int i = 0; i < properties.arraySize; i++)
                {
                    SerializedProperty element = properties.GetArrayElementAtIndex(i);
                    SerializedProperty enabledProp = element.FindPropertyRelative("enabled");
                    SerializedProperty nameProp = element.FindPropertyRelative("name");
                    if (enabledProp.boolValue)
                        property_names.Add(nameProp.stringValue);
                }

                settings.PropertyNames = property_names.ToArray();
                
                Transform parent = serializedObject.FindProperty("_parentAnimator").objectReferenceValue as Transform;
                Transform target_transform = (serializedObject.targetObject as Component).transform;
                settings.ObjectName = AnimationUtility.CalculateTransformPath(target_transform, parent);
                settings.BPM = serializedObject.FindProperty("bpm").floatValue;

                settings.StartBeatOffset = serializedObject.FindProperty("startOffset").floatValue;
                
                Dictionary<string, object> event_data = MaterialBaker.GetBakeResults(settings); 
            
                JsonSerializer serializer = JsonSerializer.Create();
                string path = EditorUtility.SaveFilePanel("Save Bake Result", ".", $"bake_{settings.Clip.name}.json", ".json");
                using (StreamWriter writer = new StreamWriter(path, false))
                {
                    serializer.Serialize(writer, event_data);
                    writer.Close();
                }
                Debug.Log("Output success");
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}