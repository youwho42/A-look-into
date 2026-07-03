Shader "Custom/SpriteSheetAnimation"
{
    Properties
    {
        _MainTex ("Sprite Sheet", 2D) = "white" {}
        _Columns ("Columns", Float) = 4
        _Rows ("Rows", Float) = 4
        _FPS ("Frames Per Second", Float) = 12
        _TotalFrames ("Total Frames", Float) = 16
        // Optional: manually override the frame (set _ManualFrame >= 0 to use)
        _ManualFrame ("Manual Frame Override (-1 = auto)", Float) = -1
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
            "PreviewType" = "Plane"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _Columns;
            float _Rows;
            float _FPS;
            float _TotalFrames;
            float _ManualFrame;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Pick frame: manual override or auto time-driven
                float frame;
                if (_ManualFrame >= 0)
                    frame = floor(_ManualFrame) % _TotalFrames;
                else
                    frame = floor(_Time.y * _FPS) % _TotalFrames;

                // Figure out which row and column this frame sits on
                float col = fmod(frame, _Columns);
                float row = floor(frame / _Columns);

                // Sprite sheets typically go top-left to bottom-right,
                // but UV origin in Unity is bottom-left, so flip the row
                row = (_Rows - 1) - row;

                // Scale UVs down to one cell, then offset to the right cell
                float2 cellSize = float2(1.0 / _Columns, 1.0 / _Rows);
                float2 uv = i.uv * cellSize;
                uv += float2(col * cellSize.x, row * cellSize.y);

                fixed4 col_out = tex2D(_MainTex, uv);

                // Respect vertex color (Unity UI tinting, alpha, etc.)
                col_out *= i.color;

                return col_out;
            }
            ENDCG
        }
    }
}
