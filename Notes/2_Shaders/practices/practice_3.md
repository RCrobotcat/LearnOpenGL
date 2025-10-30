### 3. 使用out关键字把顶点位置输出到片段着色器，并将片段的颜色设置为与顶点位置相等（来看看连顶点位置值都在三角形中被插值的结果）  
### 做完这些后，尝试回答下面的问题：为什么在三角形的左下角是黑的? (因为OpenGL的坐标系是从-1到1，而颜色值是从0到1，所以负值会被截断为0，导致左下角显示为黑色)
- `main.cpp`
```c++
#include <glad/glad.h>
#include <GLFW/glfw3.h>

#include <iostream>
#include <valarray>
#include "Shader.h"

void framebuffer_size_callback(GLFWwindow *window, int width, int height);

void processInput(GLFWwindow *window);

const unsigned int SCR_WIDTH = 800;
const unsigned int SCR_HEIGHT = 600;

int main() {
    glfwInit();
    glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 3);
    glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 3);
    glfwWindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);

#ifdef __APPLE__
    glfwWindowHint(GLFW_OPENGL_FORWARD_COMPAT, GL_TRUE);
#endif

    GLFWwindow *window = glfwCreateWindow(SCR_WIDTH, SCR_HEIGHT, "LearnOpenGL", NULL, NULL);
    if (window == NULL) {
        std::cout << "Failed to create GLFW window" << std::endl;
        glfwTerminate();
        return -1;
    }
    glfwMakeContextCurrent(window);
    glfwSetFramebufferSizeCallback(window, framebuffer_size_callback);

    if (!gladLoadGLLoader((GLADloadproc) glfwGetProcAddress)) {
        std::cout << "Failed to initialize GLAD" << std::endl;
        return -1;
    }

    // shaders
    Shader ourShader("../shaders/vertexShader.vs", "../shaders/fragmentShader.fs");

    // VBO, VAO
    float vertices[] = {
            0.5f, 0.5f, 0.0f, 1.0f, 0.0f, 0.0f,   // 右上角坐标+颜色
            0.5f, -0.5f, 0.0f, 0.0f, 1.0f, 0.0f,  // 右下角+颜色
            -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, 1.0f, // 左下角+颜色
            -0.5f, 0.5f, 0.0f, 1.0f, 1.0f, 0.0f,   // 左上角+颜色
            0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 1.0f    // 中心点+颜色
    };
    unsigned int indices[] = {
            // 注意索引从0开始!
            // 此例的索引(0,1,2,3)就是顶点数组vertices的下标，
            // 这样可以由下标代表顶点组合成矩形

            1, 2,  4
    };
    unsigned int VAO, VBO, EBO;

    // VAO负责存储顶点属性配置
    glGenVertexArrays(1, &VAO);
    glBindVertexArray(VAO);

    // VBO负责存储顶点数据
    glGenBuffers(1, &VBO);
    glBindBuffer(GL_ARRAY_BUFFER, VBO);
    glBufferData(GL_ARRAY_BUFFER, sizeof(vertices), vertices, GL_STATIC_DRAW);

    // EBO负责存储索引数据
    glGenBuffers(1, &EBO);
    glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, EBO);
    glBufferData(GL_ELEMENT_ARRAY_BUFFER, sizeof(indices), indices, GL_STATIC_DRAW);

    glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 6 * sizeof(float), (void *) 0);
    glEnableVertexAttribArray(0);
    glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, 6 * sizeof(float), (void *) (3 * sizeof(float)));
    glEnableVertexAttribArray(1);

    // 之前已经绑定到GL_ARRAY_BUFFER的VBO会被VAO记录下来，所以这里可以解绑
    glBindBuffer(GL_ARRAY_BUFFER, 0);
    glBindVertexArray(0);

    // 线框模式 GL_LINE
    // 填充模式 GL_FILL (默认)
    glPolygonMode(GL_FRONT_AND_BACK, GL_FILL);

    while (!glfwWindowShouldClose(window)) {
        processInput(window);

        glClearColor(0.2f, 0.3f, 0.3f, 1.0f);
        glClear(GL_COLOR_BUFFER_BIT);

        ourShader.use();

        float time = glfwGetTime();
        float translationXValue = sin(time) / 2.0f;
        float translationYValue = cos(time) / 2.0f;
        ourShader.setFloat("translationX", translationXValue);
        ourShader.setFloat("translationY", translationYValue);

        glBindVertexArray(VAO);
        glDrawElements(GL_TRIANGLES, 6, GL_UNSIGNED_INT, 0);

        glfwSwapBuffers(window);
        glfwPollEvents();
    }

    glDeleteVertexArrays(1, &VAO);
    glDeleteBuffers(1, &VBO);
    glDeleteBuffers(1, &EBO);
    glfwTerminate();
    return 0;
}

void processInput(GLFWwindow *window) {
    if (glfwGetKey(window, GLFW_KEY_ESCAPE) == GLFW_PRESS)
        glfwSetWindowShouldClose(window, true);
}

void framebuffer_size_callback(GLFWwindow *window, int width, int height) {
    glViewport(0, 0, width, height);
}

```
---
- `vertexShader.vs`
```glsl
#version 330 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aColor;

out vec3 ourColor;
out vec4 vertexPosition;

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
}

```
---
- `fragmentShader.fs`
```glsl
#version 330 core
in vec3 ourColor;
in vec4 vertexPosition;
out vec4 FragColor;

void main()
{
    FragColor = vertexPosition;
}

```