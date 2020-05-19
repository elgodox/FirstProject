using Godot;
using System;
using System.Collections.Generic;

public class GameGenerator
{
    double[] bonusMultipliers = { 1, 1.5d, 2, 3};
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
                        //GD.Print("Bonus en el índice " + i + " en el nivel " + levelInfo.Length + " con " + bonusChance + " de probabilidad");
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

    public string GetBonusDescription(int[] bonusInfo)
    {
        string result = default;
        
        for (int i = 0; i < bonusInfo.Length; i++)
        {
            var currentIndex = bonusInfo[i];
            
            if(currentIndex == 0){ result+= Constants.BONUS_MULTIPLIERS[0] + "*"; }
            else if(currentIndex == 1) { result+= Constants.BONUS_MULTIPLIERS[1] + "*"; }
            else if(currentIndex == 2) { result+= Constants.BONUS_MULTIPLIERS[2] + "*"; }
            else if(currentIndex == 3) { result+= Constants.BONUS_MULTIPLIERS[3] + "*"; }

            if (i == bonusInfo.Length - 1)
            {
                result = result.Remove(result.Length - 1); // borra la ultima coma
            }
        }
        
        result = result.Replace(',', '.');
        result = result.Replace('*', ',');
        
        //GD.Print("Bonus generado en el siguiente orden de posiciones: " + result);

        return result;
    }
    public void ResetBonus()
    {
        //GD.Print("Resetting bonus");
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
            //GD.Print("Bonus Generado!");
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

    public int[] GenerateBonusInfo()
    {
        List<int> doubleList = new List<int>();
        int[] randoms = new int[4];
        
        for (int i = 0; i < randoms.Length; i++)
        {
            Random randomNumber = new Random();
            int activeRand = (randomNumber.Next() % randoms.Length);

            if(!doubleList.Contains(activeRand))
            {
                doubleList.Add(activeRand);
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
