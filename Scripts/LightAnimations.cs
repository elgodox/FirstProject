using Godot;
using System;

public class LightAnimations : TextureRect
{
    [Export] String pathAnimation;
    AnimationPlayer animation;
    public override void _Ready()
    {
        animation = GetNode<AnimationPlayer>(pathAnimation);
    }

    public void Idle()
    {
        animation.CurrentAnimation = "Idle";
    }
    public void TurnOn()
    {
        animation.CurrentAnimation = "TurnOn";
    }
    public void IsGood()
    {
        animation.CurrentAnimation = "Good";
    }

    public void TurnOff()
    {
        animation.CurrentAnimation = "TurnOff";
    }
    public void IsBonus()
    { 
        animation.CurrentAnimation = "Bonus";
    }
    public void SetBackgroundAnimation(bool win, bool bonus) //La llama 
    {
        if (bonus)
        {
            animation.CurrentAnimation = "Bonus";
        }
        else if (!win)
        {
            animation.CurrentAnimation = "Bad";
        }
        else
        {
            animation.CurrentAnimation = "Good";
        }
    }
    public void _on_AnimationPlayer_animation_finished(string animationIdle)
    {
        if (animationIdle == "Good")
        {
            Idle();
        }
        if(animationIdle=="Bad")
        {
            TurnOff();
        }
    }
}
