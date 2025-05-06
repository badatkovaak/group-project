using Avalonia;

namespace Chess;

class Program
{
    [STAThread]
    public static void Main(string[] args) =>
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>().UsePlatformDetect().WithInterFont().LogToTrace();

    // public static void Main(string[] args)
    // {
    //     string fen1 = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
    //     string fen2 = "rnbqkbnr/pp1p1ppp/8/2pPp3/8/8/PPP1PPPP/RNBQKBNR w KQkq c6 0 3";
    //
    //     Position? p = FEN.PositionFromFEN(fen2);
    //     Console.WriteLine(p);
    //     if (p is null)
    //         return;
    //
    //     List<Square> moves = p.GetLegalMoves(Square.New("d5"));
    // }
}
