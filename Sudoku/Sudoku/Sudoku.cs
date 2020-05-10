using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;


namespace Sudoku
{
    public class Sudoku
    {
        private SudokuCell[][] sudokuCells = new SudokuCell[9][];

        private Sudoku9NumberGroup[] sudokuRows = new Sudoku9NumberGroup[9];
        private Sudoku9NumberGroup[] sudokuColumns = new Sudoku9NumberGroup[9];
        private Sudoku9NumberGroup[] sudokuBoxes = new Sudoku9NumberGroup[9];

        public Sudoku(int[,] sudokuCellValues)
        {
            initializeCells();

            for (int rowIndex = 0; rowIndex <= 8; rowIndex++)
            {

                for (int colIndex = 0; colIndex <= 8; colIndex++)
                {
                    sudokuCells[rowIndex][colIndex].Value = sudokuCellValues[rowIndex, colIndex];
                }


            }

        }

        private void initializeCells()
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
                    sudokuCells[rowIndex][colIndex] = new SudokuCell(rowIndex,colIndex);
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
                    int firtCellColIndex =  (boxIndex % 3)*3;

                    int rowIndex = firtCellRowIndex + cellIndexInABox/3;
                    int colIndex = firtCellColIndex + cellIndexInABox % 3;

                    message += $"[BoxCell({boxIndex}, {cellIndexInABox})={rowIndex},{colIndex}] - ";

                    sudokuBoxes[boxIndex].cells[cellIndexInABox] = sudokuCells[rowIndex][colIndex];
                }

