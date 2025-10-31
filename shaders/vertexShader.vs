#version 330 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aColor;
layout (location = 2) in vec2 aTexCoord;

out vec3 ourColor;
out vec4 vertexPosition;
out vec2 texCoord;

uniform float translationX;
uniform float translationY;

void main()
{
    // 沿 X 轴平移 translationX (转置后的矩阵)
    mat4 T = mat4(
        1.0, 0.0, 0.0, 0.0,
        0.0, 1.0, 0.0, 0.0,
        0.0, 0.0, 1.0, 0.0,
        translationX, translationY, 0.0, 1.0
    );

    // 平移后再旋转
    vec4 finalPos = T * vec4(aPos, 1.0);
    vertexPosition = finalPos;

    gl_Position = finalPos;
    ourColor = aColor;
    texCoord = aTexCoord;
}
