using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;


//texture 



using System.Timers;

namespace EON.Native
{



 
    public class Shader
    {
      public  int Handle;
        int VertexShader;
        int FragmentShader;
        int shader;
        int status;
         int program ;
        public Shader(string vertextPath, string fragmentPath)
        {
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

        public int GetAttribLocation(string name){

            return GL.GetAttribLocation(Handle,name);
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


        //static System.Timers.Timer _timer = new System.Timers.Timer(1000); //one second

        EONTexture etexture;
        string ShaderFrg = Path.GetFullPath(@"../shader/shader.frag");
        string ShaderVert = Path.GetFullPath(@"../shader/shader.vert");

        int VertexBufferObject;
        public Shader? shaderP;
        public int VertextArrayObject;
        public Native(int width, int height, string title) :
          base(GameWindowSettings.Default,
                new NativeWindowSettings() { Size = (width, height), Title = title ,
                StartVisible=false,
                API = ContextAPI.OpenGL,
                WindowBorder=WindowBorder.Fixed
                })
        {
  

        }
        public float[] vertices = {
              //Position                     //texture coordinates
              0.5f, 0.5f, 0.0f ,1.0f, 1.0f, //top right
              0.5f, -0.5f, 0.0f ,1.0f, 0.0f, //bottom right
             -0.5f, -0.5f, 0.0f ,0.0f, 0.0f, //bottom left
             -0.5f,  0.5f, 0.0f ,0.0f, 1.0f, //top left
            };

           //texture sampling coordinate 

           float[] texCoords = {

                 0.0f ,0.0f,
                 1.0f,0.0f,
                 0.5f,0.1f
           }; 

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
            this.IsVisible = true;

            base.OnLoad();

            shaderP = new Shader(this.ShaderVert, this.ShaderFrg);
            etexture = new EONTexture(Path.GetFullPath(@"../Texture/container.jpg"));
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            VertexBufferObject = GL.GenBuffer();
            VertextArrayObject = GL.GenVertexArray();


            GL.BindVertexArray(VertextArrayObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);

            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
             
            int textCoordLocation  = shaderP.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(textCoordLocation);

            GL.VertexAttribPointer(textCoordLocation,2,VertexAttribPointerType.Float,false,5*sizeof(float),3*sizeof(float));


            //first argument specify position in Vertex Array buffer(VBA).
            ///This array  array buffer is array of pointer to an array .
            //This pointer to array pointing to the position in the VBO
          
            //For more deatil check this link:https://opentk.net/learn/chapter1/2-hello-triangle.html#vertex-array-object
        
          //  GL.VertexAttribPointer(1,3,VertexAttribPointerType.Float,false,6*sizeof(float),3*sizeof(float)); 
            
         
             
            //Enable it so that with each read bu gpu it will be read to  at each vertex component read.

            //GL.EnableVertexAttribArray(1);



/*
 
     
     (0) VAB ->VBO [0][1][2][3][4][5][6][7]
                       ^
                       |
     (1) VAB -----------
     pointer to an array with different moving pattern on other set of embeded data.

*/
            
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            if (shaderP != null)
            {

            shaderP?.Use();
            GL.BindVertexArray(VertextArrayObject);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
            //Using element to use triangles to construct own shapes.
            //GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
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
