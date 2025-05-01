using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace Backup;

public class MainWindow : Window
{
    public MainWindow()
    {
        this.Title = "Chess";
        Grid maingrid = new Grid();
        maingrid.HorizontalAlignment = HorizontalAlignment.Stretch;
        maingrid.VerticalAlignment = VerticalAlignment.Stretch;

        maingrid.RowDefinitions.Add(new RowDefinition(1, GridUnitType.Star));
        maingrid.RowDefinitions.Add(new RowDefinition(1, GridUnitType.Auto));

        Board board = new Board();
        board.HorizontalAlignment = HorizontalAlignment.Stretch;
        board.VerticalAlignment = VerticalAlignment.Stretch;
        board.Background = Brushes.White;

        Grid.SetRow(board, 0);
        maingrid.Children.Add(board);

        TextBlock text = new TextBlock();
        text.Text = "Hello There !";

        Grid.SetRow(text, 1);
        maingrid.Children.Add(text);

        this.Content = maingrid;
    }

    public void OnDimensionsChange(object? sender, EffectiveViewportChangedEventArgs e) { }
}
