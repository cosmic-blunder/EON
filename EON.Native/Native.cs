using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;


using System;

using StbImageSharp;
using System.IO;
using OpenTK.Mathematics;

namespace EON.Native
{

   // This is the camera class as it could be set up after the tutorials on the website.
    // It is important to note there are a few ways you could have set up this camera.
    // For example, you could have also managed the player input inside the camera class,
    // and a lot of the properties could have been made into functions.

    // TL;DR: This is just one of many ways in which we could have set up the camera.
    // Check out the web version if you don't know why we are doing a specific thing or want to know more about the code.
    public class Camera
    {
        // Those vectors are directions pointing outwards from the camera to define how it rotated.
        private Vector3 _front = -Vector3.UnitZ;

        private Vector3 _up = Vector3.UnitY;

        private Vector3 _right = Vector3.UnitX;

        // Rotation around the X axis (radians)
        private float _pitch;

        // Rotation around the Y axis (radians)
        private float _yaw = -MathHelper.PiOver2; // Without this, you would be started rotated 90 degrees right.

        // The field of view of the camera (radians)
        private float _fov = MathHelper.PiOver2;

        public Camera(Vector3 position, float aspectRatio)
        {
            Position = position;
            AspectRatio = aspectRatio;
        }

        // The position of the camera
        public Vector3 Position { get; set; }

        // This is simply the aspect ratio of the viewport, used for the projection matrix.
        public float AspectRatio { private get; set; }

        public Vector3 Front => _front;

        public Vector3 Up => _up;

        public Vector3 Right => _right;

        // We convert from degrees to radians as soon as the property is set to improve performance.
        public float Pitch
        {
            get => MathHelper.RadiansToDegrees(_pitch);
            set
            {
                // We clamp the pitch value between -89 and 89 to prevent the camera from going upside down, and a bunch
                // of weird "bugs" when you are using euler angles for rotation.
                // If you want to read more about this you can try researching a topic called gimbal lock
                var angle = MathHelper.Clamp(value, -89f, 89f);
                _pitch = MathHelper.DegreesToRadians(angle);
                UpdateVectors();
            }
        }

        // We convert from degrees to radians as soon as the property is set to improve performance.
        public float Yaw
        {
            get => MathHelper.RadiansToDegrees(_yaw);
            set
            {
                _yaw = MathHelper.DegreesToRadians(value);
                UpdateVectors();
            }
        }

        // The field of view (FOV) is the vertical angle of the camera view.
        // This has been discussed more in depth in a previous tutorial,
        // but in this tutorial, you have also learned how we can use this to simulate a zoom feature.
        // We convert from degrees to radians as soon as the property is set to improve performance.
        public float Fov
        {
            get => MathHelper.RadiansToDegrees(_fov);
            set
            {
                var angle = MathHelper.Clamp(value, 1f, 90f);
                _fov = MathHelper.DegreesToRadians(angle);
            }
        }

        // Get the view matrix using the amazing LookAt function described more in depth on the web tutorials
        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(Position, Position + _front, _up);
        }

