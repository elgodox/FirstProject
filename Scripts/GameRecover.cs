using Godot;
using System;
using System.Collections.Generic;

public class GameRecover
{
    Dictionary<int, string> myLevels = new Dictionary<int, string>();

    Dictionary<int, string> myPlays = new Dictionary<int, string>();
    string betDescription;
    int minLevel = 2;
    public int bonuSlot;
    public GameRecover(String description)
    {
        betDescription = description;
    }

    public bool GetPossibleBonus()
    {
        string sum = default;
        for (int i = 0; i < betDescription.Length; i++)
        {
            var currentChar = betDescription[i];
            if(currentChar == ';' || currentChar == '|') { bonuSlot = 0; }
            if(currentChar == '0'|| currentChar == '1'||currentChar == '2' || currentChar == '3') { bonuSlot++; }
            if(sum == default)
            {
                if(currentChar == ',' || currentChar == ';' || currentChar == 'P')
                {
                    sum += currentChar;
                }
            }
            else
            {
                if(currentChar == '3')
                {
                    GD.Print("Encontré un Bonus Generado! en el slot " + (bonuSlot -= 1));
                    return true;
                }
                else
                {
                    sum = default;
                }
            }
        }
        
        GD.Print("No encontré Bonus Generado");
        bonuSlot = 0;
        return false;
    }

    public bool GetIfBonusGained()
    {
        int levelOfBonus = 8;
        if(!myPlays.ContainsKey(levelOfBonus)) return false;

        if(myPlays[levelOfBonus] == bonuSlot.ToString())
        {
            GD.Print("El jugador había conseguido el Bonus!");
            return true;
        }

        GD.Print("El jugador NO había conseguido el Bonus");
        return false;
    }

    public int GetLevelReached()
    {
        int levelReached = 10;

        for (int i = 0; i < betDescription.Length; i++)
        {
            var currentChar = betDescription[i];

            if (currentChar == ';')
            {
                levelReached--;
            }
        }

        return levelReached;
    }

    public void FillDictionarys(int levels)
    {
        int pipesCounter = levels;
        bool levelDone = false;
        string levelString = default;
        string playString = default;

        for (int i = 0; i < betDescription.Length; i++)
        {
            var currentChar = betDescription[i];

            if (currentChar == ';')
            {
                levelDone = false;
                myPlays.Add(pipesCounter + 1, playString);
                playString = default;
            }
            if (currentChar == '|')
            {
                levelDone = true;
                myLevels.Add(pipesCounter, levelString);
                levelString = default;
                pipesCounter--;
            }
            else if (currentChar != ';' && currentChar != 'P' && currentChar != ',' && !levelDone)
            {
                levelString += currentChar;
            }
            else if (levelDone)
            {
                playString += currentChar;
            }
        }
        #region  CheckDictionaryPrinter
        // for (int i = levels; i > levels - myLevels.Count; i--)
        // {
        //     GD.Print("el string en el slot " + i + " es: " + myLevels[i] + " con " + myLevels[i].Length + " slots en total");

        //     if (i > (levels - myPlays.Count))
        //     {
        //         GD.Print("Se jugó el slot: " + myPlays[i]);
        //     }
        //     else
        //     {
        //         GD.Print("Se espera continuar el juego desde el nivel " + i);
        //     }
        // }
        #endregion 
    }

    public int[] GetLastLevelInfo(int currentLevel)
    {
        currentLevel = Mathf.Clamp(currentLevel, minLevel, currentLevel);
        int[] levelInfo = new int[currentLevel];

        int counter = 0;
        int badOnesCounter = 0;
        
        for (int i = 0; i < myLevels[currentLevel].Length; i++)
        {
            var currentChar = myLevels[currentLevel][i];
            if(currentChar == '1' || currentChar == '3')
            {
                levelInfo[counter] = i;
                counter++;
            }
            if(currentChar == '2')
            {
                levelInfo[(levelInfo.Length - 1) - badOnesCounter] = i;
                badOnesCounter += 1;
            }

        }

        return levelInfo;
    }
}