                message += "\n";

            }

            //MessageBox.Show(message);

        }

        private bool RemovedRemainingPossibilitiesForUnsetCells(Sudoku9NumberGroup sudokuGroup)
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


        private void RemovedRemainingPossibilitiesForUnsetCells()
        {
            
            for (int index = 0; index <= 8; index++)
            {
                RemovedRemainingPossibilitiesForUnsetCells(sudokuRows[index]);
                RemovedRemainingPossibilitiesForUnsetCells(sudokuColumns[index]);
                RemovedRemainingPossibilitiesForUnsetCells(sudokuBoxes[index]);
            }
        }


        private bool processPreemptiveSetsInRows(int rowIndexForPreEmptiveSetsSearch)
        {

            bool possibilitiesReducedToOne = false;

         

            int remainingPossibitySizeMax = 0;

            for (int colIndex = 0; colIndex <= 8; colIndex++)
            {

                if (sudokuCells[rowIndexForPreEmptiveSetsSearch][colIndex].Value == 0 )
                {
                    remainingPossibitySizeMax++;
                }
            }

           for(int remainingPossibitySize=2;remainingPossibitySize<= remainingPossibitySizeMax; remainingPossibitySize++)
            {


                {
                    //1. Adjust possibilities for a Column

                    HashSet<int> remainingPossibilitiesOnUnsetCells = new HashSet<int>();
                    HashSet<int> cellsWithCommonPossilities = new HashSet<int>();

                    for (int colIndex = 0; colIndex <= 8; colIndex++)
                    {
                        var remainingPossibilitiesSet = sudokuCells[rowIndexForPreEmptiveSetsSearch][colIndex].remainingPossibilities;


                        if (sudokuCells[rowIndexForPreEmptiveSetsSearch][colIndex].Value == 0 && remainingPossibilitiesSet.Count <= remainingPossibitySize)
                        {
                            var tempSet = new HashSet<int>(remainingPossibilitiesSet);

                            tempSet.UnionWith(remainingPossibilitiesOnUnsetCells);

                            if (tempSet.Count <= remainingPossibitySize)
                            {
                                cellsWithCommonPossilities.Add(colIndex);
                                remainingPossibilitiesOnUnsetCells.UnionWith(remainingPossibilitiesSet);

                            }


                            if (cellsWithCommonPossilities.Count == remainingPossibilitiesOnUnsetCells.Count)
                            {
                                break;
                            }
                        }
                    }

                    if (cellsWithCommonPossilities.Count == remainingPossibilitiesOnUnsetCells.Count 
                        && remainingPossibilitiesOnUnsetCells.Count>0)
                    {
                        for (int colIndex = 0; colIndex <= 8; colIndex++)
                        {
                            if (sudokuCells[rowIndexForPreEmptiveSetsSearch][colIndex].Value == 0 && !cellsWithCommonPossilities.Contains(colIndex))
                            {

                                //Reduce the possiblities.
                                sudokuCells[rowIndexForPreEmptiveSetsSearch][colIndex].remainingPossibilities.ExceptWith(remainingPossibilitiesOnUnsetCells);

                                if (sudokuCells[rowIndexForPreEmptiveSetsSearch][colIndex].remainingPossibilities.Count == 0)
                                {
                                    //MessageBox.Show($"Error Condition cell {currentRowIndex}, {colIndex} remainingPossiblities=" + remainingPossiblities[currentRowIndex][colIndex].Count+" ");
                                }

                                //MessageBox.Show($" processPreemptiveSetsInRows {currentRowIndex}, {colIndex} remainingPossiblities=" + remainingPossiblities[currentRowIndex][colIndex].Count + " ");


                                if (sudokuCells[rowIndexForPreEmptiveSetsSearch][colIndex].remainingPossibilities.Count == 1)
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


        private bool processPreemptiveSetsInColumns( int currentColIndex)
        {

            bool possibilitiesReducedToOne = false;

            bool continueLoop = false;

            int remainingPossibitySizeMax = 0;

            for (int rowIndex = 0; rowIndex <= 8; rowIndex++)
            {

                if (sudokuCells[rowIndex][currentColIndex].Value == 0)
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

                    for (int rowIndex = 0; rowIndex <= 8; rowIndex++)
                    {
                        var remainingPossibilitiesSet = sudokuCells[rowIndex][currentColIndex].remainingPossibilities;


                        if (sudokuCells[rowIndex][currentColIndex].Value == 0 && remainingPossibilitiesSet.Count <= remainingPossibitySize)
                        {
                            var tempSet = new HashSet<int>(remainingPossibilitiesSet);

                            tempSet.UnionWith(remainingPossibilitiesOnUnsetCells);

                            if (tempSet.Count <= remainingPossibitySize)
                            {
                                cellsWithCommonPossilities.Add(rowIndex);
                                remainingPossibilitiesOnUnsetCells.UnionWith(remainingPossibilitiesSet);

                            }
                                

                            if (cellsWithCommonPossilities.Count == remainingPossibilitiesOnUnsetCells.Count)
                            {
                                break;
                            }
                        }
                    }

                    if (cellsWithCommonPossilities.Count == remainingPossibilitiesOnUnsetCells.Count && remainingPossibilitiesOnUnsetCells.Count > 0)
                    {
                        for (int rowIndex = 0; rowIndex <= 8; rowIndex++)
                        {
                            if (sudokuCells[rowIndex][currentColIndex].Value == 0 && !cellsWithCommonPossilities.Contains(rowIndex))
                            {

                                //Reduce the possiblities everywhere.
                                sudokuCells[rowIndex][currentColIndex].remainingPossibilities.ExceptWith(remainingPossibilitiesOnUnsetCells);
                                //MessageBox.Show("processPreemptiveSetsInColumns");

                                if (sudokuCells[rowIndex][currentColIndex].remainingPossibilities.Count == 1)
                                {
                                    possibilitiesReducedToOne = true;
                                    

                                }
                            }


                        }
                    }


                }





                remainingPossibitySize++;

            }




            return possibilitiesReducedToOne;

        }


        private bool processPreemptiveSetsIn3x3Box(int boxRowStartIndex, int boxColStartIndex)
        {

            bool possibilitiesReducedToOne = false;

   
            int remainingPossibitySizeMax = 0;

            int boxRowEndIndex = boxRowStartIndex + 2;


            int boxColEndIndex = boxColStartIndex + 2;

            for (int rowIndex = boxRowStartIndex; rowIndex <= boxRowEndIndex; rowIndex++)
            {
                for (int colIndex = boxColStartIndex; colIndex <= boxColEndIndex; colIndex++)
                {
                    if (sudokuCells[rowIndex][colIndex].Value == 0)
                    {
                        remainingPossibitySizeMax++;
                    }
                }
            }

            for (int remainingPossibitySize = 2; remainingPossibitySize <= remainingPossibitySizeMax; remainingPossibitySize++)
            {


                {


                    HashSet<int> remainingPossibilitiesOnUnsetCells = new HashSet<int>();
                    HashSet<int> cellsWithCommonPossilities = new HashSet<int>();
                    


                    for (int rowIndex = boxRowStartIndex; rowIndex <= boxRowEndIndex; rowIndex++)
                    {
                        for (int colIndex = boxColStartIndex; colIndex <= boxColEndIndex; colIndex++)
                        {

                            var remainingPossibilitiesSet = sudokuCells[rowIndex][colIndex].remainingPossibilities;


                            if (sudokuCells[rowIndex][colIndex].Value == 0 && remainingPossibilitiesSet.Count <= remainingPossibitySize)
                            {

                                var tempSet = new HashSet<int>(remainingPossibilitiesSet);

                                tempSet.UnionWith(remainingPossibilitiesOnUnsetCells);

                                if (tempSet.Count <= remainingPossibitySize)
                                {
                                    cellsWithCommonPossilities.Add(rowIndex * 10 + colIndex);
                                    remainingPossibilitiesOnUnsetCells.UnionWith(remainingPossibilitiesSet);

                                }


                                if (cellsWithCommonPossilities.Count == remainingPossibilitiesOnUnsetCells.Count)
                                {
                                    break;
                                }
                            }
                        }

                    }

                    if (cellsWithCommonPossilities.Count == remainingPossibilitiesOnUnsetCells.Count && remainingPossibilitiesOnUnsetCells.Count > 0)
                    {

                        for (int rowIndex = boxRowStartIndex; rowIndex <= boxRowEndIndex; rowIndex++)
                        {
                            for (int colIndex = boxColStartIndex; colIndex <= boxColEndIndex; colIndex++)
                            {

                                if (!cellsWithCommonPossilities.Contains(rowIndex * 10 + colIndex))
                                {
                                    sudokuCells[rowIndex][colIndex].remainingPossibilities.ExceptWith(remainingPossibilitiesOnUnsetCells);

                                   // MessageBox.Show("Remaining possibities reduced.. in 3x3 square");


                                    if (sudokuCells[rowIndex][colIndex].remainingPossibilities.Count == 1)
                                    {
                                        possibilitiesReducedToOne = true;
                                        
                                    }
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

                        RemovedRemainingPossibilitiesForUnsetCells();

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
            for (int rowIndex = 0; rowIndex <= 8; rowIndex++)
            {


                processPreemptiveSetsInRows(rowIndex);


            }

            for (int colIndex = 0; colIndex <= 8; colIndex++)
            {

                processPreemptiveSetsInColumns(colIndex);
            }

            for (int rowIndex = 0; rowIndex <= 8; rowIndex += 3)
            {
                for (int colIndex = 0; colIndex <= 8; colIndex += 3)
                {
                    processPreemptiveSetsIn3x3Box(rowIndex, colIndex);
                }


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

            RemovedRemainingPossibilitiesForUnsetCells();

            ProcessPreemptiveSets();



            while (SetCellsWithUniquePossibility()) ;

            return IsSolved();

        }

        public bool Solve()
        {


            RemovedRemainingPossibilitiesForUnsetCells();

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
