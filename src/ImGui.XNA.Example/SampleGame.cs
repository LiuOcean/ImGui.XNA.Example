namespace Example;

public class SampleGame : Game
{
    private GraphicsDeviceManager _graphics;
    private ImGuiRenderer         _im_gui_renderer;

    private float   f;
    private bool    show_test_window;
    private bool    show_another_window;
    private Vector3 clear_color = new(114f / 255f, 144f / 255f, 154f / 255f);
    private byte[]  _textBuffer = new byte[100];

    public SampleGame()
    {
        _graphics                           = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth  = 1024;
        _graphics.PreferredBackBufferHeight = 768;
        _graphics.PreferMultiSampling       = true;

        IsMouseVisible = true;

        Window.AllowUserResizing =  true;
        Window.ClientSizeChanged += (_, _) => { _graphics.ApplyChanges(); };
    }

    protected override void Initialize()
    {
        _im_gui_renderer = new ImGuiRenderer(this);
        _im_gui_renderer.RebuildFontAtlas();

        base.Initialize();

        var io = ImGui.GetIO();
        io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
        _SetStyle();
        
        // Magic so ctrl+c and ctrl+v work in mac and linux!
        if((OperatingSystem.IsMacOS() || OperatingSystem.IsLinux()))
        {
            if(!OperatingSystemHelpers.ClipboardDependencyExists())
            {
                var missing_dependency = OperatingSystem.IsMacOS()
                    ? OperatingSystemHelpers.AppKit
                    : OperatingSystemHelpers.SDL;
                Console.WriteLine(
                    $"Clipboard support is disabled. Could not load necessary dependency: '{missing_dependency}'."
                );
                return;
            }

            if(OperatingSystemHelpers.GetFnPtr is { } getFnPtr)
            {
                io.GetClipboardTextFn = getFnPtr;
            }

            if(OperatingSystemHelpers.SetFnPtr is { } setFnPtr)
            {
                io.SetClipboardTextFn = setFnPtr;
            }
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(new Color(clear_color.X, clear_color.Y, clear_color.Z));

        // Call BeforeLayout first to set things up
        _im_gui_renderer.BeforeLayout(gameTime);

        // Draw our UI
        ImGuiLayout();

        // Call AfterLayout now to finish up and draw all the things
        _im_gui_renderer.AfterLayout();

        base.Draw(gameTime);
    }

    protected virtual void ImGuiLayout()
    {
        var viewport = ImGui.GetMainViewport();
        ImGui.SetNextWindowPos(viewport.WorkPos);
        ImGui.SetNextWindowSize(viewport.WorkSize);
        ImGui.SetNextWindowViewport(viewport.ID);

        var window_flags = ImGuiWindowFlags.NoTitleBar            |
                           ImGuiWindowFlags.NoCollapse            |
                           ImGuiWindowFlags.NoResize              |
                           ImGuiWindowFlags.NoMove                |
                           ImGuiWindowFlags.NoBringToFrontOnFocus |
                           ImGuiWindowFlags.NoNavFocus            |
                           ImGuiWindowFlags.MenuBar               |
                           ImGuiWindowFlags.NoDocking;

        ImGui.Begin("MenuBar", window_flags);
        var id = ImGui.GetID("MyDockSpace");
        ImGui.DockSpace(id, Vector2.Zero);

        if(ImGui.BeginMenuBar())
        {
            if(ImGui.BeginMenu("中文菜单"))
            {
                ImGui.MenuItem("我是菜单 1");
                ImGui.MenuItem("中文测试");
                ImGui.EndMenu();
            }

            ImGui.EndMenuBar();
        }

        // 1. Show a simple window
        // Tip: if we don't call ImGui.Begin()/ImGui.End() the widgets appears in a window automatically called "Debug"
        {
            ImGui.Begin("My GUI");
            ImGui.Text("Hello, world!");
            ImGui.SliderFloat("float", ref f, 0.0f, 1.0f, string.Empty);
            ImGui.ColorEdit3("clear color", ref clear_color);
            if(ImGui.Button("Test Window")) show_test_window       = !show_test_window;
            if(ImGui.Button("Another Window")) show_another_window = !show_another_window;
            ImGui.Text(
                $"Application average {1000f / ImGui.GetIO().Framerate:F3} ms/frame ({ImGui.GetIO().Framerate:F1} FPS)"
            );

            ImGui.InputText("Text input", _textBuffer, 100);
            ImGui.End();
        }

        // 2. Show another simple window, this time using an explicit Begin/End pair
        if(show_another_window)
        {
            ImGui.SetNextWindowSize(new Vector2(200, 100), ImGuiCond.FirstUseEver);
            ImGui.Begin("Another Window", ref show_another_window);
            ImGui.Text("Hello");
            ImGui.End();
        }

        // 3. Show the ImGui test window. Most of the sample code is in ImGui.ShowTestWindow()
        if(show_test_window)
        {
            ImGui.SetNextWindowPos(new Vector2(650, 20), ImGuiCond.FirstUseEver);
            ImGui.ShowDemoWindow(ref show_test_window);
        }
    }


    private static void _SetStyle()
    {
        var colors = ImGui.GetStyle().Colors;
        colors[(int) ImGuiCol.Text]                  = new Vector4(1.00f, 1.00f, 1.00f, 1.00f);
        colors[(int) ImGuiCol.TextDisabled]          = new Vector4(0.50f, 0.50f, 0.50f, 1.00f);
        colors[(int) ImGuiCol.WindowBg]              = new Vector4(0.10f, 0.10f, 0.10f, 1.00f);
        colors[(int) ImGuiCol.ChildBg]               = new Vector4(0.00f, 0.00f, 0.00f, 0.00f);
        colors[(int) ImGuiCol.PopupBg]               = new Vector4(0.19f, 0.19f, 0.19f, 0.92f);
        colors[(int) ImGuiCol.Border]                = new Vector4(0.19f, 0.19f, 0.19f, 0.29f);
        colors[(int) ImGuiCol.BorderShadow]          = new Vector4(0.00f, 0.00f, 0.00f, 0.24f);
        colors[(int) ImGuiCol.FrameBg]               = new Vector4(0.05f, 0.05f, 0.05f, 0.54f);
        colors[(int) ImGuiCol.FrameBgHovered]        = new Vector4(0.19f, 0.19f, 0.19f, 0.54f);
        colors[(int) ImGuiCol.FrameBgActive]         = new Vector4(0.20f, 0.22f, 0.23f, 1.00f);
        colors[(int) ImGuiCol.TitleBg]               = new Vector4(0.00f, 0.00f, 0.00f, 1.00f);
        colors[(int) ImGuiCol.TitleBgActive]         = new Vector4(0.06f, 0.06f, 0.06f, 1.00f);
        colors[(int) ImGuiCol.TitleBgCollapsed]      = new Vector4(0.00f, 0.00f, 0.00f, 1.00f);
        colors[(int) ImGuiCol.MenuBarBg]             = new Vector4(0.14f, 0.14f, 0.14f, 1.00f);
        colors[(int) ImGuiCol.ScrollbarBg]           = new Vector4(0.05f, 0.05f, 0.05f, 0.54f);
        colors[(int) ImGuiCol.ScrollbarGrab]         = new Vector4(0.34f, 0.34f, 0.34f, 0.54f);
        colors[(int) ImGuiCol.ScrollbarGrabHovered]  = new Vector4(0.40f, 0.40f, 0.40f, 0.54f);
        colors[(int) ImGuiCol.ScrollbarGrabActive]   = new Vector4(0.56f, 0.56f, 0.56f, 0.54f);
        colors[(int) ImGuiCol.CheckMark]             = new Vector4(0.33f, 0.67f, 0.86f, 1.00f);
        colors[(int) ImGuiCol.SliderGrab]            = new Vector4(0.34f, 0.34f, 0.34f, 0.54f);
        colors[(int) ImGuiCol.SliderGrabActive]      = new Vector4(0.56f, 0.56f, 0.56f, 0.54f);
        colors[(int) ImGuiCol.Button]                = new Vector4(0.5f,  0.5f,  0.5f,  0.5f);
        colors[(int) ImGuiCol.ButtonHovered]         = new Vector4(0.19f, 0.19f, 0.19f, 0.54f);
        colors[(int) ImGuiCol.ButtonActive]          = new Vector4(0.20f, 0.22f, 0.23f, 1.00f);
        colors[(int) ImGuiCol.Header]                = new Vector4(0.00f, 0.00f, 0.00f, 0.52f);
        colors[(int) ImGuiCol.HeaderHovered]         = new Vector4(0.00f, 0.00f, 0.00f, 0.36f);
        colors[(int) ImGuiCol.HeaderActive]          = new Vector4(0.20f, 0.22f, 0.23f, 0.33f);
        colors[(int) ImGuiCol.Separator]             = new Vector4(0.28f, 0.28f, 0.28f, 0.29f);
        colors[(int) ImGuiCol.SeparatorHovered]      = new Vector4(0.44f, 0.44f, 0.44f, 0.29f);
        colors[(int) ImGuiCol.SeparatorActive]       = new Vector4(0.40f, 0.44f, 0.47f, 1.00f);
        colors[(int) ImGuiCol.ResizeGrip]            = new Vector4(0.28f, 0.28f, 0.28f, 0.29f);
        colors[(int) ImGuiCol.ResizeGripHovered]     = new Vector4(0.44f, 0.44f, 0.44f, 0.29f);
        colors[(int) ImGuiCol.ResizeGripActive]      = new Vector4(0.40f, 0.44f, 0.47f, 1.00f);
        colors[(int) ImGuiCol.Tab]                   = new Vector4(0.00f, 0.00f, 0.00f, 0.52f);
        colors[(int) ImGuiCol.TabHovered]            = new Vector4(0.14f, 0.14f, 0.14f, 1.00f);
        colors[(int) ImGuiCol.TabActive]             = new Vector4(0.20f, 0.20f, 0.20f, 0.36f);
        colors[(int) ImGuiCol.TabUnfocused]          = new Vector4(0.00f, 0.00f, 0.00f, 0.52f);
        colors[(int) ImGuiCol.TabUnfocusedActive]    = new Vector4(0.14f, 0.14f, 0.14f, 1.00f);
        colors[(int) ImGuiCol.DockingPreview]        = new Vector4(0.33f, 0.67f, 0.86f, 1.00f);
        colors[(int) ImGuiCol.DockingEmptyBg]        = new Vector4(0.00f, 0.00f, 0.00f, 1.00f);
        colors[(int) ImGuiCol.PlotLines]             = new Vector4(1.00f, 0.00f, 0.00f, 1.00f);
        colors[(int) ImGuiCol.PlotLinesHovered]      = new Vector4(1.00f, 0.00f, 0.00f, 1.00f);
        colors[(int) ImGuiCol.PlotHistogram]         = new Vector4(1.00f, 0.00f, 0.00f, 1.00f);
        colors[(int) ImGuiCol.PlotHistogramHovered]  = new Vector4(1.00f, 0.00f, 0.00f, 1.00f);
        colors[(int) ImGuiCol.TableHeaderBg]         = new Vector4(0.00f, 0.00f, 0.00f, 0.52f);
        colors[(int) ImGuiCol.TableBorderStrong]     = new Vector4(0.00f, 0.00f, 0.00f, 0.52f);
        colors[(int) ImGuiCol.TableBorderLight]      = new Vector4(0.28f, 0.28f, 0.28f, 0.29f);
        colors[(int) ImGuiCol.TableRowBg]            = new Vector4(0.00f, 0.00f, 0.00f, 0.00f);
        colors[(int) ImGuiCol.TableRowBgAlt]         = new Vector4(1.00f, 1.00f, 1.00f, 0.06f);
        colors[(int) ImGuiCol.TextSelectedBg]        = new Vector4(0.20f, 0.22f, 0.23f, 1.00f);
        colors[(int) ImGuiCol.DragDropTarget]        = new Vector4(0.33f, 0.67f, 0.86f, 1.00f);
        colors[(int) ImGuiCol.NavHighlight]          = new Vector4(1.00f, 0.00f, 0.00f, 1.00f);
        colors[(int) ImGuiCol.NavWindowingHighlight] = new Vector4(1.00f, 0.00f, 0.00f, 0.70f);
        colors[(int) ImGuiCol.NavWindowingDimBg]     = new Vector4(1.00f, 0.00f, 0.00f, 0.20f);
        colors[(int) ImGuiCol.ModalWindowDimBg]      = new Vector4(1.00f, 0.00f, 0.00f, 0.35f);

        var style = ImGui.GetStyle();
        style.WindowPadding     = new Vector2(8.00f, 8.00f);
        style.FramePadding      = new Vector2(5.00f, 2.00f);
        style.CellPadding       = new Vector2(6.00f, 6.00f);
        style.ItemSpacing       = new Vector2(6.00f, 6.00f);
        style.ItemInnerSpacing  = new Vector2(6.00f, 6.00f);
        style.TouchExtraPadding = new Vector2(0.00f, 0.00f);
        style.IndentSpacing     = 25;
        style.ScrollbarSize     = 15;
        style.GrabMinSize       = 10;
        style.WindowBorderSize  = 1;
        style.ChildBorderSize   = 1;
        style.PopupBorderSize   = 1;
        style.FrameBorderSize   = 1;
        style.TabBorderSize     = 1;
        style.WindowRounding    = 7;
        style.ChildRounding     = 4;
        style.FrameRounding     = 3;
        style.PopupRounding     = 4;
        style.ScrollbarRounding = 9;
        style.GrabRounding      = 3;
        style.LogSliderDeadzone = 4;
        style.TabRounding       = 4;
    }
}