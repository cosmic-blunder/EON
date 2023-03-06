#version 420 core
layout (location = 0) in vec3 aPosition;

layout (location = 1) in vec2 aTexCoord; //color variable has attribute position1

out vec2 texCoord; //output a color to  the fragment shader



void main()
{

        texCoord = aTexCoord; //set color to the input color we got from teh vertext data
        gl_Position = vec4(aPosition, 1.0);


}

