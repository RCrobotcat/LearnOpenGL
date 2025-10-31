### 1. 修改片段着色器，仅让笑脸图案朝另一个方向看
- `fragmentShader.fs`
```glsl
#version 330 core
in vec3 ourColor;
in vec4 vertexPosition;
in vec2 texCoord;

uniform sampler2D texture1;
uniform sampler2D texture2;

out vec4 FragColor;

void main()
{
    // mix(x, y, a) 是一个非常常用的 线性插值（Linear Interpolation, LERP）函数
    // mix(x, y, a) = x * (1 − a) + y * a
    FragColor = mix(texture(texture1, texCoord), texture(texture2,  vec2(1.0 - texCoord.x, texCoord.y)), 0.2) * vec4(ourColor, 1.0);
}

```