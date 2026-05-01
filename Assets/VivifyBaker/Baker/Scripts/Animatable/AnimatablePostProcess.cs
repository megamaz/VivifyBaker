using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace VivifyBaker.Baker.Scripts.Animatable
{
    [Serializable]
    public struct PostProcessLayer
    {
        public Material material;
        public int pass;
    }
    public class AnimatablePostProcess : MonoBehaviour
    {
        private static readonly int mainTexID = Shader.PropertyToID("_MainTex");
        
        public bool scene_view_enabled = false;
        public List<PostProcessLayer> post_processes;
        

        private Camera postProcessingCamera;
        private CommandBuffer cmd;
        
        void Awake()
        {
            postProcessingCamera = GetComponent<Camera>();
            cmd = new CommandBuffer();
        }
        
        #if UNITY_EDITOR
        private void OnValidate() => RenderPostProcess();
        #endif

        void Update()
        {
            RenderPostProcess();
        }
        
        private void RenderPostProcess()
        {
            if(postProcessingCamera == null)
                postProcessingCamera = GetComponent<Camera>();
            if(cmd == null)
                cmd = new CommandBuffer();
            cmd.name = "AnimatablePostProcessBuffer";
            postProcessingCamera.RemoveAllCommandBuffers();
            
            RenderTargetIdentifier source = new RenderTargetIdentifier(BuiltinRenderTextureType.CurrentActive);
            RenderTargetIdentifier destination = new RenderTargetIdentifier(BuiltinRenderTextureType.CameraTarget);
            RenderTargetIdentifier target = new RenderTargetIdentifier(mainTexID);
            
            cmd.GetTemporaryRT(mainTexID, -1, -1, 24, FilterMode.Bilinear, RenderTextureFormat.ARGB64);
            foreach (var post_process in post_processes)
            {
                if (post_process.material.HasProperty(mainTexID))
                {
                    cmd.Blit(source, target);
                    cmd.Blit(target, destination, post_process.material);
                }
            }
            cmd.ReleaseTemporaryRT(mainTexID);
            postProcessingCamera.AddCommandBuffer(CameraEvent.BeforeImageEffects, cmd);
        }
    }
}