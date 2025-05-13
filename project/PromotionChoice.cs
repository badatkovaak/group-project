using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media.Imaging;
using ChessLogic;

public class PromotionChoice : DockPanel
{
    public PromotionChoice(Color c, double cellSize)
    {
        foreach (PieceType t in PieceTypeUtils.GetPromotableTypes())
        {
            Image i = new Image();
            string image_path = (new Piece(t, c)).ToString();
            image_path += c == Color.White ? "" : "1";
            i.Source = new Bitmap("./assets/" + image_path + ".png");
            i.Height = cellSize;
            i.Width = cellSize;

            Button b = new Button();
            b.HorizontalAlignment = HorizontalAlignment.Stretch;
            b.VerticalAlignment = VerticalAlignment.Stretch;
            b.Content = i;

            this.Children.Add(b);
        }
    }
}
