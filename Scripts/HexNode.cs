using Godot;
using System;

public class HexNode : Node
{
    public delegate void SendPressedNode(HexNode node);
    public SendPressedNode nodePressed;
    public bool goodOne = true;
    public bool asigned;
    public bool pressed = false;
    AnimatedSprite sprite;
    public override void _Ready()
    {
        sprite = GetChild(0) as AnimatedSprite;
        sprite.Animation = "Idle";
        sprite.Play();
    }
    void _on_button_down()
    {
        if (!pressed)
        {
            pressed = true;
            if (goodOne)
            {
                sprite.Animation = "isGood";
            }
            else
            {
                sprite.Animation = "isBad";
            }
            nodePressed(this);
        }

    }
}

