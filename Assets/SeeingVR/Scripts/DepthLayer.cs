// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// Modified from Unity Standard assets
// This file is based on PostEffectsBase, a class from Legacy Image Effects asset
// https://assetstore.unity.com/packages/essentials/legacy-image-effects-83913

using UnityEngine;


namespace UnityStandardAssets.ImageEffects
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]

    public class DepthLayer : PostEffectsBase
    {

        public Shader depthShader;
        private Material depthMaterial = null;

        public override bool CheckResources()
        {
            CheckSupport(true);

            depthMaterial = CheckShaderAndCreateMaterial(depthShader, depthMaterial);

            if (!isSupported)
                ReportAutoDisable();
            return isSupported;
        }


        new void Start()
        {

        }

        void SetCameraFlag()
        {
            GetComponent<Camera>().depthTextureMode |= DepthTextureMode.DepthNormals;
        }

        void OnEnable()
        {
            SetCameraFlag();
        }

        [ImageEffectOpaque]
        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (CheckResources() == false)
            {
                Graphics.Blit(source, destination);
                return;
            }

            Graphics.Blit(source, destination, depthMaterial);
        }
    }
}

