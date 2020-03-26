using Godot;
using System;

public class UIButtonsManager : Control
{
    
    [Signal] public delegate void restartGame();
    public override void _Ready()
    {
        
    }
    void PlayButtonUp()
    {
        EmitSignal(nameof(restartGame));
    }
}
