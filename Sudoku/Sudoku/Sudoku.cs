using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;


//Research and Nomenclature material.
//https://en.wikipedia.org/wiki/Glossary_of_Sudoku
//http://www.sudokudragon.com/sudokustrategy.htm
//http://www.sudokudragon.com/advancedstrategy.htm
//http://lipas.uwasa.fi/~timan/sudoku/
//http://sudopedia.enjoysudoku.com/Aligned_Pair_Exclusion.html
//https://www.researchgate.net/publication/261217550_Difficulty_Rating_of_Sudoku_Puzzles_An_Overview_and_Evaluation

namespace Sudoku
{
    public class Sudoku
    {


        public bool Solve()
        {
            int initialUnsolvedCells = CountUnsolvedCells();

            bool isAValidState = IsAValidState();

            if (!isAValidState)
            {
                //Problem went into invalid state. It's likely to happen during backtracking. This possibility will be rejected.
                return false;
            }

            EliminatePossibilitiesFromUnsetCells();
            ProcessPreemptiveSets();
            SetADigitToOnlyAvailablePlace();

            isAValidState = IsAValidState();

            if (!isAValidState)
            {
                //Problem went into invalid state. It's likely to happen during backtracking. This possibility will be rejected.
                return false;
            }

            int unsolvedCells = CountUnsolvedCells();
          

            if (unsolvedCells == 0)
            {
                //We have a complete and valid solution.
                return true;
            }

         
            
            if (unsolvedCells < initialUnsolvedCells)
            {
                Solve();
            }

            if (!SolveByTryingPossibilities())
            {
                //Unsolvable case.It's likely to happen during backtracking. This possibility will be rejected.
                return false;

            }




            unsolvedCells = CountUnsolvedCells();


            if (unsolvedCells > 0)
            {

                isAValidState = IsAValidState();
                if (isAValidState)
                {
                    //THis case should not appear. Represents a bug somewhere
                    Logger.WriteLine("THis case should not appear. Represents a bug somewhere. Problem is solved yet not not sovled.", _cloningDepth);
                    Logger.WriteLine(ToStringFormatted(), _cloningDepth);
                }

                return false;
                

            }

            DifficultyRating = CalculateDifficulty();

            return IsAValidState();
        }

        public void CopyUnSolvedCellsFromSolution(Sudoku clone)
        {
            for (int rowIndex = 0; rowIndex <= 8; rowIndex++)
            {

                for (int colIndex = 0; colIndex <= 8; colIndex++)
                {
                    if (sudokuCells[rowIndex][colIndex].Value == 0)
                    {
                        sudokuCells[rowIndex][colIndex].Value = clone.sudokuCells[rowIndex][colIndex].Value;
                        sudokuCells[rowIndex][colIndex].SolvingDifficulty = clone.sudokuCells[rowIndex][colIndex].SolvingDifficulty;
                    }

                }
            }
        }



        public bool SolveOneIterationOnly()
        {



            int initialUnsolvedCells = CountUnsolvedCells();

            EliminatePossibilitiesFromUnsetCells();
            ProcessPreemptiveSets();
            SetADigitToOnlyAvailablePlace();

            bool isAValidState = IsAValidState();

            if (!isAValidState)
            {
                Logger.WriteLine($"Problem went into invalid state. It's likely to happen during backtracking. This possiblity will be rejected. Level = {_cloningDepth}. State ={ToString()}", _cloningDepth);
                return false;
            }

            int unsolvedCells = CountUnsolvedCells();


            if (unsolvedCells == 0)
            {
                //We have a complete and valid solution.
                return true;
            }

            //If something was solved in this step.. leave rest for next step.
            if(initialUnsolvedCells== unsolvedCells)

                SolveByTryingPossibilities();

            DifficultyRating = CalculateDifficulty();

            unsolvedCells = CountUnsolvedCells();


            if (unsolvedCells == 0)
            {
                //We have a complete and valid solution.
                return true;
            }

            return false;
        }


