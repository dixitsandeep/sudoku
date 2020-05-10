using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;


namespace Sudoku
{
    public class Sudoku
    {
        private int[][] sudokuCells = new int[9][];

        private HashSet<int>[][] remainingPossiblities = new HashSet<int>[9][];

       

        public Sudoku(int[,] SudokuCells)
        {
            initializeCells();

            for (int rowIndex = 0; rowIndex <= 8; rowIndex++)
            {


                for (int colIndex = 0; colIndex <= 8; colIndex++)
                {

                    SetCellValue(rowIndex, colIndex, SudokuCells[rowIndex,colIndex]);


                }


            }

        }

        private void initializeCells()
        {

            for (int i = 0; i <= 8; i++)
            {
                sudokuCells[i] = new int[9];
                remainingPossiblities[i] = new HashSet<int>[9];
            }

            for (int rowIndex = 0; rowIndex <= 8; rowIndex++)
            {
                

                for (int colIndex = 0; colIndex <= 8; colIndex++)
                {

                    remainingPossiblities[rowIndex][colIndex] = new HashSet<int>();

                    remainingPossiblities[rowIndex][colIndex].Add(1);
                    remainingPossiblities[rowIndex][colIndex].Add(2);
                    remainingPossiblities[rowIndex][colIndex].Add(3);

                    remainingPossiblities[rowIndex][colIndex].Add(4);
                    remainingPossiblities[rowIndex][colIndex].Add(5);
                    remainingPossiblities[rowIndex][colIndex].Add(6);

                    remainingPossiblities[rowIndex][colIndex].Add(7);
                    remainingPossiblities[rowIndex][colIndex].Add(8);
                    remainingPossiblities[rowIndex][colIndex].Add(9);


                }


            }

        }

        private void SetCellValue(int currentRowIndex, int currentColIndex,int value)
        {

            if (sudokuCells[currentRowIndex][currentColIndex] == 0 )
            {
                sudokuCells[currentRowIndex][currentColIndex] = value;

                if (value != 0)
                {
                   
                    remainingPossiblities[currentRowIndex][currentColIndex].Clear();
                   
                }

            }
            

        }

        private bool removeCellValuesFromRowColumnBoxPossibilities(int currentRowIndex, int currentColIndex, int value)
        {

            bool possibilitiesReducedToOne = false;

            //1. Adjust possibilities for a row

            for (int colIndex = 0; colIndex <= 8; colIndex++)
            {
                HashSet<int> remainingPossibilitiesSet = remainingPossiblities[currentRowIndex][colIndex];

                if (colIndex != currentColIndex && remainingPossibilitiesSet.Count>1)
                {

                    remainingPossibilitiesSet.Remove(value);

                    if (remainingPossibilitiesSet.Count == 1)
                    {
                        possibilitiesReducedToOne = true;
                    }

                }

            }

            //2. Adjust possibilities for a column

            for (int rowIndex = 0; rowIndex <= 8; rowIndex++)
            {
                HashSet<int> remainingPossibilitiesSet = remainingPossiblities[rowIndex][currentColIndex];

                if (rowIndex != currentRowIndex && remainingPossibilitiesSet.Count > 1)
                {

                    remainingPossibilitiesSet.Remove(value);

                    if (remainingPossibilitiesSet.Count == 1)
                    {
                        possibilitiesReducedToOne = true;
                    }

                }

            }

            //3. Adjust possibilities for a box

            // if currentColIndex 0,1,2 => boxColStartIndex 0 boxColEndIndex 2
            // if currentColIndex 3,4,5 => boxColStartIndex 3 boxColEndIndex 5
            // if currentColIndex 6,7,8 => boxColStartIndex 6 boxColEndIndex 8


            int boxColStartIndex = 3*(currentColIndex/3);
            int boxColEndIndex = boxColStartIndex + 2;

            int boxRowStartIndex = 3*(currentRowIndex / 3);
            int boxRowEndIndex = boxRowStartIndex + 2;

            for (int rowIndex = boxRowStartIndex; rowIndex <= boxRowEndIndex; rowIndex++)
               
            {
                for (int colIndex = boxColStartIndex; colIndex <= boxColEndIndex; colIndex++)
                {
                    HashSet<int> remainingPossibilitiesSet = remainingPossiblities[rowIndex][colIndex];

                    if (!(rowIndex == currentRowIndex && colIndex == currentColIndex) && remainingPossibilitiesSet.Count > 1)
                    {

                        remainingPossibilitiesSet.Remove(value);

                        if (remainingPossibilitiesSet.Count == 1)
                        {
                            possibilitiesReducedToOne = true;
                        }

                    }

                }

            }

            if(possibilitiesReducedToOne)

                         SetCellsWithUniquePossibility();

            return possibilitiesReducedToOne;

        }


