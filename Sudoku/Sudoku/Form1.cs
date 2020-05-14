using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace Sudoku
{
    public partial class Form1 : Form
    {
        
        private TextBox[][] sudokuTextBoxes;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            int cellWidth = 20;

           
            sudokuTextBoxes = new TextBox[9][];

            for (int rowIndex = 0; rowIndex <= 8; rowIndex++)
            {
                int top = 100 + rowIndex * cellWidth + rowIndex / 3;

                sudokuTextBoxes[rowIndex] = new TextBox[9];

                for (int colIndex = 0; colIndex <= 8; colIndex++)
                {
                    int left = 100 + colIndex * cellWidth+ colIndex/3;

                    TextBox sudokuCell = new TextBox();

                    sudokuTextBoxes[rowIndex][colIndex] = sudokuCell;

                    sudokuCell.Top = top;
                    sudokuCell.Left = left;
                    sudokuCell.Width = cellWidth;
                    sudokuCell.Height = cellWidth;

                    this.Controls.Add(sudokuCell);
                }
            }

            initializeSudoku();
        }

       

        private void initializeSudoku()
        {
            setHardProblem();


        }

        private void setEasyProblem()
        {
  

            var sudokuString  = "700000301361204005008001040085027010000000000090810760050700100600903478907000006";

            var sudoku = new Sudoku(sudokuString);

            RenderNewEntries(sudoku, Color.Black);


        }

        private void setMediumProblem()
        {
            int[,] values =
                new int[9, 9]
                {
                    { 0, 0, 0, 0, 9, 0, 5, 3, 0},
                    { 0, 6, 0, 8, 3, 5, 7, 0, 0},
                    { 0, 0, 5, 0, 0, 0, 0, 6, 1},

                    { 0, 0, 0, 6, 0, 0, 0, 0, 5},
                    { 7, 0, 0, 0, 1, 0, 0, 0, 6},
                    { 1, 0, 0, 0, 0, 4, 0, 0, 0},

                    { 4, 1, 0, 0, 0, 0, 8, 0, 0},
                    { 0, 0, 7, 5, 8, 3, 0, 4, 0},
                    { 0, 8, 3, 0, 2, 0, 0, 0, 0}

                };

            var sudoku = new Sudoku(values);

            RenderNewEntries(sudoku, Color.Black);


        }

        private void setHardProblem()
        {


            var values = "380000000000400785009020300060090000800302009000040070001070500495006000000000092";

            var sudoku = new Sudoku(values);

            RenderNewEntries(sudoku, Color.Black);


        }

        private void setAIEscargotProblem()
        {

            var values =  @"100 007 090
                            030 020 008
                            009 600 500

                            005 300 900
                            010 080 002
                            600 004 000

                            300 000 010
                            040 000 007
                            007 000 300";



          

            var sudoku = new Sudoku(values);

            RenderNewEntries(sudoku, Color.Black);


        }

        private void ClearSudokuGrid()
        {
            //Clear
            int[,] values =
                new int[9, 9]
                {
                    { 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    { 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    { 0, 0, 0, 0, 0, 0, 0, 0, 0},

                    { 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    { 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    { 0, 0, 0, 0, 0, 0, 0, 0, 0},

                    { 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    { 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    { 0, 0, 0, 0, 0, 0, 0, 0, 0}
                };



            var sudoku = new Sudoku(values);

            RenderNewEntries(sudoku, Color.Blue);
            this.message.Text = $"";
        }

        private void RenderNewEntries(Sudoku sudoku, Color color)
        {
            SudokuCell[][] cells = sudoku.GetSudokuCellsCopy();

            for (int rowIndex = 0; rowIndex <= 8; rowIndex++)
            {

                for (int colIndex = 0; colIndex <= 8; colIndex++)
                {

                    if (cells[rowIndex][colIndex].Value != 0)
                    {
                        if (string.IsNullOrEmpty(sudokuTextBoxes[rowIndex][colIndex].Text))
                        {
                            sudokuTextBoxes[rowIndex][colIndex].ForeColor = color;

                        }

                        sudokuTextBoxes[rowIndex][colIndex].Text = cells[rowIndex][colIndex].Value.ToString();
                    }
                    else
                    {
                        sudokuTextBoxes[rowIndex][colIndex].Text = null;
                    }
                }
            }
        }

        private Sudoku CreateSudokuObjectFromTextGrid()
        {
            int count = 0;
            int[,] values = new int[9, 9];

            for (int rowIndex = 0; rowIndex <= 8; rowIndex++)
            {
                for (int colIndex = 0; colIndex <= 8; colIndex++)
                {
                    if (!string.IsNullOrEmpty(sudokuTextBoxes[rowIndex][colIndex].Text))
                    {

                        values[rowIndex, colIndex] = int.Parse(sudokuTextBoxes[rowIndex][colIndex].Text);
                        count++;
                    }
                }
            }

            this.message.Text = $"Solving [ filled cells = {count} ]";

            var sudoku = new Sudoku(values);

            return sudoku;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var sudoku = CreateSudokuObjectFromTextGrid();
            var isSolved = sudoku.Solve();

            //MessageBox.Show($"Done ! now Refreshing");

            RenderNewEntries(sudoku, Color.Blue);

            if (isSolved)
            {
                this.message.Text = $"Solved";
            }
            else
            {
                this.message.Text = $"Problem Could not be solved.";
                this.message.ForeColor = Color.Red;

            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            ClearSudokuGrid();
        }

        private void logger_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            ClearSudokuGrid();
            setEasyProblem();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ClearSudokuGrid();
            setMediumProblem();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ClearSudokuGrid();
            setHardProblem();
        }

        private void button5_Click(object sender, EventArgs e)
        {

            this.message.Text = $"running..";
            var sudoku = CreateSudokuObjectFromTextGrid();
            var isSolved = sudoku.SolveOneIterationOnly();

            //MessageBox.Show($"Done ! now Refreshing");

            RenderNewEntries(sudoku, Color.Blue);

            if (isSolved)
            {
                this.message.Text = $"Complete";
            }
            else
            {
                this.message.Text = $"Try more steps..";
                this.message.ForeColor = Color.Red;

            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            setAIEscargotProblem();
        }
    }


}
