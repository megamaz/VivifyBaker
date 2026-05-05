using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using VivifyBaker.Baker.Scripts.Constants;

namespace VivifyBaker.Baker.Scripts.Animatable
{
    [Serializable] [ExecuteInEditMode] [RequireComponent(typeof(MeshRenderer))]
    public class AnimatablePostProcessLayer : MonoBehaviour
    {
        [NotKeyable] public Material material;
        [NotKeyable] public int pass = -1;
        [HideInInspector] public MaterialPropertyBlock block;

        private PostProcessAnimationController parent_controller;
        private MeshRenderer own_smr;

        void Awake()
        {
            TryGetParent();
            // the MeshRenderer isn't added yet so we have to fetch it in OnValidate
        }
        
        void OnValidate()
        {
            if (!TryGetParent())
                return;
            if (own_smr == null)
            {
                own_smr = GetComponent<MeshRenderer>();
                own_smr.GetPropertyBlock(block);
            }
            
            own_smr.sharedMaterial = material;
            
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