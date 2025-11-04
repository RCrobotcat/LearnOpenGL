#version 330 core
in vec2 texCoord;

uniform sampler2D texture1;
uniform sampler2D texture2;

uniform float mixT;

out vec4 FragColor;

void main()
{
    // mix(x, y, a) 是一个非常常用的 线性插值（Linear Interpolation, LERP）函数
    // mix(x, y, a) = x * (1 − a) + y * a
    FragColor = mix(texture(texture1, texCoord), texture(texture2,  vec2(1.0 - texCoord.x, texCoord.y)), mixT);
}