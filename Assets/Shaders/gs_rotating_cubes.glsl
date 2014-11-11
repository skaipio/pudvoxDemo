// Geometry shader.

#version 430 core

layout (points) in;
layout (triangle_strip, max_vertices = 24) out;

uniform mat4 projectionMatrix;
uniform vec3 viewpoint;

in VertexData
{
	vec4 color;
	vec3 dimensions;
	mat4 modelViewMatrix;
} gsIn[];

out vec4 gsColor;

void emitVertexAt(vec3 pos, vec4 color){
	gl_Position = projectionMatrix * gsIn[0].modelViewMatrix * vec4(pos,1.0);
	gsColor = color;
	EmitVertex();
}

float culling(vec3 normal, float size){
	vec3 transformedNormal = mat3(gsIn[0].modelViewMatrix) * normal; // this is enough, when uniform scaling
	vec4 worldspace = gsIn[0].modelViewMatrix * vec4(normal * size,1.0); // translate surface point to cam space
	vec3 vt = normalize(-worldspace.xyz);	// surface to cam direction
	return dot(transformedNormal, vt);
}

void main(void)
{
	vec4 color = vec4(1.0,0.0,0.0,1.0);
	vec3 d = gsIn[0].dimensions * 0.5;
	
	// top face
	if (culling(vec3(0.0,0.0,1.0), d.z) > 0.0){		
		emitVertexAt(vec3(-d.x,-d.y,d.z),color);
		emitVertexAt(vec3(d.x,-d.y,d.z),color);
		emitVertexAt(vec3(-d.x,d.y,d.z),color);
		emitVertexAt(vec3(d.x,d.y,d.z),color);
		EndPrimitive();
		
	}

	// bottom face
	else {// (culling(vec3(0.0,0.0,-1.0), d.z) > 0){
		color = vec4(0.0,0.0,1.0,1.0);
		emitVertexAt(vec3(-d.x,d.y,-d.z),color);
		emitVertexAt(vec3(d.x,d.y,-d.z),color);
		emitVertexAt(vec3(-d.x,-d.y,-d.z),color);
		emitVertexAt(vec3(d.x,-d.y,-d.z),color);
		EndPrimitive();
	}

	// south face
	if (culling(vec3(0.0,-1.0,0.0), d.y) > 0.0){		
		color = vec4(1.0,0.0,1.0,1.0);
		emitVertexAt(vec3(-d.x,-d.y,-d.z),color);
		emitVertexAt(vec3(d.x,-d.y,-d.z),color);
		emitVertexAt(vec3(-d.x,-d.y,d.z),color);
		emitVertexAt(vec3(d.x,-d.y,d.z),color);
		EndPrimitive();
	}

	// north face
	else{ //(culling(vec3(0.0,1.0,0.0), d.y) > 0){
		color = vec4(1.0,0.0,1.0,1.0);
		emitVertexAt(vec3(-d.x,d.y,d.z),color);
		emitVertexAt(vec3(d.x,d.y,d.z),color);
		emitVertexAt(vec3(-d.x,d.y,-d.z),color);
		emitVertexAt(vec3(d.x,d.y,-d.z),color);
		EndPrimitive();
	}
	
	// east face
	
	if (culling(vec3(1.0,0.0,0.0), d.x) > 0.0){
		color = vec4(0.0,1.0,0.0,1.0);
		emitVertexAt(vec3(d.x,-d.y,-d.z),color);
		emitVertexAt(vec3(d.x, d.y,-d.z),color);
		emitVertexAt(vec3(d.x,-d.y,d.z),color);
		emitVertexAt(vec3(d.x, d.y,d.z),color);
		EndPrimitive();
	}
	

	// west face
	else{ // (culling(vec3(-1.0,0.0,0.0), d.x) > 0){
		color = vec4(0.0,1.0,0.0,1.0);
		emitVertexAt(vec3(-d.x,-d.y,d.z),color);
		emitVertexAt(vec3(-d.x, d.y,d.z),color);
		emitVertexAt(vec3(-d.x,-d.y,-d.z),color);
		emitVertexAt(vec3(-d.x, d.y,-d.z),color);
		EndPrimitive();
	}
}

