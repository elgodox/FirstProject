using Godot;
using System;
using System.Collections.Generic;
using System.Text;

public class BonusManager : LevelManager
{
	[Signal] public delegate void BonusPicked();

	private AnimatedSprite _initAnimation;
	private Control _bonusCircleInteractive;

	CurrencyLabel[] bonusPrices = new CurrencyLabel[4];
    public override void _Ready()
    {
	    _initAnimation = GetNode("InitAnimation") as AnimatedSprite;
	    _bonusCircleInteractive = GetNode("BonusCircleInteractive") as Control;
	    bonusPrices[0] = GetNode("BonusCircleInteractive/BTN/BonusPrice") as CurrencyLabel;
	    bonusPrices[1] = GetNode("BonusCircleInteractive/BTN2/BonusPrice") as CurrencyLabel;
	    bonusPrices[2] = GetNode("BonusCircleInteractive/BTN3/BonusPrice") as CurrencyLabel;
	    bonusPrices[3] = GetNode("BonusCircleInteractive/BTN4/BonusPrice") as CurrencyLabel;
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
				item.ShowMultiply();
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
		    _nodes[i].bonus = true;
		    _nodes[i].goodOne = false;
		    var price = _nodes[i].bonusMultiplier.ToString();
		    price = price.Replace(',', '.');
		    bonusPrices[i].UpdateMyType(price);
		    _nodes[i].SetMultiply("X"+bonusPrices[i].myType);
		    GD.Print(bonusPrices[i].myType);
	    }
    }

    public void ShowBonusCircleInteractive()
    {
	    _initAnimation.Hide();
	    _bonusCircleInteractive.Show();
    }
}
