#version 330 core
out vec4 FragColor;

in vec3 FragPos;
in vec2 TexCoords;
in vec3 Tangent;
in vec3 BiTangent;
in mat3 TBN;

uniform sampler2D texture_diffuse1;
uniform sampler2D texture_normal1;
uniform sampler2D texture_specular1;
uniform float shininess;

uniform vec3 cameraPos;
uniform samplerCube skybox;

void main()
{
    // unpack normal map
    vec3 tangentNormal = texture(texture_normal1, TexCoords).rgb;
    tangentNormal = tangentNormal * 2.0 - 1.0;  // [0,1] → [-1,1]
    vec3 norm = normalize(TBN * tangentNormal);

    float ratio = 1.00 / 1.52;
    vec3 I = normalize(FragPos - cameraPos);
    vec3 R = refract(I, norm, ratio);

    FragColor = vec4(texture(skybox, R).rgb, 1.0);
}
