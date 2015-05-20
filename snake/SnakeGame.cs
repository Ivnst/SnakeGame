using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows.Forms.VisualStyles;
using Timer = System.Timers.Timer;

namespace snake
{
    /// <summary>
    /// Главный класс игры
    /// </summary>
    public class SnakeGame
    {
        #region <C-tors>

        public SnakeGame(int areaWidth, int areaHeight, int snakeInitialLength)
        {
            if (areaWidth < 10) throw new ArgumentException("areaWidth < 10");
            if (areaHeight < 10) throw new ArgumentException("areaHeight < 10");
            if (snakeInitialLength < 1) throw new ArgumentException("snakeInitialLength < 10");

            this.areaHeight = areaHeight;
            this.areaWidth = areaWidth;
            this.snakeInitialLength = snakeInitialLength;

            InitSnake(snakeInitialLength);
        }

        #endregion

        #region <Fields>

        private int areaWidth = 30;
        private int areaHeight = 30;
        private int snakeInitialLength = 5;

        private bool isPaused = false;
        private bool isStoped = false;

        private LinkedList<Point> snake = new LinkedList<Point>();
        private int speed = 400;
        private TurnDirection currentDirection = TurnDirection.Right;

        private Point nextPoint;

        private Thread clockThread;

        #endregion

        #region <Properties>

        /// <summary>
        /// Возвращает координаты змейки
        /// </summary>
        public List<Point> SnakePoints
        {
            get { return snake.ToList(); }
        }

        /// <summary>
        /// Координаты точки, которую змейка должна достичь
        /// </summary>
        public Point NextPoint
        {
            get { return nextPoint; }
        }

        /// <summary>
        /// Ширина поля (в клетках)
        /// </summary>
        public int AreaWidth
        {
            get { return areaWidth; }
        }

        /// <summary>
        /// Высота поля (в клетках)
        /// </summary>
        public int AreaHeight
        {
            get { return areaHeight; }
        }

        #endregion

        #region <Events>

        //Событие для обновление экрана
        public event EventHandler Updated;

        //событие завершения игры (если змейка врезалась в себя)
        public event EventHandler Fault;

        #endregion

        #region <Methods>

        /// <summary>
        /// Инициализация змейки. В начале игры.
        /// </summary>
        /// <param name="initialLength"></param>
        private void InitSnake(int initialLength)
        {
            int midX = areaWidth/2;
            int midY = areaHeight/2;
            for (int i = 0; i < initialLength; i++)
            {
                snake.AddLast(new Point(midX - i, midY));
            }
        }

        /// <summary>
        /// Запуск змейки. Шаги выполняются в отдельном потоке
        /// </summary>
        public void Start()
        {
            if(clockThread != null) throw new InvalidOperationException();

            snake.Clear();
            InitSnake(snakeInitialLength);
            GenerateNextPoint();

            clockThread = new Thread(new ThreadStart(Clock));
            clockThread.IsBackground = true;
            clockThread.Start();
        }

        /// <summary>
        /// Метод выполняемый в отдельном потоке. Делает шаг змейки через определённый интервал времени.
        /// </summary>
        private void Clock()
        {
            while (true)
            {
                Thread.Sleep(speed);
                if(isPaused) continue;
                MakeStep();
                if(isStoped) break;
            }
        }

        /// <summary>
        /// Пауза игры
        /// </summary>
        public void Pause()
        {
            isPaused = !isPaused;
        }

        /// <summary>
        /// Остановка игры
        /// </summary>
        public void Stop()
        {
            if (clockThread == null) throw new InvalidOperationException();
            isStoped = true;
            clockThread = null;
        }

        /// <summary>
        /// Поворот змейки в нужном направлении. Если направление не меняется, то производится дополнительный шаг (ускорение)
        /// </summary>
        /// <param name="direction"></param>
        public void Turn(TurnDirection direction)
        {
            if (direction == currentDirection)
            {
                MakeStep();
                return;
            }
            currentDirection = direction;
        }

        /// <summary>
        /// Выполнение одного шага змейки
        /// </summary>
        public void MakeStep()
        {
            lock (snake)
            {
                var next = GetNextCell();
                if (IsPartOfSnake(next)) EndGame();
                snake.AddFirst(next);

                if (!nextPoint.Equals(next))
                    snake.RemoveLast();
                else
                    GenerateNextPoint();

                if (Updated != null)
                    Updated(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Возвращает координаты следующей клетки, в которую попадает змейка с учётом текущего направления
        /// </summary>
        /// <returns></returns>
        private Point GetNextCell()
        {
            int X = snake.First.Value.X;
            int Y = snake.First.Value.Y;
            switch (currentDirection)
            {
                case TurnDirection.Down:
                    Y++;
                    break;

                case TurnDirection.Left:
                    X--;
                    break;
                case TurnDirection.Right:
                    X++;
                    break;
                case TurnDirection.Up:
                    Y--;
                    break;
                default:
                    throw new InvalidOperationException();
            }

            if (X < 0) X = areaWidth - 1;
            if (X >= areaWidth) X = 0;
            if (Y < 0) Y = areaHeight - 1;
            if (Y >= areaHeight) Y = 0;
            return new Point(X, Y);
        }

        /// <summary>
        /// Проверяем является ли указанная клетка частью змейки
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        private bool IsPartOfSnake(Point part)
        {
            return snake.Any(snakePart => snakePart.Equals(part));
        }

        /// <summary>
        /// Завершение игры. Приватный метод
        /// </summary>
        private void EndGame()
        {
            Stop();

            if (Fault != null)
                Fault(this, EventArgs.Empty);
        }

        /// <summary>
        /// Генерация новой красной клетки.
        /// </summary>
        private void GenerateNextPoint()
        {
            var rnd = new Random((int)DateTime.Now.Ticks);
            while (true)
            {
                var pt = new Point(rnd.Next(areaWidth), rnd.Next(areaHeight));
                if (!IsPartOfSnake(pt))
                {
                    nextPoint = pt;
                    speed -= 10;
                    break;
                }
            }
        }

        #endregion
    }
}
