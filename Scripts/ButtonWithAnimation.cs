using Godot;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

public class ButtonWithAnimation : TextureButton
{
    [Signal] public delegate void OnDisabled();

    AnimationPlayer buttonAnimator;
    bool _disabled;
    
    

    public override void _Ready()
    {
        //_disabled = Disabled;
        //bool _disabled = Disabled;
        //buttonAnimator = GetNode()
    }
    // public override bool DisabledButton
    // {
    //     get
    //     {
    //         return _disabled;
    //     }
    //
    //     set
    //     {
    //         //#3
    //         _disabled = value;
    //         GD.Print("Disabled: " + _disabled);
    //         EmitSignal(nameof(OnDisabled));
    //     }
    // }
    
    public bool Disabled
    {
        get
        {
            GD.Print("get");
            return this.IsDisabled();
        }
        set
        {
            this.SetDisabled(value);
            GD.Print("adasd");
        }
    }
}
