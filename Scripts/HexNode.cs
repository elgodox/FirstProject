using Godot;
using System;

public class HexNode : Node
{
    public delegate void SendPressedNode(HexNode node);
    public SendPressedNode nodePressed;
    public AudioStreamPlayer2D audio;
    [Export] AudioStream isGood;
    [Export] AudioStream isBad;
    [Export] AudioStream isBonus;
    [Signal] public delegate void EndAnimaionIsBonus();

    public bool goodOne = true;
    public bool bonus = false;
    public bool asigned = false;
    public bool pressed = false;
    public double bonusMultiplier;
    AnimatedSprite sprite;

    public Label myName;
    public override void _Ready()
    {
        sprite = GetChild(0) as AnimatedSprite;
        myName = GetChild(1) as Label;
        myName.Text = Name;
        sprite.Playing = false;
        audio = GetNode("HexAudio") as AudioStreamPlayer2D;
    }
    void _on_button_down()
    {
        if (!pressed)
        {
            pressed = true;
            if (bonus)
            {
                sprite.Animation = "isBonus";
                audio.Stream = isBonus;    
            }
            else if (goodOne)
            {
                sprite.Animation = "isGood";
                audio.Stream = isGood;   
            }
            else
            {
                sprite.Animation = "isBad";
                audio.Stream = isBad;
            }

            audio.VolumeDb = 1;
            audio.Play();
            nodePressed(this);
        }

    }

    public void ShowMe()
    {
        sprite.Animation = "Appear";
        sprite.Playing = true;
        sprite.Play();
    }

    public void HideMe()
    {
        sprite.Animation = "Exit";
        sprite.Playing = true;
        sprite.Play();
    }

    public void SetMultiply(string multipliers)
    {
        myName.Text = multipliers;
    }
    
    public void ShowMultiply()
    {
        myName.Show();
    }
    
    public void HideMultiply()
    {
        myName.Hide();
    }

    public void IdleAnimation()
    {
        if (sprite.Animation == "Appear")
        { 
            sprite.Animation = "Idle";
            sprite.Playing = true;
            sprite.Play();
        }

        if (sprite.Animation == "isBonus")
        {
            EmitSignal(nameof(EndAnimaionIsBonus));
        }
    }
    
}

