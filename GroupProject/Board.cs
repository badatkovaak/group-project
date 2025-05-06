using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using ChessLogic;

namespace Chess;

class Board : Canvas
{
    const string default_fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
    const double image_factor = 1;

    Position position;
    List<Rectangle> squares;
    List<PieceLabel> pieces;
    Square? selectedSquare;

    private List<Rectangle> highlightedSquares = new List<Rectangle>();

    public Board(string fen = default_fen)
    {
        this.Width = 400;
        this.Height = 400;
        this.Background = Brushes.Transparent;
        Position? p = FEN.PositionFromFEN(fen);

        if (p is null)
            throw new ArgumentException();

        this.position = (Position)p;
        Console.WriteLine(p);

        this.squares = new List<Rectangle>();
        this.pieces = new List<PieceLabel>();
        bool isLight = true;

        double squareSize = this.Width / 8;
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                Rectangle r = new Rectangle();
                r.Height = squareSize;
                r.Width = squareSize;
                Canvas.SetTop(r, 0);
                Canvas.SetLeft(r, 0);

                if (isLight)
                    r.Fill = Brushes.Beige;
                else
                    r.Fill = Brushes.SandyBrown;

                isLight = !isLight;

                this.squares.Add(r);
                this.Children.Add(r);
            }

            isLight = !isLight;
        }

        for (int i = 0; i < 8; i++)
        for (int j = 0; j < 8; j++)
        {
            Piece? piece = this.position[Square.NewUnchecked(i, j)];

            if (piece is null)
                continue;

            PieceLabel label = new PieceLabel(piece, Square.NewUnchecked(i, j));

            this.pieces.Add(label);
            this.Children.Add(label);
        }

        this.EffectiveViewportChanged += OnDimensionsChange;
        this.PointerReleased += OnMouseReleased;
    }

    public void Resize(double newSize)
    {
        this.Width = newSize;
        this.Height = newSize;

        double squareSize = newSize / 8;

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                var square = this.squares[i * 8 + j];
                square.Width = squareSize;
                square.Height = squareSize;
                Canvas.SetLeft(square, j * squareSize);
                Canvas.SetTop(square, i * squareSize);
            }
        }

        foreach (var piece in this.pieces)
        {
            Canvas.SetLeft(piece, piece.square.X * squareSize);
            Canvas.SetTop(piece, (7 - piece.square.Y) * squareSize);

            if (piece.Content is Image img)
            {
                img.Width = squareSize * image_factor;
                img.Height = squareSize * image_factor;
            }
        }

        this.InvalidateArrange();
        this.InvalidateMeasure();
    }

    public void OnDimensionsChange(object? sender, EffectiveViewportChangedEventArgs e)
    {
        double availableWidth = this.Bounds.Width;
        double availableHeight = this.Bounds.Height;
        double boardSize = Math.Min(availableWidth, availableHeight);

        double size = Math.Min(this.Bounds.Height, this.Bounds.Width);
        if (boardSize <= 0) return;
        double squareSize = boardSize / 8;

        Console.WriteLine(
    $"boarsSize - {boardSize}, ch - {squareSize}, Bounds - {this.Bounds.Height}, {this.Bounds.Width}");

        for (int i = 0; i <0; i++)
        {
            for(int j = 0; j < 8; j++)
            {
                Rectangle r = this.squares[i * 8 + j];
                r.Width = squareSize;
                r.Height = squareSize;
                Canvas.SetTop(r, i * squareSize);
                Canvas.SetLeft(r, i * squareSize);
            }
        }

        foreach (PieceLabel p in this.pieces)
        {
            Canvas.SetLeft(p, p.square.X * squareSize);
            Canvas.SetTop(p, (7 - p.square.Y) * squareSize);

            Image? image = p.Content as Image;

            if (image is null)
                continue;

            image.Height = squareSize * Board.image_factor;
            image.Width = squareSize * Board.image_factor;
        }
    }

    public void HighlightLegalMoves(Square s)
    {
        ClearHighlights();

        var legalMoves = this.position.GetLegalMoves(s);
        double squareSize = this.Width / 8;

        foreach (Square move in legalMoves)
        {
            Rectangle highlight = new Rectangle()
            {
                Width = squareSize,
                Height = squareSize,
                Fill = new SolidColorBrush(Colors.Yellow) { Opacity = 0.4 },
                Stroke = Brushes.Gold,
                StrokeThickness = 2,
                IsHitTestVisible = false
            };

            Canvas.SetLeft(highlight, move.X * squareSize);
            Canvas.SetTop(highlight, (7 - move.Y) * squareSize);

            this.highlightedSquares.Add(highlight);
            this.Children.Add(highlight);

            highlight.SetValue(Canvas.ZIndexProperty, 1);

        }

        foreach (var piece in this.pieces)
        {
            piece.SetValue(Canvas.ZIndexProperty, 2);
        }
    }
    public void ClearHighlights()
    {
        foreach (var highlight in highlightedSquares)
        {
            this.Children.Remove(highlight);
        }

        highlightedSquares.Clear();
    }

    public static async Task<Piece> PromotePawn(ChessInternals.Color pawnColor, Window parentWindow)
    {
        var dialog = new PromotionChoiceWindow(pawnColor);
        var chosenType = await dialog.ShowDialog<PieceType>(parentWindow);
        return new Piece(chosenType, pawnColor);
    }

    public class PromotionChoiceWindow : Window
    {
        public PieceType SelectedPiece { get; private set; }

        public PromotionChoiceWindow(ChessInternals.Color pieceColor)
        {
            this.Title = "Choose promotion";
            this.Width = 300;
            this.Height = 100;

            var panel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Spacing = 10
            };
            var options = new[] { PieceType.Queen, PieceType.Rook, PieceType.Bishop, PieceType.Knight };

            foreach (var type in options)
            {
                var btn = new Button
                {
                    Content = GetPieceSymbol(type, pieceColor),
                    Tag = type,
                    FontSize = 20,
                    Width = 50,
                    Height = 50
                };
                btn.Click += (s, e) =>
                {
                    SelectedPiece = (PieceType)btn.Tag;
                    Close();
                };
                panel.Children.Add(btn);
            }
            this.Content = panel;
        }
        private string GetPieceSymbol(PieceType type, ChessInternals.Color color)
        {
            return type switch
            {
                PieceType.Queen => color == ChessInternals.Color.White ? "♕" : "♛",
                PieceType.Rook => color == ChessInternals.Color.White ? "♖" : "♜",
                PieceType.Bishop => color == ChessInternals.Color.White ? "♗" : "♝",
                PieceType.Knight => color == ChessInternals.Color.White ? "♘" : "♞",
                _ => "?"
            };
        }
    }



    public void OnMouseReleased(object? sender, PointerReleasedEventArgs e)
    {
        //Console.WriteLine($"parent window - {(this.Parent as Grid).Parent as Window}");
        //var result = PromotePawn(ChessInternals.Color.White, (this.Parent as Grid).Parent as Window);

        if (e.InitialPressMouseButton != MouseButton.Left)
            return;

        Point p = e.GetCurrentPoint(this).Position;
        Console.WriteLine(p);

        double squareHeight = this.Bounds.Height / 8;
        double squareWidth = this.Bounds.Width / 8;

        double x = p.X;
        int i = 0;

        while (x >= squareWidth)
        {
            x -= squareWidth;
            i++;
        }

        double y = p.Y;
        int j = 7;

        while (y >= squareHeight)
        {
            y -= squareHeight;
            j--;
        }

        Square s = Square.NewUnchecked(i, j);
        Piece? capturedPiece = this.position.GetPieceOnSquare(s);

        if (selectedSquare is null)
        {
            if (capturedPiece is null)
                return;

            if (capturedPiece.color != this.position.colorToMove)
                return;

            HighlightLegalMoves(s);

            this.selectedSquare = s;
            Console.WriteLine($"selected Square - {s}");
            return;
        }

        if (this.selectedSquare is null)    
            throw new Exception();

        if (this.selectedSquare.Equals(s))
        {
            this.selectedSquare = null;
            ClearHighlights();
            Console.WriteLine("diselected square");
            return;
        }

        if (capturedPiece is null || capturedPiece.color != this.position.colorToMove)
        {
            bool isLegal = this.position.GetLegalMoves(this.selectedSquare).Contains(s);

            if (!isLegal)
            {
                this.selectedSquare = null;
                ClearHighlights();
                Console.WriteLine($"trying to make an illegal move - {this.selectedSquare} {s}");
                return;
            }

            Square start = this.selectedSquare;
            Square end = s;

            Console.WriteLine(this.position);

            Position? pos = this.position.MakeAMove(start, end);

            if (pos is null)
            {
                Console.WriteLine("this should not happen");
                return;
            }

            Console.WriteLine($"Made legal move - {start} {end}");

            Console.WriteLine(this.position);

            this.selectedSquare = null;
            ClearHighlights();

            if (capturedPiece is not null)
            {
                PieceLabel? label = null;

                foreach (PieceLabel l in this.pieces)
                    if (l.square.Equals(end))
                        label = l;

                if (label is null)
                {
                    Console.WriteLine("should not happen");
                    return; 
                }

                this.pieces.Remove(label);
                this.Children.Remove(label);
            }

            foreach (PieceLabel l in this.pieces)
            {
                if (l.square.Equals(start))
                {
                    l.square = end;

                    Canvas.SetLeft(l, end.X * squareWidth);
                    Canvas.SetTop(l, (7 - end.Y) * squareHeight);

                    Image? image = l.Content as Image;

                    if (image is not null)
                    {
                        image.Height = squareHeight * Board.image_factor;
                        image.Width = squareWidth * Board.image_factor;
                    }
                }
            }
        }
    }
}
