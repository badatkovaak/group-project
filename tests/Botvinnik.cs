using ChessLogic;

namespace tests;

public class Tests
{
    Position position1;

    [SetUp]
    public void Setup()
    {
        string fen = "2kr1b1r/pb1n1p2/1q2pP2/2pP2B1/1pp5/2N3P1/PP3PBP/R2Q1RK1 w - - 0 16";
        string fen2 = "2kr1b1r/pb1n1p2/1q2pP2/2pP2B1/1pp5/2N3P1/PP3PBP/R2Q1RK1 w - - 0 16";
        Position? pos = FEN.PositionFromFEN(fen);

        if (pos is null)
            throw new Exception();

        this.position1 = pos;
    }

    [Test]
    public void Test1()
    {
        Square start = Square.New("b");
        Square[] correctLegalMoves = new { Square.NewUnchecked("") };

        List<Square> moves = this.position1.GetLegalMoves(start);
    }
}
