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
        string image_path = piece.ToString();
        image_path += piece.color == Color.White ? "" : "1";
        i.Source = new Bitmap("./assets/" + image_path + ".png");

        i.Height = 50;
        i.Width = 50;

        this.Content = i;
    }
}
