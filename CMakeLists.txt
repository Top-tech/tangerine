cmake_minimum_required(VERSION 3.14)
project(tangerine)

set(CMAKE_CXX_STANDARD 20)

set(Qt5_DIR "~/Qt/5.13.2/clang_64/lib/cmake/Qt5")

set(CMAKE_AUTOMOC ON)

set(CMAKE_AUTOUIC ON)

find_package(Qt5 COMPONENTS Core Widgets REQUIRED)

add_executable(tangerine main.cpp)

target_link_libraries(tangerine Qt5::Core Qt5::Widgets)
