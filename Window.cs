using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace MazeMasters
{
    public partial class Window : Form
    {
        static readonly int maxWidth;

        static Window()
        {
            using (PrintDocument doc = CreatePrintDocument())
            {
                maxWidth = Math.Min((int)doc.DefaultPageSettings.PrintableArea.Width, (int)doc.DefaultPageSettings.PrintableArea.Height);
            }
        }

        private static PrintDocument CreatePrintDocument()
        {
            PrintDocument doc = new PrintDocument();
            doc.DefaultPageSettings.PrinterResolution = doc.PrinterSettings.PrinterResolutions[0];
            return doc;
        }

        private Maze maze;
        private bool debugRender = false;
        private Maze.Difficulty difficulty = Maze.Difficulty.MiniMaze;
        private string title;

        public Window()
        {
            InitializeComponent();

            NewMaze(false);
        }

        private void NewMaze(int seed, bool debug)
        {
            maze = new Maze(seed, difficulty, maxWidth);
            Size = new System.Drawing.Size(maze.PixelSize.Width + 16, maze.PixelSize.Height + 38);
            title = string.Format("Seed: {0} Difficulty: {1}", maze.Seed.ToString(), difficulty);

            if (!debug)
            {
                for (int i = 0; true; i++)
                {
                    if (!maze.FillNext())
                        break;
                }
            }

            Text = title;
            DrawMaze();
        }

        private int GetSeed()
        {
            GetSeed form = new GetSeed();
            form.ShowDialog();
            return form.seed;
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            switch (e.KeyChar)
            {
                case 'e':
                case 'E':
                    int seed = GetSeed();
                    NewMaze(seed, false);
                    break;

                case 's':
                case 'S':
                    ToggleDebug();
                    break;

                case 'p':
                case 'P':
                    Print();
                    break;

                case ' ':
                    FillNext();
                    break;

                case 'n':
                case 'N':
                    NewMaze(false);
                    break;

                case 'r':
                case 'R':
                    NewMaze(maze.Seed, true);
                    break;

                case 'd':
                case 'D':
                    ChangeDifficulty();
                    break;
            }
        }

        private void ChangeDifficulty()
        {
            Array array = Enum.GetValues(typeof(Maze.Difficulty));

            for (int i = 0; i < array.Length; i++)
            {
                if ((Maze.Difficulty)array.GetValue(i) == difficulty)
                {
                    int newIndex = (i + 1) % array.Length;
                    difficulty = (Maze.Difficulty)array.GetValue(newIndex);

                    NewMaze(false);
                    break;
                }
            }
        }

        private void Print()
        {
            using (PrintDocument doc = CreatePrintDocument())
            {
                doc.DocumentName = title;

                doc.PrintPage +=
                    (sender, e) =>
                    {
                        maze.Draw(e.Graphics, debugRender);
                    };

                doc.Print();
            }
        }

        private void ToggleDebug()
        {
            debugRender = !debugRender;
            DrawMaze();
        }

        private void DrawMaze()
        {
            using (Graphics graphics = CreateGraphics())
            {
                maze.Draw(graphics, debugRender);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            DrawMaze();
        }

        protected override void OnClick(EventArgs e)
        {
            FillNext();
        }

        private void NewMaze(bool debug)
        {
            NewMaze(new Random().Next(), debug);
        }

        private void FillNext()
        {
            maze.FillNext();
            DrawMaze();
        }
    }
}
