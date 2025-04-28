namespace ChessHandlers
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private (int x, int y)? selectedSquare = null;
        private Rectangle selectionHighlight;
        private List<Rectangle> moveHighlights = new List<Rectangle>();

        public MainWindow()
        {
            InitializeComponent();
            InitializeSelectionHighlight();
        }

        private void InitializeSelectionHighlight()
        {
            selectionHighlight = new Rectangle
            {
                Width = 100,
                Height = 100,
                Stroke = Brushes.Yellow,
                StrokeThickness = 3,
                Fill = Brushes.Transparent,
            };
        }

        private void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point clickPos = e.GetPosition(ChessBoard); // получаем координаты клика
            int x = (int)(clickPos.X / 100);
            int y = (int)(clickPos.Y / 100);

            if (selectedSquare == null)
            {
                // заглушка: предположим что есть фигуры на клетках (1,1) и (2,2)
                bool hasPiece = (x == 1 && y == 1) || (x == 2 && y == 2);

                if (hasPiece)
                {
                    selectedSquare = (x, y);
                    ShowSelection(x, y);
                    // заглушка: возможные ходы
                    var moves = new[] { (x + 1, y + 1) };
                    ShowPossibleMoves(moves);
                }
            }
            else
            {
                // проверяем можно ли ходить на эту клетку
                bool isValidMove = true; // заглушка

                if (isValidMove)
                {
                    MessageBox.Show($"Ход с {selectedSquare} на ({x},{y})"); // здесь должно быть перемещщение фигуры
                }
                ClearSelection();
                selectedSquare = null;
            }
        }

        private void ShowSelection(int x, int y)
        {
            Canvas.SetLeft(selectionHighlight, x * 100);
            Canvas.SetTop(selectionHighlight, y * 100);
            ChessBoard.Children.Add(selectionHighlight);
        }

        private void ClearSelection()
        {
            ChessBoard.Children.Remove(selectionHighlight);
            ClearPossibleMoves();
        }

        private void ShowPossibleMoves((int x, int y)[] moves)
        {
            ClearPossibleMoves();

            foreach (var move in moves)
            {
                var highlight = new Rectangle
                {
                    Width = 100,
                    Height = 100,
                    Fill = Brushes.LightGreen,
                    Opacity = 0.5,
                };
                Canvas.SetLeft(highlight, move.x * 100);
                Canvas.SetTop(highlight, move.y * 100);
                ChessBoard.Children.Add(highlight);
                moveHighlights.Add(highlight);
            }
        }

        private void ClearPossibleMoves()
        {
            foreach (var highlight in moveHighlights)
            {
                ChessBoard.Children.Remove(highlight);
            }
            moveHighlights.Clear();
        }
    }
}
