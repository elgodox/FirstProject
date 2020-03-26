using Godot;
using System;
using System.Collections.Generic;

public class HexManager : Node
{
	public GameManager myGameManager;
	[Export] public int badOnes;
	[Export] String pathAnimation;

	List <HexNode> _hexes = new List<HexNode>();

	AnimationPlayer animation;
	public override void _Ready()
	{
		CheckChildsHexNodes();
		MakeBadOnes();
		foreach (HexNode item in _hexes)
		{
			item.nodePressed += ReceiveNodePressed;
		}
		animation = GetNode<AnimationPlayer>(pathAnimation);
		animation.CurrentAnimation = "Init";
	}

	private void CheckChildsHexNodes()
	{
		var allChilds = GetChildren();

		HexNode e = new HexNode();

		foreach(var hex in allChilds) //Todos los hijos estan obligados a ser HexNode	
		{
			if(hex.GetType() == e.GetType())
			{
				_hexes.Add(hex as HexNode);
			}
			else continue;
		}
	}

	private void MakeBadOnes()
	{
		badOnes = Mathf.Clamp(badOnes, 0, _hexes.Count);

		List<int> badOnesList = new List<int>();
		
		for (int i = 0; i < badOnes; i++)
		{
			Random randomNumber = new Random();
			int badRand = (randomNumber.Next() % _hexes.Count);
			if(!badOnesList.Contains(badRand))
			{
				badOnesList.Add(badRand);
				//GD.Print("Uno de los malos va a ser el slot: " + badRand);
				_hexes[badRand].goodOne = false;
				_hexes[badRand].asigned = true;
			}
			else
			{
				i --;
			}
		}
	}

	public void DestroyHexManager()
	{
		if(myGameManager.currentHexMngr == this)
		{
			myGameManager.currentHexMngr = null;
		}
		this.QueueFree();
	}

	private void ReceiveNodePressed(HexNode node)
	{

		myGameManager.CheckHexSelected((node.goodOne));

		foreach (var item in _hexes)
		{
			item.pressed = true;
		}
		
		if(node.goodOne)
		{
			animation.CurrentAnimation="Exit";
		}
		else
		{
			
		}
	}

	
	
}
