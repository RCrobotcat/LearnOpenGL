cmake_minimum_required(VERSION 3.28)
project(LearnOpenGL)

set(CMAKE_CXX_STANDARD 17)

add_executable(LearnOpenGL
        dependencies/src/glad.c
        main.cpp
        Shader.cpp
        Mesh.cpp
        Mesh.h
        Model.cpp
        Model.h
)

# 头文件包含路径
include_directories(dependencies/include)

# 库文件路径
link_directories(dependencies/lib)

# 链接库
target_link_libraries(LearnOpenGL
        ${PROJECT_SOURCE_DIR}/dependencies/lib/libassimpd.dll.a
        ${PROJECT_SOURCE_DIR}/dependencies/lib/libglfw3.a
        opengl32
)
