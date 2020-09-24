using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sudoku;

namespace UnitTests
{
    [TestClass]
    public class UnitTest
    {

        string inputBasePath = @"D:\Sandeep Dixit\Git\sudoku\Sudoku\UnitTests\sudokuDbFull\inputs";


        [TestMethod]
        public void EasyProblems()
        {


            SolveAFile($@"{inputBasePath}\EasyProblems.txt");

        }

        [TestMethod]
        public void HardProblems()
        {


            SolveAFile($@"{inputBasePath}\HardProblems.txt");

        }
        [TestMethod]
        public void AIEscargotProblems()
        {


            SolveAFile($@"{inputBasePath}\AIEscargotProblems.txt");

        }


        public void SolveAFile(string filePath)
        {

            string line;

            System.IO.StreamReader file =
                new System.IO.StreamReader(filePath);

            int currentLineIndex = 0;

            while ((line = file.ReadLine()) != null)
            {



                string[] sudokulineElements = line.Split(',');

                string inputProblem = sudokulineElements[0];
                string expectedSolution = sudokulineElements[1];


                Sudoku.Sudoku sudoku = new Sudoku.Sudoku(inputProblem);



                //RenderNewEntries(sudoku, Color.Black);

                //MessageBox.Show($"Problem {counter} loaded from DB. Click to Solve ");

                bool isSovled = sudoku.Solve();
                string solution = sudoku.ToString();

                if (isSovled)
                {
                   
                    if (!solution.Equals(expectedSolution))
                    {
                        throw new Exception($"Sudoku solution does not match input solution excped: {expectedSolution}.\n Actual {solution}");
                    }
                    Console.WriteLine("Solved a Problem. Rating="+sudoku.DifficultyRating);
                }
                else
                {
                    throw new Exception($"Sudoku could not be solved. Incompelete solution is {solution} expected solution was {expectedSolution}");
                }





                currentLineIndex++;
            }

            file.Close();
        }

        [TestMethod]
        public void zGeneralTests()
        {

            var values = @"004300209005009001070060043006002087190007400050083000600000105003508690042910300";

            Regex regularExpression = new Regex("[^0-9]");


            string result = regularExpression.Replace(values,"");

            Sudoku.Sudoku cloneWithSortedCells = new Sudoku.Sudoku(result);

            cloneWithSortedCells.EliminatePossibilitiesFromUnsetCells();
            cloneWithSortedCells.EliminatePossibilitiesFromUnsetCells();

            SudokuCell[][] cells =  cloneWithSortedCells.GetSudokuCellsCopy();

            SudokuCell[] linearArray = new SudokuCell[81];

            for (int rowIndex = 0; rowIndex <= 8; rowIndex++)
            {
                for (int colIndex = 0; colIndex <= 8; colIndex++)
                {
                   
                    linearArray[rowIndex * 9 + colIndex] = cells[rowIndex][colIndex];
                    
                }
            }

            Array.Sort(linearArray);


            for (int index = 0; index < linearArray.Length; index++)
            {
                if(linearArray[index].Value==0)
                Console.Write($"({linearArray[index].RemainingPossibilities.Count}) " );
            }


           

        }



        private void AppendSingleLineToFile(string path, string singleLine)
        {

            List<string> lines = new List<string>();
            lines.Add(singleLine);


            File.AppendAllLines(path, lines);

        }
    }
}
