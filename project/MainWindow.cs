using Avalonia.Controls;
using Avalonia.Layout;
using ChessLogic;

namespace Chess;

public class MainWindow : Window
{
    TextBox fenInputBox;
    Board board;

    public MainWindow()
    {
        this.Title = "Chess";

        this.MinWidth = 200;
        this.MinHeight = 200;

        Grid maingrid = new Grid();
        maingrid.HorizontalAlignment = HorizontalAlignment.Stretch;
        maingrid.VerticalAlignment = VerticalAlignment.Stretch;

        maingrid.RowDefinitions.Add(new RowDefinition(0.9, GridUnitType.Star));
        maingrid.RowDefinitions.Add(new RowDefinition(1, GridUnitType.Auto));
        maingrid.RowDefinitions.Add(new RowDefinition(0.1, GridUnitType.Star));

        Board board = new Board();

        board.HorizontalAlignment = HorizontalAlignment.Stretch;
        board.VerticalAlignment = VerticalAlignment.Stretch;

        this.board = board;

        Grid.SetRow(board, 0);
        maingrid.Children.Add(board);

        TextBlock text1 = new TextBlock();
        text1.Text = "Enter FEN:";
        Grid.SetRow(text1, 1);

        TextBox text2 = new TextBox();
        // text2.Text = Board.default_fen;
        text2.Text = "r3k2r/Pppp1ppp/1b3nbN/nP6/BBP1P3/q4N2/Pp1P2PP/R2Q1RK1 w kq - 0 1";
        text2.TextChanged += OnTextChanged;
        this.fenInputBox = text2;

        Grid.SetRow(text2, 2);
        maingrid.Children.Add(text2);

        this.Content = maingrid;
    }

    public void OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if(this.fenInputBox.Text.Trim() == Board.default_fen)
            return;

        Console.WriteLine($"input changed to - {this.fenInputBox.Text}");

        Position? res = ChessLogic.FEN.PositionFromFEN(this.fenInputBox.Text.Trim());

        if (res is null)
        {
            Console.WriteLine($"incorrect fen");
            return;
        }

        Console.WriteLine($"changed position to \n{res}");

        this.board.ChangePosition(res);
    }
}
