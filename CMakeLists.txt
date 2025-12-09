cmake_minimum_required(VERSION 3.28)
project(LearnOpenGL)

set(CMAKE_CXX_STANDARD 17)

add_executable(LearnOpenGL
        dependencies/src/glad.c
        main.cpp
        Shader.cpp
        Mesh.cpp
        Model.cpp

        # ImGui core files
        dependencies/include/imgui/cpp/imgui.cpp
        dependencies/include/imgui/cpp/imgui_draw.cpp
        dependencies/include/imgui/cpp/imgui_widgets.cpp
        dependencies/include/imgui/cpp/imgui_tables.cpp
        dependencies/include/imgui/cpp/imgui_demo.cpp

        # ImGui backend
        dependencies/include/imgui/cpp/imgui_impl_glfw.cpp
        dependencies/include/imgui/cpp/imgui_impl_opengl3.cpp
)

# 头文件包含路径
include_directories(dependencies/include)

# 链接库路径
link_directories(dependencies/lib)

# 链接第三方库
target_link_libraries(LearnOpenGL
        ${PROJECT_SOURCE_DIR}/dependencies/lib/libassimpd.dll.a
        ${PROJECT_SOURCE_DIR}/dependencies/lib/libglfw3.a
        opengl32
)
