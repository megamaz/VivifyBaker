using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using VivifyBaker.Baker.Scripts.Bakers.Components;

namespace VivifyBaker.Baker.Scripts.Animatable
{
    [ExecuteAlways]
    public class PostProcessAnimationController : MonoBehaviour
    {
        private static readonly int mainTexID = Shader.PropertyToID("_MainTex");
        private static readonly string bufferName = "AnimatablePostProcessBuffer";
        
        // since the camera probably won't be attached to the animated object
        // let the user decide which camera to apply this to
        public Camera postProcessingCamera; 
        
        
        public bool scene_view_enabled = false;
        
        private CommandBuffer cmd;
        
        #if UNITY_EDITOR
        void OnValidate() => RenderPostProcess(this.enabled);
        #endif
        
        void LateUpdate() => RenderPostProcess(this.enabled);
        
        
        void OnDisable() => RenderPostProcess(false);

        void OnDestroy() => RenderPostProcess(false);
        
        
        public void RenderPostProcess(bool enabled)
        {
            // Debug.Log(enabled);
            if (postProcessingCamera == null)
            {
                Debug.LogWarning("Camera Not Set");
                return;
            }
            
            if (postProcessingCamera.GetCommandBuffers(CameraEvent.BeforeImageEffects).Any(c => c.name == bufferName))
            {
                foreach (var buff in postProcessingCamera.GetCommandBuffers(CameraEvent.BeforeImageEffects).Where(buff => bufferName == buff.name))
                {
                    postProcessingCamera.RemoveCommandBuffer(CameraEvent.BeforeImageEffects, buff);
                }   
            }
            
            #if UNITY_EDITOR
            foreach (SceneView view in SceneView.sceneViews)
            {
                Camera viewCamera = view.camera;
                if (viewCamera.GetCommandBuffers(CameraEvent.BeforeImageEffects).Any(buff => bufferName == buff.name))
                {
                    viewCamera.RemoveCommandBuffer(CameraEvent.BeforeImageEffects, cmd);
                }
            }
            #endif

            if (!enabled)
            {
                cmd.Release();
                return;
            }
            
            cmd = new CommandBuffer();
            cmd.name = bufferName;
            
            RenderTargetIdentifier source = new RenderTargetIdentifier(BuiltinRenderTextureType.CurrentActive);
            RenderTargetIdentifier destination = new RenderTargetIdentifier(BuiltinRenderTextureType.CameraTarget);
            RenderTargetIdentifier target = new RenderTargetIdentifier(mainTexID);
            
            cmd.GetTemporaryRT(mainTexID, -1, -1, 24, FilterMode.Bilinear, RenderTextureFormat.ARGB64);
            foreach (var post_process in GetComponentsInChildren<AnimatablePostProcessLayer>())
            {
                if(post_process == null || post_process.material == null)
                    continue;
                if (post_process.material.HasProperty(mainTexID))
                {
                    MeshRenderer mr = post_process.GetComponent<MeshRenderer>();
                    Material rendermat = mr.sharedMaterial;
                    #if UNITY_EDITOR
                    if (AnimationMode.InAnimationMode())
                    {
                        rendermat = new Material(mr.sharedMaterial.shader);
                        MaterialPropertyBlock block = new MaterialPropertyBlock();
                        mr.GetPropertyBlock(block);
                        CopyAnimatedProperties(block, rendermat);
                    }
                    #endif
                    cmd.Blit(source, target);
                    cmd.Blit(target, destination, rendermat, post_process.pass < 0 ? -1 : post_process.pass);
                }
            }
            cmd.ReleaseTemporaryRT(mainTexID);
            postProcessingCamera.AddCommandBuffer(CameraEvent.BeforeImageEffects, cmd);
            
            #if UNITY_EDITOR
            if (scene_view_enabled)
            {
                foreach (SceneView view in SceneView.sceneViews)
                {
                    Camera viewCamera = view.camera;
                    viewCamera.AddCommandBuffer(CameraEvent.BeforeImageEffects, cmd);
                }
            }
            #endif
        }
        
        void CopyAnimatedProperties(MaterialPropertyBlock block, Material dst)
        {
            // they should have the same shader so it shouldn't matter if i use dst or src
            Shader shader = dst.shader;

            int count = shader.GetPropertyCount();

            for (int i = 0; i < count; i++)
            {
                int id = shader.GetPropertyNameId(i);
                switch (shader.GetPropertyType(i))
                {
                    case ShaderPropertyType.Float:
                    case ShaderPropertyType.Range:
                        dst.SetFloat(id, block.GetFloat(id));
                        break;
                    case ShaderPropertyType.Color:
                        dst.SetColor(id, block.GetColor(id));
                        break;
                    case ShaderPropertyType.Vector:
                        dst.SetVector(id, block.GetVector(id));
                        break;
                    case ShaderPropertyType.Texture:
                        dst.SetTexture(id, block.GetTexture(id));
                        break;
                    default:
                        break;
                }
            }
        }
        
        #if UNITY_EDITOR
        public void AddLayer()
        {
            GameObject new_layer = new GameObject($"Post Process Layer {transform.childCount+1}");
            new_layer.transform.SetParent(transform);
            
            new_layer.AddComponent<MeshRenderer>();
            new_layer.AddComponent<AnimatablePostProcessLayer>();
            new_layer.AddComponent<MaterialBakerComponent>();
            
            Selection.activeGameObject = new_layer;
        }
        #endif
    }
}