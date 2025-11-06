#version 330 core
in vec2 texCoord;

uniform vec3 lightColor;
uniform vec3 objectColor;

out vec4 FragColor;

void main()
{
    FragColor = vec4(1.0);
}