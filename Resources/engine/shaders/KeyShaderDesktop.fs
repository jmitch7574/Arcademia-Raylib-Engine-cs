#version 330

in vec2 fragTexCoord;
in vec4 fragColor;
out vec4 finalColor;

uniform sampler2D texture0;
uniform vec4 colDiffuse;

uniform vec4 replaceColor0;
uniform vec4 replaceColor1;
uniform vec4 replaceColor2;
uniform float tolerance;

const vec3 targetColor0 = vec3(1.0, 0.0, 0.0); // Pure Red
const vec3 targetColor1 = vec3(0.0, 1.0, 0.0); // Pure Green
const vec3 targetColor2 = vec3(0.0, 0.0, 1.0); // Pure Blue

void main() {
    vec4 texelColor = texture(texture0, fragTexCoord);
    vec4 outColor = texelColor;
    
    if (distance(texelColor.rgb, targetColor0) < tolerance) {
        outColor = vec4(replaceColor0.rgb, texelColor.a);
    } else if (distance(texelColor.rgb, targetColor1) < tolerance) {
        outColor = vec4(replaceColor1.rgb, texelColor.a);
    } else if (distance(texelColor.rgb, targetColor2) < tolerance) {
        outColor = vec4(replaceColor2.rgb, texelColor.a);
    }
    
    finalColor = outColor * colDiffuse * fragColor;
}