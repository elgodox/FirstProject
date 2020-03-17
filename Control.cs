using Godot;
using System;

public class Control : Godot.Control
{
    [Export]String pathanimationControl; 
    [Export]int maxNumPressed;
    AnimationControl animationControl;

    int numHexPressed = 0;
    public override void _Ready()
    {
        animationControl = GetNode<AnimationControl>(pathanimationControl);
    }
    public override void _Process(float delta)
    {
       
    }
    
    public void _on_TextureButton_button_down()
    {
        numHexPressed++;
        if(numHexPressed>=maxNumPressed)
        {
            EjecuteAnimation();
        }
    }

    public void EjecuteAnimation()
    {
        animationControl.StartExitAnimation();
    }
}
