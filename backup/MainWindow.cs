using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace Backup;

public class MainWindow : Window
{
    public MainWindow()
    {
        this.Title = "Chess";

        this.Width = 600;
        this.Height = 600;
        this.MinWidth = 100;
        this.MinHeight = 100;

        Grid maingrid = new Grid();
        maingrid.HorizontalAlignment = HorizontalAlignment.Stretch;
        maingrid.VerticalAlignment = VerticalAlignment.Stretch;

        maingrid.RowDefinitions.Add(new RowDefinition(1, GridUnitType.Star));
        maingrid.RowDefinitions.Add(new RowDefinition(1, GridUnitType.Auto));

        Board board = new Board();

        board.HorizontalAlignment = HorizontalAlignment.Center;
        board.VerticalAlignment = VerticalAlignment.Center;
        board.Background = Brushes.White;

        Grid.SetRow(board, 0);
        maingrid.Children.Add(board);

        TextBlock text = new TextBlock();
        text.Text = "Hello There !";
        text.HorizontalAlignment = HorizontalAlignment.Center;
        text.Margin = new Thickness(0, 10, 0, 10);

        Grid.SetRow(text, 1);
        maingrid.Children.Add(text);

        this.Content = maingrid;

        this.SizeChanged += (sender, e) =>
        {
            double availableHeight = maingrid.Bounds.Height - text.Bounds.Height;
            double availableWidth = maingrid.Bounds.Width;
            double boardSize = Math.Min(availableWidth, availableHeight);
            board.Resize(boardSize);
        };
    }

    public void OnDimensionsChange(object? sender, EffectiveViewportChangedEventArgs e) { }
}
