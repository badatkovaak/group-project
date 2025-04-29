using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media.Imaging;
using ChessInternals;

namespace Backup;

class PieceLabel : Label
{
    public Piece piece;
    public Square square;

    public PieceLabel(Piece piece, Square square)
    {
        this.piece = piece;
        this.square = square;

        // TextBlock text = new TextBlock();
        // text.Text = this.piece.ToString();
        // text.FontSize = 60;
        // text.Foreground = Brushes.Black;
        //
        // text.HorizontalAlignment = HorizontalAlignment.Center;
        // text.VerticalAlignment = VerticalAlignment.Center;

        Image i = new Image();
        i.Source = new Bitmap("./assets/" + this.piece.ToString() + ".png");

        i.Height = 50;
        i.Width = 50;
        i.SizeChanged += OnSizeChanged;
        // i.EffectiveViewportChanged += OnEffectiveViewportChanged;

        // this.Content = text;
        this.Content = i;
    }

    public void OnSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        this.Height = e.NewSize.Height;
        this.Width = e.NewSize.Width;
    }

    // public void OnEffectiveViewportChanged(object? sender, EffectiveViewportChangedEventArgs e)
    // {
    //     // this.Bounds
    // }
}
