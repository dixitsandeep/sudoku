using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;


//Research and Nomenclature material.
//http://www.sudokudragon.com/sudokustrategy.htm
//http://www.sudokudragon.com/advancedstrategy.htm
//http://lipas.uwasa.fi/~timan/sudoku/
//https://www.researchgate.net/publication/261217550_Difficulty_Rating_of_Sudoku_Puzzles_An_Overview_and_Evaluation

namespace Sudoku
{
    public class Sudoku
    {
        private SudokuCell[][] sudokuCells = new SudokuCell[9][];

        private Sudoku9NumberGroup[] sudokuRows = new Sudoku9NumberGroup[9];
        private Sudoku9NumberGroup[] sudokuColumns = new Sudoku9NumberGroup[9];
        private Sudoku9NumberGroup[] sudokuBoxes = new Sudoku9NumberGroup[9];

        private  Sudoku()
        {

            for (int i = 0; i <= 8; i++)
            {
                sudokuCells[i] = new SudokuCell[9];
                sudokuRows[i] = new Sudoku9NumberGroup(i);
                sudokuColumns[i] = new Sudoku9NumberGroup(i);
                sudokuBoxes[i] = new Sudoku9NumberGroup(i);



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
        public Sudoku(int[,] sudokuCellValues):this()
        {
            
           
            for (int rowIndex = 0; rowIndex <= 8; rowIndex++)
            {

                for (int colIndex = 0; colIndex <= 8; colIndex++)
                {
                    sudokuCells[rowIndex][colIndex].Value = sudokuCellValues[rowIndex, colIndex];
                }


            }

        }

        public Sudoku(string sudokuCellValues) : this()
        {
            char[] numbers = sudokuCellValues.ToCharArray();

            int numberIndex;

            for (int rowIndex = 0; rowIndex <= 8; rowIndex++)
            {

                for (int colIndex = 0; colIndex <= 8; colIndex++)
                {
                    sudokuCells[rowIndex][colIndex].Value = int.Parse(""+numbers[rowIndex*9+ colIndex]);
                }

            }

        }

      

        private bool EliminatePossibilitiesFromUnsetCells(Sudoku9NumberGroup sudokuGroup)
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

            int numberOfUnsetCells = 9 - allNumbersPresentInGroup.Count;


            for (int cellIndex = 0; cellIndex <= 8; cellIndex++)
            {
                int cellValue = sudokuGroup.cells[cellIndex].Value;

                if (cellValue == 0)
                {
                    sudokuGroup.cells[cellIndex].remainingPossibilities.ExceptWith(allNumbersPresentInGroup);

                    if (sudokuGroup.cells[cellIndex].remainingPossibilities.Count == 1)
                    {
                        possibilitiesReducedToOne = true;
                    }
                }

            }


            return possibilitiesReducedToOne;

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



        private bool EliminatePreemptiveSetsInAGroup(Sudoku9NumberGroup sudokuGroup)
        {

            bool possibilitiesReducedToOne = false;

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

                    HashSet<int> remainingPossibilitiesOnUnsetCells = new HashSet<int>();
                    HashSet<int> cellsWithCommonPossilities = new HashSet<int>();

                    for (int cellIndex = 0; cellIndex <= 8; cellIndex++)
                    {
                        var remainingPossibilitiesSet = sudokuGroup.cells[cellIndex].remainingPossibilities;


                        if (sudokuGroup.cells[cellIndex].Value == 0 && remainingPossibilitiesSet.Count <= remainingPossibitySize)
                        {
                            var tempSet = new HashSet<int>(remainingPossibilitiesSet);

                            tempSet.UnionWith(remainingPossibilitiesOnUnsetCells);

                            if (tempSet.Count <= remainingPossibitySize)
                            {
                                cellsWithCommonPossilities.Add(cellIndex);
                                remainingPossibilitiesOnUnsetCells.UnionWith(remainingPossibilitiesSet);

                            }


                            if (cellsWithCommonPossilities.Count == remainingPossibilitiesOnUnsetCells.Count)
                            {
                                break;
                            }
                        }
                    }

                    if (cellsWithCommonPossilities.Count == remainingPossibilitiesOnUnsetCells.Count
                        && remainingPossibilitiesOnUnsetCells.Count > 0)
                    {
                        for (int cellIndex = 0; cellIndex <= 8; cellIndex++)
                        {
                            if (sudokuGroup.cells[cellIndex].Value == 0 && !cellsWithCommonPossilities.Contains(cellIndex))
                            {

                                //Reduce the possiblities.
                                sudokuGroup.cells[cellIndex].remainingPossibilities.ExceptWith(remainingPossibilitiesOnUnsetCells);

                                if (sudokuGroup.cells[cellIndex].remainingPossibilities.Count == 0)
                                {
                                    //MessageBox.Show($"Error Condition cell {currentRowIndex}, {colIndex} remainingPossiblities=" + remainingPossiblities[currentRowIndex][colIndex].Count+" ");
                                }

                                //MessageBox.Show($" processPreemptiveSetsInRows {currentRowIndex}, {colIndex} remainingPossiblities=" + remainingPossiblities[currentRowIndex][colIndex].Count + " ");


                                if (sudokuGroup.cells[cellIndex].remainingPossibilities.Count == 1)
                                {
                                    possibilitiesReducedToOne = true;

                                    // MessageBox.Show($"processPreemptiveSetsInRows cell {currentRowIndex}, {colIndex} remainingPossiblities=1" );


                                }
                            }


                        }
                    }
                }

                remainingPossibitySize++;

            }




            return possibilitiesReducedToOne;

        }




