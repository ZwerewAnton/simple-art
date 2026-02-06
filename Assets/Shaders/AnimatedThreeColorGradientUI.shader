Shader "UI/AnimatedThreeColorGradient"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _LeftColorA ("Left Color A", Color) = (1,0,0,1)
        _LeftColorB ("Left Color B", Color) = (1,1,0,1)

        _CenterColor ("Center Color", Color) = (1,1,1,1)

        _RightColorA ("Right Color A", Color) = (0,1,1,1)
        _RightColorB ("Right Color B", Color) = (0,0,1,1)

        _Speed ("Animation Speed", Float) = 1
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref 1
            Comp Always
            Pass Keep
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "UI"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 uv       : TEXCOORD0;
                float4 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            fixed4 _Color;

            fixed4 _LeftColorA;
            fixed4 _LeftColorB;
            fixed4 _CenterColor;
            fixed4 _RightColorA;
            fixed4 _RightColorB;

            float _Speed;
            float4 _ClipRect;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                o.color = v.color * _Color;
                o.worldPos = v.vertex;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sprite texture
                fixed4 tex = tex2D(_MainTex, i.uv) * i.color;

                // Mask support
                tex.a *= UnityGet2DClipping(i.worldPos.xy, _ClipRect);

                // Time animation
                float t = (sin(_Time.y * _Speed) + 1) * 0.5;

                // Animated edge colors
                fixed4 leftColor  = lerp(_LeftColorA,  _LeftColorB,  t);
                fixed4 rightColor = lerp(_RightColorA, _RightColorB, t);

                // UV.x split
                float x = i.uv.x;

                fixed4 gradientColor;

                if (x < 0.5)
                {
                    float lt = saturate(x / 0.5);
                    gradientColor = lerp(leftColor, _CenterColor, lt);
                }
                else
                {
                    float rt = saturate((x - 0.5) / 0.5);
                    gradientColor = lerp(_CenterColor, rightColor, rt);
                }

                tex.rgb *= gradientColor.rgb;

                return tex;
            }
            ENDCG
        }
    }
}
