#version 330 core
layout (location = 0) out vec3 gPosition;
layout (location = 1) out vec3 gNormal;
// layout (location = 2) out vec4 gAlbedo;
layout (location = 2) out vec4 gAlbedoSpec;

uniform sampler2D texture_diffuse1;
uniform sampler2D texture_specular1;

uniform bool cube;

in vec2 TexCoords;
in vec3 FragPos;
in vec3 Normal;

void main()
{
    // store the fragment position vector in the first gbuffer texture
    gPosition = FragPos;
    // also store the per-fragment normals into the gbuffer
    gNormal = normalize(Normal);
    // and the diffuse per-fragment color
    if(cube) {
        gAlbedoSpec.rgb = vec3(0.95);
        gAlbedoSpec.a = 1.0;
    } else {
        gAlbedoSpec.rgb = texture(texture_diffuse1, TexCoords).rgb;
        gAlbedoSpec.a = texture(texture_specular1, TexCoords).r;
    }
}
