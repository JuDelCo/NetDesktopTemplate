using ImGuiNET;
using Raylib_cs;
using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace NetTemplate
{
	public static class ImGuiImpl
	{
		private static IntPtr context;
		private static Texture2D fontTexture;
		private static Vector2 windowSize;
		private static Vector2 scaleFactor = Vector2.One;

		public static unsafe void Startup()
		{
			// Creates a texture and loads the font data from ImGui.
			context = ImGui.CreateContext();
			ImGui.SetCurrentContext(context);
			var io = ImGui.GetIO();
			io.Fonts.AddFontDefault();
			io.NativePtr->IniFilename = null;

			io.SetClipboardTextFn = (IntPtr)(delegate* managed<void*, char*, void>)&SetClipboardText;
			io.GetClipboardTextFn = (IntPtr)(delegate* managed<void*, char*>)&GetClipboardText;
			io.ClipboardUserData = IntPtr.Zero;

			windowSize = new Vector2(Raylib.GetScreenWidth(), Raylib.GetScreenHeight());
			LoadFontTexture();
			SetupInput();
			ImGui.NewFrame();
		}

		public static void OnBeginFrame(float deltaTime)
		{
			// dt = Raylib.GetFrameTime();

			if (Raylib.IsWindowResized())
			{
				windowSize = new Vector2(Raylib.GetScreenWidth(), Raylib.GetScreenHeight());

				var io = ImGui.GetIO();
				io.DisplaySize = windowSize / scaleFactor;
			}

			SetPerFrameData(deltaTime);
			UpdateInput();

			ImGui.NewFrame();
		}

		public static void OnEndFrame()
		{
			// Gets the geometry as set up by ImGui and sends it to the graphics device

			ImGui.Render();
			RenderCommandLists(ImGui.GetDrawData());
		}

		public static void Shutdown()
		{
			ImGui.DestroyContext(context);
			Raylib.UnloadTexture(fontTexture);
		}

		private static unsafe char* GetClipboardText(void* _)
		{
			var clipboard = Raylib.GetClipboardText();

			return (char*)Marshal.StringToHGlobalAuto(clipboard);
		}

		private static unsafe void SetClipboardText(void* _, char* text)
		{
			var managedString = Marshal.PtrToStringAnsi(new IntPtr(text));

			Raylib.SetClipboardText(managedString);
		}

		private static unsafe void LoadFontTexture()
		{
			var io = ImGui.GetIO();

			// Load as RGBA 32-bit (75% of the memory is wasted, but default font is so small) because it is more likely to be compatible with user's existing shaders.
			// If your ImTextureId represent a higher-level concept than just a GL texture id, consider calling GetTexDataAsAlpha8() instead to save on GPU memory.
			io.Fonts.GetTexDataAsRGBA32(out byte* pixels, out var width, out var height);

			// Upload texture to graphics system
			var data = new IntPtr(pixels);
			var image = new Image
			{
				data = data,
				width = width,
				height = height,
				mipmaps = 1,
				format = PixelFormat.PIXELFORMAT_UNCOMPRESSED_R8G8B8A8,
			};
			fontTexture = Raylib.LoadTextureFromImage(image);

			// Store our identifier
			io.Fonts.SetTexID(new IntPtr(fontTexture.id));

			// Clears font data on the CPU side
			io.Fonts.ClearTexData();
		}

		private static void SetupInput()
		{
			// Setup back-end capabilities flags
			ImGuiIOPtr io = ImGui.GetIO();
			io.BackendFlags |= ImGuiBackendFlags.HasMouseCursors;
			io.BackendFlags |= ImGuiBackendFlags.HasSetMousePos;

			// Keyboard mapping. ImGui will use those indices to peek into the io.KeysDown[] array.
			io.KeyMap[(int)ImGuiKey.Tab] = (int)KeyboardKey.KEY_TAB;
			io.KeyMap[(int)ImGuiKey.LeftArrow] = (int)KeyboardKey.KEY_LEFT;
			io.KeyMap[(int)ImGuiKey.RightArrow] = (int)KeyboardKey.KEY_RIGHT;
			io.KeyMap[(int)ImGuiKey.UpArrow] = (int)KeyboardKey.KEY_UP;
			io.KeyMap[(int)ImGuiKey.DownArrow] = (int)KeyboardKey.KEY_DOWN;
			io.KeyMap[(int)ImGuiKey.PageUp] = (int)KeyboardKey.KEY_PAGE_UP;
			io.KeyMap[(int)ImGuiKey.PageDown] = (int)KeyboardKey.KEY_PAGE_DOWN;
			io.KeyMap[(int)ImGuiKey.Home] = (int)KeyboardKey.KEY_HOME;
			io.KeyMap[(int)ImGuiKey.End] = (int)KeyboardKey.KEY_END;
			io.KeyMap[(int)ImGuiKey.Insert] = (int)KeyboardKey.KEY_INSERT;
			io.KeyMap[(int)ImGuiKey.Delete] = (int)KeyboardKey.KEY_DELETE;
			io.KeyMap[(int)ImGuiKey.Backspace] = (int)KeyboardKey.KEY_BACKSPACE;
			io.KeyMap[(int)ImGuiKey.Space] = (int)KeyboardKey.KEY_SPACE;
			io.KeyMap[(int)ImGuiKey.Enter] = (int)KeyboardKey.KEY_ENTER;
			io.KeyMap[(int)ImGuiKey.Escape] = (int)KeyboardKey.KEY_ESCAPE;
			io.KeyMap[(int)ImGuiKey.A] = (int)KeyboardKey.KEY_A;
			io.KeyMap[(int)ImGuiKey.C] = (int)KeyboardKey.KEY_C;
			io.KeyMap[(int)ImGuiKey.V] = (int)KeyboardKey.KEY_V;
			io.KeyMap[(int)ImGuiKey.X] = (int)KeyboardKey.KEY_X;
			io.KeyMap[(int)ImGuiKey.Y] = (int)KeyboardKey.KEY_Y;
			io.KeyMap[(int)ImGuiKey.Z] = (int)KeyboardKey.KEY_Z;
		}

		private static void SetPerFrameData(float dt)
		{
			// Sets per-frame data based on the associated window.
			// This is called by Update(float).

			var io = ImGui.GetIO();
			io.DisplaySize = windowSize / scaleFactor;
			io.DisplayFramebufferScale = Vector2.One;
			io.DeltaTime = dt;
		}

		private static void UpdateInput()
		{
			UpdateMousePosAndButtons();
			UpdateMouseCursor();
			UpdateGamepads();

			var io = ImGui.GetIO();
			var key = Raylib.GetCharPressed();

			while (key > 0)
			{
				if ((key >= 32) && (key <= 125))
				{
					io.AddInputCharacter((char)key);
				}

				key = Raylib.GetCharPressed();
			}
		}

		private static void UpdateMousePosAndButtons()
		{
			// Update mouse buttons
			var io = ImGui.GetIO();
			for (var i = 0; i < io.MouseDown.Count; i++)
			{
				io.MouseDown[i] = Raylib.IsMouseButtonDown((MouseButton)i);
			}

			// Modifiers are not reliable across systems
			io.KeyCtrl = io.KeysDown[(int)KeyboardKey.KEY_LEFT_CONTROL] || io.KeysDown[(int)KeyboardKey.KEY_RIGHT_CONTROL];
			io.KeyShift = io.KeysDown[(int)KeyboardKey.KEY_LEFT_SHIFT] || io.KeysDown[(int)KeyboardKey.KEY_RIGHT_SHIFT];
			io.KeyAlt = io.KeysDown[(int)KeyboardKey.KEY_LEFT_ALT] || io.KeysDown[(int)KeyboardKey.KEY_RIGHT_ALT];
			io.KeySuper = io.KeysDown[(int)KeyboardKey.KEY_LEFT_SUPER] || io.KeysDown[(int)KeyboardKey.KEY_RIGHT_SUPER];

			// Mouse scroll
			io.MouseWheel += Raylib.GetMouseWheelMove();

			// Key states
			for (var i = (int)KeyboardKey.KEY_SPACE; i < (int)KeyboardKey.KEY_KB_MENU + 1; i++)
			{
				io.KeysDown[i] = Raylib.IsKeyDown((KeyboardKey)i);
			}

			// Update mouse position
			var mousePositionBackup = io.MousePos;
			io.MousePos = new Vector2(-float.MaxValue, -float.MaxValue);
			//const bool focused = true;

			//if (focused)
			//{
			if (io.WantSetMousePos)
			{
				Raylib.SetMousePosition((int)mousePositionBackup.X, (int)mousePositionBackup.Y);
			}
			else
			{
				io.MousePos = Raylib.GetMousePosition();
			}
			//}
		}

		private static void UpdateMouseCursor()
		{
			var io = ImGui.GetIO();

			/*if ((io.ConfigFlags & ImGuiConfigFlags.NoMouseCursorChange) == 0 || Raylib.IsCursorHidden())
			{
				return;
			}*/

			var imGuiCursor = ImGui.GetMouseCursor();

			if (imGuiCursor == ImGuiMouseCursor.None || io.MouseDrawCursor)
			{
				Raylib.HideCursor();
			}
			else
			{
				Raylib.ShowCursor();

				switch (imGuiCursor)
				{
					case ImGuiMouseCursor.Arrow:
						Raylib.SetMouseCursor(MouseCursor.MOUSE_CURSOR_ARROW);
						break;
					case ImGuiMouseCursor.TextInput:
						Raylib.SetMouseCursor(MouseCursor.MOUSE_CURSOR_IBEAM);
						break;
					case ImGuiMouseCursor.ResizeAll:
						Raylib.SetMouseCursor(MouseCursor.MOUSE_CURSOR_RESIZE_ALL);
						break;
					case ImGuiMouseCursor.ResizeNS:
						Raylib.SetMouseCursor(MouseCursor.MOUSE_CURSOR_RESIZE_NS);
						break;
					case ImGuiMouseCursor.ResizeEW:
						Raylib.SetMouseCursor(MouseCursor.MOUSE_CURSOR_RESIZE_EW);
						break;
					case ImGuiMouseCursor.ResizeNESW:
						Raylib.SetMouseCursor(MouseCursor.MOUSE_CURSOR_RESIZE_NESW);
						break;
					case ImGuiMouseCursor.ResizeNWSE:
						Raylib.SetMouseCursor(MouseCursor.MOUSE_CURSOR_RESIZE_NWSE);
						break;
					case ImGuiMouseCursor.Hand:
						Raylib.SetMouseCursor(MouseCursor.MOUSE_CURSOR_POINTING_HAND);
						break;
					case ImGuiMouseCursor.NotAllowed:
						Raylib.SetMouseCursor(MouseCursor.MOUSE_CURSOR_NOT_ALLOWED);
						break;
					case ImGuiMouseCursor.COUNT:
						Raylib.SetMouseCursor(MouseCursor.MOUSE_CURSOR_CROSSHAIR);
						break;
					default:
						Raylib.SetMouseCursor(MouseCursor.MOUSE_CURSOR_DEFAULT);
						break;
				}
			}
		}

		private static void UpdateGamepads()
		{
			// TODO: Gamepad support
			//var io = ImGui.GetIO();
		}

		private static Color GetColor(uint hexValue)
		{
			// Returns a Color struct from hexadecimal value

			Color color;

			color.r = (byte)(hexValue & 0xFF);
			color.g = (byte)(hexValue >> 8 & 0xFF);
			color.b = (byte)(hexValue >> 16 & 0xFF);
			color.a = (byte)(hexValue >> 24 & 0xFF);

			return color;
		}

		private static void DrawTriangleVertex(ImDrawVertPtr idxVert)
		{
			var c = GetColor(idxVert.col);
			Rlgl.rlColor4ub(c.r, c.g, c.b, c.a);
			Rlgl.rlTexCoord2f(idxVert.uv.X, idxVert.uv.Y);
			Rlgl.rlVertex2f(idxVert.pos.X, idxVert.pos.Y);
		}

		private static void DrawTriangles(uint count, ImVector<ushort> idxBuffer, ImPtrVector<ImDrawVertPtr> idxVert, int idxOffset, int vtxOffset, IntPtr textureId)
		{
			// Draw the imgui triangle data

			var texId = (uint)textureId;

			Rlgl.rlCheckRenderBatchLimit((int)count * 3);
			/*if (Rlgl.rlCheckBufferLimit((int)count * 3))
			{
				Rlgl.rlglDraw();
			}*/

			Rlgl.rlPushMatrix();
			Rlgl.rlBegin(Rlgl.RL_TRIANGLES);
			Rlgl.rlSetTexture(texId);
			//Rlgl.rlEnableTexture(texId);

			for (var i = 0; i <= count - 3; i += 3)
			{
				var index = idxBuffer[idxOffset + i];
				var vertex = idxVert[vtxOffset + index];
				DrawTriangleVertex(vertex);

				index = idxBuffer[idxOffset + i + 2];
				vertex = idxVert[vtxOffset + index];
				DrawTriangleVertex(vertex);

				index = idxBuffer[idxOffset + i + 1];
				vertex = idxVert[vtxOffset + index];
				DrawTriangleVertex(vertex);
			}

			//Rlgl.rlDisableTexture();
			Rlgl.rlEnd();
			Rlgl.rlPopMatrix();
		}

		private static void RenderCommandLists(ImDrawDataPtr drawData)
		{
			// Scale coordinates for retina displays (screen coordinates != framebuffer coordinates)
			var fbWidth = (int)(drawData.DisplaySize.X * drawData.FramebufferScale.X);
			var fbHeight = (int)(drawData.DisplaySize.Y * drawData.FramebufferScale.Y);

			// Avoid rendering if display is minimized or if the command list is empty
			if (fbWidth <= 0 || fbHeight <= 0 || drawData.CmdListsCount == 0)
			{
				return;
			}

			drawData.ScaleClipRects(ImGui.GetIO().DisplayFramebufferScale);
			Rlgl.rlDisableBackfaceCulling();

			for (var n = 0; n < drawData.CmdListsCount; n++)
			{
				var cmdList = drawData.CmdListsRange[n];

				// Vertex buffer and index buffer generated by Dear ImGui
				var vtxBuffer = cmdList.VtxBuffer;
				var idxBuffer = cmdList.IdxBuffer;

				for (var index = 0; index < cmdList.CmdBuffer.Size; index++)
				{
					var cmdPtr = cmdList.CmdBuffer[index];
					if (cmdPtr.UserCallback != IntPtr.Zero)
					{
						// pcmd.UserCallback(cmdList, pcmd);
					}
					else
					{
						var pos = drawData.DisplayPos;
						var rectX = (int)((cmdPtr.ClipRect.X - pos.X) * drawData.FramebufferScale.X);
						var rectY = (int)((cmdPtr.ClipRect.Y - pos.Y) * drawData.FramebufferScale.Y);
						var rectW = (int)((cmdPtr.ClipRect.Z - rectX) * drawData.FramebufferScale.X);
						var rectH = (int)((cmdPtr.ClipRect.W - rectY) * drawData.FramebufferScale.Y);

						if (!(rectX < fbWidth && rectY < fbHeight && rectW >= 0.0f && rectH >= 0.0f)) continue;

						Raylib.BeginScissorMode(rectX, rectY, rectW, rectH);
						DrawTriangles(cmdPtr.ElemCount, idxBuffer, vtxBuffer, (int)cmdPtr.IdxOffset, (int)cmdPtr.VtxOffset, cmdPtr.TextureId);
					}
				}
			}

			Raylib.EndScissorMode();
			Rlgl.rlEnableBackfaceCulling();
		}
	}
}
