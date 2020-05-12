using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class BulkTest
    {

        string inputBasePath = @"D:\Sandeep Dixit\Git\sudoku\Sudoku\UnitTests\sudokuDbFull\inputs";
        string resultBasePath = @"D:\Sandeep Dixit\Git\sudoku\Sudoku\UnitTests\sudokuDbFull\results";


        [TestMethod]
        public void ResumeBulkTest()
        {
          

            int currentLineIndex = 0;
            int lastReadLineNumber = 0;
            int wrongSolutions = 0;

            string csvLine;

         
   
            string sudokuTestResultsCounterFile = $@"{resultBasePath}\sudokuTestResultsCounter.txt";

            if (File.Exists(sudokuTestResultsCounterFile))
            {
                string resultsCounterString = File.ReadAllText(sudokuTestResultsCounterFile);

                if (!string.IsNullOrEmpty(resultsCounterString))
                {
                    lastReadLineNumber = int.Parse(resultsCounterString);
                }


            }


            // Read the file and display it line by line.  
            System.IO.StreamReader file =
                new System.IO.StreamReader($@"{inputBasePath}\sudoku.csv");

            string sudokuOriginalcsv = $@"{resultBasePath}\sudokuOriginal-{lastReadLineNumber}.csv";

            string sudokuTestResultsAll = $@"{resultBasePath}\sudokuTestResults-{lastReadLineNumber}.csv";
            string sudokuTestResultsFailed = $@"{resultBasePath}\sudokuTestResultsFailed-{lastReadLineNumber}.csv";


            while ((csvLine = file.ReadLine()) != null)
            {
                if (currentLineIndex <= lastReadLineNumber)
                {
                    //Ignore header line.
                    currentLineIndex++;
                    continue;

                }

               

                string[] sudokulineElements = csvLine.Split(',');

                string csvProblem = sudokulineElements[0];
                string csvSolution = sudokulineElements[1];
                string csvEntry = $"{currentLineIndex},{csvProblem},{csvSolution}";

                AppendSingleLineToFile(sudokuOriginalcsv, csvEntry);

                Sudoku.Sudoku sudoku = new Sudoku.Sudoku(csvProblem);



                //RenderNewEntries(sudoku, Color.Black);

                //MessageBox.Show($"Problem {counter} loaded from DB. Click to Solve ");

                sudoku.Solve();

                string solution = sudoku.ToString();



               

                if (solution.Equals(csvSolution))
                {
                    System.Diagnostics.Debug.WriteLine($"Solved problem number {currentLineIndex}. Total Wrong solutions {wrongSolutions}");

                    csvEntry = $"{currentLineIndex},{csvProblem},{solution}";
                    AppendSingleLineToFile(sudokuTestResultsAll, csvEntry);
                    File.WriteAllText(sudokuTestResultsCounterFile, "" + currentLineIndex);


                }
                else
                {
                    string csvEntryForWrongSolution = $"{currentLineIndex},{csvProblem},{solution},Wrong,{csvSolution}";
                    wrongSolutions++;
                    System.Diagnostics.Debug.WriteLine($"Wrong solution problem number {currentLineIndex}");



                    AppendSingleLineToFile(sudokuTestResultsFailed, csvEntryForWrongSolution);
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
