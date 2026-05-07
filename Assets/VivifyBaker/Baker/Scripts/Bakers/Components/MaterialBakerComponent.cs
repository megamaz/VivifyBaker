using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using VivifyBaker.Baker.Scripts.Bakers.MaterialBaker;
using Newtonsoft.Json;

namespace VivifyBaker.Baker.Scripts.Bakers.Components
{
    [Serializable]
    public struct PropertyEnabler
    {
        public bool enabled;
        public string name;
    }
    [ExecuteAlways]
    [RequireComponent(typeof(MeshRenderer))]
    public class MaterialBakerComponent : MonoBehaviour
    {
        [HideInInspector] public Transform _parentAnimator;
        [HideInInspector] public List<PropertyEnabler> properties; // we're gonna draw it ourselves

        public AnimationClip clip;
        public float bpm;
        public float startOffset;
        [HideInInspector] public Material mat;

        private void OnValidate()
        {
            if (!FindAnimatorInParents())
                return;

            // serialize material properties
            // we can only animate float/ranges, vectors, and colors.
            // basically everything but textures
            MeshRenderer renderer = GetComponent<MeshRenderer>();
            if (mat == null || mat.GetInstanceID() != renderer.sharedMaterial.GetInstanceID())
            {
                // ^^ avoid re-serializing (it resets our selection)
                properties = new List<PropertyEnabler>();
                mat = renderer.sharedMaterial;
                Shader mat_shader = renderer.sharedMaterial.shader;
                for (int i = 0; i < mat_shader.GetPropertyCount(); i++)
                {
                    if(mat_shader.GetPropertyType(i) == ShaderPropertyType.Texture)
                        continue;
                    PropertyEnabler new_property = new PropertyEnabler();
                    new_property.enabled = false;
                    new_property.name = mat_shader.GetPropertyName(i);
                    properties.Add(new_property);
                }
            }
        }

        private bool FindAnimatorInParents()
        {
            // traverse parent tree to find animator, if none is found then log error
            try
            {
                Transform p = transform.parent;
                Animator temp;
                while (!p.TryGetComponent<Animator>(out temp))
                {
                    p = p.parent;
                }

                _parentAnimator = p;
                return true;
            }
            catch (NullReferenceException)
            {
                Debug.LogError("No Animator found in parents.");
                return false;
            }
        }
    }
    
    [CustomEditor(typeof(MaterialBakerComponent))]
    public class MatComponentEditor : Editor
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
                
                Dictionary<string, object> event_data = MaterialBaker.MaterialBaker.GetBakeResults(settings); 
            
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