using System;
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
            GUIExportProperties();
            
            GUILayout.EndVertical();
            GUILayout.Space(10);
            GUILayout.EndHorizontal();
            
            GUILayout.FlexibleSpace();
            GUIUtilities.GUIBake(() => { Debug.Log("Pressed~"); });
        }

        private void GUIClip()
        {
            EditorGUILayout.LabelField("Animation Clip", _labelStyle);
            _settings.Clip = EditorGUILayout.ObjectField(_settings.Clip, typeof(AnimationClip), false) as AnimationClip;
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
            _settings.PropertyNames = StringArrayField("Property Names", ref _isPropertyNamesOpened, _settings.PropertyNames);
        }

        private void GUIExportProperties()
        {
            EditorGUILayout.LabelField("Export Properties", _labelStyle);
            _settings.SamplesPerSecond = EditorGUILayout.IntField("Samples per Second", _settings.SamplesPerSecond);
            
        }

        [MenuItem("VivifyBaker/Bakers/Material Baker")]
        public static void ShowWindow()
        {
            var window = GetWindow<MaterialBakerWindow>();
            window.titleContent = new GUIContent("Material Baker");
            window.minSize = new Vector2(250, 400);
            window.maxSize = new Vector2(250, 1000);
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