        private bool SetCellsWithUniquePossibility()
        {

            bool aNewCellHasBeenSet = false;

            for (int rowIndex = 0; rowIndex <= 8; rowIndex++)
              
            {
                for (int colIndex = 0; colIndex <= 8; colIndex++)
                {
                    HashSet<int> remainingPossibilitiesSet = sudokuCells[rowIndex][colIndex].remainingPossibilities;

                    //MessageBox.Show($"{rowIndex + 1},{colIndex + 1} = possibiliites {remainingPossibilitiesSet.Count}");

                    if (remainingPossibilitiesSet.Count == 1 && sudokuCells[rowIndex][colIndex].Value==0)
                    {
                        sudokuCells[rowIndex][colIndex].Value = remainingPossibilitiesSet.First();
                        sudokuCells[rowIndex][colIndex].remainingPossibilities.Clear();

                        EliminatePossibilitiesFromUnsetCells();

                        ProcessPreemptiveSets();

                       

                        aNewCellHasBeenSet = true;

                        //MessageBox.Show($"{rowIndex+1},{colIndex+1} = {sudokuCellsByRowAndColumn[rowIndex][colIndex]}");

                    }

                }

            }




            return aNewCellHasBeenSet;


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

        private bool IsSolved()
        {
            bool isSolved = true;

            for (int rowIndex = 0; rowIndex <= 8; rowIndex++)
            {


                for (int colIndex = 0; colIndex <= 8; colIndex++)
                {

                    if (sudokuCells[rowIndex][colIndex].Value == 0)
                    {
                        isSolved = false;
                    }

                }


            }



            return isSolved;

        }

        public bool SolveNextCell()
        {

            EliminatePossibilitiesFromUnsetCells();

            ProcessPreemptiveSets();



            while (SetCellsWithUniquePossibility()) ;

            return IsSolved();

        }

        public bool Solve()
        {


            EliminatePossibilitiesFromUnsetCells();

            ProcessPreemptiveSets();



            while (SetCellsWithUniquePossibility());

            return IsSolved();

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



        public string Solution()
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


    }


    public class SudokuCell
    {
        public int RowIndex { get; private set; }
        public int ColIndex { get; private set; }

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
            
            remainingPossibilities = new HashSet<int>();

            remainingPossibilities.Add(1);
            remainingPossibilities.Add(2);
            remainingPossibilities.Add(3);

            remainingPossibilities.Add(4);
            remainingPossibilities.Add(5);
            remainingPossibilities.Add(6);

            remainingPossibilities.Add(7);
            remainingPossibilities.Add(8);
            remainingPossibilities.Add(9);
        }

       
    }

    public class Sudoku9NumberGroup
    {
        public int CellIndex { get; private set; }

        public  SudokuCell[] cells = new SudokuCell[9];
      

        public Sudoku9NumberGroup(int cellIndex)
        {
            this.CellIndex = cellIndex;
            
        }


    }



}