        private void ProcessPreemptiveSets()
        {
            for (int index = 0; index <= 8; index++)
            {
                EliminatePreemptiveSetsInAGroup(sudokuRows[index]);
                EliminatePreemptiveSetsInAGroup(sudokuColumns[index]);
                EliminatePreemptiveSetsInAGroup(sudokuBoxes[index]);
            }
        }

        public void EliminatePossibilitiesFromUnsetCells()
        {

            for (int index = 0; index <= 8; index++)
            {
                EliminatePossibilitiesFromUnsetCells(sudokuRows[index]);
                EliminatePossibilitiesFromUnsetCells(sudokuColumns[index]);
                EliminatePossibilitiesFromUnsetCells(sudokuBoxes[index]);
            }
        }



      

        private void EliminatePossibilitiesFromUnsetCells(SudokuGroup sudokuGroup)
        {
            HashSet<int> allNumbersPresentInGroup = new HashSet<int>();

            for (int cellIndex = 0; cellIndex <= 8; cellIndex++)
            {
               int cellValue = sudokuGroup.Cells[cellIndex].Value;

               if (cellValue != 0)
               {
                   allNumbersPresentInGroup.Add(cellValue);
               }

            }

    

            for (int cellIndex = 0; cellIndex <= 8; cellIndex++)
            {
                int cellValue = sudokuGroup.Cells[cellIndex].Value;

                if (cellValue == 0)
                {
                    SudokuCell cell = sudokuGroup.Cells[cellIndex];
                    cell.RemainingPossibilities.ExceptWith(allNumbersPresentInGroup);

                    if (cell.RemainingPossibilities.Count == 1)
                    {
                        cell.Value = cell.RemainingPossibilities.First();
                        cell.SolvingDifficulty = SolvingDifficulty.Easy;
                    }
                }

            }
        }
        

