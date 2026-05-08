using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace VivifyBaker.Baker.Scripts.Components.Bakers.MaterialBaker
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
}