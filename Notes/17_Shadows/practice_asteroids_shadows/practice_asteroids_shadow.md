- `shadow_mapping_depth.vs`, `shadow_mapping_depth.fs`
```glsl
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

```
```glsl
#version 330 core

void main()
{
    // gl_FragDepth = gl_FragCoord.z;
}

```
---
- `planet_shadow.vs`, `planet_shadow.fs`
```glsl
#version 330 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoords;

out VS_OUT {
    vec3 FragPos;
    vec3 Normal;
    vec2 TexCoords;
    vec4 FragPosLightSpace;
} vs_out;

uniform mat4 projection;
uniform mat4 view;
uniform mat4 model;
uniform mat4 lightSpaceMatrix;

void main()
{
    vs_out.FragPos = vec3(model * vec4(aPos, 1.0));
    vs_out.TexCoords = aTexCoords;
    vs_out.Normal = transpose(inverse(mat3(model))) * aNormal;
    vs_out.FragPosLightSpace = lightSpaceMatrix * vec4(vs_out.FragPos, 1.0);

    gl_Position = projection * view * model * vec4(aPos, 1.0f);
}

```
```glsl
#version 330 core
out vec4 FragColor;

in VS_OUT {
    vec3 FragPos;
    vec3 Normal;
    vec2 TexCoords;
    vec4 FragPosLightSpace;
} fs_in;

uniform sampler2D texture_diffuse1;
uniform sampler2D shadowMap;

uniform vec3 lightPos;
uniform vec3 viewPos;

float ShadowCalculation(vec4 fragPosLightSpace, float bias)
{
    // perform perspective divide 透视除法
    vec3 projCoords = fragPosLightSpace.xyz / fragPosLightSpace.w;
    // transform to [0,1] range
    projCoords = projCoords * 0.5 + 0.5;
    // get closest depth value from light's perspective (using [0,1] range fragPosLight as coords)
    float closestDepth = texture(shadowMap, projCoords.xy).r;
    // get depth of current fragment from light's perspective
    float currentDepth = projCoords.z;
    // check whether current frag pos is in shadow
    // float shadow = currentDepth - bias > closestDepth ? 1.0 : 0.0;
    // PCF
    float shadow = 0.0;
    vec2 texelSize = 1.0 / textureSize(shadowMap, 0);
    for(int x = -1; x <= 1; ++x)
    {
        for(int y = -1; y <= 1; ++y)
        {
            float pcfDepth = texture(shadowMap, projCoords.xy + vec2(x, y) * texelSize).r;
            shadow += currentDepth - bias > pcfDepth  ? 1.0 : 0.0;
        }
    }
    shadow /= 9.0;

    if(projCoords.z > 1.0)
        shadow = 0.0;

    return shadow;
}

void main()
{
    vec3 color = texture(texture_diffuse1, fs_in.TexCoords).rgb;
    vec3 normal = normalize(fs_in.Normal);
    vec3 lightColor = vec3(0.8f);
    // ambient
    vec3 ambient = 0.3 * lightColor;
    // diffuse
    vec3 lightDir = normalize(lightPos - fs_in.FragPos);
    float diff = max(dot(lightDir, normal), 0.0);
    vec3 diffuse = diff * lightColor;
    // specular
    vec3 viewDir = normalize(viewPos - fs_in.FragPos);
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = 0.0;
    vec3 halfwayDir = normalize(lightDir + viewDir);
    spec = pow(max(dot(normal, halfwayDir), 0.0), 64.0);
    vec3 specular = spec * lightColor;
    // calculate shadow
    float bias = max(0.05 * (1.0 - dot(normal, lightDir)), 0.005);
    float shadow = ShadowCalculation(fs_in.FragPosLightSpace, bias);
    vec3 lighting = (ambient + (1.0 - shadow) * (diffuse + specular)) * color;

    FragColor = vec4(lighting, 1.0);
}

```
---
- `asteroids_shadow.vs`, `asteroids_shadow.fs`
```glsl
#version 330 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoords;
layout (location = 3) in mat4 aInstanceMatrix;

out VS_OUT {
    vec3 FragPos;
    vec3 Normal;
    vec2 TexCoords;
    vec4 FragPosLightSpace;
} vs_out;

uniform mat4 projection;
uniform mat4 view;
uniform mat4 lightSpaceMatrix;

void main()
{
    vs_out.FragPos = vec3(aInstanceMatrix * vec4(aPos, 1.0));
    vs_out.TexCoords = aTexCoords;
    vs_out.Normal = transpose(inverse(mat3(aInstanceMatrix))) * aNormal;
    vs_out.FragPosLightSpace = lightSpaceMatrix * vec4(vs_out.FragPos, 1.0);

    gl_Position = projection * view * aInstanceMatrix * vec4(aPos, 1.0f);
}

```
```glsl
#version 330 core
out vec4 FragColor;

in VS_OUT {
    vec3 FragPos;
    vec3 Normal;
    vec2 TexCoords;
    vec4 FragPosLightSpace;
} fs_in;

uniform sampler2D texture_diffuse1;
uniform sampler2D shadowMap;

uniform vec3 lightPos;
uniform vec3 viewPos;

float ShadowCalculation(vec4 fragPosLightSpace, float bias)
{
    // perform perspective divide 透视除法
    vec3 projCoords = fragPosLightSpace.xyz / fragPosLightSpace.w;
    // transform to [0,1] range
    projCoords = projCoords * 0.5 + 0.5;
    // get closest depth value from light's perspective (using [0,1] range fragPosLight as coords)
    float closestDepth = texture(shadowMap, projCoords.xy).r;
    // get depth of current fragment from light's perspective
    float currentDepth = projCoords.z;
    // check whether current frag pos is in shadow
    // float shadow = currentDepth - bias > closestDepth ? 1.0 : 0.0;
    // PCF
    float shadow = 0.0;
    vec2 texelSize = 1.0 / textureSize(shadowMap, 0);
    for(int x = -1; x <= 1; ++x)
    {
        for(int y = -1; y <= 1; ++y)
        {
            float pcfDepth = texture(shadowMap, projCoords.xy + vec2(x, y) * texelSize).r;
            shadow += currentDepth - bias > pcfDepth  ? 1.0 : 0.0;
        }
    }
    shadow /= 9.0;

    if(projCoords.z > 1.0)
        shadow = 0.0;

    return shadow;
}

void main()
{
    vec3 color = texture(texture_diffuse1, fs_in.TexCoords).rgb;
    vec3 normal = normalize(fs_in.Normal);
    vec3 lightColor = vec3(0.5f);
    // ambient
    vec3 ambient = 0.3 * lightColor;
    // diffuse
    vec3 lightDir = normalize(lightPos - fs_in.FragPos);
    float diff = max(dot(lightDir, normal), 0.0);
    vec3 diffuse = diff * lightColor;
    // specular
    vec3 viewDir = normalize(viewPos - fs_in.FragPos);
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = 0.0;
    vec3 halfwayDir = normalize(lightDir + viewDir);
    spec = pow(max(dot(normal, halfwayDir), 0.0), 64.0);
    vec3 specular = spec * lightColor;
    // calculate shadow
    float bias = max(0.05 * (1.0 - dot(normal, lightDir)), 0.005);
    float shadow = ShadowCalculation(fs_in.FragPosLightSpace, bias);
    vec3 lighting = (ambient + (1.0 - shadow) * (diffuse + specular)) * color;

    FragColor = vec4(lighting, 1.0);
}

```
---
- `main.cpp`
```c++
#include <glad/glad.h>
#include <GLFW/glfw3.h>

#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>
#include <glm/gtc/type_ptr.hpp>

#include <iostream>
#include <valarray>
#include "Shader.h"

#define STB_IMAGE_IMPLEMENTATION
#include <map>

#include "Camera.h"
#include "Model.h"
#include "stb_image.h"

void mouse_callback(GLFWwindow *window, double xpos, double ypos);

void scroll_callback(GLFWwindow *window, double xoffset, double yoffset);

void framebuffer_size_callback(GLFWwindow *window, int width, int height);

void processInput(GLFWwindow *window);

unsigned int loadTexture(char const *path);

const unsigned int SCR_WIDTH = 1280;
const unsigned int SCR_HEIGHT = 720;

// Camera
Camera camera(glm::vec3(0.0f, 20.0f, 155.0f));
float lastX = SCR_WIDTH / 2.0f;
float lastY = SCR_HEIGHT / 2.0f;
bool firstMouse = true;

float deltaTime = 0.0f; // 当前帧与上一帧的时间差
float lastFrame = 0.0f; // 上一帧的时间

int main()
{
    glfwInit();
    glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 3);
    glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 3);
    glfwWindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);

#ifdef __APPLE__
    glfwWindowHint(GLFW_OPENGL_FORWARD_COMPAT, GL_TRUE);
#endif

    GLFWwindow *window = glfwCreateWindow(SCR_WIDTH, SCR_HEIGHT, "LearnOpenGL", NULL, NULL);
    if (window == NULL)
    {
        std::cout << "Failed to create GLFW window" << std::endl;
        glfwTerminate();
        return -1;
    }
    glfwMakeContextCurrent(window);
    glfwSetFramebufferSizeCallback(window, framebuffer_size_callback);
    glfwSetScrollCallback(window, scroll_callback);

    if (!gladLoadGLLoader((GLADloadproc) glfwGetProcAddress))
    {
        std::cout << "Failed to initialize GLAD" << std::endl;
        return -1;
    }

    glfwSetInputMode(window, GLFW_CURSOR, GLFW_CURSOR_DISABLED);
    glfwSetCursorPosCallback(window, mouse_callback);

    glEnable(GL_DEPTH_TEST);

    // models
    Model planetModel("../Notes/15_Instancing_Anti-Aliasing/Instancing/planet/planet.obj");
    Model rockModel("../Notes/15_Instancing_Anti-Aliasing/Instancing/rock/rock.obj");

    // shaders
    Shader planetShader("../shaders/Instancing/shadows/planet_shadow.vs",
                        "../shaders/Instancing/shadows/planet_shadow.fs");
    Shader simpleDepthShader("../shaders/Instancing/shadows/shadow_mapping_depth.vs",
                             "../shaders/Instancing/shadows/shadow_mapping_depth.fs");
    Shader asteroidsShader("../shaders/Instancing/shadows/asteroids_shadow.vs",
                           "../shaders/Instancing/shadows/asteroids_shadow.fs");

    // shadow map
    // configure depth map FBO
    // -----------------------
    const unsigned int SHADOW_WIDTH = 2000, SHADOW_HEIGHT = 2000;
    unsigned int depthMapFBO;
    glGenFramebuffers(1, &depthMapFBO);
    // create depth texture
    unsigned int depthMap;
    glGenTextures(1, &depthMap);
    glBindTexture(GL_TEXTURE_2D, depthMap);
    glTexImage2D(GL_TEXTURE_2D, 0, GL_DEPTH_COMPONENT, SHADOW_WIDTH, SHADOW_HEIGHT, 0, GL_DEPTH_COMPONENT,
                 GL_FLOAT,NULL);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_BORDER);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_BORDER);
    float borderColor[] = {1.0, 1.0, 1.0, 1.0};
    glTexParameterfv(GL_TEXTURE_2D, GL_TEXTURE_BORDER_COLOR, borderColor);
    // attach depth texture as FBO's depth buffer
    glBindFramebuffer(GL_FRAMEBUFFER, depthMapFBO);
    glFramebufferTexture2D(GL_FRAMEBUFFER, GL_DEPTH_ATTACHMENT, GL_TEXTURE_2D, depthMap, 0);
    glDrawBuffer(GL_NONE);
    glReadBuffer(GL_NONE);
    glBindFramebuffer(GL_FRAMEBUFFER, 0);

    planetShader.use();
    planetShader.setInt("shadowMap", 8);

    // 配置小行星带的实例矩阵
    unsigned int amount = 1000;
    glm::mat4 *modelMatrices;
    modelMatrices = new glm::mat4[amount];
    srand(static_cast<unsigned int>(glfwGetTime())); // 初始化随机种子
    float radius = 150.0;
    float offset = 25.0f;
    for (unsigned int i = 0; i < amount; i++)
    {
        glm::mat4 model = glm::mat4(1.0f);
        // 1. 位移：分布在半径为 'radius' 的圆形上，偏移的范围是 [-offset, offset]
        float angle = (float) i / (float) amount * 360.0f;
        float displacement = (rand() % (int) (2 * offset * 100)) / 100.0f - offset;
        float x = sin(angle) * radius + displacement;
        displacement = (rand() % (int) (2 * offset * 100)) / 100.0f - offset;
        float y = displacement * 0.4f; // 让行星带的高度比x和z的宽度要小
        displacement = (rand() % (int) (2 * offset * 100)) / 100.0f - offset;
        float z = cos(angle) * radius + displacement;
        model = glm::translate(model, glm::vec3(x, y, z));

        // 2. 缩放：在 0.05 和 0.25f 之间缩放
        float scale = static_cast<float>((rand() % 20) / 10.0 + 0.5);
        model = glm::scale(model, glm::vec3(scale));

        // 3. 旋转：绕着一个（半）随机选择的旋转轴向量进行随机的旋转
        float rotAngle = static_cast<float>((rand() % 360));
        model = glm::rotate(model, rotAngle, glm::vec3(0.4f, 0.6f, 0.8f));

        // 4. 添加到矩阵的数组中
        modelMatrices[i] = model;
    }

    // configure instanced array
    // -------------------------
    unsigned int buffer;
    glGenBuffers(1, &buffer);
    glBindBuffer(GL_ARRAY_BUFFER, buffer);
    glBufferData(GL_ARRAY_BUFFER, sizeof(glm::mat4) * amount, &modelMatrices[0], GL_STATIC_DRAW);

    for (unsigned int i = 0; i < rockModel.getMeshes().size(); i++)
    {
        unsigned int VAO = rockModel.getMeshes()[i].VAO;
        glBindVertexArray(VAO);
        // set attribute pointers for matrix (4 times vec4)
        glEnableVertexAttribArray(3);
        glVertexAttribPointer(3, 4, GL_FLOAT, GL_FALSE, sizeof(glm::mat4), (void *) 0);
        glEnableVertexAttribArray(4);
        glVertexAttribPointer(4, 4, GL_FLOAT, GL_FALSE, sizeof(glm::mat4), (void *) (sizeof(glm::vec4)));
        glEnableVertexAttribArray(5);
        glVertexAttribPointer(5, 4, GL_FLOAT, GL_FALSE, sizeof(glm::mat4), (void *) (2 * sizeof(glm::vec4)));
        glEnableVertexAttribArray(6);
        glVertexAttribPointer(6, 4, GL_FLOAT, GL_FALSE, sizeof(glm::mat4), (void *) (3 * sizeof(glm::vec4)));

        glVertexAttribDivisor(3, 1);
        glVertexAttribDivisor(4, 1);
        glVertexAttribDivisor(5, 1);
        glVertexAttribDivisor(6, 1);

        glBindVertexArray(0);
    }

    // lighting info
    // -------------
    glm::vec3 lightPos = glm::vec3(-200.0f, 5.0f, 0.0f);

    while (!glfwWindowShouldClose(window))
    {
        float currentFrame = static_cast<float>(glfwGetTime());
        deltaTime = currentFrame - lastFrame;
        lastFrame = currentFrame;

        processInput(window);

        glClearColor(0.1f, 0.1f, 0.1f, 1.0f);
        glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

        // rotate light position around world origin over time
        {
            const float PI = 3.14159265359f;
            float lightRadius = 200.0f;
            float lightHeight = 5.0f;
            float rotationsPerSecond = 0.05f; // adjust speed
            float angle = static_cast<float>(glfwGetTime()) * rotationsPerSecond * 2.0f * PI;
            lightPos.x = cosf(angle) * lightRadius;
            lightPos.z = sinf(angle) * lightRadius;
            lightPos.y = lightHeight;
        }

        // configure transformation matrices
        glm::mat4 projection = glm::perspective(glm::radians(45.0f),
                                                (float) SCR_WIDTH / (float) SCR_HEIGHT, 0.1f, 1000.0f);
        glm::mat4 view = camera.GetViewMatrix();

        // 1. render depth of scene to texture (from light's perspective)
        // --------------------------------------------------------------
        glViewport(0, 0, SHADOW_WIDTH, SHADOW_HEIGHT);
        glBindFramebuffer(GL_FRAMEBUFFER, depthMapFBO);
        glClear(GL_DEPTH_BUFFER_BIT); // 清除深度缓冲（正确时机）
        glCullFace(GL_FRONT);

        // prepare matrices
        glm::mat4 lightProjection, lightView;
        glm::mat4 lightSpaceMatrix;
        float sceneSize = 150.0f;
        lightProjection = glm::ortho(-sceneSize, sceneSize, -sceneSize, sceneSize, 1.0f, 400.0f);
        lightView = glm::lookAt(lightPos, glm::vec3(0.0f), glm::vec3(0, 1, 0));
        lightSpaceMatrix = lightProjection * lightView;

        simpleDepthShader.use();
        simpleDepthShader.setMat4("lightSpaceMatrix", lightSpaceMatrix);

        // -------------------- Draw planet --------------------
        glm::mat4 model = glm::mat4(1.0f);
        model = glm::translate(model, glm::vec3(0, -3, 0));
        model = glm::scale(model, glm::vec3(10.0f));
        simpleDepthShader.setMat4("model", model); // planet 用 model
        planetModel.Draw(simpleDepthShader);

        // -------------------- Draw asteroids (instanced) --------------------
        simpleDepthShader.setMat4("model", glm::mat4(1.0f)); // asteroid 用 Identity

        for (unsigned int i = 0; i < rockModel.getMeshes().size(); i++)
        {
            glBindVertexArray(rockModel.getMeshes()[i].VAO);
            glDrawElementsInstanced(
                GL_TRIANGLES,
                static_cast<unsigned int>(rockModel.getMeshes()[i].indices.size()),
                GL_UNSIGNED_INT,
                0,
                amount
            );
            glBindVertexArray(0);
        }

        // finish rendering depth map
        glBindFramebuffer(GL_FRAMEBUFFER, 0);
        glCullFace(GL_BACK);

        // Reset viewport for normal rendering
        glViewport(0, 0, SCR_WIDTH, SCR_HEIGHT);
        glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

        // 2. render scene as normal using the generated depth/shadow map
        // --------------------------------------------------------------
        asteroidsShader.use();
        asteroidsShader.setMat4("projection", projection);
        asteroidsShader.setMat4("view", view);
        planetShader.use();
        planetShader.setMat4("projection", projection);
        planetShader.setMat4("view", view);

        // draw planet
        planetShader.setMat4("model", model);
        planetShader.setVec3("lightPos", lightPos);
        planetShader.setVec3("viewPos", camera.Position);
        planetShader.setMat4("lightSpaceMatrix", lightSpaceMatrix);
        glActiveTexture(GL_TEXTURE8);
        glBindTexture(GL_TEXTURE_2D, depthMap);
        planetModel.Draw(planetShader);

        // draw meteorites
        asteroidsShader.use();
        asteroidsShader.setInt("texture_diffuse1", 0);
        glActiveTexture(GL_TEXTURE0);
        glBindTexture(GL_TEXTURE_2D, rockModel.getTextures()[0].id);
        asteroidsShader.setVec3("lightPos", lightPos);
        asteroidsShader.setVec3("viewPos", camera.Position);
        asteroidsShader.setMat4("lightSpaceMatrix", lightSpaceMatrix);
        asteroidsShader.setInt("shadowMap", 8);
        glActiveTexture(GL_TEXTURE8);
        glBindTexture(GL_TEXTURE_2D, depthMap);
        // note: we also made the textures_loaded vector public (instead of private) from the model class.
        for (unsigned int i = 0; i < rockModel.getMeshes().size(); i++)
        {
            glBindVertexArray(rockModel.getMeshes()[i].VAO);
            glDrawElementsInstanced(GL_TRIANGLES, static_cast<unsigned int>(rockModel.getMeshes()[i].indices.size()),
                                    GL_UNSIGNED_INT, 0, amount);
            glBindVertexArray(0);
        }

        glfwSwapBuffers(window);
        glfwPollEvents();
    }

    glfwTerminate();
    return 0;
}

void processInput(GLFWwindow *window)
{
    if (glfwGetKey(window, GLFW_KEY_ESCAPE) == GLFW_PRESS)
        glfwSetWindowShouldClose(window, true);

    float cameraSpeed = 5.0f * deltaTime; // adjust accordingly
    if (glfwGetKey(window, GLFW_KEY_W) == GLFW_PRESS) // w: forward
        camera.ProcessKeyboard(FORWARD, deltaTime);
    if (glfwGetKey(window, GLFW_KEY_S) == GLFW_PRESS) // s: backward
        camera.ProcessKeyboard(BACKWARD, deltaTime);
    if (glfwGetKey(window, GLFW_KEY_A) == GLFW_PRESS) // a: left
        camera.ProcessKeyboard(LEFT, deltaTime);
    if (glfwGetKey(window, GLFW_KEY_D) == GLFW_PRESS) // d: right
        camera.ProcessKeyboard(RIGHT, deltaTime);
}

void mouse_callback(GLFWwindow *window, double xposIn, double yposIn)
{
    float xpos = static_cast<float>(xposIn);
    float ypos = static_cast<float>(yposIn);

    if (firstMouse)
    {
        lastX = xpos;
        lastY = ypos;
        firstMouse = false;
    }

    float xoffset = xpos - lastX;
    float yoffset = lastY - ypos; // reversed since y-coordinates go from bottom to top

    lastX = xpos;
    lastY = ypos;

    camera.ProcessMouseMovement(xoffset, yoffset);
}

void scroll_callback(GLFWwindow *window, double xoffset, double yoffset)
{
    camera.ProcessMouseScroll(static_cast<float>(yoffset));
}

void framebuffer_size_callback(GLFWwindow *window, int width, int height)
{
    glViewport(0, 0, width, height);
}

// utility function for loading a 2D texture from file
// ---------------------------------------------------
unsigned int loadTexture(char const *path)
{
    unsigned int textureID;
    glGenTextures(1, &textureID);

    int width, height, nrComponents;
    unsigned char *data = stbi_load(path, &width, &height, &nrComponents, 0);
    if (data)
    {
        GLenum format;
        if (nrComponents == 1)
            format = GL_RED;
        else if (nrComponents == 3)
            format = GL_RGB;
        else if (nrComponents == 4)
            format = GL_RGBA;

        glBindTexture(GL_TEXTURE_2D, textureID);
        glTexImage2D(GL_TEXTURE_2D, 0, format, width, height, 0, format, GL_UNSIGNED_BYTE, data);
        glGenerateMipmap(GL_TEXTURE_2D);

        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, format == GL_RGBA ? GL_CLAMP_TO_EDGE : GL_REPEAT);
        // for this tutorial: use GL_CLAMP_TO_EDGE to prevent semi-transparent borders. Due to interpolation it takes texels from next repeat
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, format == GL_RGBA ? GL_CLAMP_TO_EDGE : GL_REPEAT);
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR_MIPMAP_LINEAR);
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);

        stbi_image_free(data);
    } else
    {
        std::cout << "Texture failed to load at path: " << path << std::endl;
        stbi_image_free(data);
    }

    return textureID;
}

```