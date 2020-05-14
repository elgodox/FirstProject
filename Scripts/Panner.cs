using Godot;
using System;
using System.Collections.Generic;

public class Panner : Node
{
    private AnimationPlayer _animation;
    int allLevels = 10;
    

    public override void _Ready()
    {
        _animation = GetNode("Background/AnimationPlayer") as AnimationPlayer;
        _animation.Play();
    }

    public void BackToOrigin(bool win, bool bonus)
    {
        if (win)
        {
            Panning(0);
        }
        else
        {
            Panning(10);
        }
    }

    public void BackToInitAnimation()
    {
        _animation.CurrentAnimation = "10";
        _animation.Play();
    }
    
    public void Panning(int currentLevel)
    {
        float currentAnimation = currentLevel;
        _animation.CurrentAnimation = currentAnimation.ToString();
        _animation.Play();
    }
}
