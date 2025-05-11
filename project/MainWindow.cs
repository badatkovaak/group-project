using Avalonia.Controls;
using Avalonia.Layout;

namespace Chess;

public class MainWindow : Window
{
    public MainWindow()
    {
        this.Title = "Chess";

        this.MinWidth = 200;
        this.MinHeight = 200;

        Grid maingrid = new Grid();
        maingrid.HorizontalAlignment = HorizontalAlignment.Stretch;
        maingrid.VerticalAlignment = VerticalAlignment.Stretch;

        maingrid.RowDefinitions.Add(new RowDefinition(1, GridUnitType.Star));
        // maingrid.RowDefinitions.Add(new RowDefinition(1, GridUnitType.Auto));

        Board board = new Board();

        board.HorizontalAlignment = HorizontalAlignment.Stretch;
        board.VerticalAlignment = VerticalAlignment.Stretch;

        Grid.SetRow(board, 0);
        maingrid.Children.Add(board);

        // TextBlock text = new TextBlock();
        // text.Text = "Hi Mom !";

        // Grid.SetRow(text, 1);
        // maingrid.Children.Add(text);

        this.Content = maingrid;

        // this.SizeChanged += (sender, e) =>
        // {
        //     double availableHeight = maingrid.Bounds.Height - text.Bounds.Height;
        //     double availableWidth = maingrid.Bounds.Width;
        //     double boardSize = Math.Min(availableWidth, availableHeight);
        //     board.Resize(boardSize);
        // };
    }

    // public void OnDimensionsChange(object? sender, EffectiveViewportChangedEventArgs e) { }
}
