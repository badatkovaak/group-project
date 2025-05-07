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

    // List<Rectangle> squares;
    Rectangle[,] squares;
    PieceLabel?[,] pieces;
    Square? selectedSquare;

    // private List<Rectangle> highlightedSquares = new List<Rectangle>();

    public Board(string fen = default_fen)
    {
        this.Background = Brushes.Gray;
        Position? p = FEN.PositionFromFEN(fen);

        if (p is null)
            throw new ArgumentException();

        this.position = (Position)p;
        Console.WriteLine(p);

        this.squares = new Rectangle[8, 8];
        this.pieces = new PieceLabel[8, 8];

        // double boardSize = Math.Min(this.Bounds.Height, this.Bounds.Width);
        // double squareSize = boardSize / 8;

        for (int i = 0; i < 8; i++)
        for (int j = 0; j < 8; j++)
        {
            Rectangle r = new Rectangle();
            r.Height = 10;
            r.Width = 10;
            Canvas.SetTop(r, 0);
            Canvas.SetLeft(r, 0);

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

            this.pieces[i, j] = label;
            this.Children.Add(label);
        }

        this.EffectiveViewportChanged += OnDimensionsChange;
        this.PointerReleased += OnMouseReleased;
    }

    public void RepositionPieces(List<Move> moves){
        List<PieceLabel> deletedPieces = new List<PieceLabel>();

        foreach(Move move in moves){
            Square? start = move.start;
            Square? end = move.end;

            Console.WriteLine($"moved from {start} to {end}");

            if(end is not null && start is not null)
            {
                if(this.pieces[end.X, end.Y] is not null)
                    deletedPieces.Add(this.pieces[end.X, end.Y]);

                this.pieces[end.X, end.Y] = this.pieces[start.X, start.Y];

                if(this.pieces[end.X, end.Y] is not null)
                    this.pieces[end.X, end.Y].square = end;

                this.pieces[start.X, start.Y] = null;
            }
            else if(end is null && start is not null)
            {
                if(this.pieces[start.X, start.Y] is not null)
                    deletedPieces.Add(this.pieces[start.X, start.Y]);

                this.pieces[start.X, start.Y] = null;
            }
            else if(start is null && end is not null)
            {
                Piece? p1 = this.position[end] ;

                if(p1 is null)
                {
                    Console.WriteLine($"shouldnt happen 4 ");
                    return;
                }

                PieceLabel l = new PieceLabel(p1, end);
                this.pieces[end.X, end.Y] = l;
            }
        }

        foreach(PieceLabel? l in deletedPieces){
            if(l is null)
                continue;

            this.Children.Remove(l);
        }

        double boardSize = Math.Min(this.Bounds.Width, this.Bounds.Height);
        double squareSize = boardSize / 8;

        for(int i = 0; i < 8; i++)
        for(int j = 0; j < 8; j++)
        {
            PieceLabel? l = this.pieces[i,j];

            if(l is null)
                continue;

            Canvas.SetLeft(l, i * squareSize);
            Canvas.SetTop(l, (7 - j)*squareSize);
        }
    }

    public void RedrawPieces(){
        double boardSize = Math.Min(this.Bounds.Width, this.Bounds.Height);
        double squareSize = boardSize / 8;

        for(int i = 0; i < 8; i++)
        for(int j = 0; j < 8; j++)
        {
            PieceLabel? l = this.pieces[i,j];

            if(l is null)
                continue;

            Canvas.SetLeft(l, i * squareSize);
            Canvas.SetTop(l, (7 - j)*squareSize);
        }
    }

    public void OnDimensionsChange(object? sender, EffectiveViewportChangedEventArgs e)
    {
        double boardSize = Math.Min(this.Bounds.Height, this.Bounds.Width);
        double squareSize = boardSize / 8;

        Console.WriteLine(
            $"boardSize - {boardSize}, squareSize - {squareSize}, Bounds - {this.Bounds.Height}, {this.Bounds.Width}"
        );

        for (int i = 0; i < 8; i++)
        for (int j = 0; j < 8; j++)
        {
            Rectangle r = this.squares[i, j];
            r.Width = squareSize;
            r.Height = squareSize;
            Canvas.SetLeft(r, i * squareSize);
            Canvas.SetTop(r, j * squareSize);
        }

        for (int i = 0; i < 8; i++)
        for (int j = 0; j < 8; j++)
        {
            PieceLabel? p = this.pieces[i, j];

            if (p is null)
                continue;

            Canvas.SetLeft(p, i * squareSize);
            Canvas.SetTop(p, (7 - j) * squareSize);

            Image? image = p.Content as Image;

            if (image is null)
                continue;

            image.Height = squareSize * Board.image_factor;
            image.Width = squareSize * Board.image_factor;
        }
    }

    public void OnMouseReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (e.InitialPressMouseButton != MouseButton.Left)
            return;

        Point p = e.GetCurrentPoint(this).Position;
        Console.WriteLine(p);

        double boardSize = Math.Min(this.Bounds.Height, this.Bounds.Width);
        double squareSize = boardSize / 8;

        double x = p.X;
        int i = 0;

        while (x >= squareSize)
        {
            x -= squareSize;
            i++;
        }

        double y = p.Y;
        int j = 7;

        while (y >= squareSize)
        {
            y -= squareSize;
            j--;
        }

        Square? s = Square.New(i, j);
        
        if(s is null)
            return;

        Piece? piece = this.position.GetPieceOnSquare(s);

        Console.WriteLine($"pressed on square {s}, {piece}, currently selected {this.selectedSquare}");

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

        List<Square> legalMoves = this.position.GetLegalMoves(this.selectedSquare);
        // bool isLegal = legalMoves.

        if (!legalMoves.Contains(s))
        {
            Console.WriteLine($"trying to make an illegal move - {this.selectedSquare} {s}");
            return;
        }

        List<Move>? moves = this.position.MakeAMove(this.selectedSquare, s);

        if(moves is null)
        {
            Console.WriteLine($"moves is null");
            return;
        }
        
        this.RepositionPieces((List<Move>)moves);

        // foreach(Move move in moves){
        //     Square? start = move.start;
        //     Square? end = move.end;
        //
        //     if(end is not null && start is not null)
        //         this.pieces[end.X, end.Y] = this.pieces[start.X, start.Y];
        //     else if(end is null && start is not null)
        //         this.pieces[start.X, start.Y] = null;
        //     else if(start is null && end is not null){
        //         Piece? p1 = this.position[end] ;
        //
        //         if(p1 is null)
        //         {
        //             Console.WriteLine($"shouldnt happen 4 ");
        //             return;
        //         }
        //
        //         PieceLabel l = new PieceLabel(p1, end);
        //         this.pieces[end.X, end.Y] = l;
        //     }
        // }

        Console.WriteLine(this.position);
        
        this.selectedSquare = null;
        // this.RepositionPieces();

        return;
    }

    // public void OnMouseReleased(object? sender, PointerReleasedEventArgs e)
    // {
    //     //Console.WriteLine($"parent window - {(this.Parent as Grid).Parent as Window}");
    //     // var result = PromotePawn(ChessInternals.Color.White, (this.Parent as Grid).Parent as Window);
    //
    //     if (e.InitialPressMouseButton != MouseButton.Left)
    //         return;
    //
    //     Point p = e.GetCurrentPoint(this).Position;
    //     Console.WriteLine(p);
    //
    //     double boardSize = Math.Min(this.Bounds.Height, this.Bounds.Width);
    //     double squareSize = boardSize / 8;
    //
    //     double x = p.X;
    //     int i = 0;
    //
    //     while (x >= squareSize)
    //     {
    //         x -= squareSize;
    //         i++;
    //     }
    //
    //     double y = p.Y;
    //     int j = 7;
    //
    //     while (y >= squareSize)
    //     {
    //         y -= squareSize;
    //         j--;
    //     }
    //
    //     Square s = Square.NewUnchecked(i, j);
    //     Piece? capturedPiece = this.position.GetPieceOnSquare(s);
    //
    //     if (selectedSquare is null)
    //     {
    //         if (capturedPiece is null)
    //             return;
    //
    //         if (capturedPiece.color != this.position.colorToMove)
    //         {
    //             // this.selectedSquare = s;
    //             // Console.WriteLine($"changed selected square to {s}");
    //             return;
    //         }
    //
    //         this.selectedSquare = s;
    //         Console.WriteLine($"selected Square - {s}");
    //         return;
    //     }
    //
    //     if (this.selectedSquare is null)
    //         throw new Exception();
    //
    //     if (this.selectedSquare.Equals(s))
    //     {
    //         this.selectedSquare = null;
    //         // ClearHighlights();
    //         Console.WriteLine("diselected square");
    //         return;
    //     }
    //
    //     if (capturedPiece is not null && capturedPiece.color == this.position.colorToMove)
    //     {
    //         this.selectedSquare = s;
    //         Console.WriteLine($"changed selected square to {s}");
    //         return;
    //     }
    //
    //     if (capturedPiece is null || capturedPiece.color != this.position.colorToMove)
    //     {
    //         bool isLegal = this.position.GetLegalMoves(this.selectedSquare).Contains(s);
    //
    //         if (!isLegal)
    //         {
    //             this.selectedSquare = null;
    //             // ClearHighlights();
    //             Console.WriteLine($"trying to make an illegal move - {this.selectedSquare} {s}");
    //             return;
    //         }
    //
    //         Square start = this.selectedSquare;
    //         Square end = s;
    //
    //         Console.WriteLine(this.position);
    //
    //         List<Move>? pos = this.position.MakeAMove(start, end);
    //
    //         if (pos is null)
    //         {
    //             Console.WriteLine("this should not happen");
    //             return;
    //         }
    //
    //         Console.WriteLine($"Made legal move - {start} {end}");
    //
    //         Console.WriteLine(this.position);
    //
    //         this.selectedSquare = null;
    //         // ClearHighlights();
    //
    //         if (capturedPiece is not null)
    //         {
    //             PieceLabel? label = this.pieces[end.X, end.Y];
    //
    //             // foreach (PieceLabel l in this.pieces)
    //             //     if (l.square.Equals(end))
    //             //         label = l;
    //
    //             if (label is null)
    //             {
    //                 Console.WriteLine("should not happen");
    //                 return;
    //             }
    //
    //             this.pieces[end.X, end.Y] = null;
    //             this.Children.Remove(label);
    //         }
    //
    //         // for(int i = 0; i < 8; i++)
    //         //     for(int j= 0; j < 8; j++){
    //         //
    //         PieceLabel? l = this.pieces[start.X, start.Y];
    //
    //         if (l is null)
    //         {
    //             Console.WriteLine("shouldnt happen 3");
    //             return;
    //         }
    //
    //         l.square = end;
    //
    //         Canvas.SetLeft(l, end.X * squareSize);
    //         Canvas.SetTop(l, (7 - end.Y) * squareSize);
    //
    //         Image? image = l.Content as Image;
    //
    //         if (image is not null)
    //         {
    //             image.Height = squareSize * Board.image_factor;
    //             image.Width = squareSize * Board.image_factor;
    //         }
    //     }
    // }

    // public void HighlightLegalMoves(Square s)
    // {
    //     ClearHighlights();
    //
    //     var legalMoves = this.position.GetLegalMoves(s);
    //     double squareSize = this.Width / 8;
    //
    //     foreach (Square move in legalMoves)
    //     {
    //         Rectangle highlight = new Rectangle()
    //         {
    //             Width = squareSize,
    //             Height = squareSize,
    //             Fill = new SolidColorBrush(Colors.Yellow) { Opacity = 0.4 },
    //             Stroke = Brushes.Gold,
    //             StrokeThickness = 2,
    //             IsHitTestVisible = false,
    //         };
    //
    //         Canvas.SetLeft(highlight, move.X * squareSize);
    //         Canvas.SetTop(highlight, (7 - move.Y) * squareSize);
    //
    //         this.highlightedSquares.Add(highlight);
    //         this.Children.Add(highlight);
    //
    //         highlight.SetValue(Canvas.ZIndexProperty, 1);
    //     }
    //
    //     foreach (var piece in this.pieces)
    //     {
    //         piece.SetValue(Canvas.ZIndexProperty, 2);
    //     }}

    // public void ClearHighlights()
    // {
    //     foreach (var highlight in highlightedSquares)
    //     {
    //         this.Children.Remove(highlight);
    //     }
    //
    //     highlightedSquares.Clear();
    // }

    // public static async Task<Piece> PromotePawn(ChessLogic.Color pawnColor, Window parentWindow)
    // {
    //     var dialog = new PromotionChoiceWindow(pawnColor);
    //     var chosenType = await dialog.ShowDialog<PieceType>(parentWindow);
    //     return new Piece(chosenType, pawnColor);
    // }
    //
    // public class PromotionChoiceWindow : Window
    // {
    //     public PieceType SelectedPiece { get; private set; }
    //
    //     public PromotionChoiceWindow(ChessLogic.Color pieceColor)
    //     {
    //         this.Title = "Choose promotion";
    //         this.Width = 300;
    //         this.Height = 100;
    //
    //         var panel = new StackPanel
    //         {
    //             Orientation = Orientation.Horizontal,
    //             HorizontalAlignment = HorizontalAlignment.Center,
    //             VerticalAlignment = VerticalAlignment.Center,
    //             Spacing = 10,
    //         };
    //         var options = new[]
    //         {
    //             PieceType.Queen,
    //             PieceType.Rook,
    //             PieceType.Bishop,
    //             PieceType.Knight,
    //         };
    //
    //         foreach (var type in options)
    //         {
    //             var btn = new Button
    //             {
    //                 Content = GetPieceSymbol(type, pieceColor),
    //                 Tag = type,
    //                 FontSize = 20,
    //                 Width = 50,
    //                 Height = 50,
    //             };
    //             btn.Click += (s, e) =>
    //             {
    //                 SelectedPiece = (PieceType)btn.Tag;
    //                 Close();
    //             };
    //             panel.Children.Add(btn);
    //         }
    //         this.Content = panel;
    //     }

    // private string GetPieceSymbol(PieceType type, ChessLogic.Color color)
    // {
    //     return type switch
    //     {
    //         PieceType.Queen => color == ChessLogic.Color.White ? "♕" : "♛",
    //         PieceType.Rook => color == ChessLogic.Color.White ? "♖" : "♜",
    //         PieceType.Bishop => color == ChessLogic.Color.White ? "♗" : "♝",
    //         PieceType.Knight => color == ChessLogic.Color.White ? "♘" : "♞",
    //         _ => "?",
    //     };
    // }
}
