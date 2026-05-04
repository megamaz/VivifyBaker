using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using VivifyBaker.Baker.Scripts.Constants;

namespace VivifyBaker.Baker.Scripts.Animatable
{
    [Serializable] [ExecuteInEditMode] [RequireComponent(typeof(SkinnedMeshRenderer))]
    public class AnimatablePostProcessLayer : MonoBehaviour
    {
        [HideInInspector] public Material localCopy;

        [NotKeyable] public Material material;
        [NotKeyable] public int pass = -1;

        private PostProcessAnimationController parent_controller;
        private SkinnedMeshRenderer own_smr;

        void Awake()
        {
            TryGetParent();
        }

        void Start()
        {
            // not sure when Unity adds the component when RequireComponent is added
            // so im doing this after awake 
            own_smr = GetComponent<SkinnedMeshRenderer>();
        }
        
        void OnValidate()
        {
            if (!TryGetParent())
                return;
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

                own_smr.material = localCopy;
            }
            
            parent_controller.RenderPostProcess(parent_controller.enabled);
        }

        private bool TryGetParent()
        {
            if (!transform.parent.TryGetComponent<PostProcessAnimationController>(out parent_controller))
            {
                Debug.LogError("Parent does not have PostProcessAnimationController.");
                return false;
            }

            return true;
        }
    }
}