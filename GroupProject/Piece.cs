using Avalonia.Controls;
using Avalonia.Media.Imaging;
using ChessLogic;

namespace Chess;

public class PieceLabel : Label
{
    public Piece piece;
    public Square square;

    public PieceLabel(Piece piece, Square square)
    {
        this.piece = piece;
        this.square = square;

        Image i = new Image();
        i.Source = new Bitmap("./assets/" + this.piece.ToString() + ".png");

        i.Height = 50;
        i.Width = 50;

        this.Content = i;
    }
}
