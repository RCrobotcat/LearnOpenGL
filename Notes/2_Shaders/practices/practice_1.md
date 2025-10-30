### 1. 修改顶点着色器让三角形上下颠倒
- `vertexShader.vs`
```glsl
#version 330 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aColor;

out vec3 ourColor;

void main()
{
    mat3 Rz180 = mat3(
        -1.0,  0.0,  0.0,
         0.0, -1.0,  0.0,
         0.0,  0.0,  1.0
    );
    vec3 rotatedPos = Rz180 * aPos;
    gl_Position = vec4(rotatedPos, 1.0);
    ourColor = aColor;
}

```