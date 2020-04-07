using Godot;
using System;
using System.Collections.Generic;

public class GameGenerator
{
    int hexNodes = 19;
    int badOnes;

    public string levelDescription;
    
    public int[] GenerateLevelInfo(int level)
    {
        int[] levelInfo = new int[hexNodes];

        if(level <= 1)
        {
            level = 2;
        }

        levelInfo = GenerateRandoms(level);

        return levelInfo;
    }

    int[] GenerateRandoms(int randomAmount)
    {
		List<int> activeOnesList = new List<int>();
        int[] randoms = new int[randomAmount];

		for (int i = 0; i < randomAmount; i++)
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
        GetLevelDescription(randoms);
        return randoms;
    }

    public String GetLevelDescription(int[] levelInfo)
    {
        string descriptionInfo = default;

        for (int i = 0; i < hexNodes; i++)
        {
            if(i > 0 && i < hexNodes)
            {
                descriptionInfo += ",";
            }
            for (int x = 0; x < levelInfo.Length; x++)
            {
                if(i == levelInfo[x])
                {
                    if(x >= levelInfo.Length - badOnes)
                    {
                        descriptionInfo += "2";
                        break;
                    }
                    else
                    {
                        descriptionInfo += "1";
                        break;
                    }
                }
                else if(x >= levelInfo.Length - 1)
                {
                    descriptionInfo += "0";
                }
            }
        }
        levelDescription = descriptionInfo;
        return descriptionInfo;
    }

    public void SetBadOnes(int badOnesAmount)
    {   
        badOnes = badOnesAmount;
    }
}
