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
uniform sampler2D texture_height1;
uniform float shininess;

uniform vec3 cameraPos;
uniform samplerCube skybox;

void main()
{
    // unpack normal map
    vec3 tangentNormal = texture(texture_normal1, TexCoords).rgb;
    tangentNormal = tangentNormal * 2.0 - 1.0;  // [0,1] → [-1,1]
    vec3 norm = normalize(TBN * tangentNormal);

    // reflection
    vec3 viewDir = normalize(FragPos - cameraPos);
    vec3 reflectDir = reflect(viewDir, norm);
    vec3 refl = texture(skybox, reflectDir).rgb;
    vec3 reflection = vec3(texture(texture_height1, TexCoords)) * refl;

    // base color
    vec3 albedo = texture(texture_diffuse1, TexCoords).rgb;

    vec3 finalColor = albedo + reflection;

    FragColor = vec4(finalColor, 1.0);
}
