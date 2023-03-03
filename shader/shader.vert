#version 420 core
layout (location = 0) in vec3 aPosition;

out vec4 vertexColor; //specify color output



void main()
{
    gl_Position = vec4(aPosition, 1.0);

    vertexColor = vec4(0.5,0.0,0.0,1.0); //output variable to read-dark color

}

