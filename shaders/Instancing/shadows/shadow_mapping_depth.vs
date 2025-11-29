#version 330 core
layout (location = 0) in vec3 aPos;
layout (location = 3) in mat4 instanceMatrix;

uniform mat4 lightSpaceMatrix;
uniform mat4 model;

void main()
{
    // 转换顶点位置到光源空间
    mat4 modelMatrix = model == mat4(1.0) ? instanceMatrix : model;
    gl_Position = lightSpaceMatrix * modelMatrix * vec4(aPos, 1.0);
}
