Shader "Custom/Water"
{
    Properties
    {
        // Color of the water
        _Color("Color", Color) = (1,1,1,1)
        
        // The DuDv map texture
        _DuDvMap("DuDv Map", 2D) = "bump" {}
        
        // Strength of the distortion effect
        _DistortionStrength("Distortion Strength", Float) = 0.05
        
        // Speed of the distortion animation
        _ScrollSpeed("Scroll Speed", Vector) = (0.05, 0.05, 0, 0)
    }
    SubShader
    {
        // This tag is important for URP to recognize the shader
        Tags 
        { 
            "RenderType" = "Transparent" 
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Transparent" 
        }
        
        // We need to grab the screen behind the object before rendering this shader
        // In URP, this is done with a "Blit" via the _CameraOpaqueTexture, which we access below.
        
        Pass
        {
            Name "WaterRefraction"
            Tags { "LightMode" = "UniversalForward" } // This is the standard pass for URP
            
            // Enable alpha blending for transparency
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off // Disable depth writing for transparent objects
            
            HLSLPROGRAM
            
            // Includes necessary URP functions and macros
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            // Register our properties
            #pragma vertex vert
            #pragma fragment frag
            
            // Texture & Samplers
            TEXTURE2D(_DuDvMap);
            SAMPLER(sampler_DuDvMap);
            
            // This is the key: URP provides a texture of the scene before transparent objects are drawn.
            // This is our equivalent of GrabPass.
            TEXTURE2D(_CameraOpaqueTexture);
            SAMPLER(sampler_CameraOpaqueTexture);
            
            // Properties from the Inspector
            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float4 _DuDvMap_ST; // Needed for texture tiling/offset
                float _DistortionStrength;
                float2 _ScrollSpeed;
            CBUFFER_END
            
            // Data passed from the vertex shader to the fragment shader
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 screenPos : TEXCOORD1; // Used to calculate UVs for the screen texture
            };
            
            // The vertex shader
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                
                // Transform object space position to clip space
                VertexPositionInputs vertexInput = GetVertexPositionInputs(IN.positionOS.xyz);
                OUT.positionHCS = vertexInput.positionCS;
                
                // Apply tiling and offset to the DuDv UVs
                OUT.uv = TRANSFORM_TEX(IN.uv, _DuDvMap);
                
                // Calculate the screen position for use in the fragment shader
                OUT.screenPos = ComputeScreenPos(OUT.positionHCS);
                
                return OUT;
            }
            
            // The fragment shader (where the main effect happens)
            half4 frag(Varyings IN) : SV_Target
            {
                // Animate the DuDv map UVs by adding time-based scrolling
                float2 scrolledUV = IN.uv + _ScrollSpeed * _Time.y;
                
                // Sample the DuDv map
                half4 dudvColor = SAMPLE_TEXTURE2D(_DuDvMap, sampler_DuDvMap, scrolledUV);
                
                // Remap the DuDv values from [0, 1] to [-1, 1] to get distortion vectors
                // THIS IS THE "*2 - 1" PART WE TALKED ABOUT
                half2 distortion = (dudvColor.rg * 2 - 1) * _DistortionStrength;
                
                // Calculate the UV coordinates for the screen texture
                // We need to 'undo' the perspective division from the projection
                float2 screenUV = (IN.screenPos.xy / IN.screenPos.w);
                
                // Apply our distortion to the screen UVs
                screenUV += distortion;
                
                // Sample the screen texture (the scene behind this object) using the distorted UVs
                half4 sceneColor = SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, screenUV);
                
                // Combine the refracted scene color with the water's tint color
                half4 finalColor = sceneColor * _Color;
                
                return finalColor;
            }
            ENDHLSL
        }
    }
}