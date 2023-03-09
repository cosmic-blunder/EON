using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;


using System;

using StbImageSharp;
using System.IO;

namespace EON.Native
{

    
    public class Texture{
         int Handle{get;set;}
         public Texture(string path,TextureUnit unit = TextureUnit.Texture0){
         Handle  = GL.GenTexture();
     
         Use(unit);
         //loading texture
         StbImage.stbi_set_flip_vertically_on_load(1);
         using(Stream stream = File.OpenRead(path)){

         ImageResult image  = ImageResult.FromStream(stream,ColorComponents.RedGreenBlueAlpha);


         GL.TexImage2D(TextureTarget.Texture2D,0,PixelInternalFormat.Rgba,image.Width,image.Height,0,PixelFormat.Rgba,PixelType.UnsignedByte,image.Data);
        
                       // First, we set the min and mag filter. These are used for when the texture is scaled down and up, respectively.
            // Here, we use Linear for both. This means that OpenGL will try to blend pixels, meaning that textures scaled too far will look blurred.
            // You could also use (amongst other options) Nearest, which just grabs the nearest pixel, which makes the texture look pixelated if scaled too far.
            // NOTE: The default settings for both of these are LinearMipmap. If you leave these as default but don't generate mipmaps,
            // your image will fail to render at all (usually resulting in pure black instead).
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            // Now, set the wrapping mode. S is for the X axis, and T is for the Y axis.
            // We set this to Repeat so that textures will repeat when wrapped. Not demonstrated here since the texture coordinates exactly match
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
         GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
         }
        
        }
        public void Use(TextureUnit unit = TextureUnit.Texture0){
          GL.ActiveTexture(unit);

          GL.BindTexture(TextureTarget.Texture2D,Handle);
          
        }
}
    public class Shader
    {
      public  int Handle;
        int VertexShader;
        int FragmentShader;
        int shader;
        int status;
        public Shader(string vertextPath, string fragmentPath) {
            string VertexShaderSource = File.ReadAllText(vertextPath);
            string FragmentShaderSource = File.ReadAllText(fragmentPath);

            VertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(VertexShader, VertexShaderSource);

            FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(FragmentShader, FragmentShaderSource);

            //Compile these shaders .

            GL.CompileShader(VertexShader);
            GL.GetShader(shader, ShaderParameter.CompileStatus, out status);

            if (status == 0)
            {
                string infoLog = GL.GetShaderInfoLog(VertexShader);
                Console.WriteLine(infoLog);
            }
            GL.CompileShader(FragmentShader);
            GL.GetShader(shader, ShaderParameter.CompileStatus, out status);

            if (status == 0)
            {
                string infoLog = GL.GetShaderInfoLog(FragmentShader);
                Console.WriteLine(infoLog);
            }

            Handle = GL.CreateProgram();
            GL.AttachShader(Handle, VertexShader);
            GL.AttachShader(Handle, FragmentShader);

            GL.LinkProgram(Handle);
            int program = 0;
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out status);
            if (status == 0)
            {
                string infoLog = GL.GetProgramInfoLog(program);
                Console.WriteLine(infoLog);

            }

            GL.DetachShader(Handle, VertexShader);
            GL.DetachShader(Handle, FragmentShader);

            GL.DeleteShader(FragmentShader);
            GL.DeleteShader(VertexShader);

        }


        public void SetInt(string name , int val){
            int location   = GL.GetUniformLocation(Handle,name);

            GL.Uniform1(location,val);
        }
       public  int GetAttrib(string name){
           return GL.GetAttribLocation(this.Handle,name);
       }
            public void Use()
        {
            GL.UseProgram(Handle);
        }

        private bool disposedValue = false;
        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue != true)
            {
                GL.DeleteProgram(Handle);
                disposedValue = true;
            }
        }
        ~Shader()
        {
            GL.DeleteProgram(Handle);
        }

        public void Disposed()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }


    public class Native : GameWindow
    {
        static System.Timers.Timer _timer = new System.Timers.Timer(1000); //one second

        string ShaderFrg  = Path.GetFullPath(@"../shader/shader.frag");
        string ShaderVert = Path.GetFullPath(@"../shader/shader.vert");

        double elapsed;
        int VertexBufferObject;
        int ElementBufferObject;
        public Shader? shaderP;
        public int VertextArrayObject;

        public Native(int width, int height, string title) :
          base(GameWindowSettings.Default,
                new NativeWindowSettings() { Size = (width, height), Title = title })
        {
                texWall=null;
        }
        public float[] vertices = {
                 0.5f, 0.5f,0.0f,1.0f,1.0f,//top right
                 0.5f,-0.5f,0.0f,1.0f,0.0f, //Bottom  right vertix
                -0.5f,-0.5f,0.0f,0.0f,0.0f,//bottom left
                -0.5f, 0.5f,0.0f,0.0f,1.0f  //top  left
           };

        uint[] indices =  {
            0,1,3,
            1,2,3
        };

        //textures
        Texture texWall {get;set;}
        Texture texFace{ get;set;}

        /**
           
            0       3


            1       2    

        
        */

        
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            KeyboardState input = KeyboardState;

            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            VertextArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(VertextArrayObject);
            
 

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            VertexBufferObject = GL.GenBuffer();
     

            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            ElementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);
            
            shaderP = new Shader(this.ShaderVert, this.ShaderFrg);
            shaderP.Use();
            
            var vertexLocation = shaderP.GetAttrib("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            int texCoord = shaderP.GetAttrib("aTexCoord");
            GL.VertexAttribPointer(texCoord,2,VertexAttribPointerType.Float,false,5*sizeof(float),3*sizeof(float));
            GL.EnableVertexAttribArray(texCoord);

            
            texWall =  new Texture(Path.GetFullPath(@"../Texture/wall.jpg"));
            texFace =  new Texture(Path.GetFullPath(@"../Texture/awesomeface.png"),TextureUnit.Texture1);

            shaderP.SetInt("texture0", 0);
            shaderP.SetInt("texture1", 1);

        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit);
           
            texWall.Use(TextureUnit.Texture0);
            texFace.Use(TextureUnit.Texture1);
          
             if (shaderP != null)
            {

             shaderP?.Use();
             
             GL.BindVertexArray(VertextArrayObject);
             
             //GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

             GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
             SwapBuffers();
            }
        }
        protected override void OnResize(ResizeEventArgs e)
        {

            base.OnResize(e);

            GL.Viewport(0, 0, e.Width, e.Height);
        }

        protected override void OnUnload()
        {
            if (shaderP != null)
            {
                shaderP.Disposed();

            }
        }
    }
}