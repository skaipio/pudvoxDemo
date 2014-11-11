#version 430 core

in vec3 position;
in float size;

out VertexData
{
	vec4 color;
	vec3 dimensions;
	mat4 modelViewMatrix;
	mat4 modelMatrix;
} vsOut;

uniform float time;
uniform mat4 viewMatrix;

void main(void)
{
	float rotationX = time+0.01*gl_VertexID;
	float rotationY = time+0.01*gl_VertexID;
	float rotationZ = time+0.01*gl_VertexID;
	vsOut.color = vec4(1.0,0.0,1.0,1.0);
	vsOut.dimensions = vec3(size,size,size);

	mat4 xRot = mat4(vec4(1.0,0.0,0.0,0.0),
					vec4(0.0,cos(rotationX),sin(rotationX),0.0),
					vec4(0.0,-sin(rotationX),cos(rotationX),0.0),
					vec4(0.0,0.0,0.0,1.0));
	mat4 yRot = mat4(vec4(cos(rotationY),0.0,-sin(rotationY),0.0),
					vec4(0.0,1.0,0.0,0.0),
					vec4(sin(rotationY),0.0,cos(rotationY),0.0),
					vec4(0.0,0.0,0.0,1.0));
	mat4 zRot = mat4(vec4(cos(rotationZ),sin(rotationZ),0.0,0.0),
					vec4(-sin(rotationZ),cos(rotationZ),0.0,0.0),
					vec4(0.0,0.0,1.0,0.0),
					vec4(0.0,0.0,0.0,1.0));

	vec4 p = vec4(position,1.0);
	
	mat4 translationMatrix = mat4(vec4(1.0,0.0,0.0,0.0),
									vec4(0.0,1.0,0.0,0.0),
									vec4(0.0,0.0,1.0,0.0),
									vec4(p.x,p.y,p.z,1.0));
	vsOut.modelViewMatrix = viewMatrix*(translationMatrix*yRot);
	gl_Position = vec4(position,1.0);
}