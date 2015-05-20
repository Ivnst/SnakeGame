using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace snake
{
    public partial class MainForm : Form
    {
        #region <C-tors>

        public MainForm()
        {
            InitializeComponent();
            InitImage();
            RefreshButtons();
        }

        #endregion

        #region <Fields>

        //главный объект, управляющий игрокй
        private SnakeGame game;

        //объект, с помощью которого можно рисовать примитивные объекты
        private Graphics graphics;

        #endregion

        #region <Methods>

        /// <summary>
        /// Инициализация картинки, на которой будет рисоваться игра
        /// </summary>
        private void InitImage()
        {
            var image = new Bitmap(pbArea.Width, pbArea.Height);
            pbArea.Image = image;
            graphics = Graphics.FromImage(image);
            graphics.Clear(Color.White);

        }

        /// <summary>
        /// Обновление картинки. Перерисовка элементов на форме
        /// </summary>
        private void RefreshArea()
        {
            if (game == null) return;
            int cellWidth = pbArea.Width/(game.AreaWidth - 0);
            int cellHeight = pbArea.Height/(game.AreaHeight - 0);

            graphics.Clear(Color.White);

            var snakePoints = game.SnakePoints;
            foreach (var snakePoint in snakePoints)
            {
                graphics.FillEllipse(Brushes.Green, snakePoint.X*cellWidth, snakePoint.Y*cellHeight, cellWidth,
                    cellHeight);
            }

            graphics.FillEllipse(Brushes.DarkOrange, snakePoints[0].X*cellWidth, snakePoints[0].Y*cellHeight, cellWidth,
                cellHeight);


            graphics.FillRectangle(Brushes.Red, game.NextPoint.X*cellWidth, game.NextPoint.Y*cellHeight, cellWidth,
                cellHeight);
            this.Invoke(new Action(UpdatePictureBox));
        }

        /// <summary>
        /// Обновление Picture box. Вынес в отдельный метод, чтобы было удобней запускать с помощью метода Invoke (в методе выше)
        /// </summary>
        private void UpdatePictureBox()
        {
            pbArea.Refresh();
        }

        /// <summary>
        /// Обновление кнопок на форме. 
        /// </summary>
        private void RefreshButtons()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(RefreshButtons));
                return;
            }

            bool flag = game != null;
            btnNewGame.Enabled = !flag;
            btnPause.Enabled = flag;
            btnStop.Enabled = flag;
            btnUp.Enabled = flag;
            btnDown.Enabled = flag;
            btnLeft.Enabled = flag;
            btnRight.Enabled = flag;
        }

        #endregion

        #region <Form Events>

        private void btnNewGame_Click(object sender, EventArgs e)
        {
            //Создаём игру, и подписываемся на её события
            game = new SnakeGame(50,25, 5);
            game.Updated += game_Updated;
            game.Fault += game_Fault;
            game.Start();

            RefreshButtons();
            RefreshArea();
        }

        void game_Fault(object sender, EventArgs e)
        {
            if (game == null) return;
            game.Updated -= game_Updated;
            game.Fault -= game_Fault;
            game = null;

            MessageBox.Show("End game!");
            RefreshButtons();
        }

        void game_Updated(object sender, EventArgs e)
        {
            RefreshArea();
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            if (game == null) return;
            game.Pause();
            RefreshButtons();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (game == null) return;
            game.Stop();
            game.Updated -= game_Updated;
            game.Fault -= game_Fault;
            game = null;
            
            RefreshButtons();
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            if (game == null) return;
            game.Turn(TurnDirection.Up);
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            if (game == null) return;
            game.Turn(TurnDirection.Down);
        }

        private void btnLeft_Click(object sender, EventArgs e)
        {
            if (game == null) return;
            game.Turn(TurnDirection.Left);
        }

        private void btnRight_Click(object sender, EventArgs e)
        {
            if (game == null) return;
            game.Turn(TurnDirection.Right);
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            InitImage();
            RefreshArea();
        }

        #endregion

        /// <summary>
        /// Управление с помощью клавиатуры
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (game == null) return;
            if (e.KeyCode == Keys.Up)
                game.Turn(TurnDirection.Up);
            if (e.KeyCode == Keys.Down)
                game.Turn(TurnDirection.Down);
            if (e.KeyCode == Keys.Left)
                game.Turn(TurnDirection.Left);
            if (e.KeyCode == Keys.Right)
                game.Turn(TurnDirection.Right);

        }

    }
}
