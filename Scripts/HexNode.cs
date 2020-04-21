using Godot;
using System;

public class HexNode : Node
{
    public delegate void SendPressedNode(HexNode node);
    public SendPressedNode nodePressed;
    public AudioStreamPlayer2D audio;
    [Export] AudioStream isGood;
    [Export] AudioStream isBad;

    public bool goodOne = true;
    public bool bonus = false;
    public bool asigned = false;
    public bool pressed = false;
    AnimatedSprite sprite;

    Label myName;
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
            if (goodOne)
            {
                sprite.Animation = "isGood";
                audio.Stream = isGood;   
            }
            else
            {
                sprite.Animation = "isBad";
                audio.Stream = isBad;
            }
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
}

