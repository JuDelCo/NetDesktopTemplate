using ImGuiNET;
using Raylib_cs;
using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace NetTemplate
{
	public class Program
	{
		private static void Main() // string[] args
		{
			Raylib.InitWindow(1280, 720, "Hello World");
			Raylib.SetWindowState(ConfigFlags.FLAG_WINDOW_RESIZABLE);
			Raylib.SetTargetFPS(60);
			ImGuiImpl.Startup();

			// ------------------------------------------------------------
			// Setup state
			// ------------------------------------------------------------

			Texture2D lenna = Raylib.LoadTexture("Resources/lenna.png");

			var whitenoise = new Image();
			var totalBytes = 128 * 128 * sizeof(byte) * 4;
			whitenoise.data = Marshal.AllocHGlobal(totalBytes);
			unsafe
			{
				byte* data = (byte*)whitenoise.data;
				for (int i = 0; i < totalBytes; i += 4)
				{
					data[i + 0] = (byte)Raylib.GetRandomValue(0, 255);
					data[i + 1] = (byte)Raylib.GetRandomValue(0, 255);
					data[i + 2] = (byte)Raylib.GetRandomValue(0, 255);
					data[i + 3] = 255;
				}
			}
			whitenoise.width = 128;
			whitenoise.height = 128;
			whitenoise.mipmaps = 1;
			whitenoise.format = PixelFormat.PIXELFORMAT_UNCOMPRESSED_R8G8B8A8;
			Texture2D noise = Raylib.LoadTextureFromImage(whitenoise);
			Raylib.UnloadImage(whitenoise);

			var cube_w = 2.0f;
			var cube_h = 2.0f;
			var cube_l = 2.0f;
			var cube_color = new Color(255, 255, 255, 255);

			var camera = new Camera3D
			{
				position = new Vector3(0.0f, 10.0f, 10.0f),
				target = new Vector3(0.0f, 0.0f, 0.0f),
				up = new Vector3(0.0f, 1.0f, 0.0f),
				fovy = 45.0f
			};

			// ------------------------------------------------------------

			while (!Raylib.WindowShouldClose())
			{
				Raylib.BeginDrawing();
				Raylib.ClearBackground(Color.BLACK);
				ImGuiImpl.OnBeginFrame(Raylib.GetFrameTime());

				// ------------------------------------------------------------
				// Frame logic
				// ------------------------------------------------------------

				Raylib.DrawText("Hello, world!", 12, 40, 20, Color.WHITE);
				Raylib.DrawFPS(10, 10);

				Raylib.BeginMode3D(camera);
				Raylib.DrawCube(Vector3.Zero, cube_w, cube_h, cube_l, cube_color);
				Raylib.DrawCubeWires(Vector3.Zero, cube_w, cube_h, cube_l, Color.MAROON);
				Raylib.DrawGrid(10, 1.0f);
				Raylib.EndMode3D();

				ImGui.Begin("Image");
				ImGui.Image(new IntPtr(noise.id), new Vector2(noise.width, noise.height));
				ImGui.Image(new IntPtr(lenna.id), new Vector2(lenna.width, lenna.height));
				ImGui.End();

				ImGui.Begin("Cube");
				ImGui.Text("fps = " + Raylib.GetFPS());
				ImGui.SliderFloat("cube w", ref cube_w, 0.0f, 5.0f);
				ImGui.SliderFloat("cube h", ref cube_h, 0.0f, 5.0f);
				ImGui.SliderFloat("cube d", ref cube_l, 0.0f, 5.0f);
				var cube_color_vector = new Vector3(cube_color.r / 255f, cube_color.g / 255f, cube_color.b / 255f);
				ImGui.ColorPicker3("cube color", ref cube_color_vector);
				cube_color = new Color((byte)(cube_color_vector.X * 255), (byte)(cube_color_vector.Y * 255), (byte)(cube_color_vector.Z * 255), cube_color.a);
				ImGui.End();

				ImGui.ShowDemoWindow();

				// ------------------------------------------------------------

				ImGuiImpl.OnEndFrame();
				Raylib.EndDrawing();
			}

			Raylib.CloseWindow();
		}
	}
}