        private void SetADigitToOnlyAvailablePlace(SudokuGroup sudokuGroup)
        {
            var digitsYetToBePlaced = new HashSet<int>(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            int[] digitOccurancesMap = new int[10];

            var allUnsetCells = new HashSet<SudokuCell>();

            for (int cellIndex = 0; cellIndex <= 8; cellIndex++)
            {
                var cell = sudokuGroup.Cells[cellIndex];

                if (cell.Value!= 0)
                {
                    digitsYetToBePlaced.Remove(cell.Value);
                }
                else
                {
                    allUnsetCells.Add(cell);
                    foreach (var unsetDigit in cell.RemainingPossibilities)
                    {
                        digitOccurancesMap[unsetDigit]++;
                    }

                }

            }

            foreach (var digit in digitsYetToBePlaced)
            {
                if (digitOccurancesMap[digit] == 1)
                {
                    //digit can be set only at this place.

                    foreach (var unsetCell in allUnsetCells)
                    {
                        if (unsetCell.RemainingPossibilities.Contains(digit))
                        {
                            unsetCell.Value = digit;
                            
                            unsetCell.SolvingDifficulty = SolvingDifficulty.Easy;

                        }
                        
                    }

                     
                }
            }
        }


        private void SetADigitToOnlyAvailablePlace()
        {

            for (int index = 0; index <= 8; index++)
            {
                SetADigitToOnlyAvailablePlace(sudokuRows[index]);
                SetADigitToOnlyAvailablePlace(sudokuColumns[index]);
                SetADigitToOnlyAvailablePlace(sudokuBoxes[index]);
            }
        }


        private void EliminatePreemptiveSetsInAGroup(SudokuGroup sudokuGroup)
        {
           

            int remainingPossibitySizeMax = 0;

            for (int cellIndex = 0; cellIndex <= 8; cellIndex++)
            {

                if (sudokuGroup.Cells[cellIndex].Value == 0)
                {
                    remainingPossibitySizeMax++;
                }
            }

            for (int remainingPossibitySize = 2; remainingPossibitySize <= remainingPossibitySizeMax; remainingPossibitySize++)
            {


                {
                    //1. Adjust possibilities for a Column

                    HashSet<int> preemptiveSet = new HashSet<int>();
                    HashSet<int> cellsWithCommonPossilities = new HashSet<int>();

                    for (int cellIndex = 0; cellIndex <= 8; cellIndex++)
                    {
                        var remainingPossibilitiesSet = sudokuGroup.Cells[cellIndex].RemainingPossibilities;


                        if (sudokuGroup.Cells[cellIndex].Value == 0 && remainingPossibilitiesSet.Count <= remainingPossibitySize)
                        {
                            var tempSet = new HashSet<int>(remainingPossibilitiesSet);

                            tempSet.UnionWith(preemptiveSet);

                            if (tempSet.Count <= remainingPossibitySize)
                            {
                                cellsWithCommonPossilities.Add(cellIndex);
                                preemptiveSet.UnionWith(remainingPossibilitiesSet);

                            }


                            if (cellsWithCommonPossilities.Count == preemptiveSet.Count)
                            {
                                break;
                            }
                        }
                    }

                    if (cellsWithCommonPossilities.Count == preemptiveSet.Count
                        && preemptiveSet.Count > 0)
                    {
                        for (int cellIndex = 0; cellIndex <= 8; cellIndex++)
                        {

                            SudokuCell cell = sudokuGroup.Cells[cellIndex];
                            if (cell.Value == 0 && !cellsWithCommonPossilities.Contains(cellIndex))
                            {

                                //Reduce the possibilities.
                                cell.RemainingPossibilities.ExceptWith(preemptiveSet);


                                if (cell.RemainingPossibilities.Count == 1)
                                {
                                    cell.Value =
                                        cell.RemainingPossibilities.First();

                                   // Logger.WriteLine($"Solved a cell by preemptive Set. {cell.ColIndex},{cell.RowIndex}={cell.Value}");

                                    if(preemptiveSet.Count==2)
                                        cell.SolvingDifficulty = SolvingDifficulty.Medium;
                                    if (preemptiveSet.Count == 3)
                                        cell.SolvingDifficulty = SolvingDifficulty.Hard;

                                    if (preemptiveSet.Count == 4)
                                        cell.SolvingDifficulty = SolvingDifficulty.MaxDifficulty;
                                }
                            }


                        }
                    }
                }

                remainingPossibitySize++;

            }

        }


        private bool SolveByTryingPossibilities()
        {
            if (!IsAValidState())
            {
                return false;
            }

            int unsolvedCells = CountUnsolvedCells();
            if (unsolvedCells == 0)
            {
              
                return Solved;
            }
           

            SudokuCell[] linearArray = new SudokuCell[81];

            for (int rowIndex = 0; rowIndex <= 8; rowIndex++)
            {
                for (int colIndex = 0; colIndex <= 8; colIndex++)
                {

                    linearArray[rowIndex * 9 + colIndex] = sudokuCells[rowIndex][colIndex];

                }
            }
            //Sorted array by remaining possibilities.
            Array.Sort(linearArray);



            foreach (var cell in linearArray)
            {
                int rowIndex = cell.RowIndex;
                int colIndex = cell.ColIndex;

                if (cell.Value == 0)
                {
                    var remainingDigitsOnThisCell = cell.RemainingPossibilities;

                    if (remainingDigitsOnThisCell.Count == 0)
                    {
                        Logger.WriteLine("Invalid situation. remainingDigitsOnThisCell.Count == 0 ", _cloningDepth);
                        return false;
                    }

                    //if (cloningDepth == 0)
                    {
                        Logger.WriteLine($"Evaluating Cell@{cell} ", _cloningDepth);
                        Logger.WriteLine(ToStringFormatted(), _cloningDepth);
                       
                    }

                    foreach (var possibility in remainingDigitsOnThisCell)
                    {
                        //Set this possibility and try to solve.
                        Sudoku clone = new Sudoku(this.ToString());
                        clone._cloningDepth = this._cloningDepth+1;
                        
                        clone.sudokuCells[rowIndex][colIndex].Value = possibility;

                        if (clone.Solve())
                        {
                            //if (cloningDepth == 0)
                            {
                                Logger.WriteLine($"Valid Solution found for Cell@{cell}  new Value {possibility}", _cloningDepth);
                                Logger.WriteLine($"\n{clone.ToStringFormatted()}", _cloningDepth);

                            }

                            cell.SolvingDifficulty = SolvingDifficulty.MaxDifficulty;
                            cell.Value = possibility;

                            CopyUnSolvedCellsFromSolution(clone);
                          
                            return Solved;
                        }

                       
                    }

                    if (_cloningDepth == 0)
                    {
                        throw new Exception("No possibility worked. Root problem is not right");
                    }

                    Logger.WriteLine($"No possibility worked for  Cell@{cell}. Impossible situation. Should be backtraced.", _cloningDepth);
                    
                    return Notsolved;

                }
            }


           
            return Notsolved;
        }



   



        private int CountUnsolvedCells()
        {
            int unsolved = 0;

            for (int rowIndex = 0; rowIndex <= 8; rowIndex++)
            {
                for (int colIndex = 0; colIndex <= 8; colIndex++)
                {

                    if (sudokuCells[rowIndex][colIndex].Value == 0)
                    {
                        unsolved ++;
                    }

                }
            }

            return unsolved;

        }

        private bool IsAValidState()
        {
            


            foreach (var sudokuGroup in sudokuRows)
            {
                if (!IsGroupValid(sudokuGroup))
                {
                    Logger.WriteLine($"\nProblem went into invalid state. Invalid Row {sudokuGroup}", _cloningDepth);
                    
                    return false;
                }

            }

            foreach (var sudokuGroup in sudokuColumns)
            {
                if (!IsGroupValid(sudokuGroup))
                {
                    Logger.WriteLine($"\nProblem went into invalid state. Invalid Column {sudokuGroup}", _cloningDepth);
                    return false;
                }

            }

            foreach (var sudokuGroup in sudokuBoxes)
            {
                if (!IsGroupValid(sudokuGroup))
                {
                    Logger.WriteLine($"\nProblem went into invalid state. Invalid Box {sudokuGroup}", _cloningDepth);
                    return false;
                }

            }

            return true;
        }


        

        private bool IsGroupValid(SudokuGroup sudokuGroup)
        {
            HashSet<int> allDigitsInGroup = new HashSet<int>();

            int solvedCells = 0;

            foreach (var sudokuGroupCell in sudokuGroup.Cells)
            {
                if (sudokuGroupCell.Value == 0 && sudokuGroupCell.RemainingPossibilities.Count == 0)
                {
                    //Unsolved cells must have remaining possibilities on valid Sudoku.
                    return false;
                }

                if (sudokuGroupCell.Value > 0)
                {
                    solvedCells++;

                    allDigitsInGroup.Add(sudokuGroupCell.Value);
                }



            }

            if (solvedCells == 9)
            {

                if (allDigitsInGroup.Count == 9 && allDigitsInGroup.Sum()==45)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }



            if (allDigitsInGroup.Count == solvedCells)
            {
                
                return true;
            }

            return false;

        }




        private double CalculateDifficulty()
        {
            List<int> topN = new List<int>(10);
            topN.Add(0);

            for (int rowIndex = 0; rowIndex <= 8; rowIndex++)
            {
                for (int colIndex = 0; colIndex <= 8; colIndex++)
                {
                    SudokuCell cell = sudokuCells[rowIndex][colIndex];

                    int cellDifficulty =  (int) cell.SolvingDifficulty;

                    if (cellDifficulty > topN.Min())
                    {
                        if (topN.Count < topN.Capacity)
                        {
                            topN.Add(cellDifficulty);
                        }
                        else
                        {
                            topN.Remove(topN.Min());
                            topN.Add(cellDifficulty);
                        }
                    }


                }
            }

            double topNAverage = topN.Average() ;



            return topNAverage;
        }




        public string ToStringFormatted()
        {

            string sudokuSolution = $"[co]";

            for (int colIndex = 0; colIndex <= 8; colIndex++)
            {
                if (colIndex % 3 == 0)
                {
                    sudokuSolution += "  ";
                }

                sudokuSolution += $"{colIndex} ";

            }
            sudokuSolution += $"\n--------------------";

            for (int rowIndex = 0; rowIndex <= 8; rowIndex++)
            {
                sudokuSolution += $"\n[r{rowIndex}]";
                for (int colIndex = 0; colIndex <= 8; colIndex++)
                {
                    if (colIndex % 3 == 0)
                    {
                        sudokuSolution += "  ";
                    }

                    sudokuSolution += $"{sudokuCells[rowIndex][colIndex].Value} ";

                }

            }

            return sudokuSolution;

        }


        override public string ToString()
        {

            string sudokuSolution = "";

            for (int rowIndex = 0; rowIndex <= 8; rowIndex++)
            {
                for (int colIndex = 0; colIndex <= 8; colIndex++)
                {

                    sudokuSolution += $"{sudokuCells[rowIndex][colIndex].Value}";

                }

            }

            return sudokuSolution;

        }




        private Sudoku()
        {

            for (int i = 0; i <= 8; i++)
            {
                sudokuCells[i] = new SudokuCell[9];
                sudokuRows[i] = new SudokuGroup(i);
                sudokuColumns[i] = new SudokuGroup(i);
                sudokuBoxes[i] = new SudokuGroup(i);
            }

            //1. Initialize Cells and create row view

            for (int rowIndex = 0; rowIndex <= 8; rowIndex++)
            {
                for (int colIndex = 0; colIndex <= 8; colIndex++)
                {
                    sudokuCells[rowIndex][colIndex] = new SudokuCell(rowIndex, colIndex);
                    sudokuRows[rowIndex].Cells[colIndex] = sudokuCells[rowIndex][colIndex];
                    sudokuColumns[colIndex].Cells[rowIndex] = sudokuCells[rowIndex][colIndex];
                }
            }


            //2. create box view
            //([0] ,[1] , [2])
            //([3] ,[4] , [5])
            //([6] ,[7] , [8])

            for (int boxIndex = 0; boxIndex <= 8; boxIndex++)
            {
                for (int cellIndexInABox = 0; cellIndexInABox <= 8; cellIndexInABox++)
                {
                    //Box,cell 0,0 == Row Col >0,0
                    //1,0 ==>  0,3
                    //2,2 ==>0,8
                    //3,3 ==>4,0
                    //8,0 ==>6,6 
                    //8,8 ==>8,8 

                    int firtCellRowIndex = (boxIndex - (boxIndex % 3));
                    int firtCellColIndex = (boxIndex % 3) * 3;

                    int rowIndex = firtCellRowIndex + cellIndexInABox / 3;
                    int colIndex = firtCellColIndex + cellIndexInABox % 3;

                    sudokuBoxes[boxIndex].Cells[cellIndexInABox] = sudokuCells[rowIndex][colIndex];
                }
            }

            //MessageBox.Show(message);

        }
        public Sudoku(int[,] sudokuCellValues) : this()
        {


            for (int rowIndex = 0; rowIndex <= 8; rowIndex++)
            {

                for (int colIndex = 0; colIndex <= 8; colIndex++)
                {
                    sudokuCells[rowIndex][colIndex].Value = sudokuCellValues[rowIndex, colIndex];
                    sudokuCells[rowIndex][colIndex].SolvingDifficulty = SolvingDifficulty.PreSolved;
                }


            }

        }

        public Sudoku(string sudokuCellValuesFormatted) : this()
        {

            Regex regularExpression = new Regex("[^0-9]");


            string sudokuCellValues = regularExpression.Replace(sudokuCellValuesFormatted, "");


            if (sudokuCellValues.Length != 81)
            {
                throw new Exception("Invalid input. String length must be 81.");
            }

            char[] numbers = sudokuCellValues.ToCharArray();


            for (int rowIndex = 0; rowIndex <= 8; rowIndex++)
            {

                for (int colIndex = 0; colIndex <= 8; colIndex++)
                {
                    
                    sudokuCells[rowIndex][colIndex].Value = int.Parse("" + numbers[rowIndex * 9 + colIndex]);
                }

            }

        }

        public SudokuCell[][] GetSudokuCellsCopy()
        {
            Sudoku clone = new Sudoku(ToString());
            
            return clone.sudokuCells;
        }

        //Primary data container
        private SudokuCell[][] sudokuCells = new SudokuCell[9][];

        //Row, column, Box views on above data container.
        private SudokuGroup[] sudokuRows = new SudokuGroup[9];
        private SudokuGroup[] sudokuColumns = new SudokuGroup[9];
        private SudokuGroup[] sudokuBoxes = new SudokuGroup[9];
        private int _cloningDepth;

        public double DifficultyRating { get; set; }

        const bool Solved = true;
        const bool Notsolved = true;

    }


