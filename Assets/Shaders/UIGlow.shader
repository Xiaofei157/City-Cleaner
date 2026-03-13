Shader "UI/TalentNodeGlow"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint Color", Color) = (1,1,1,1)
        
        [Header(Glow Settings)]
        [Toggle(_GLOW_ON)] _GlowOn ("Enable Glow", Float) = 1
        _GlowWidth ("Glow Width", Range(0, 50)) = 15
        _GlowColor ("Glow Color", Color) = (0, 0.5, 1, 1)
        _GlowIntensity ("Glow Intensity", Range(0, 5)) = 2
        _GlowSoftness ("Glow Softness", Range(0, 1)) = 0.5
        
        [Header(Stencil)]
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        
        [Header(Rect Mask)]
        _ClipRect ("Clip Rect", Vector) = (-32767, -32767, 32767, 32767)
        
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }
    
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }
        
        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
        
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask RGB
        
        Pass
        {
            Name "Default"
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"
            
            #pragma multi_compile __ UNITY_UI_CLIP_RECT
            #pragma multi_compile __ UNITY_UI_ALPHACLIP
            #pragma multi_compile _ _GLOW_ON
            
            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct v2f
            {
                float4 vertex       : SV_POSITION;
                fixed4 color        : COLOR;
                float2 texcoord     : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_TexelSize;
            
            // Glow properties
            float _GlowWidth;
            fixed4 _GlowColor;
            float _GlowIntensity;
            float _GlowSoftness;
            
            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
                OUT.texcoord = v.texcoord;
                OUT.color = v.color * _Color;
                return OUT;
            }
            
            // 计算到图像边缘的距离
            half CalculateGlow(sampler2D tex, float2 uv)
            {
                half glow = 0;
                
                // 8 方向采样，固定步长
                half2 offsets[8] = {
                    half2(_GlowWidth, 0),
                    half2(-_GlowWidth, 0),
                    half2(0, _GlowWidth),
                    half2(0, -_GlowWidth),
                    half2(_GlowWidth * 0.707, _GlowWidth * 0.707),
                    half2(-_GlowWidth * 0.707, _GlowWidth * 0.707),
                    half2(-_GlowWidth * 0.707, -_GlowWidth * 0.707),
                    half2(_GlowWidth * 0.707, -_GlowWidth * 0.707)
                };
                
                // 手动展开循环，避免编译器错误
                glow += tex2D(tex, uv + offsets[0] * _MainTex_TexelSize.xy).a;
                glow += tex2D(tex, uv + offsets[1] * _MainTex_TexelSize.xy).a;
                glow += tex2D(tex, uv + offsets[2] * _MainTex_TexelSize.xy).a;
                glow += tex2D(tex, uv + offsets[3] * _MainTex_TexelSize.xy).a;
                glow += tex2D(tex, uv + offsets[4] * _MainTex_TexelSize.xy).a;
                glow += tex2D(tex, uv + offsets[5] * _MainTex_TexelSize.xy).a;
                glow += tex2D(tex, uv + offsets[6] * _MainTex_TexelSize.xy).a;
                glow += tex2D(tex, uv + offsets[7] * _MainTex_TexelSize.xy).a;
                
                return glow / 8.0;
            }
            
            fixed4 frag(v2f IN) : SV_Target
            {
                // 采样原始颜色
                half4 originalColor = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;
                
                // UI 矩形裁剪
                #ifdef UNITY_UI_CLIP_RECT
                originalColor.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif
                
                // UI Alpha 裁剪
                #ifdef UNITY_UI_ALPHACLIP
                clip(originalColor.a - 0.001);
                #endif
                
                // 只有当 Glow 被启用时才应用效果
                #ifdef _GLOW_ON
                // 计算发光强度
                half glowIntensity = CalculateGlow(_MainTex, IN.texcoord);
                
                if (glowIntensity > 0.1)
                {
                    // 如果是透明区域，只显示发光
                    if (originalColor.a < 0.1)
                    {
                        half finalAlpha = glowIntensity * _GlowColor.a;
                        fixed3 glowResult = _GlowColor.rgb * _GlowIntensity * glowIntensity;
                        return fixed4(glowResult, finalAlpha);
                    }
                    else
                    {
                        // 如果是不透明区域，混合原始颜色和发光
                        half mixFactor = glowIntensity * 0.3;
                        fixed3 mixedColor = lerp(originalColor.rgb, _GlowColor.rgb, mixFactor);
                        return fixed4(mixedColor, originalColor.a);
                    }
                }
                #endif
                
                // 如果 Glow 未启用，或者不在发光范围内，返回原始颜色
                return originalColor;
            }
            ENDCG
        }
    }
    
    FallBack "UI/Default"
}
