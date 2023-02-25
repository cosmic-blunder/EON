using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace EON.Native{
  
  
    public class Shader {
        int Handle;
        int VertexShader;
        int FragmentShader;
        int shader;
        int status;
        public Shader(string vertextPath,string fragmentPath){
            string VertexShaderSource  = File.ReadAllText(vertextPath);
            string FragmentShaderSource = File.ReadAllText(fragmentPath);

            VertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(VertexShader,VertexShaderSource);

            FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(FragmentShader,FragmentShaderSource);

            //Compile these shaders .

            GL.CompileShader(VertexShader);
            GL.GetShader(shader,ShaderParameter.CompileStatus,out  status);

            if (status == 0) {
                string infoLog = GL.GetShaderInfoLog(VertexShader);
                Console.WriteLine(infoLog);
            }
            GL.CompileShader(FragmentShader);
            GL.GetShader(shader,ShaderParameter.CompileStatus,out  status);

            if (status == 0) {
                string infoLog = GL.GetShaderInfoLog(FragmentShader);
                Console.WriteLine(infoLog);
            }

            Handle = GL.CreateProgram();
            GL.AttachShader(Handle,VertexShader);
            GL.AttachShader(Handle,FragmentShader);

            GL.LinkProgram(Handle);
            int program = 0;
            GL.GetProgram(program,GetProgramParameterName.LinkStatus,out status);
            if (status == 0) {
                string infoLog = GL.GetProgramInfoLog(program);
                Console.WriteLine(infoLog);

            }

            GL.DetachShader(Handle,VertexShader);
            GL.DetachShader(Handle,FragmentShader);

            GL.DeleteShader(FragmentShader);
            GL.DeleteShader(VertexShader);

        }

        public void Use() {
            GL.UseProgram(Handle);
        }

        private bool disposedValue = false;
        protected virtual void Dispose(bool disposing) {
            if (disposedValue != true) {
                GL.DeleteProgram(Handle);
                disposedValue = true;
            }
        }
        ~Shader() {
            GL.DeleteProgram(Handle);
        }

       public void Disposed() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }


public class Native:GameWindow
{
        string ShaderFrg = Path.GetFullPath(@"../shader/shader.frag");
        string ShaderVert= Path.GetFullPath(@"../shader/shader.vert");

        int VertexBufferObject;
        int ElementBufferObject;
        public Shader? shaderP; 
        public int VertextArrayObject;
        public Native(int width,int height,string title):
          base(GameWindowSettings.Default,
                new NativeWindowSettings(){  Size=(width,height),Title=title}){  
      }

      public float[ ] vertices = {
                0.5f,0.5f,0.0f,//top right
                 0.5f,-0.5f,0.0f, //Bottom  right vertix
                 -0.5f,-0.5f,0.0f,//bottom left
                 -0.5f,0.5f,0.0f   //top  left
           };

        uint [] indices =  {
            0,1,3,
            1,2,3
        };
        protected override void OnUpdateFrame(FrameEventArgs args) {
            base.OnUpdateFrame(args);

            KeyboardState input = KeyboardState;

            if (input.IsKeyDown(Keys.Escape)) {
                Close();
            }
        }

        protected override void OnLoad() {

            base.OnLoad();
            shaderP = new Shader(this.ShaderVert,this.ShaderFrg);
            GL.ClearColor(0.2f, 0.3f,0.3f,1.0f);
             
            VertexBufferObject = GL.GenBuffer();
            VertextArrayObject = GL.GenVertexArray();

          
            GL.BindVertexArray(VertextArrayObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer,VertexBufferObject);

            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            
            ElementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer,ElementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer,indices.Length*sizeof(uint),indices,BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0,3,VertexAttribPointerType.Float,false,3*sizeof(float),0);
            GL.EnableVertexAttribArray(0);


        }

        protected override void OnRenderFrame(FrameEventArgs e) {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            
            if(shaderP!=null){

                shaderP?.Use();
            }
            
            //GL.BindVertexArray(VertextArrayObject);
            //GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
             GL.DrawElements(PrimitiveType.Triangles,indices.Length,DrawElementsType.UnsignedInt,0);
            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e) {

            base.OnResize(e);

            GL.Viewport(0,0,e.Width,e.Height);
        }

        protected override void OnUnload() {
            if (shaderP != null) {
                         shaderP.Disposed();
       
            }
         }
    }
}