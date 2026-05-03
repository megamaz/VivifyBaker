using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using VivifyBaker.Baker.Scripts.Editor.Utility;
using UnityEditor;
using UnityEngine;

namespace VivifyBaker.Baker.Scripts.Editor.MaterialBaker
{
    public class MaterialBakerWindow : EditorWindow
    {
        private GUIStyle _labelStyle;

        private MaterialBakeSettings _settings;

        private bool _isPropertyNamesOpened;

        private float _time;

        private void OnEnable()
        {
            _settings = new MaterialBakeSettings();
            _labelStyle = new GUIStyle
            {
                fontStyle = FontStyle.Bold,
                fontSize = 15,
                normal =
                {
                    textColor = Color.white,
                }
            };
            _settings.PropertyNames = Array.Empty<string>();
        }
        
        private void OnGUI()
        {
            GUIStyle titleStyle = new GUIStyle
            {
                fontStyle =  FontStyle.Bold,
                fontSize = 30,
                normal =
                {
                    textColor = Color.white
                }
            };

            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space(10);
            EditorGUILayout.BeginVertical();
            
            EditorGUILayout.LabelField("Material Baker", titleStyle);
            EditorGUILayout.Space(30);
            
            GUIClip();
            EditorGUILayout.Space(20);
            GUISongInfo();
            EditorGUILayout.Space(20);
            GUIMaterialProperties();
            EditorGUILayout.Space(20);
            // GUIExportProperties(); // will always sample once per animation frame anyways, since I can't sample between frames.
            
            GUILayout.EndVertical();
            GUILayout.Space(10);
            GUILayout.EndHorizontal();
            
            GUILayout.FlexibleSpace();
            GUIGetPropertyNames();
            
            // test stuff
            _time = EditorGUILayout.FloatField("Test Time", _time);
            if (GUILayout.Button("Fetch Property At Frame"))
            {
                Debug.Log(AnimationSampler.GetPropertyValueAtFrame(_settings.Clip, $"material.{_settings.PropertyNames[0]}", _settings.ObjectName, typeof(MeshRenderer), (int)_time));
            }
            
            GUIUtilities.GUIBake(() => GUIHandleBakeResult());
        }

        private void GUIClip()
        {
            EditorGUILayout.LabelField("Animation Clip", _labelStyle);
            _settings.Clip = EditorGUILayout.ObjectField(_settings.Clip, typeof(AnimationClip), false, GUILayout.Width(180)) as AnimationClip;
        }

        private void GUISongInfo()
        {
            EditorGUILayout.LabelField("Song Info", _labelStyle);
            _settings.BPM = EditorGUILayout.FloatField("BPM", _settings.BPM);
            _settings.StartBeatOffset = EditorGUILayout.FloatField("Start Beat Offset", _settings.StartBeatOffset);
        }

        private void GUIMaterialProperties()
        {
            EditorGUILayout.LabelField("Material Properties", _labelStyle);
            _settings.MaterialName = EditorGUILayout.TextField("Material Name", _settings.MaterialName);
            _settings.ObjectName = EditorGUILayout.TextField("Object Name", _settings.ObjectName);
            _settings.PropertyNames = StringArrayField("Property Names", ref _isPropertyNamesOpened, _settings.PropertyNames);
        }

        private void GUIExportProperties()
        {
            EditorGUILayout.LabelField("Export Properties", _labelStyle);
            _settings.SamplesPerSecond = EditorGUILayout.IntField("Samples per Second", _settings.SamplesPerSecond);
        }

