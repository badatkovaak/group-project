// using Avalonia;
using ChessLogic;

namespace Chess;

class Program
{
    // [STAThread]
    // public static void Main(string[] args) =>
    //     BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    //
    // public static AppBuilder BuildAvaloniaApp() =>
    //     AppBuilder.Configure<App>().UsePlatformDetect().WithInterFont().LogToTrace();

    public static void Main(string[] args)
    {
        string fen1 = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        // string fen2 = "rnbqkbnr/pp1p1ppp/8/2pPp3/8/8/PPP1PPPP/RNBQKBNR w KQkq c6 0 3";
        // string fen3 = "rnbqkbnr/pppppppp/8/8/8/5N2/PPPPPPPP/RNBQKB1R b KQkq - 1 1";
        // string fen4 = "rnbqkbnr/pp1ppppp/2p5/8/8/5N2/PPPPPPPP/RNBQKB1R w KQkq - 0 2";
        string fen5 = "r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1";
        // string fen6 = "r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/1R2K2R b Kkq - 1 1";
        // string fen7 = "1r2k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/1R2K2R w Kk - 2 2";
        string fen8 = "r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/2KR3R b kq - 1 1";
        string fen9 = "8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - 0 1";
        string fen10 = "r3k2r/Pppp1ppp/1b3nbN/nP6/BBP1P3/q4N2/Pp1P2PP/R2Q1RK1 w kq - 0 1";
        string fen11 = "r3k2r/Pppp1ppp/1b3nbN/nPP5/BB2P3/q4N2/Pp1P2PP/R2Q1RK1 b kq - 0 1";
        string fen12 = "rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8";
        string fen13 = "2kr1b1r/pb1n1p2/1q2pP2/2pP2B1/1pp5/2N3P1/PP3PBP/R2Q1RK1 w - - 0 16";

        Position? p = FEN.PositionFromFEN(fen13);

        Console.WriteLine(p);

        if (p is null)
            return;

        // List<Move>? ms = p.MakeAMove(Square.NewUnchecked("e1"), Square.NewUnchecked("c1"));
        //
        // if (ms is null)
        //     return;
        //
        // Console.WriteLine(p);

        Console.WriteLine(p.CountPossibleMoves(4));

        // List<Square> moves = p.GetLegalMoves(Square.NewUnchecked("e1"));
        // foreach (Square move in moves)
        //     Console.WriteLine(move);
    }
}
