using Godot;
using System;
using System.Collections.Generic;

public class GameGenerator
{
    int hexNodes = 19;
    int badOnes;
    public bool bonusGenerated;
    public bool bonusAssigned;
    int bonusChance = 100;
    int bonusLevel = 8;
    public int bonusSlot;

    public string levelDescription;
    public int[] GenerateLevelInfo(int level)
    {
        if(!bonusGenerated && level == bonusLevel)
        {
            CheckBonusChance();
        }
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

        for (int i = 0; i < hexNodes; i++) // for 19 veces
        {
            if(i > 0 && i < hexNodes)
            {
                descriptionInfo += ",";
            }

            for (int x = 0; x < levelInfo.Length; x++) // for de cantidad de nodos activos
            {
                if(i == levelInfo[x])
                {
                    if(x == levelInfo.Length - (badOnes + 1) && bonusGenerated && !bonusAssigned)
                    {
                        bonusAssigned = true;
                        GD.Print("Bonus en el índice " + i + " en el nivel " + levelInfo.Length + " con " + bonusChance + " de probabilidad");
                        bonusSlot = i;
                        descriptionInfo += "3";
                        break;
                    }
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
    public void ResetBonus()
    {
        GD.Print("Resetting bonus");
        bonusGenerated = false;
        bonusAssigned = false;
    }
    public void CheckBonusChance()
    {
        bonusChance = Mathf.Clamp(bonusChance, 0, 100);

        Random rand = new Random();
        if (rand.Next(1, 101) <= bonusChance)
        {
            bonusGenerated = true;
            GD.Print("Bonus Generado!");
        }
    }
    public void SetBadOnes(int badOnesAmount)
    {   
        badOnes = badOnesAmount;
    }
    public void SetBonusChance(int chance)
    {   
        bonusChance = chance;
    }
}
