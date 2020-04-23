using Godot;
using System;

public class BonusManager : LevelManager
{
    float[] multipliers = new float[4];
    public override void _Ready()
    {
        _nodes = new HexNode[4];
        base._Ready();
    }
    public override void ReceiveNodePressed(HexNode node)
    {
        myGameManager.EndGame(true);
        foreach (var item in _nodes)
		{
			if(item != null)
			{
				item.pressed = true;
			}
		}
        animation.CurrentAnimation = "Exit";
    }
}
