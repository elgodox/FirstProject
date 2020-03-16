using Godot;
using System;

public class Node2DAlan : Node2D
{
    [Export] Color _alpha;
    [Export] Vector2 _scale;
    [Export] float _rotation;
    Vector2 _startPos = new Vector2(0, 0);
    public override void _Ready()
    {
        _startPos = Transform.origin;
        _rotation = 0.1f;
        _alpha = new Color(0,0,0,0.01f);
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        Scale += _scale;
        RotationDegrees+=_rotation;
        Modulate -=_alpha;
        
        GD.Print(Modulate.a);
        if(Modulate.a<=0){
            this.RemoveAndSkip();
            
        }
    }
}
