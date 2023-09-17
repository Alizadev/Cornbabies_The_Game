// Simplified Additive Particle shader. Differences from regular Additive Particle one:
// - no Tint color
// - no Smooth particle support
// - no AlphaTest
// - no ColorMask

Shader "Mobile/Particles/Additive-Tint"
{
Properties {
    _Color ("Color", Color) = (0.5,0.5,0.5,1)
    _MainTex ("Particle Texture", 2D) = "white" {}
}
 
Category {
    Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
    Blend SrcAlpha One
    Cull Off Lighting Off ZWrite Off Fog { Mode off }
   
    BindChannels {
        Bind "Color", color
        Bind "Vertex", vertex
        Bind "TexCoord", texcoord
    }
   
    SubShader {
        Pass {
            SetTexture [_MainTex] {
                combine texture * primary
            }
           
            SetTexture [_MainTex]    {
                constantColor [_Color]
                combine previous * constant DOUBLE
            }
        }
    }
}
}