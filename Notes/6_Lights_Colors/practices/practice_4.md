### 尝试实现一个Gouraud着色（而不是风氏着色）。如果你做对了话，立方体的光照应该会看起来有些奇怪，尝试推理为什么它会看起来这么奇怪 =>（在顶点着色器中实现的风氏光照模型叫做Gouraud着色 Gouraud Shading）
#### 解答：因为Gouraud着色是在顶点处计算光照，然后将颜色插值到片段上。这意味着如果一个面有很少的顶点（例如立方体的每个面只有4个顶点），那么在面内部的颜色变化会非常有限，导致光照效果看起来不自然或“奇怪”。相比之下，Phong着色是在片段级别计算光照，可以更精确地反映光照变化，从而产生更平滑和真实的效果。
- `vertexShader.glsl`:
```glsl
#version 330 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

uniform vec3 lightPos;
uniform vec3 lightColor;
uniform vec3 viewPos;

out vec3 PhongColor;

void main()
{
    gl_Position = projection * view * model * vec4(aPos, 1.0);

    vec3 Normal = mat3(transpose(inverse(model))) * aNormal; // Transform normal to world space
    vec3 FragPos = vec3(model * vec4(aPos, 1.0));

    float ambientStrength = 0.1;
    vec3 ambient = ambientStrength * lightColor;

    vec3 norm = normalize(Normal);
    vec3 lightDir = normalize(lightPos - FragPos);
    vec3 diffuse = max(dot(norm, lightDir), 0.0) * lightColor;

    vec3 viewDir = normalize(viewPos - FragPos);
    vec3 reflectDir = reflect(-lightDir, norm);
    // vec3 reflectDir = 2 * dot(norm, lightDir) * norm - lightDir; // Manual reflection calculation
    vec3 specular = pow(max(dot(viewDir, reflectDir), 0.0), 32) * lightColor;

    PhongColor = ambient + diffuse + specular;
}

```
---
- `fragmentShader.glsl`:
```glsl
#version 330 core
in vec3 PhongColor;

uniform vec3 objectColor;

out vec4 FragColor;

void main()
{
    vec3 result = PhongColor * objectColor;
    FragColor = vec4(result, 1.0);
}

```