using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VivifyBaker.Baker.Scripts.Editor.Utility;
using VivifyBaker.Baker.Scripts.Constants;

namespace VivifyBaker.Baker.Scripts.Editor.MaterialBaker
{
    public class BakedMaterialProperty<T>
    {
        public string ID;
        public string Type;
        public PointDefinition<T> Points;
    }
    
    public class MaterialBaker
    {
        private MaterialBakeSettings _settings;

        public static object[] GetBakeResults(MaterialBakeSettings settings)
        {
            MaterialBaker baker = new MaterialBaker(settings);
            return baker.Bake();
        }
        
        private MaterialBaker(MaterialBakeSettings settings)
        {
            _settings = settings;
        }
        private object[] Bake()
        {
            List<object> baked_properties = new List<object>(); // since each property will have a different subtype
            foreach (string property in _settings.PropertyNames)
            {
                PropertyType? property_type = ResolvePropertyType(property);
                if (property_type == null)
                {
                    Debug.LogWarning($"{property} not found in clip.");
                    continue;
                }

                switch (property_type)
                {
                    case PropertyType.Color:
                    case PropertyType.Vector:
                        baked_properties.Add(BakeVectorProperty(property,  property_type.ToString()));
                        break;
                    case PropertyType.Float:
                        baked_properties.Add(BakeFloatProperty(property));
                        break;
                }
            }
            return baked_properties.ToArray();
        }

        /// <summary>
        ///     Bakes all properties that are 4 parameters long, with an optional fourth.
        /// The baker ignores optional however, and animates all four, since all four are provided.
        /// </summary>
        /// <param name="propertyName">The property from the animation clip to bake.</param>
        /// <returns>A BakedMaterialProperty object.</returns>
        private BakedMaterialProperty<Vector4> BakeVectorProperty(string propertyName, string type)
        {
            // The type will always be MeshRenderer since we're animating a material.
            // has to be an int in the end
            int frame_count = (int)(_settings.Clip.length * _settings.Clip.frameRate);
            BakedMaterialProperty<Vector4> baked_properties = new BakedMaterialProperty<Vector4>();
            baked_properties.ID = propertyName;
            baked_properties.Type = type;
            
            PointDefinition<Vector4> points = new PointDefinition<Vector4>();
            List<Point<Vector4>> points_list = new List<Point<Vector4>>();
            // there's probably a better way of doing this
            string indexes = type == "Color" ? "rgba" : "xyzw";
            string x = indexes.ElementAt(0).ToString();
            string y = indexes.ElementAt(1).ToString();
            string z = indexes.ElementAt(2).ToString();
            string w = indexes.ElementAt(3).ToString();
            for (int f = 0; f < frame_count; f++)
            {
                float value_x = AnimationSampler.GetPropertyValueAtFrame(_settings.Clip,
                    $"material.{propertyName}.{x}", _settings.ObjectName, typeof(MeshRenderer), f);
                float value_y = AnimationSampler.GetPropertyValueAtFrame(_settings.Clip,
                    $"material.{propertyName}.{y}", _settings.ObjectName, typeof(MeshRenderer), f);
                float value_z = AnimationSampler.GetPropertyValueAtFrame(_settings.Clip,
                    $"material.{propertyName}.{z}", _settings.ObjectName, typeof(MeshRenderer), f);
                float value_w = AnimationSampler.GetPropertyValueAtFrame(_settings.Clip,
                    $"material.{propertyName}.{w}", _settings.ObjectName, typeof(MeshRenderer), f);
                Vector4 values = new Vector4(value_x, value_y, value_z, value_w);
                float time = (float)f / (float)frame_count;
                Point<Vector4> new_point = new Point<Vector4>();
                new_point._values = values;
                new_point._time = time;
                points_list.Add(new_point);
            }
            points.Points = points_list.ToArray();
            baked_properties.Points = points;
            return baked_properties;
        }

        private BakedMaterialProperty<float> BakeFloatProperty(string propertyName)
        {
            int frame_count = (int)(_settings.Clip.length * _settings.Clip.frameRate);
            BakedMaterialProperty<float> baked_properties = new BakedMaterialProperty<float>();
            baked_properties.ID = propertyName;
            baked_properties.Type = "Float";
            PointDefinition<float> points = new PointDefinition<float>();
            List<Point<float>> points_list = new List<Point<float>>();
            for (int f = 0; f < frame_count; f++)
            {
                float value = AnimationSampler.GetPropertyValueAtFrame(_settings.Clip, propertyName, _settings.ObjectName, typeof(MeshRenderer), f);
                float time = (float)f / (float)frame_count;
                Point<float> new_point = new Point<float>();
                new_point._values = value;
                new_point._time = time;
                points_list.Add(new_point);
            }
            points.Points = points_list.ToArray();
            baked_properties.Points = points;
            return baked_properties;
        }
        
        private PropertyType? ResolvePropertyType(string propertyName)
        {
            // vector / color bindings have 4 parameters, we'll check for RGBA and XYZW in the end of each string to tell which is which.
            // Everything else is just float.
            EditorCurveBinding[] bindings = AnimationUtility.GetCurveBindings(_settings.Clip);
            EditorCurveBinding[] property_bindings = bindings.Where(x => x.propertyName.Contains(propertyName)).ToArray();
            if (property_bindings.Length == 0)
                return null;
            
            if (property_bindings.Length == 4)
            {
                // these will always be in order
                if(property_bindings[0].propertyName.EndsWith(".r"))
                    return PropertyType.Color;
                if (property_bindings[0].propertyName.EndsWith(".x"))
                    return PropertyType.Vector;
            }
            return PropertyType.Float;
        }
    }
}