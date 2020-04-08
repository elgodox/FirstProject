using Godot;
using System;
using System.Collections.Generic;

public class GameRecover
{
    Dictionary<int, string> myLevels = new Dictionary<int, string>();

    Dictionary<int, string> myPlays = new Dictionary<int, string>();
    string betDescription;
    int minLevel = 2;
    public GameRecover(String description)
    {
        betDescription = description;
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

        //GD.Print("Se llegó hasta el nivel " + levelReached);
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
            if(currentChar == '1')
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