        /// <summary>
        /// Takes the bake result and prompts you to save it.
        /// Saves a json file of a singular vivify event containing the baked data.
        /// </summary>
        private void GUIHandleBakeResult()
        {
            object[] bake_result = MaterialBaker.GetBakeResults(_settings);
            List<Dictionary<string, object>> properties = new List<Dictionary<string, object>>();

            foreach (object bake in bake_result)
            {
                if (bake.GetType() == typeof(BakedMaterialProperty<float>))
                {
                    var data = (BakedMaterialProperty<float>)bake;
                    Dictionary<string, object> new_property = new Dictionary<string, object>
                    {
                        { "id", data.ID },
                        { "type", data.Type }
                    };
                    List<float[]> points = new List<float[]>();
                    foreach (Point<float> p in data.Points.Points)
                    {
                        points.Add(new float[]{p._values, p._time});
                    }
                    new_property.Add("value",  points.ToArray());
                    properties.Add(new_property);
                }

                if (bake.GetType() == typeof(BakedMaterialProperty<Vector4>))
                {
                    var data = (BakedMaterialProperty<Vector4>)bake;
                    Dictionary<string, object> new_property = new Dictionary<string, object>
                    {
                        { "id", data.ID },
                        { "type", data.Type }
                    };
                    List<float[]> points = new List<float[]>();
                    foreach (Point<Vector4> p in data.Points.Points)
                    {
                        points.Add(new float[]{p._values.x, p._values.y, p._values.z, p._values.w, p._time});
                    }
                    new_property.Add("value",  points.ToArray());
                    properties.Add(new_property);
                }
            }   
            Dictionary<string, object> event_data = new Dictionary<string, object>
            {
                {"b", _settings.StartBeatOffset},
                {"t", "SetMaterialProperty"},
                {"d", new Dictionary<string, object>{
                    {"asset",  _settings.MaterialName},
                    {"duration", (_settings.BPM / 60) * _settings.Clip.length},
                    {"values", properties.ToArray()}
                } }
            };
            Debug.Log(event_data.Keys.ToString());
            Debug.Log(event_data["b"]);
            
            JsonSerializer serializer = JsonSerializer.Create();
            string path = EditorUtility.SaveFilePanel("Save Bake Result", ".", $"bake_{_settings.MaterialName}.json", ".json");
            using (StreamWriter writer = new StreamWriter(path, false))
            {
                serializer.Serialize(writer, event_data);
                writer.Close();
            }
            Debug.Log("Output success");
        }

        private void GUIGetPropertyNames()
        {
            if (GUILayout.Button("Get Property Names\n(will appear in console)"))
            {
                EditorCurveBinding[] bindings = AnimationUtility.GetCurveBindings(_settings.Clip);
                foreach (var b in bindings)
                {
                    if(b.propertyName.StartsWith("material"))
                        Debug.Log($"\nObject Name: '{b.path}' | Property Name: {b.propertyName}");
                }
            }
        }

        [MenuItem("VivifyBaker/Bakers/Material Baker")]
        public static void ShowWindow()
        {
            var window = GetWindow<MaterialBakerWindow>();
            window.titleContent = new GUIContent("Material Baker");
            window.minSize = new Vector2(500, 400);
            window.maxSize = new Vector2(500, 1000);
        }
        
        // utility (doesn't work in its own class for some reason)
        //// https://stackoverflow.com/questions/71980696/unity-custom-editor-with-arrays
        private static string[] StringArrayField(string label, ref bool open, string[] array) {
            // Create a foldout
            open = EditorGUILayout.Foldout(open, label);
            int newSize = array.Length;

            // Show values if foldout was opened.
            if (open) {
                // Int-field to set array size
                newSize = EditorGUILayout.IntField("Size", newSize);
                newSize = newSize < 0 ? 0 : newSize;

                // Creates a spacing between the input for array-size, and the array values.
                EditorGUILayout.Space();

                // Resize if user input a new array length
                if (newSize != array.Length) {
                    array = ResizeArray(array, newSize);
                }

                // Make multiple int-fields based on the length given
                for (var i = 0; i < newSize; ++i) {
                    array[i] = EditorGUILayout.TextField($"Property {i+1}", array[i]);
                }
            }
            return array;
        }
        
        private static T[] ResizeArray<T>(T[] array, int size) {
            T[] newArray = new T[size];

            for (var i = 0; i < size; i++) {
                if (i < array.Length) {
                    newArray[i] = array[i];
                }
            }

            return newArray;
        }
    }
}
