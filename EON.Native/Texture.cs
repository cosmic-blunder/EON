using System;
using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace EON.Native
{
    public class EONTexture{
 

         public  int Handle {get;set;}

         public EONTexture(string path){
             Handle = GL.GenTexture();

             
             StbImage.stbi_set_flip_vertically_on_load(1);

             ImageResult image  =  ImageResult.FromStream(File.OpenRead(path),ColorComponents.RedGreenBlueAlpha);

             GL.TexImage2D(TextureTarget.Texture2D,0, PixelInternalFormat.Rgba,image.Width,image.Height,0,PixelFormat.Rgba,PixelType.UnsignedByte,image.Data);             
             Use();

         }


         public void Use(){

             GL.BindTexture(TextureTarget.Texture2D,Handle);
         }

    }
}