Shader "Vertex Colored"
{
    Properties
    {
        _Emission("Emmisive Color", Color) = (0,0,0,0)
    }

        SubShader
    {
        Pass
        {
            Material
            {
                Emission[_Emission]
            }

            ColorMaterial AmbientAndDiffuse
            Lighting On
        }
    }
        Fallback "VertexLit", 1
}