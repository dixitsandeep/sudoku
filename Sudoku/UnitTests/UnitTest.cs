using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
                string inputSolution = sudokulineElements[1];


                Sudoku.Sudoku sudoku = new Sudoku.Sudoku(inputProblem);



                //RenderNewEntries(sudoku, Color.Black);

                //MessageBox.Show($"Problem {counter} loaded from DB. Click to Solve ");

                if (sudoku.Solve())
                {
                    string solution = sudoku.Solution();
                    if (!solution.Equals(inputSolution))
                    {
                        throw new Exception($"Sudoku solution does not match input solution excped: {inputSolution}.\n Actual {solution}");
                    }
                    Console.WriteLine("Solved a Problem");
                }
                else
                {
                    throw new Exception($"Sudoku could not be solved {line}");
                }





                currentLineIndex++;
            }

            file.Close();
        }



        private void AppendSingleLineToFile(string path, string singleLine)
        {

            List<string> lines = new List<string>();
            lines.Add(singleLine);


            File.AppendAllLines(path, lines);

        }
    }
}
