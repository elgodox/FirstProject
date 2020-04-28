using Godot;
using System;

public class BonusManager : LevelManager
{
    public override void _Ready()
    {
        _nodes = new HexNode[4];
        base._Ready();
    }
    public override void ReceiveNodePressed(HexNode node)
    {
        myGameManager.BonusFinished();
        GD.Print("El nodo presionado tenía un multiplicador de x" + node.bonusMultiplier);
        foreach (var item in _nodes)
		{
			if(item != null)
			{
				item.pressed = true;
			}
		}
        animation.CurrentAnimation = "Exit";
    }

    public void SetMultipliersPositions(int[] randomPos)
    {
	    for (int i = 0; i < randomPos.Length; i++)
	    {
		    var currentRandomIndex = randomPos[i];

		    _nodes[i].bonusMultiplier = Constants.BONUS_MULTIPLIERS[currentRandomIndex];
	    }
    }
}
