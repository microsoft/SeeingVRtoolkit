// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// Modified from Unity Standard Assets
// This class is based on the class PostEffectsBase from the Legacy Image Effects assets
// https://assetstore.unity.com/packages/essentials/legacy-image-effects-83913

using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    
    public class BrightnessAdjustment : PostEffectsBase
    {
        
        public float intensity = 0.5f;

        
        private Material brightnessMaterial;

        
        public Shader brightnessShader = null;


        public override bool CheckResources()
        {
            CheckSupport(false);

            brightnessMaterial = CheckShaderAndCreateMaterial(brightnessShader, brightnessMaterial);
           

            if (!isSupported)
                ReportAutoDisable();
            return isSupported;
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (CheckResources() == false)
            {
                Graphics.Blit(source, destination);
                return;
            }
            brightnessMaterial.SetFloat("intensity", intensity);

            Graphics.Blit(source, destination, brightnessMaterial);
        }
    }
}
