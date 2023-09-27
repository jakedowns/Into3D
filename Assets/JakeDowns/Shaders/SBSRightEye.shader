Shader "JakeDowns/SBSRightEye"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "black" {}
        _AspectRatio ("Aspect Ratio", Range(0, 100)) = 1
        _Gamma("Gamma", Range(0.1, 10)) = 1
        _Brightness("Brightness", Range(0, 2)) = 0.5
        _Contrast("Contrast", Range(0, 2)) = 1
        _CyanRed("Cyan-Red", Range(-1, 1)) = 0
        _MagentaGreen("Magenta-Green", Range(-1, 1)) = 0
        _YellowBlue("Yellow-Blue", Range(-1, 1)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        

        Pass
        {
            // Disable culling for this Pass.
            // You would typically do this for special effects, such as transparent objects or double-sided walls.
            //Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            //#include "MyCustomFunctions.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _AspectRatio;
            float _Gamma;
            float _Brightness;
            float _Contrast;
            float _CyanRed;
            float _MagentaGreen;
            float _YellowBlue;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 coord = i.uv;
                // adjust coord.x to sample just the right half of the texture
                coord.x = (coord.x * 0.5) + 0.5;

                // Calculate the UV offset based on the aspect ratio difference
                float2 uvOffset = float2(0.0, 0.0);

                float inputAspectRatio = _AspectRatio;
                float outputAspectRatio = _ScreenParams.x / _ScreenParams.y;
                if (inputAspectRatio < 1.75)
                {
                    //uvOffset.x -= (inputAspectRatio - outputAspectRatio) * 0.25;
                }

                // Sample the input texture using the adjusted UV coordinates
                coord = coord + uvOffset;

                //

                // sample the texture
                fixed4 col = tex2D(_MainTex, coord.xy);

                // Perform Color Balance Adjustments
                /*fixed3 lab = RGBToLAB(col.rgb);
                lab.b += _CyanRed;
                lab.g += _MagentaGreen;
                lab.r += _YellowBlue;
                col.rgb = LABToRGB(lab);*/
                col.r += _CyanRed;
                col.g += _MagentaGreen;
                col.b += _YellowBlue;

                // Apply gamma correction
                col.rgb = pow(col.rgb, float3(
                    1.0 / _Gamma, 
                    1.0 / _Gamma, 
                    1.0 / _Gamma
                ));

                // Apply brightness correction
                col *= _Brightness;
                col = clamp(col, 0, 1);

                // Contrast
                // Calculate the midpoint value
                float midpoint = 0.5;

                // Scale the color values around the midpoint
                col.rgb = (col.rgb - midpoint) * _Contrast + midpoint;

                // Clamp the col values to ensure they remain within the range of -1 to 1
                col = clamp(col, -1.0, 1.0);

                return col;
            }
            ENDCG
        }
    }
}
