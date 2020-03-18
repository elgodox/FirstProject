using Godot;
using System;
using System.Collections.Generic;

public class HexManager : Node
{
	[Export] int badOnes;

	List <HexNode> _hexes = new List<HexNode>();

	public override void _Ready()
	{
		CheckChildsHexNodes();
		MakeBadOnes();
	}

    private void CheckChildsHexNodes()
    {
        var allChilds = GetChildren();

		foreach(HexNode hex in allChilds) //Todos los hijos estan obligados a ser HexNode	
		{
			_hexes.Add(hex);
		}
    }

    private void MakeBadOnes()
    {
		badOnes = Mathf.Clamp(badOnes, 0, _hexes.Count);

		GD.Print("badOnes: " + badOnes);

		List<int> badOnesList = new List<int>();
		
		for (int i = 0; i < badOnes; i++)
		{
			Random randomNumber = new Random();
			int badRand = (randomNumber.Next() % _hexes.Count);
			if(!badOnesList.Contains(badRand))
			{
				badOnesList.Add(badRand);
				GD.Print("Uno de los malos va a ser el slot: " + badRand);
				_hexes[badRand].goodOne = false;
				_hexes[badRand].asigned = true;
			}
			else
			{
				GD.Print(badRand + " salió repetido :o");
				i --;
			}
		}
    }


    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
