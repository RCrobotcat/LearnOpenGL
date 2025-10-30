cmake_minimum_required(VERSION 3.28)
project(LearnOpenGL)

set(CMAKE_CXX_STANDARD 17)

add_executable(LearnOpenGL dependencies/src/glad.c main.cpp
        Shader.cpp
        Shader.h)

INCLUDE_DIRECTORIES(dependencies/include) # 头文件和外部库的位置
link_directories(dependencies/lib)

target_link_libraries(LearnOpenGL ${PROJECT_SOURCE_DIR}/dependencies/lib/libglfw3dll.a opengl32)
