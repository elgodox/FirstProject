using Godot;
using System;
using System.Collections.Generic;
using System.Text;

public class BonusManager : LevelManager
{
	[Signal] public delegate void BonusPicked();

	CurrencyLabel[] bonusPrices = new CurrencyLabel[4];
	private AnimatedSprite _imageBonusCenter;
    public override void _Ready()
    {
	    _imageBonusCenter = GetNode("ImageBonus/AnimatedSprite") as AnimatedSprite;
	    bonusPrices[0] = GetNode("Hex0/BonusPrice0") as CurrencyLabel;
	    bonusPrices[1] = GetNode("Hex1/BonusPrice1") as CurrencyLabel;
	    bonusPrices[2] = GetNode("Hex2/BonusPrice2") as CurrencyLabel;
	    bonusPrices[3] = GetNode("Hex3/BonusPrice3") as CurrencyLabel;
        _nodes = new HexNode[4];
        base._Ready();
        _imageBonusCenter.Playing = true;
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
				item.ShowMultiply();
			}
		}
    }

    public void SetMultipliersPositions(int[] randomPos)
    {
	    for (int i = 0; i < randomPos.Length; i++)
	    {
		    var currentRandomIndex = randomPos[i];

		    _nodes[i].bonusMultiplier = Constants.BONUS_MULTIPLIERS[currentRandomIndex];
		    _nodes[i].bonus = true;
		    _nodes[i].goodOne = false;
		    var price = _nodes[i].bonusMultiplier.ToString();
		    price = price.Replace(',', '.');
		    bonusPrices[i].UpdateMyType(price);
		    _nodes[i].SetMultiply("X"+bonusPrices[i].myType);
	    }
    }

    protected override void DestroyHexManager()
    {
	    base.DestroyHexManager();
    }

    public void DestroyBonusAnimation()//La llama el nodo cuando finaliza su animacion de isBonus
    {
	    animation.CurrentAnimation = "Exit";
    }
}
