#version 430 core

// Input from the vertex shader

in vec4 gsColor;

// Output to framebuffer
out vec4 Color;

void main(void)
{
	Color = gsColor;
}