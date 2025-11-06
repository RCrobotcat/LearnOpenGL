#version 330 core
in vec3 PhongColor;

uniform vec3 objectColor;

out vec4 FragColor;

void main()
{
    vec3 result = PhongColor * objectColor;
    FragColor = vec4(result, 1.0);
}