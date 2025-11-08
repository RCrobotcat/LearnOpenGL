### 调整光源的环境光、漫反射和镜面光向量，看看它们如何影响箱子的视觉输出。

### 尝试在片段着色器中反转镜面光贴图的颜色值，让木头显示镜面高光而钢制边缘不反光（由于钢制边缘中有一些裂缝，边缘仍会显示一些镜面高光，虽然强度会小很多）
- `fragmentShader.fs`
```glsl
#version 330 core
struct Material {
    sampler2D diffuse;
    sampler2D specular;
    float shininess;
};

uniform Material material;

struct Light {
    vec3 position;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

uniform Light light;

in vec3 Normal;
in vec3 FragPos;
in vec2 TexCoords;

uniform vec3 lightColor;
uniform vec3 objectColor;
uniform vec3 viewPos;

out vec4 FragColor;

void main()
{
    vec3 ambient = light.ambient * vec3(texture(material.diffuse, TexCoords));

    vec3 norm = normalize(Normal);
    vec3 lightDir = normalize(light.position - FragPos);
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = light.diffuse * diff * vec3(texture(material.diffuse, TexCoords));

    vec3 viewDir = normalize(viewPos - FragPos);
    vec3 reflectDir = reflect(-lightDir, norm);
    // vec3 reflectDir = 2 * dot(norm, lightDir) * norm - lightDir; // Manual reflection calculation
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    // here we inverse the sampled specular color. Black becomes white and white becomes black.
    vec3 specular = light.specular * spec * (vec3(1.0) - vec3(texture(material.specular, TexCoords)));

    vec3 result = (ambient + diffuse + specular) * objectColor;
    FragColor = vec4(result, 1.0);
}

```