        private bool processPreemptiveSetsInRows(int currentRowIndex)
        {

            bool possibilitiesReducedToOne = false;

         

            int remainingPossibitySizeMax = 0;

            for (int colIndex = 0; colIndex <= 8; colIndex++)
            {

                if (sudokuCells[currentRowIndex][colIndex] == 0 )
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
                        var remainingPossibilitiesSet = remainingPossiblities[currentRowIndex][colIndex];


                        if (sudokuCells[currentRowIndex][colIndex] == 0 && remainingPossibilitiesSet.Count <= remainingPossibitySize)
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
                            if (sudokuCells[currentRowIndex][colIndex] == 0 && !cellsWithCommonPossilities.Contains(colIndex))
                            {
                               
                                //Reduce the possiblities.
                                remainingPossiblities[currentRowIndex][colIndex].ExceptWith(remainingPossibilitiesOnUnsetCells);

                                if (remainingPossiblities[currentRowIndex][colIndex].Count == 0)
                                {
                                    //MessageBox.Show($"Error Condition cell {currentRowIndex}, {colIndex} remainingPossiblities=" + remainingPossiblities[currentRowIndex][colIndex].Count+" ");
                                }

                                //MessageBox.Show($" processPreemptiveSetsInRows {currentRowIndex}, {colIndex} remainingPossiblities=" + remainingPossiblities[currentRowIndex][colIndex].Count + " ");


                                if (remainingPossiblities[currentRowIndex][colIndex].Count == 1)
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

                if (sudokuCells[rowIndex][currentColIndex] == 0)
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
                        var remainingPossibilitiesSet = remainingPossiblities[rowIndex][currentColIndex];


                        if (sudokuCells[rowIndex][currentColIndex] == 0 && remainingPossibilitiesSet.Count <= remainingPossibitySize)
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
                            if (sudokuCells[rowIndex][currentColIndex] == 0 && !cellsWithCommonPossilities.Contains(rowIndex))
                            {

                                //Reduce the possiblities everywhere.
                                remainingPossiblities[rowIndex][currentColIndex].ExceptWith(remainingPossibilitiesOnUnsetCells);
                                //MessageBox.Show("processPreemptiveSetsInColumns");

                                if (remainingPossiblities[rowIndex][currentColIndex].Count == 1)
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
                    if (sudokuCells[rowIndex][colIndex] == 0)
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

                            var remainingPossibilitiesSet = remainingPossiblities[rowIndex][colIndex];


                            if (sudokuCells[rowIndex][colIndex] == 0 && remainingPossibilitiesSet.Count <= remainingPossibitySize)
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
                                    remainingPossiblities[rowIndex][colIndex].ExceptWith(remainingPossibilitiesOnUnsetCells);

                                   // MessageBox.Show("Remaining possibities reduced.. in 3x3 square");


                                    if (remainingPossiblities[rowIndex][colIndex].Count == 1)
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
                    HashSet<int> remainingPossibilitiesSet = remainingPossiblities[rowIndex][colIndex];

                    //MessageBox.Show($"{rowIndex + 1},{colIndex + 1} = possibiliites {remainingPossibilitiesSet.Count}");

                    if (remainingPossibilitiesSet.Count == 1 && sudokuCells[rowIndex][colIndex]==0)
                    {
                        sudokuCells[rowIndex][colIndex] = remainingPossibilitiesSet.First();
                        remainingPossiblities[rowIndex][colIndex].Clear();

                        removeCellValuesFromRowColumnBoxPossibilities(rowIndex, colIndex, sudokuCells[rowIndex][colIndex]);

                        ProcessPreemptiveSets();

                       

                        aNewCellHasBeenSet = true;

                        //MessageBox.Show($"{rowIndex+1},{colIndex+1} = {sudokuCells[rowIndex][colIndex]}");

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

        public bool Solve()
        {
            bool isSolved = false;

            for (int rowIndex = 0; rowIndex <= 8; rowIndex++)
            {


                for (int colIndex = 0; colIndex <= 8; colIndex++)
                {

                    removeCellValuesFromRowColumnBoxPossibilities (rowIndex, colIndex, sudokuCells[rowIndex][colIndex]);
                }


            }

            ProcessPreemptiveSets();



            while (SetCellsWithUniquePossibility());

            return isSolved;

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

                    copyOfCells[rowIndex][colIndex] = sudokuCells[rowIndex][colIndex];


                    if (sudokuCells[rowIndex][colIndex]==0 && remainingPossiblities[rowIndex][colIndex].Count>=2)
                    {
                        int[] arr = remainingPossiblities[rowIndex][colIndex].ToArray();

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
}