    public enum SolvingDifficulty
    {
       
        Easy = 2,
        Medium = 5,
        Hard = 8,
        PreSolved = 1,
        MaxDifficulty = 10,

    }

public class SudokuCell : IComparable<SudokuCell>
    {
       
        public int RowIndex { get; private set; }
        public int ColIndex { get; private set; }

        public SolvingDifficulty SolvingDifficulty { get; set; }

        private int _value;

        public int Value
        {
            get
            {
                return _value;
            }

            set
            {
                if (this._value == 0)
                {
                    this._value = value;

                    if (value != 0)
                    {

                        RemainingPossibilities.Clear();

                    }

                }
            }
        }

        public HashSet<int> RemainingPossibilities;

        public SudokuCell(int rowIndex, int colIndex)
        {
            this.RowIndex = rowIndex;
            this.ColIndex = colIndex;
            
            RemainingPossibilities = new HashSet<int>(new[]{1,2,3,4,5,6,7,8,9});


        }

        override public string ToString()
        {
            if(Value==0)
                return  $"({RowIndex},{ColIndex}){RemainingPossibilities.ToDebugString()}";
            else
            {
                return $"({RowIndex},{ColIndex})[{Value}]";
            }
        }



        public int CompareTo(SudokuCell other)
        {
            if (other == null)
            {
                return 100;
            }

            return this.RemainingPossibilities.Count- other.RemainingPossibilities.Count;
        }
    }

