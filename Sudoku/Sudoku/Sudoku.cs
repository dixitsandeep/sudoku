using System;
using System.Collections.Generic;
using System.Linq;


//Research and Nomenclature material.
//https://en.wikipedia.org/wiki/Glossary_of_Sudoku
//http://www.sudokudragon.com/sudokustrategy.htm
//http://www.sudokudragon.com/advancedstrategy.htm
//http://lipas.uwasa.fi/~timan/sudoku/
//https://www.researchgate.net/publication/261217550_Difficulty_Rating_of_Sudoku_Puzzles_An_Overview_and_Evaluation

namespace Sudoku
{
    public class Sudoku
    {

        public bool Solve()
        {



            int initialUnsolvedCells = CountUnsolvedCells();

            EliminatePossibilitiesFromUnsetCells();
            ProcessPreemptiveSets();
            SetADigitToOnlyAvailablePlace();

            bool isAValidState = IsAValidState();

            if (!isAValidState)
            {
                Console.WriteLine($"Problem went into invalid state. It's likely to happen during backtracking. This possiblity will be rejected. Level = {cloningDepth}. State ={ToString()}");
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
            else
            {
                TryPossibilitiesBackTrackAndContinueToSolve();
            }


            DifficultyRating = CalculateDifficulty();

            return IsAValidState();
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

        private void EliminatePossibilitiesFromUnsetCells()
        {

            for (int index = 0; index <= 8; index++)
            {
                EliminatePossibilitiesFromUnsetCells(sudokuRows[index]);
                EliminatePossibilitiesFromUnsetCells(sudokuColumns[index]);
                EliminatePossibilitiesFromUnsetCells(sudokuBoxes[index]);
            }
        }



      

        private bool EliminatePossibilitiesFromUnsetCells(SudokuGroup sudokuGroup)
        {
            bool possibilitiesReducedToOne = false;

            HashSet<int> allNumbersPresentInGroup = new HashSet<int>();

            for (int cellIndex = 0; cellIndex <= 8; cellIndex++)
            {
               int cellValue = sudokuGroup.cells[cellIndex].Value;

               if (cellValue != 0)
               {
                   allNumbersPresentInGroup.Add(cellValue);
               }

            }

    

            for (int cellIndex = 0; cellIndex <= 8; cellIndex++)
            {
                int cellValue = sudokuGroup.cells[cellIndex].Value;

                if (cellValue == 0)
                {
                    SudokuCell cell = sudokuGroup.cells[cellIndex];
                    cell.remainingPossibilities.ExceptWith(allNumbersPresentInGroup);

                    if (cell.remainingPossibilities.Count == 1)
                    {
                        cell.Value = cell.remainingPossibilities.First();
                        cell.SolvingDifficulty = SolvingDifficulty.EASY;
                      
                        possibilitiesReducedToOne = true;
                    }
                }

            }


            return possibilitiesReducedToOne;

        }





        private bool SetADigitToOnlyAvailablePlace(SudokuGroup sudokuGroup)
        {
            bool aNewCellWasSolved = false;

           
            var digitsYetToBePlaced = new HashSet<int>(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            int[] digitOccurancesMap = new int[10];

            var allUnsetCells = new HashSet<SudokuCell>();

            for (int cellIndex = 0; cellIndex <= 8; cellIndex++)
            {
                var cell = sudokuGroup.cells[cellIndex];

                if (cell.Value!= 0)
                {
                    digitsYetToBePlaced.Remove(cell.Value);
                }
                else
                {
                    allUnsetCells.Add(cell);
                    foreach (var unsetDigit in cell.remainingPossibilities)
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

                    aNewCellWasSolved = true;

                    foreach (var unsetCell in allUnsetCells)
                    {
                        if (unsetCell.remainingPossibilities.Contains(digit))
                        {
                            unsetCell.Value = digit;
                            
                            unsetCell.SolvingDifficulty = SolvingDifficulty.EASY;

                        }
                        
                    }

                     
                }
            }
            


            return aNewCellWasSolved;

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

                if (sudokuGroup.cells[cellIndex].Value == 0)
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
                        var remainingPossibilitiesSet = sudokuGroup.cells[cellIndex].remainingPossibilities;


                        if (sudokuGroup.cells[cellIndex].Value == 0 && remainingPossibilitiesSet.Count <= remainingPossibitySize)
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
                            if (sudokuGroup.cells[cellIndex].Value == 0 && !cellsWithCommonPossilities.Contains(cellIndex))
                            {

                                //Reduce the possibilities.
                                sudokuGroup.cells[cellIndex].remainingPossibilities.ExceptWith(preemptiveSet);


                                if (sudokuGroup.cells[cellIndex].remainingPossibilities.Count == 1)
                                {
                                    

                                    sudokuGroup.cells[cellIndex].SolvingDifficulty = SolvingDifficulty.HARD;
                                }
                            }


                        }
                    }
                }

                remainingPossibitySize++;

            }






        }


