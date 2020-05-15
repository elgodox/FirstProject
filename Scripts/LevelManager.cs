using Godot;
using System;
using System.Collections.Generic;

public class LevelManager : Node
{
	int bonusIndex;
	public bool gotABonus;
	public GameManager myGameManager;
	[Export] String pathAnimation;

	protected HexNode[] _nodes = new HexNode[19];
	protected AnimationPlayer animation;
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
			_nodes[arrayIndex] = _hexesList[i];
		}

		foreach (HexNode item in _nodes)
		{
			if(item != null)
			{
				item.nodePressed += ReceiveNodePressed;
			}
		}
	}

	protected int RecursiveCheckNodesNames(string HexName, int counter)
	{
		if(counter < 0)
		{
			return _nodes.Length - 1;
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

	protected virtual void DestroyHexManager() //La llama la animación de Exit
	{
		if(myGameManager.currentLevelMngr == this)
		{
			myGameManager.currentLevelMngr = null;
		}
		
		this.QueueFree();
	}

	public void HideNodes()
	{
	foreach (var item in _nodes)
		{
			if(item != null)
			{
				item.HideMe();
			}
		}
	}

	public void ExitAnimationBonus()
	{
		DisableSelectAllNodes();
		animation.CurrentAnimation = "ExitBonus";
		animation.Play();
	}
	public void ExitAnimation()
	{
		DisableSelectAllNodes();
		animation.CurrentAnimation = "Exit";
		animation.Play();
	}

	public virtual void ReceiveNodePressed(HexNode node)
	{
		if (node.goodOne)
		{
			node.audio.PitchScale += .1f * (10 - myGameManager.currentLevel);
		}
		myGameManager.CheckHexSelected(node.goodOne, node.Name, node.bonus);
		if (node.bonus)
		{
			ExitAnimationBonus();
		}
		else
		{
			ExitAnimation();
		}
	}

	public void DisableSelectAllNodes()
	{
		foreach (var item in _nodes)
		{
			if(item != null)
			{
				item.pressed = true;
			}
		}
	}

	public void SetActivesPositions(int[] actives, int badOnes)
	{
		for (int i = 0; i < actives.Length; i++) // asigna los nodos activos, los últinmos son considerados malos
		{
			_nodes[actives[i]].pressed = true;

			if(actives[i] == bonusIndex && gotABonus)
			{
				GD.Print("HexManager recibe que el bonus está en el índice " + actives[i]);
				_nodes[actives[i]].bonus = true;
				gotABonus = false;
			}
			if(i >= actives.Length - badOnes)
			{
				_nodes[actives[i]].goodOne = false;
			}
			else
			{
				_nodes[actives[i]].goodOne = true;
			}

			_nodes[actives[i]].asigned = true;
			//GD.Print(_hexes[actives[i]].Name + " asigned: " + _hexes[actives[i]].asigned);
		}

		for (int i = 0; i < _nodes.Length; i++) // borra los que no estan asignados (activos)
		{
			if(!_nodes[i].asigned)
			{
				_nodes[i].QueueFree();
				_nodes[i] = null;
				_nodes[i] = default;
			}
		}
	}

	public void ShowActivesOnes()
	{
		for (int i = 0; i < _nodes.Length; i++) //muestra los asignados activos
		{
			if(_nodes[i] != null)
			{
				_nodes[i].pressed = false;
				_nodes[i].ShowMe();
			}
		}
	}

	public void SetBonusPosition(int index)
	{
		bonusIndex = index;
	}

	void ChooseRandomNode()
	{
		Random random = new Random();
		int randomNumber = (random.Next() % _nodes.Length);
		
		_nodes[randomNumber]._on_button_down();
	}
}
