using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using VivifyBaker.Baker.Scripts.Constants;

namespace VivifyBaker.Baker.Scripts.Animatable
{
    [Serializable]
    public struct AnimatedProperty
    {
        [NotKeyable] public PropertyType propertyType;
        [NotKeyable] public string PropertyName;

        public float float_value;
        public Color color_value;
        public float x, y, z, w; // for vector mode
        
    }
    
    [Serializable]
    [RequireComponent(typeof(PostProcessAnimationController))]
    public class AnimatablePostProcessLayer : MonoBehaviour
    {
        [HideInInspector] public Material localCopy;

        [NotKeyable] public Material material;
        [NotKeyable] public int pass;
        
        // FIXME you can't animate elements inside of arrays
        public AnimatedProperty[] animatedProperties;

        // this can be animated, since it's a serialaizsldflsble struct, but in an array, Unity just wont
        public AnimatedProperty property_test;
        private PostProcessAnimationController parent_controller;
        
        private void OnValidate()
        {
            if (parent_controller == null)
            {
                parent_controller = this.GetComponent<PostProcessAnimationController>();
            }
            // check for material change
            if (material != null)
            {
                if (localCopy == null)
                {
                    localCopy = new Material(material);
                }
                else if (localCopy.name != material.name)
                {
                    localCopy = new Material(material);
                }
            }
            
            parent_controller.RenderPostProcess(parent_controller.enabled);
        }
    }
    
    [CustomPropertyDrawer(typeof(AnimatedProperty))]
    public class AnimatedPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            // Split into two halves horizontally
            // Left half is property info, right half is value
            float left_half_field_width = (position.width / 2) / 3;
    
            var floatprop = property.FindPropertyRelative(nameof(AnimatedProperty.float_value));
            var colorprop = property.FindPropertyRelative(nameof(AnimatedProperty.color_value));
            
            EditorGUI.PropertyField(new Rect(position.x, position.y, left_half_field_width, position.height), property.FindPropertyRelative(nameof(AnimatedProperty.propertyType)), GUIContent.none);
            EditorGUI.LabelField(new Rect(position.x+left_half_field_width, position.y, left_half_field_width, position.height), "Name");
            EditorGUI.PropertyField(new Rect(position.x+left_half_field_width+50, position.y, left_half_field_width*1.45f, position.height), property.FindPropertyRelative(nameof(AnimatedProperty.PropertyName)), GUIContent.none);
            
            
            // the next part depends on the selected type
            // This rect will contain our whole value
            Rect value_rect = new Rect(position.x + position.width/2, position.y, position.width / 2, position.height);
            int selected_type = property.FindPropertyRelative(nameof(AnimatedProperty.propertyType)).intValue;
            float width_offset = value_rect.width * 0.3f;
            if (selected_type == (int)PropertyType.Float)
            {
                Rect field_rect = new Rect(value_rect.x + width_offset, value_rect.y, value_rect.width - width_offset, value_rect.height);
    
                EditorGUI.BeginProperty(field_rect, GUIContent.none, floatprop);
                EditorGUI.LabelField(new Rect(value_rect.x, value_rect.y, value_rect.width / 4, value_rect.height), "Value");
                EditorGUI.PropertyField(field_rect, floatprop, GUIContent.none);
                EditorGUI.EndProperty();
            }
    
            if (selected_type == (int)PropertyType.Color)
            {
                Rect field_rect = new Rect(value_rect.x + width_offset, value_rect.y, value_rect.width - width_offset, value_rect.height);
    
                EditorGUI.BeginProperty(field_rect, GUIContent.none, colorprop);
                EditorGUI.LabelField(new Rect(value_rect.x, value_rect.y, value_rect.width / 4, value_rect.height), "Value");
                EditorGUI.PropertyField(field_rect, colorprop, GUIContent.none);
                EditorGUI.EndProperty();
            }
    
            if (selected_type == (int)PropertyType.Vector)
            {
                float step = value_rect.width / 4.5f;
                float fieldW = step * 0.75f;
                float startX = value_rect.x;
                
                string[] labels = { "X", "Y", "Z", "W" };
                string[] channels = { "x", "y", "z", "w" };
                for (int i = 0; i < 4; i++)
                {
                    EditorGUI.LabelField(new Rect(startX + i * step, position.y, 50, position.height), labels[i]);
                    EditorGUI.PropertyField(new Rect(startX + i * step + 15, position.y, fieldW, position.height),
                        property.FindPropertyRelative(channels[i]), GUIContent.none);
                }
            }
            EditorGUI.EndProperty();
        }
    }
}