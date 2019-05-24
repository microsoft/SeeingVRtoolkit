// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// Modified from Unity standard assets
// This file is based on PostEffectsBase class from the Legacy Image Effects assets
// https://assetstore.unity.com/packages/essentials/legacy-image-effects-83913

using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
    [RequireComponent(typeof(Camera))]
    public class magnificationEffect : PostEffectsBase
    {
        public Shader magnificationShader;
        private Material magMaterial = null;
        public float magnificationLevel = 1;
        public override bool CheckResources()
        {
            CheckSupport(true);

            magMaterial = CheckShaderAndCreateMaterial(magnificationShader, magMaterial);

            if (!isSupported)
                ReportAutoDisable();
            return isSupported;
        }


        [ImageEffectOpaque]
        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (CheckResources() == false)
            {
                Graphics.Blit(source, destination);
                return;
            }


            magMaterial.SetFloat("_Magnification", magnificationLevel);

            Graphics.Blit(source, destination, magMaterial);
        }
    }
}