        private bool TryPossibilitiesBackTrackAndContinueToSolve()
        {



            for (int rowIndex = 0; rowIndex <= 8; rowIndex++)
            {
                for (int colIndex = 0; colIndex <= 8; colIndex++)
                {
                    SudokuCell cell = sudokuCells[rowIndex][colIndex];

                    if (cell.Value == 0)
                    {
                        var remainingDigitsOnThisCell = cell.remainingPossibilities;

                        if (cloningDepth == 0)
                        {
                            Console.WriteLine($"Try  backtracking to set Cell@ {rowIndex},{colIndex} current value {cell.Value}");

                        }

                        foreach (var possibility in remainingDigitsOnThisCell)
                        {
                            //Set this possibility and try to solve.
                            Sudoku clone = new Sudoku(this.ToString());
                            clone.cloningDepth = this.cloningDepth+1;
                            
                            clone.sudokuCells[rowIndex][colIndex].Value = possibility;

                            if (clone.Solve())
                            {
                                if (cloningDepth == 0)
                                {
                                    Console.WriteLine($"Done backtracking to set Cell@ {rowIndex} ,{colIndex} current value {cell.Value} new Value {possibility}");
                                    Console.WriteLine($"{ToString()}");

                                }

                                cell.SolvingDifficulty = SolvingDifficulty.MAX_DIFFICULTY;
                                cell.remainingPossibilities.Clear();
                                cell.Value = possibility;

                                return this.Solve();
                            }

                            if (cloningDepth == 0)
                            {
                                Console.WriteLine($"New value NOT valid for Cell@ {rowIndex} ,{colIndex} current value {cell.Value} new Value {possibility}");

                            }
                        }
                    }

                }

            }

            if (cloningDepth == 0)
            {
                throw new Exception("No possibility worked. Root problem is not right");
            }



            return false;
        }



