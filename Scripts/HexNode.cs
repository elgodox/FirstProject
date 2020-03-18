using Godot;
using System;

public class HexNode : Node
{
    public bool goodOne = true;
    public bool asigned;

    AnimatedSprite sprite;

    public override void _Ready()
    {
        sprite = GetChild(0) as AnimatedSprite;
    }


    void _on_button_down()
    {
        if (goodOne)
        {
            GD.Print("Soy de los buenos :D");
            sprite.Animation = "isGood";
        }
        else
        {
            GD.Print("Soy de los malos :c");
            sprite.Animation = "isBad";
        }
    }
}