    /**
     *  A sudoku group is either a Row, a Column, or a Box
     */
    public class SudokuGroup
    {
       
        public int CellIndex { get; private set; }

        public  SudokuCell[] Cells = new SudokuCell[9];
      

        public SudokuGroup(int cellIndex)
        {
            this.CellIndex = cellIndex;
            
        }

        override public string ToString()
        {
            var val = "";

            foreach (var sudokuCell in Cells)
            {
                val += $"{sudokuCell.Value}";
            }

            return $"[{val}]";
        }


    }

    public class Logger
    {
        private static bool _isFirstLog = true;
        private static DateTime _startTime;
        private static bool isDebugEnabled = false;

        public static void WriteLine(string message, int recurssionDepth)
        {
            if (!isDebugEnabled)
            {
                return;
            }
            
            string logfilePath = "sudoku.log";

            if (_isFirstLog)
            {
                //Process.Start(Directory.GetCurrentDirectory());

                _startTime = DateTime.Now;

                File.Delete(logfilePath);

                Console.WriteLine(@"writing logs to " + Directory.GetCurrentDirectory());


            }


            string newLineindent = "\n";
            //newLineindent.PadRight(recurssionDepth*3);

            int count = 0;
            while (count < recurssionDepth)
            {
                count++;
                newLineindent += ">>";
            }

            
            message = message.Replace("\n", newLineindent);

            File.AppendAllText(logfilePath, $@"
[{(DateTime.Now - _startTime).TotalMilliseconds / 1000.0}] seconds " );

            File.AppendAllText(logfilePath, newLineindent+message);



            _isFirstLog = false;
        }

       
    }

    public static class MyExtensions
    {
        public static string ToDebugString<T>(this HashSet<T> collection)
        {
            string val = "";
            foreach (var item in collection)
            {
                val += $"{item},";
            }

            return $"[{val}]" ;
        }

       
    }


}
