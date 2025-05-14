using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media.Imaging;
using ChessLogic;

public class PromotionChoice : DockPanel
{
    public PieceType? chosenType;

    public PromotionChoice(Color c, double cellSize)
    {
        foreach (PieceType t in PieceTypeUtils.GetPromotableTypes())
        {
            LabeledButton b = new LabeledButton(t, c, cellSize);
            this.Children.Add(b);
        }
    }

    public void OnClick(object? sender, RoutedEventArgs e)
    {
        if(this.chosenType is not null)
            return;

        LabeledButton? b = sender as LabeledButton;

        if(b is null)
            return;

        this.chosenType = b.type;
    }
}

public class LabeledButton : Button 
{
    public PieceType type;

    public LabeledButton(PieceType t, Color c, double cellSize)
    {
        this.type = t;

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
    }
}
