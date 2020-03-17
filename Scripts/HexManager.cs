using Godot;
using System;
using System.Collections.Generic;

public class HexManager : Node
{
	[Export] int goodOnes;
	[Export] int badOnes;

	  List <HexNode> _hexes = new List<HexNode>();

	public override void _Ready()
	{
		var allChilds = GetChildren();


		foreach(HexNode hex in allChilds) //Todos los hijos estan obligados a ser HexNode	
		{
			_hexes.Add(hex);
			GD.Print(hex.GetScript().ToString());
		}

		GD.Print("Mi cantidad de hijos es " + _hexes.Count);
		
		 goodOnes = _hexes.Count - badOnes;

		//goodOnes = Mathf.Clamp(goodOnes, 0, _hexes.Count);
		GD.Print("GoodOnes: " + goodOnes);
		GD.Print("BadOnes: " + badOnes);


		MakeBadOnes();
		 
	}

    private void MakeBadOnes()
    {
		for (int i = 0; i < badOnes; i++)
		{
			Random randomNumber = new Random();
			int badRand = (randomNumber.Next() % _hexes.Count);
			GD.Print("Uno de los malos va a ser el slot: " + badRand);
			_hexes[badRand].goodOne = false;
			_hexes[badRand].asigned = true;
		}
    }


    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
