#version 330

in vec2 fragTexCoord;
in vec4 fragColor;

out vec4 finalColor;

uniform sampler2D texture0;
uniform vec4 colDiffuse;

uniform vec2 renderSize;  // Screen/RenderTexture resolution
uniform float glowRadius; // Radius in pixels
uniform float glowStrength; // Glow Strength

void main() {
    vec2 texelSize = 1.0 / renderSize;
    vec4 coreColor = texture(texture0, fragTexCoord);
    
    // Sample surrounding texels to calculate blur/glow
    vec4 colorSum = vec4(0.0);
    float samples = 0.0;

    for (float x = -glowRadius; x <= glowRadius; x += 1.0) {
        for (float y = -glowRadius; y <= glowRadius; y += 1.0) {
            vec2 offset = vec2(x, y) * texelSize;
            colorSum += texture(texture0, fragTexCoord + offset);
            samples += 1.0;
        }
    }

    float divisions = (samples / glowStrength);

    if (divisions == 0) divisions = 1;

    finalColor = colorSum / divisions;
}