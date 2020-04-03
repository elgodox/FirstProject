using Godot;
using System;
using System.Collections.Generic;

public class GameGenerator
{
    //private int[] levelInfo;
    int hexNodes = 19;

    public List<int[]> GenerateGame(int levels)
    {
        List<int[]> gameInfo = new List<int[]>();

        int currentLevel = levels;

        for (int i = 0; i < levels; i++)
        {
            if(currentLevel == 1) // para que el los dos ultimos niveles tengan 2 nodos
            {
                currentLevel++;
            }
            gameInfo.Add(GenerateRandoms(currentLevel));
            currentLevel--;
        }
        return gameInfo;
    }

    int[] GenerateRandoms(int randomCount)
    {
		List<int> activeOnesList = new List<int>();
        int[] randoms = new int[randomCount];

		for (int i = 0; i < randomCount; i++)
		{
			Random randomNumber = new Random();
			int activeRand = (randomNumber.Next() % hexNodes);

			if(!activeOnesList.Contains(activeRand))
			{
				activeOnesList.Add(activeRand);
                randoms[i] = activeRand;
			}
			else
			{
				i--;
			}
		}
        return randoms;
    }
}