        private void SetCellsWithUniquePossibility()
        {

           
            for (int rowIndex = 0; rowIndex <= 8; rowIndex++)
              
            {
                for (int colIndex = 0; colIndex <= 8; colIndex++)
                {
                    HashSet<int> remainingPossibilitiesSet = sudokuCells[rowIndex][colIndex].remainingPossibilities;

                    //MessageBox.Show($"{rowIndex + 1},{colIndex + 1} = possibiliites {remainingPossibilitiesSet.Count}");

                    if (remainingPossibilitiesSet.Count == 1 && sudokuCells[rowIndex][colIndex].Value==0)
                    {
                        sudokuCells[rowIndex][colIndex].Value = remainingPossibilitiesSet.First();
                       
                    }

                }

            }

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

        private bool IsSolvable()
        {

            int unsolvedCells = CountUnsolvedCells();

            if (unsolvedCells > 0)
            {
                for (int rowIndex = 0; rowIndex <= 8; rowIndex++)
                {
                    for (int colIndex = 0; colIndex <= 8; colIndex++)
                    {
                        SudokuCell cell = sudokuCells[rowIndex][colIndex];

                        if (cell.Value == 0 && cell.remainingPossibilities.Count == 0)
                        {

                            return false;
                        }

                    }
                }

            }
            else
            {
                return IsAValidState();
            }

       

           

            return true;

        }

        private bool IsAValidState()
        {
            


            foreach (var sudokuGroup in sudokuRows)
            {
                if (!IsGroupValid(sudokuGroup))
                {
                    return false;
                }

            }

            foreach (var sudokuGroup in sudokuColumns)
            {
                if (!IsGroupValid(sudokuGroup))
                {
                    return false;
                }

            }

            foreach (var sudokuGroup in sudokuBoxes)
            {
                if (!IsGroupValid(sudokuGroup))
                {
                    return false;
                }

            }

            return true;
        }


            


            

        

        private bool IsGroupValid(SudokuGroup sudokuGroup)
        {
            HashSet<int> allDigitsInGroup = new HashSet<int>();

            int solvedCells = 0;

            foreach (var sudokuGroupCell in sudokuGroup.cells)
            {
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


        public bool SolveNextCell()
        {

            return IsAValidState();

        }

        private double CalculateDifficulty()
        {

            int sudokuDifficulty=0;
            List<int> topN = new List<int>(10);
            topN.Add(0);

            for (int rowIndex = 0; rowIndex <= 8; rowIndex++)
            {
                for (int colIndex = 0; colIndex <= 8; colIndex++)
                {
                    SudokuCell cell = sudokuCells[rowIndex][colIndex];

                    int cellDifficulty =  (int) cell.SolvingDifficulty;
                    sudokuDifficulty += cellDifficulty;

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

            double sudokuDifficultyAverage = sudokuDifficulty/81;
            sudokuDifficultyAverage = (topNAverage + sudokuDifficultyAverage) / 2;

            return topNAverage;
        }

 
     



        public int[][] cells()
        {
          

            int[][] copyOfCells = new int[9][];

            string msg = "";

            for (int rowIndex = 0; rowIndex <= 8; rowIndex++)
            {
                copyOfCells[rowIndex] = new int[9];
               

                for (int colIndex = 0; colIndex <= 8; colIndex++)
                {

                    copyOfCells[rowIndex][colIndex] = sudokuCells[rowIndex][colIndex].Value;


                    if (sudokuCells[rowIndex][colIndex].Value==0 && sudokuCells[rowIndex][colIndex].remainingPossibilities.Count>=2)
                    {
                        int[] arr = sudokuCells[rowIndex][colIndex].remainingPossibilities.ToArray();

                        msg += $"[";

                        foreach (var i in arr)
                        {
                            msg += $"{i}";
                        }

                        msg += $"]";
                    }
                    else
                    {
                        

                        msg += $"[{copyOfCells[rowIndex][colIndex]}]";
                    }


                }

                msg += "\n";


            }

            //MessageBox.Show(msg);

            return copyOfCells;

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
                    sudokuRows[rowIndex].cells[colIndex] = sudokuCells[rowIndex][colIndex];
                    sudokuColumns[colIndex].cells[rowIndex] = sudokuCells[rowIndex][colIndex];
                }
            }


            string message = "";

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

                    message += $"[BoxCell({boxIndex}, {cellIndexInABox})={rowIndex},{colIndex}] - ";

                    sudokuBoxes[boxIndex].cells[cellIndexInABox] = sudokuCells[rowIndex][colIndex];
                }

                message += "\n";

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
                    sudokuCells[rowIndex][colIndex].SolvingDifficulty = SolvingDifficulty.PRESOLVED;
                }


            }

        }

        public Sudoku(string sudokuCellValues) : this()
        {
            char[] numbers = sudokuCellValues.ToCharArray();


            for (int rowIndex = 0; rowIndex <= 8; rowIndex++)
            {

                for (int colIndex = 0; colIndex <= 8; colIndex++)
                {

                    sudokuCells[rowIndex][colIndex].Value = int.Parse("" + numbers[rowIndex * 9 + colIndex]);
                }

            }

        }

        //Primary data container
        private SudokuCell[][] sudokuCells = new SudokuCell[9][];

        //Row, column, Box views on above data container.
        private SudokuGroup[] sudokuRows = new SudokuGroup[9];
        private SudokuGroup[] sudokuColumns = new SudokuGroup[9];
        private SudokuGroup[] sudokuBoxes = new SudokuGroup[9];
        private int cloningDepth = 0;

        public double DifficultyRating { get; set; }

    }


    public enum SolvingDifficulty
    {
       
        EASY = 2,
        MEDIUM = 5,
        HARD = 8,
        PRESOLVED = 1,
        MAX_DIFFICULTY = 10,

    }

public class SudokuCell
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

                        remainingPossibilities.Clear();

                    }

                }
            }
        }

        public HashSet<int> remainingPossibilities = new HashSet<int>();

        public SudokuCell(int rowIndex, int colIndex)
        {
            this.RowIndex = rowIndex;
            this.ColIndex = colIndex;
            
            remainingPossibilities = new HashSet<int>(new int[]{1,2,3,4,5,6,7,8,9});


        }

       
    }

    /**
     *  A sudoku group is either a Row, a Column, or a Box
     */
    public class SudokuGroup
    {
       
        public int CellIndex { get; private set; }

        public  SudokuCell[] cells = new SudokuCell[9];
      

        public SudokuGroup(int cellIndex)
        {
            this.CellIndex = cellIndex;
            
        }


    }



}
