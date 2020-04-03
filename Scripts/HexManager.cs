using Godot;
using System;
using System.Collections.Generic;

public class HexManager : Node
{
	public GameManager myGameManager;
	[Export] public int badOnes;
	[Export] public int activeOnes;
	[Export] String pathAnimation;

	HexNode[] _hexes = new HexNode[19];
	AnimationPlayer animation;
	public override void _Ready()
	{
		CheckChildsHexNodes();
		animation = GetNode<AnimationPlayer>(pathAnimation);
		animation.CurrentAnimation = "Init";
	}

	private void CheckChildsHexNodes()
	{
		var allChilds = GetChildren();

		HexNode e = new HexNode();
		
		List <HexNode> _hexesList = new List<HexNode>();

		foreach(var hex in allChilds)
		{
			if(hex.GetType() == e.GetType())
			{
				_hexesList.Add(hex as HexNode);
			}
		}

		for (int i = 0; i < _hexesList.Count; i++)
		{
			int arrayIndex = RecursiveCheckNodesNames(_hexesList[i].Name, _hexesList.Count - 1);
			_hexes[arrayIndex] = _hexesList[i];
		}

		foreach (HexNode item in _hexes)
		{
			if(item != null)
			{
				item.nodePressed += ReceiveNodePressed;
			}
		}
	}

	int RecursiveCheckNodesNames(string HexName, int counter)
	{
		if(counter < 0)
		{
			return _hexes.Length - 1;
		}
		if("Hex"+ counter == HexName)
		{
			return counter;
		}
		else
		{
			return RecursiveCheckNodesNames(HexName,counter - 1);
		}

	}

	// private void MakeBadOnes()
	// {
	// 	badOnes = Mathf.Clamp(badOnes, 0, _hexes.Length);

	// 	List<int> badOnesList = new List<int>();
		
	// 	for (int i = 0; i < badOnes; i++)
	// 	{
	// 		Random randomNumber = new Random();
	// 		int badRand = (randomNumber.Next() % _hexes.Length);
	// 		if(!badOnesList.Contains(badRand) && _hexes[badRand] != null)
	// 		{
	// 			badOnesList.Add(badRand);
	// 			//GD.Print("Uno de los malos va a ser el slot: " + badRand);
	// 			_hexes[badRand].goodOne = false;
	// 			_hexes[badRand].asigned = true;
	// 		}
	// 		else
	// 		{
	// 			i --;
	// 		}
	// 	}
	// }

	// void MakeActives()
	// {
	// 	activeOnes = Mathf.Clamp(activeOnes, 2, _hexes.Length);
	// 	List<int> activeOnesList = new List<int>();

	// 	for (int i = 0; i < activeOnes; i++)
	// 	{
	// 		Random randomNumber = new Random();
	// 		int activeRand = (randomNumber.Next() % _hexes.Length);

	// 		if(!activeOnesList.Contains(activeRand))
	// 		{
	// 			activeOnesList.Add(activeRand);
	// 		}
	// 		else
	// 		{
	// 			i--;
	// 		}
	// 	}
	// 	for (int i = 0; i < _hexes.Length; i++)
	// 	{
	// 		if(!activeOnesList.Contains(i))
	// 		{
	// 			_hexes[i].QueueFree();
	// 			_hexes[i] = null;
	// 			_hexes[i] = default;
	// 		}
	// 	}
	// }

	public void DestroyHexManager() //La llama la animación de Exit
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
			if(item != null)
			{
				item.pressed = true;
			}
		}
		
		animation.CurrentAnimation = "Exit";
	}

	public void SetActivesPositions(int[] actives, int badOnes)
	{
		for (int i = 0; i < actives.Length; i++) // asigna los nodos activos, los últinmos son considerados malos
		{
			if(i >= actives.Length - badOnes)
			{
				_hexes[actives[i]].goodOne = false;
			}
			else
			{
				_hexes[actives[i]].goodOne = true;
			}

			_hexes[actives[i]].asigned = true;
			_hexes[actives[i]].asigned = true;
			//GD.Print(_hexes[actives[i]].Name + " asigned: " + _hexes[actives[i]].asigned);
		}

		for (int i = 0; i < _hexes.Length; i++) // borra los que no estan asignados (activos)
		{
			if(!_hexes[i].asigned)
			{
				_hexes[i].QueueFree();
				_hexes[i] = null;
				_hexes[i] = default;
			}
		}
	}
}
