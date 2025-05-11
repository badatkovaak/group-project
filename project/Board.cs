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
    const double imageFactor = 1;
    const double boardMargin = 30;

    Position position;

    Rectangle[,] squares;
    PieceLabel?[,] pieces;
    Shape?[,] HighlightedSquares;

    Square? selectedSquare;
    Rectangle? selectedSquareHighlight;

    public Board(string fen = default_fen)
    {
        this.MinHeight = 200;
        this.MinWidth = 200;

        this.Background = new SolidColorBrush(new Avalonia.Media.Color(22, 21, 18, 1));
        Position? p = FEN.PositionFromFEN(fen);

        if (p is null)
            throw new ArgumentException();

        this.position = (Position)p;
        Console.WriteLine(p);

        this.squares = new Rectangle[8, 8];
        this.pieces = new PieceLabel[8, 8];
        this.HighlightedSquares = new Rectangle[8, 8];

        for (int i = 0; i < 8; i++)
        for (int j = 0; j < 8; j++)
        {
            Rectangle r = new Rectangle();

            if ((i + j) % 2 == 0)
                r.Fill = Brushes.Beige;
            else
                r.Fill = Brushes.SandyBrown;

            this.squares[i, j] = r;
            this.Children.Add(r);
        }

        for (int i = 0; i < 8; i++)
        for (int j = 0; j < 8; j++)
        {
            Piece? piece = this.position[Square.NewUnchecked(i, j)];

            if (piece is null)
                continue;

            PieceLabel label = new PieceLabel(piece, Square.NewUnchecked(i, j));
            label.ZIndex = 2;

            this.pieces[i, j] = label;
            this.Children.Add(label);
        }

        this.EffectiveViewportChanged += OnDimensionsChange;
        this.PointerReleased += OnMouseReleased;
    }

    public SizeInfo CalculateSizeInfo(double margin)
    {
        double availableHeight = this.Bounds.Height - 2 * margin;
        double availableWidth = this.Bounds.Width - 2 * margin;

        double boardSize = Math.Min(availableHeight, availableWidth);
        double cellSize = boardSize / 8;

        double startX = margin + (availableWidth - boardSize) / 2;
        double startY = margin + (availableHeight - boardSize) / 2;

        return new SizeInfo(startX, startY, boardSize, cellSize);
    }

    public void RepositionPieces(List<PieceMove> moves)
    {
        List<PieceLabel> deletedPieces = new List<PieceLabel>();

        foreach (PieceMove move in moves)
        {
            Square? start = move.start;
            Square? end = move.end;

            Console.WriteLine($"moved from {start} to {end}");

            if (end is not null && start is not null)
            {
                if (this.pieces[end.X, end.Y] is not null)
                    deletedPieces.Add(this.pieces[end.X, end.Y]);

                this.pieces[end.X, end.Y] = this.pieces[start.X, start.Y];

                if (this.pieces[end.X, end.Y] is not null)
                    this.pieces[end.X, end.Y].square = end;

                this.pieces[start.X, start.Y] = null;
            }
            else if (end is null && start is not null)
            {
                if (this.pieces[start.X, start.Y] is not null)
                    deletedPieces.Add(this.pieces[start.X, start.Y]);

                this.pieces[start.X, start.Y] = null;
            }
            else if (start is null && end is not null)
            {
                Piece? p1 = this.position[end];

                if (p1 is null)
                {
                    Console.WriteLine($"shouldnt happen 4 ");
                    return;
                }

                PieceLabel l = new PieceLabel(p1, end);
                this.pieces[end.X, end.Y] = l;
            }
        }

        foreach (PieceLabel? l in deletedPieces)
        {
            if (l is null)
                continue;

            this.Children.Remove(l);
        }

        SizeInfo info = this.CalculateSizeInfo(Board.boardMargin);

        for (int i = 0; i < 8; i++)
        for (int j = 0; j < 8; j++)
        {
            PieceLabel? l = this.pieces[i, j];

            if (l is null)
                continue;

            Canvas.SetLeft(l, info.boardStartX + i * info.cellSize);
            Canvas.SetTop(l, info.boardStartY + (7 - j) * info.cellSize);
        }
    }

    public void RedrawPieces()
    {
        double boardSize = Math.Min(this.Bounds.Width, this.Bounds.Height);
        double squareSize = boardSize / 8;

        for (int i = 0; i < 8; i++)
        for (int j = 0; j < 8; j++)
        {
            PieceLabel? l = this.pieces[i, j];

            if (l is null)
                continue;

            Canvas.SetLeft(l, i * squareSize);
            Canvas.SetTop(l, (7 - j) * squareSize);
        }
    }

    public void OnDimensionsChange(object? sender, EffectiveViewportChangedEventArgs e)
    {
        SizeInfo info = this.CalculateSizeInfo(Board.boardMargin);

        Console.WriteLine(
            $"boardSize - {info.boardSize}, squareSize - {info.cellSize}, Bounds - {this.Bounds.Height}, {this.Bounds.Width}"
        );

        for (int i = 0; i < 8; i++)
        for (int j = 0; j < 8; j++)
        {
            Rectangle r = this.squares[i, j];
            r.Width = info.cellSize;
            r.Height = info.cellSize;
            Canvas.SetLeft(r, info.boardStartX + i * info.cellSize);
            Canvas.SetTop(r, info.boardStartY + j * info.cellSize);
        }

        for (int i = 0; i < 8; i++)
        for (int j = 0; j < 8; j++)
        {
            PieceLabel? p = this.pieces[i, j];

            if (p is null)
                continue;

            Canvas.SetLeft(p, info.boardStartX + i * info.cellSize);
            Canvas.SetTop(p, info.boardStartY + (7 - j) * info.cellSize);

            Image? image = p.Content as Image;

            if (image is null)
                continue;

            image.Height = info.cellSize * Board.imageFactor;
            image.Width = info.cellSize * Board.imageFactor;
        }
    }

    public void OnMouseReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (e.InitialPressMouseButton != MouseButton.Left)
            return;

        Point p = e.GetCurrentPoint(this).Position;

        SizeInfo info = this.CalculateSizeInfo(Board.boardMargin);

        Square? s = Board.CalculateSquareClicked(p, info);

        if (s is null)
        {
            this.selectedSquare = null;
            Console.WriteLine("pressed outside the board");
            return;
        }

        Piece? piece = this.position.GetPieceOnSquare(s);

        Console.WriteLine(
            $"pressed on square {s}, {piece}, currently selected {this.selectedSquare}"
        );

        // List<Move> moves = this.position.GetLegalMoves(s);
        List<Square> squares = this.position.GetLegalSquares(s);
        this.HighlightSquares(info, squares);

        if (this.selectedSquare is null)
        {
            if (piece is null)
                return;

            if (piece.color != this.position.colorToMove)
                return;

            this.selectedSquare = s;
            Console.WriteLine($"selected square {s}");
            return;
        }

        if (this.selectedSquare == s)
        {
            this.selectedSquare = null;
            Console.WriteLine($"disselected square");
            return;
        }

        if (piece is not null && piece.color == this.position.colorToMove)
        {
            this.selectedSquare = s;
            Console.WriteLine($"changed square to {s}");
            return;
        }

        List<Move> legalMoves = this.position.GetLegalMoves(this.selectedSquare);

        if (!legalMoves.Contains(new Move(this.selectedSquare, s)))
        {
            Console.WriteLine($"trying to make an illegal move - {this.selectedSquare} {s}");
            return;
        }

        List<PieceMove>? pieceMoves = this.position.MakeAMove(new Move(this.selectedSquare, s));

        if (pieceMoves is null)
        {
            Console.WriteLine($"moves is null");
            return;
        }

        this.RepositionPieces((List<PieceMove>)pieceMoves);

        Console.WriteLine(this.position);

        this.selectedSquare = null;
        return;
    }

    private static Square? CalculateSquareClicked(Point p, SizeInfo info)
    {
        double x = p.X - info.boardStartX;
        int i = 0;

        while (x >= info.cellSize)
        {
            x -= info.cellSize;
            i++;
        }

        if (x < 0)
            i = -1;

        double y = p.Y - info.boardStartY;
        int j = 7;

        while (y >= info.cellSize)
        {
            y -= info.cellSize;
            j--;
        }

        if (y < 0)
            j = -1;

        return Square.New(i, j);
    }

    private void HighlightSquares(SizeInfo info, Square s)
    {
        if (this.HighlightedSquares[s.X, s.Y] is not null)
            return;

        Shape r = Board.createHighlightSquare(s, info);
        this.HighlightedSquares[s.X, s.Y] = r;
    }

    private void HighlightSquares(SizeInfo info, List<Square> squares)
    {
        foreach (Square s in squares)
        {
            if (this.HighlightedSquares[s.X, s.Y] is not null)
                continue;

            Shape r = Board.createHighlightSquare(s, info);
            this.HighlightedSquares[s.X, s.Y] = r;
            this.Children.Add(r);
        }
    }

    // private static

    private static Shape createHighlightSquare(Square s, SizeInfo info)
    {
        // Rectangle r = new Rectangle();
        Ellipse r = new Ellipse();
        r.Fill = Brushes.Olive;
        r.Opacity = 0.7;
        r.Height = info.cellSize;
        r.Width = info.cellSize;

        double x = info.boardStartX + s.X * info.cellSize;
        double y = info.boardStartY + (7 - s.Y) * info.cellSize;

        Canvas.SetLeft(r, x);
        Canvas.SetTop(r, y);

        return r;
    }
}

public struct SizeInfo
{
    public readonly double boardStartX;
    public readonly double boardStartY;
    public readonly double boardSize;
    public readonly double cellSize;

    public SizeInfo(double startX, double startY, double boardSize, double cellSize)
    {
        this.boardStartX = startX;
        this.boardStartY = startY;
        this.boardSize = boardSize;
        this.cellSize = cellSize;
    }
}
