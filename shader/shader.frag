#version 420 core
out vec4 FragColor;
in vec4 vertexColor;

uniform vec4 outColor; //set this code in openGL code



void main()
{
      FragColor =   outColor;  // vec4(1.0f, 0.5f, 0.2f, 1.0f);
}