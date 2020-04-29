using Godot;
using System;
using System.Collections.Generic;

public class BonusManager : LevelManager
{
	[Signal] public delegate void BonusPicked();

	CurrencyLabel[] bonusPrices = new CurrencyLabel[4];
    public override void _Ready()
    {
	    bonusPrices[0] = GetNode("Hex0/BonusPrice0") as CurrencyLabel;
	    bonusPrices[1] = GetNode("Hex1/BonusPrice1") as CurrencyLabel;
	    bonusPrices[2] = GetNode("Hex2/BonusPrice2") as CurrencyLabel;
	    bonusPrices[3] = GetNode("Hex3/BonusPrice3") as CurrencyLabel;
        _nodes = new HexNode[4];
        base._Ready();
    }
    public override void ReceiveNodePressed(HexNode node)
    {
	    EmitSignal(nameof(BonusPicked));
        myGameManager.BonusFinished(node.bonusMultiplier);
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
		    var price = _nodes[i].bonusMultiplier.ToString();
		    price = price.Replace(',', '.');
		    bonusPrices[i].UpdateMyType(price);
		    GD.Print(bonusPrices[i].myType);
	    }
    }
}