        // Get the projection matrix using the same method we have used up until this point
        public Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio, 0.01f, 100f);
        }

        // This function is going to update the direction vertices using some of the math learned in the web tutorials.
        private void UpdateVectors()
        {
            // First, the front matrix is calculated using some basic trigonometry.
            _front.X = MathF.Cos(_pitch) * MathF.Cos(_yaw);
            _front.Y = MathF.Sin(_pitch);
            _front.Z = MathF.Cos(_pitch) * MathF.Sin(_yaw);

            // We need to make sure the vectors are all normalized, as otherwise we would get some funky results.
            _front = Vector3.Normalize(_front);

            // Calculate both the right and the up vector using cross product.
            // Note that we are calculating the right from the global up; this behaviour might
            // not be what you need for all cameras so keep this in mind if you do not want a FPS camera.
            _right = Vector3.Normalize(Vector3.Cross(_front, Vector3.UnitY));
            _up = Vector3.Normalize(Vector3.Cross(_right, _front));
        }
    }



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

        private readonly Dictionary<string, int> _uniformLocations;
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
            
                 // The shader is now ready to go, but first, we're going to cache all the shader uniform locations.
            // Querying this from the shader is very slow, so we do it once on initialization and reuse those values
            // later.

             int  numberOfUniforms;
            // First, we have to get the number of active uniforms in the shader.
            GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out numberOfUniforms );

            // Next, allocate the dictionary to hold the locations.
            _uniformLocations = new Dictionary<string, int>();

            // Loop over all the uniforms,
            for (var i = 0; i < numberOfUniforms; i++)
            {
                // get the name of this uniform,
                var key = GL.GetActiveUniform(Handle, i, out _, out _);

                // get the location,
                var location = GL.GetUniformLocation(Handle, key);

                // and then add it to the dictionary.
                _uniformLocations.Add(key, location);
            }


        }


        public void SetInt(string name , int val){
            int location   = GL.GetUniformLocation(Handle,name);

            GL.Uniform1(location,val);
        }

       public void SetMatrix4(string name, Matrix4 data)
        {
            GL.UseProgram(Handle);//load shader and changke the uniforms variables state.
            GL.UniformMatrix4(_uniformLocations[name],true, ref data);
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
       

       //tranformations

       Matrix4 matrix{get;set;}
       
       int Width {get;set;}
       int Height {get;set;}

        public Native(int width, int height, string title) :
          base(GameWindowSettings.Default,
                new NativeWindowSettings() { Size = (width, height), Title = title })
        {
                this.Width = width;
                this.Height=height;

                texWall=null;
        }
        public float[] vertices1 = {
                 0.5f, 0.5f,0.0f,1.0f,1.0f,//top right
                 0.5f,-0.5f,0.0f,1.0f,0.0f, //Bottom  right vertix
                -0.5f,-0.5f,0.0f,0.0f,0.0f,//bottom left
                -0.5f, 0.5f,0.0f,0.0f,1.0f  //top  left
           };
           float[] vertices = {
    -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
     0.5f, -0.5f, -0.5f,  1.0f, 0.0f,
     0.5f,  0.5f, -0.5f,  1.0f, 1.0f, ///Face one
     
     0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
    -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
    -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,

    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
     0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
     0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
     
     0.5f,  0.5f,  0.5f,  1.0f, 1.0f,//Face two
    -0.5f,  0.5f,  0.5f,  0.0f, 1.0f,
    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,

    -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
    -0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,//Face three
    
    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
    -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

     0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
     0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
     0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
    
     0.5f, -0.5f, -0.5f,  0.0f, 1.0f,//face four
     0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
     0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
     0.5f, -0.5f, -0.5f,  1.0f, 1.0f,
     0.5f, -0.5f,  0.5f,  1.0f, 0.0f,//Face five
    
     0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,

    -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
     0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
     0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
    
     0.5f,  0.5f,  0.5f,  1.0f, 0.0f,//Face six
    -0.5f,  0.5f,  0.5f,  0.0f, 0.0f,
    -0.5f,  0.5f, -0.5f,  0.0f, 1.0f
};

        uint[] indices =  {
            0,1,3,
            1,2,3
        };

        //textures
        Texture? texWall {get;set;}
        Texture texFace{ get;set;}

        /**
           
            0       3
            1       2
        */

        



        Matrix4 _view;
        Matrix4 _projection;
        Camera _camera;


        private bool _firstMove = true;

        private Vector2 _lastPos;

  
        protected override void OnLoad()
        {
            base.OnLoad();
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            
            VertextArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(VertextArrayObject);
            
            VertexBufferObject = GL.GenBuffer();
     

            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.DynamicDraw);

           // ElementBufferObject = GL.GenBuffer();
           // GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
           // GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);
            
            shaderP = new Shader(this.ShaderVert, this.ShaderFrg);
            shaderP.Use();
            
            var vertexLocation = shaderP.GetAttrib("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            int texCoord = shaderP.GetAttrib("aTexCoord");
            GL.VertexAttribPointer(texCoord,2,VertexAttribPointerType.Float,false,5*sizeof(float),3*sizeof(float));
            GL.EnableVertexAttribArray(texCoord);

         

            texWall =  new Texture(Path.GetFullPath(@"../Texture/wall.jpeg"));
            texFace =  new Texture(Path.GetFullPath(@"../Texture/wall.jpeg"),TextureUnit.Texture1);

            shaderP.SetInt("texture0", 0);
            shaderP.SetInt("texture1", 1);


            _view = Matrix4.CreateTranslation(0.0f,0.0f,-3.0f);
            _projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f),Size.X/(float)Size.Y,0.1f,100.0f);
         
             _camera =  new Camera(Vector3.UnitZ*3,Size.X/(float)Size.Y);



             CursorState = CursorState.Grabbed;

             GL.Enable(EnableCap.DepthTest);

      

        }
        float rot = -55.0f;
        //float scale1=0.1f;   
        double _time=0.0f;
        float moveLeft=0;
        float moveright=0; 

        float moveDown = 0;

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            
            Title = $"EON: (Vsync: {VSync}) FPS: {1f / e.Time:0}";

            GL.Clear(ClearBufferMask.ColorBufferBit|ClearBufferMask.DepthBufferBit);

            //transformation
            _time+=4.0*e.Time;

            Matrix4 model = Matrix4.Identity*Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(_time));
            texWall.Use(TextureUnit.Texture0);
            texFace.Use(TextureUnit.Texture1);
             
             if (shaderP != null)
            {

             for(int i =0;i<1;i++){
             var trans = Matrix4.CreateTranslation((float)(i + moveright), (float)(i+moveDown), i);
             var scal = Matrix4.CreateScale(3,3,3);

             model = Matrix4.Identity*Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(_time))*trans*scal;
             shaderP.SetMatrix4("model", model);
             shaderP.SetMatrix4("view", _camera.GetViewMatrix());
             shaderP.SetMatrix4("projection", _camera.GetProjectionMatrix());


             //set uniform
             shaderP?.Use();
             
             GL.BindVertexArray(VertextArrayObject);
             
             GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
  

             }

             for(int i =0;i<1;i++){
             var trans = Matrix4.CreateTranslation(4, 4, 4);

             model = Matrix4.Identity*Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(_time))*trans;

             shaderP.SetMatrix4("model", model);
             shaderP.SetMatrix4("view", _camera.GetViewMatrix());
             shaderP.SetMatrix4("projection", _camera.GetProjectionMatrix());


             //set uniform
             shaderP?.Use();
             
             GL.BindVertexArray(VertextArrayObject);
             
             GL.DrawArrays(PrimitiveType.Triangles, 0, 36);


             }

             
         
             SwapBuffers();
            }
        }
        protected override void OnResize(ResizeEventArgs e)
        {

            base.OnResize(e);

          
            GL.Viewport(0, 0, Size.X, Size.Y);

            _camera.AspectRatio = Size.X / (float)Size.Y;

        }

        protected override void OnUnload()
        {
            if (shaderP != null)
            {
                shaderP.Disposed();

            }
        }
          protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if (!IsFocused) // Check to see if the window is focused
            {
                return;
            }

            var input = KeyboardState;

            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            const float cameraSpeed = 4.5f;
            const float sensitivity = 0.2f;

            if (input.IsKeyDown(Keys.W))
            {
                _camera.Position += _camera.Front * cameraSpeed * (float)e.Time; // Forward
            }

            if(input.IsKeyDown(Keys.Right)){
                moveright = (float)(0.1 + moveright);
            }
             if(input.IsKeyDown(Keys.Left)){
                moveright=(float)(moveright-0.1);
            }

            if(input.IsKeyDown(Keys.Down)){
                 moveDown = (float)(moveDown-0.01);
            }
            if(input.IsKeyDown(Keys.Up)){
                 moveDown = (float)(moveDown+0.01);
            }

            if (input.IsKeyDown(Keys.S))
            {
                _camera.Position -= _camera.Front * cameraSpeed * (float)e.Time; // Backwards
            }
            if (input.IsKeyDown(Keys.A))
            {
                _camera.Position -= _camera.Right * cameraSpeed * (float)e.Time; // Left
            }
            if (input.IsKeyDown(Keys.D))
            {
                _camera.Position += _camera.Right * cameraSpeed * (float)e.Time; // Right
            }
            if (input.IsKeyDown(Keys.Space))
            {
                _camera.Position += _camera.Up * cameraSpeed * (float)e.Time; // Up
            }
            if (input.IsKeyDown(Keys.LeftShift))
            {
                _camera.Position -= _camera.Up * cameraSpeed * (float)e.Time; // Down
            }

            // Get the mouse state
            var mouse = MouseState;

            if (_firstMove) // This bool variable is initially set to true.
            {
                _lastPos = new Vector2(mouse.X, mouse.Y);
                _firstMove = false;
            }
            else
            {
                // Calculate the offset of the mouse position
                var deltaX = mouse.X - _lastPos.X;
                var deltaY = mouse.Y - _lastPos.Y;
                _lastPos = new Vector2(mouse.X, mouse.Y);

                // Apply the camera pitch and yaw (we clamp the pitch in the camera class)
                _camera.Yaw += deltaX * sensitivity;
                _camera.Pitch -= deltaY * sensitivity; // Reversed since y-coordinates range from bottom to top
            }
        }

        // In the mouse wheel function, we manage all the zooming of the camera.
        // This is simply done by changing the FOV of the camera.
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            
            _camera.Fov -= e.OffsetY;
        }

      

     
    }
}

