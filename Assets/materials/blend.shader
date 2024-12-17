Shader "Custom/BlendAdditive"
{
    Properties
    {
        _MainTex ("Video Texture", 2D) = "white" {}
        _BlendColor ("Blend Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Blend One One // Define o modo aditivo
        ZWrite Off    // Evita problemas de profundidade
        Cull Off      // Renderiza ambos os lados

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            fixed4 _BlendColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv); // Busca a cor do vídeo
                return col * _BlendColor;          // Aplica cor de blend
            }
            ENDCG
        }
    }
}
