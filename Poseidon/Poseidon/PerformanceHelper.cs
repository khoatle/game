using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using TimeRulerLibrary;

/// <summary>
/// Simple helper around the time ruler mark, to allow you to use the "using" statement
/// </summary>
public struct TimeRulerHelper : IDisposable
{
    private int _index;
    private string _name;
    public TimeRulerHelper(int index, string name, Color c)
    {
        _index = index;
        _name = name;
        PerformanceHelper.TimeRuler.BeginMark(_index, _name, c);
    }
    public TimeRulerHelper(string name, Color c)
        : this(0, name, c)
    {
    }

    public void Dispose()
    {
        PerformanceHelper.TimeRuler.EndMark(_index, _name);
    }
}

public static class PerformanceHelper
{
    private static TimeRuler _currentRuler;
    private static DebugManager _debugManager;
    private static DebugCommandUI _debugCommandUI;
    private static FpsCounter _fpsCounter;
    private static bool _firstFrame = true;

    public static void InitializeWithGame(Game g)
    {
        // Initialize debug manager and add it to components.
        _debugManager = new DebugManager(g);
        g.Components.Add(_debugManager);

        // Initialize debug command UI and add it to compoents.
        _debugCommandUI = new DebugCommandUI(g);

        // Change DrawOrder for render debug command UI on top of other compoents.
        _debugCommandUI.DrawOrder = int.MaxValue;

        g.Components.Add(_debugCommandUI);

        // Initialize FPS counter and add it to compoentns.
        _fpsCounter = new FpsCounter(g);
        g.Components.Add(_fpsCounter);

        // Initialize TimeRuler and add it to compoentns.
        _currentRuler = new TimeRuler(g);
        g.Components.Add(_currentRuler);

    }

    public static TimeRuler TimeRuler
    {
        get
        {
            return _currentRuler;
        }
    }

    public static FpsCounter FpsCounter 
    {
        get { return _fpsCounter; }
    }

    public static void StartFrame()
    {
        if (_firstFrame)
        {
            _firstFrame = false;
            _debugCommandUI.ExecuteCommand("tr on log:on");
            _debugCommandUI.ExecuteCommand("fps on");
        }
        _currentRuler.StartFrame();
    }
}