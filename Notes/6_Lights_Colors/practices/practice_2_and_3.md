### 尝试使用不同的环境光、漫反射和镜面强度，观察它们怎么是影响光照效果的。同样，尝试实验一下镜面光照的反光度因子。尝试理解为什么某一个值能够有着特定视觉输出

### 在观察空间（而不是世界空间）中计算风氏光照
- `vertexShader.vs`
```glsl
#version 330 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

uniform vec3 lightPos;

out vec3 Normal;
out vec3 FragPos;
out vec3 LightPos;

void main()
{
    vec4 finalPos = projection * view * model * vec4(aPos, 1.0);
    gl_Position = finalPos;
    Normal = mat3(transpose(inverse(model))) * aNormal; // Transform normal to world space
    FragPos = vec3(view * model * vec4(aPos, 1.0));
    LightPos = vec3(view * vec4(lightPos, 1.0));
}

```
---
- `fragmentShader.fs`
```glsl
#version 330 core
in vec3 Normal;
in vec3 FragPos;
in vec3 LightPos;

uniform vec3 lightColor;
uniform vec3 objectColor;
uniform vec3 viewPos;

out vec4 FragColor;

void main()
{
    float ambientStrength = 0.1;
    vec3 ambient = ambientStrength * lightColor;

    vec3 norm = normalize(Normal);
    vec3 lightDir = normalize(LightPos - FragPos);
    vec3 diffuse = max(dot(norm, lightDir), 0.0) * lightColor;

    vec3 viewDir = normalize(-FragPos); // the viewer is always at (0,0,0) in view-space, so viewDir is (0,0,0) - Position => -Position
    vec3 reflectDir = reflect(-lightDir, norm);
    // vec3 reflectDir = 2 * dot(norm, lightDir) * norm - lightDir; // Manual reflection calculation
    vec3 specular = pow(max(dot(viewDir, reflectDir), 0.0), 32) * lightColor;

    vec3 result = (ambient + diffuse + specular) * objectColor;
    FragColor = vec4(result, 1.0);
}

```