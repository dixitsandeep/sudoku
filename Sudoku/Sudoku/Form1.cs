using System;
using System.Drawing;
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
            int[,] values =
                new int[9, 9]
                {
                    { 7, 0, 0, 0, 0, 0, 3, 0, 1},
                    { 3, 6, 1, 2, 0, 4, 0, 0, 5},
                    { 0, 0, 8, 0, 0, 1, 0, 4, 0},

                    { 0, 8, 5, 0, 2, 7, 0, 1, 0},
                    { 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    { 0, 9, 0, 8, 1, 0, 7, 6, 0},

                    { 0, 5, 0, 7, 0, 0, 1, 0, 0},
                    { 6, 0, 0, 9, 0, 3, 4, 7, 8},
                    { 9, 0, 7, 0, 0, 0, 0, 0, 6},

                };

            var sudoku = new Sudoku(values);

            render(sudoku, Color.Black);


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

            render(sudoku, Color.Black);


        }

        private void setHardProblem()
        {
        

            int[,] values =
                new int[9, 9]
                {
                    { 4, 0, 0,    0, 7, 0,    0, 2, 0},
                    { 0, 0, 0,    1, 0, 0,    0, 8, 6},
                    { 0, 0, 0,    0, 0, 0,    0, 0, 9},

                    { 0, 0, 5,    0, 8, 4,    0, 3, 7},
                    { 0, 0, 0,    2, 0, 7,    0, 0, 0},
                    { 1, 7, 0,    5, 3, 0,    9, 0, 0},

                    { 8, 0, 0,    0, 0, 0,    0, 0, 0},
                    { 2, 9, 0,    0, 0, 3,    0, 0, 0},
                    { 0, 5, 0,    0, 4, 0,    0, 0, 3}

                };

            var sudoku = new Sudoku(values);

            render(sudoku, Color.Black);


        }

        private void setBlank()
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

            render(sudoku, Color.Blue);
            this.message.Text = $"";
        }

        private void render(Sudoku sudoku, Color color)
        {
            int[][] cells = sudoku.cells();

            for (int rowIndex = 0; rowIndex <= 8; rowIndex++)
            {

                for (int colIndex = 0; colIndex <= 8; colIndex++)
                {

                    if (cells[rowIndex][colIndex] != 0)
                    {
                        if (string.IsNullOrEmpty(sudokuTextBoxes[rowIndex][colIndex].Text))
                        {
                            sudokuTextBoxes[rowIndex][colIndex].ForeColor = color;

                        }

                        sudokuTextBoxes[rowIndex][colIndex].Text = cells[rowIndex][colIndex].ToString();
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

            render(sudoku, Color.Blue);

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
            setBlank();
        }

        private void logger_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            setBlank();
            setEasyProblem();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            setBlank();
            setMediumProblem();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            setBlank();
            setHardProblem();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var sudoku = CreateSudokuObjectFromTextGrid();
            var isSolved = sudoku.Solve();

            //MessageBox.Show($"Done ! now Refreshing");

            render(sudoku, Color.Blue);

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
    }


}
