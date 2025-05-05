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

    public Board(string fen = default_fen)
    {
        Position? p = FEN.PositionFromFEN(fen);

        if (p is null)
            throw new ArgumentException();

        this.position = (Position)p;
        Console.WriteLine(p);

        this.squares = new List<Rectangle>();
        this.pieces = new List<PieceLabel>();
        bool isLight = true;

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                Rectangle r = new Rectangle();
                r.Height = 10;
                r.Width = 10;
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

    public void OnDimensionsChange(object? sender, EffectiveViewportChangedEventArgs e)
    {
        double h = this.Bounds.Height;
        double w = this.Bounds.Width;

        // if (h != w)
        // {
        //     if (h > w)
        //         h = w;
        //     else
        //         w = h;
        // }

        double squareWidth = w / 8;
        double squareHeight = h / 8;

        Console.WriteLine(
            $"h - {h}, w - {w}, ch - {squareHeight}, cw - {squareWidth}, Bounds - {this.Bounds.Height}, {this.Bounds.Width}"
        );

        for (int i = 0; i < 8; i++)
        for (int j = 0; j < 8; j++)
        {
            Rectangle r = this.squares[i * 8 + j];
            r.Height = squareHeight;
            r.Width = squareWidth;
            Canvas.SetTop(r, i * squareHeight);
            Canvas.SetLeft(r, j * squareWidth);
        }

        foreach (PieceLabel p in this.pieces)
        {
            Canvas.SetLeft(p, p.square.X * squareWidth);
            Canvas.SetTop(p, (7 - p.square.Y) * squareHeight);

            Image? image = p.Content as Image;

            if (image is null)
                continue;

            image.Height = squareHeight * Board.image_factor;
            image.Width = squareWidth * Board.image_factor;
        }
    }

    public void HighlightLegalMoves(Square s)
    {
        return;
    }

    public void OnMouseReleased(object? sender, PointerReleasedEventArgs e)
    {
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

            this.selectedSquare = s;
            Console.WriteLine($"selected Square - {s}");
            return;
        }

        if (this.selectedSquare is null)
            throw new Exception();

        if (this.selectedSquare.Equals(s))
        {
            this.selectedSquare = null;
            Console.WriteLine("diselected square");
            return;
        }

        if (capturedPiece is null || capturedPiece.color != this.position.colorToMove)
        {
            bool isLegal = this.position.GetLegalMoves(this.selectedSquare).Contains(s);

            if (!isLegal)
            {
                Console.WriteLine($"trying to make an illegal move - {this.selectedSquare} {s}");
                return;
            }

            Square start = this.selectedSquare;
            Square end = s;

            Position? pos = this.position.MakeAMove(start, end);

            if (pos is null)
            {
                Console.WriteLine("this should not happen");
                return;
            }

            Console.WriteLine($"Made legal move - {start} {end}");

            this.selectedSquare = null;

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
