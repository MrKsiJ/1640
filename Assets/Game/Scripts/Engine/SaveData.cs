using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public List<int> variablesPlayer = new List<int>();
    public List<Level> levels = new List<Level>();
    public List<int> indexButtons = new List<int>();
    public int variableIndex = 0;
    public int solutionIndex = 0;
    public int currentPlayerResult = 0;
    public int currentCountWalks = 0;
}
