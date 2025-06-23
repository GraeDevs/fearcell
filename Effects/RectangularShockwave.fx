// Texture and sampler
Texture2D uImage0 : register(t0);
sampler2D uImage0Sampler : register(s0)
{
    Texture = (uImage0);
    MagFilter = LINEAR;
    MinFilter = LINEAR;
    AddressU = WRAP;
    AddressV = WRAP;
};

// Uniform parameters
float2 uScreenResolution;
float2 uScreenPosition;
float2 uTargetPosition;
float2 uDirection;
float uOpacity;
float uTime;
float uIntensity;
float uProgress;
float2 uImageSize1;
float2 uImageOffset;
float uSaturation;
float4 uSourceRect;
float uZoom;
float uSpeed;

// Additional parameters for rectangular effect
float2 uRectSize; // Width and height of the rectangle
float uBorderWidth; // Thickness of the shockwave border

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
    float2 ScreenPos : TEXCOORD1;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;

    output.Position = mul(input.Position, MatrixTransform);
    output.Color = input.Color;
    output.TextureCoordinates = input.TextureCoordinates;
    output.ScreenPos = input.Position.xy;

    return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    // Get the base texture color
    float4 color = tex2D(uImage0Sampler, input.TextureCoordinates);
    
    // Calculate screen position relative to target
    float2 screenPos = (input.ScreenPos - uTargetPosition) / uScreenResolution;
    
    // Calculate distance from center for rectangular shockwave
    float2 rectCenter = float2(0.0, 0.0);
    float2 distanceToEdge = abs(screenPos - rectCenter) - (uRectSize * 0.5);
    
    // Distance to rectangle border (negative inside, positive outside)
    float distanceToRect = length(max(distanceToEdge, 0.0)) + min(max(distanceToEdge.x, distanceToEdge.y), 0.0);
    
    // Create the shockwave effect
    float shockwave = uProgress * uIntensity;
    float shockwaveDistance = abs(distanceToRect - shockwave);
    
    // Create the distortion effect
    float distortionStrength = 1.0 - smoothstep(0.0, uBorderWidth, shockwaveDistance);
    distortionStrength *= smoothstep(0.0, 0.1, shockwave); // Fade in
    distortionStrength *= smoothstep(1.0, 0.8, uProgress); // Fade out
    
    // Apply rectangular distortion
    float2 normal = float2(0.0, 0.0);
    if (distanceToEdge.x > distanceToEdge.y)
    {
        normal.x = sign(screenPos.x - rectCenter.x);
    }
    else
    {
        normal.y = sign(screenPos.y - rectCenter.y);
    }
    
    // Distort texture coordinates
    float2 distortedUV = input.TextureCoordinates + normal * distortionStrength * uIntensity * 0.1;
    float4 distortedColor = tex2D(uImage0Sampler, distortedUV);
    
    // Mix original and distorted color
    color = lerp(color, distortedColor, distortionStrength);
    
    // Add shockwave highlight
    float highlight = distortionStrength * 0.5;
    color.rgb += highlight;
    
    // Apply opacity
    color.a *= uOpacity;
    
    return color * input.Color;
}

technique ShockwavePass
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}