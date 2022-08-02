using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = System.Random;

public class Path : MonoBehaviour
{
    private const int MinVariables = 2;
    private const int MaxActions = 4;
    private static int _currentVariables = MinVariables;

    private static int _oneValue = 0;
    private static readonly char[] Actions = {'+', '-', '*', '/'};
    private static readonly string[] IntegerDivision = {"1/1","2/1","2/2","3/1","3/3","4/1","4/2","4/4","5/1","5/5","6/1","6/2","6/3","6/6","7/1","7/7","8/1","8/2","8/4","8/8","9/1","9/3","9/9" };
    private static List<char> _tempSolutionLevel = new List<char>();
    public static Level CreateLevel(int indexLevel)
    {
        var newLevel = new Level();
        
        if (indexLevel % 5 == 0 && indexLevel != 5)
            _currentVariables++;
        

        newLevel.ID = indexLevel;
        newLevel.Result = ResultLevel();
        newLevel.CaclulatorResultStart = _oneValue;
        newLevel.LevelSolutions = LevelSolutions();
        newLevel.CountWalks = _currentVariables;
        return newLevel;
    }
    public static void RestartValues()
    {
        _currentVariables = MinVariables;
    }

    private static int ResultLevel()
    {
        _tempSolutionLevel = new List<char>();
        var formula = GetExpression(GenerateFormulaLevel(_currentVariables));
        _oneValue = int.Parse(formula[0].ToString());
        var result = Counting(formula);
        return (int)result;
    }
    
    private static double Counting(string input)
    {
        var result = 0.0; 
        var temp = new Stack<double>(); 

        for (var i = 0; i < input.Length; i++) 
        {

            if (char.IsDigit(input[i]))
            {
                var a = string.Empty;

                while (!IsDelimeter(input[i]) && !IsOperator(input[i])) 
                {
                    a += input[i]; 
                    i++;
                    if (i == input.Length) break;
                }
                temp.Push(double.Parse(a)); 
                i--;
            }
            else if (IsOperator(input[i])) 
            {
                var a = temp.Pop();
                var b = temp.Pop();

                switch (input[i]) 
                {
                    case '+': result = b + a; break;
                    case '-': result = b - a; break;
                    case '*': result = b * a; break;
                    case '/': result = b / a; break;
                    case '^': result = double.Parse(Math.Pow(double.Parse(b.ToString()), double.Parse(a.ToString())).ToString()); break;
                }
                temp.Push(result); 
            }
        }
        return temp.Peek(); 
    }
    
    
        private static bool IsDelimeter(char c)
        {
            return " =".IndexOf(c) != -1;
        }

        private static bool IsOperator(char с)
        {
            return "+-/*^()".IndexOf(с) != -1;
        }
 
        private static byte GetPriority(char s)
        {
            switch (s)
            {
                case '(': return 0;
                case ')': return 1;
                case '+': return 2;
                case '-': return 3;
                case '*': return 4;
                case '/': return 4;
                case '^': return 5;
                default: return 6;
            }
        }
 
        private static string GetExpression(string input)
        {
            var output = string.Empty; 
            var operStack = new Stack<char>(); 
 
            for (var i = 0; i < input.Length; i++) 
            {
                if (IsDelimeter(input[i]))
                    continue; 
                
                if (char.IsDigit(input[i])) 
                {
                    while (!IsDelimeter(input[i]) && !IsOperator(input[i]))
                    {
                        output += input[i]; 
                        i++;
 
                        if (i == input.Length) break; 
                    }
 
                    output += " "; 
                    i--; 
                }

                if (!IsOperator(input[i])) 
                    continue;
                switch (input[i])
                {
                    case ')':
                        operStack.Push(input[i]);
                        break;
                    case '(':
                    {
                        var s = operStack.Pop();
 
                        while (s != ')')
                        {
                            output += s.ToString() + ' ';
                            s = operStack.Pop();
                        }

                        break;
                    }
                    default:
                    {
                        if (operStack.Count > 0) 
                            if (GetPriority(input[i]) <= GetPriority(operStack.Peek())) 
                                output += operStack.Pop().ToString() + " "; 
                        operStack.Push(char.Parse(input[i].ToString()));
                        break;
                    }
                }
            }
            
            while (operStack.Count > 0)
                output += operStack.Pop() + " ";
 
            return output;
        }
    private static string GenerateFormulaLevel(int maxCurrentVariables)
    {
        var maxCurrentActions = maxCurrentVariables - 1;
        var currentVariable = 0;
        var currentAction = 0;
        
        var formula = "";
        while (currentVariable != maxCurrentVariables)
        {
            var action = Actions[UnityEngine.Random.Range(0, Actions.Length)];
            if (action == '/' && currentVariable != maxCurrentVariables-1)
            {
                formula += IntegerDivision[UnityEngine.Random.Range(0, IntegerDivision.Length)];
                currentVariable += 2;
                currentAction++;
                _tempSolutionLevel.Add(action);
                if (currentAction == maxCurrentActions) 
                    continue;
                action = Actions[UnityEngine.Random.Range(0, Actions.Length - 1)];
                formula += action.ToString();
                currentAction++;
                _tempSolutionLevel.Add(action);
            }
            else if(currentVariable == maxCurrentVariables-1)
            {
                var valueNumeric = UnityEngine.Random.Range(1, 9);
                formula += valueNumeric;
                currentVariable++;
            }
            else
            {
                var num1 = UnityEngine.Random.Range(1, 9);
                var num2 = UnityEngine.Random.Range(1, 9);
                formula += num1 + action.ToString() + num2;
                currentVariable += 2;
                currentAction++;
                _tempSolutionLevel.Add(action);
                if (currentAction == maxCurrentActions) 
                    continue;
                action = Actions[UnityEngine.Random.Range(0, Actions.Length - 1)];
                formula += action.ToString();
                currentAction++;
                _tempSolutionLevel.Add(action);
            }
        }
        return formula;
    }

    private static char[] LevelSolutions()
    {
        if (_tempSolutionLevel.Count > MaxActions)
            return Actions;
        var tmpSolutions = new List<char>();
            foreach (var solution in _tempSolutionLevel.Where(solution => !tmpSolutions.Contains(solution)))
                tmpSolutions.Add(solution);
            return tmpSolutions.ToArray();;
    }


    


}
