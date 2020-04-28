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
    public void IsBad(bool win)
    {
        if (!win)
        {
            animation.CurrentAnimation = "Bad";
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
