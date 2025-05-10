using ChessLogic;

namespace tests;

public class Tests
{
    Position position1;
    Position position2;
    Position position3;
    Position position4;
    Position position5;
    Position position6;
    Position position7;
    Position position8;
    Position position9;
    Position position10;
    Position position11;

    [SetUp]
    public void Setup()
    {
        string fen1 = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        Position? p1 = FEN.PositionFromFEN(fen1);

        if (p1 is null)
            throw new Exception();

        this.position1 = p1;

        string fen2 = "r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1";
        Position? p2 = FEN.PositionFromFEN(fen2);

        if (p2 is null)
            throw new Exception();

        this.position2 = p2;

        string fen3 = "8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - 0 1";
        Position? p3 = FEN.PositionFromFEN(fen3);

        if (p3 is null)
            throw new Exception();

        this.position3 = p3;

        string fen4 = "r3k2r/Pppp1ppp/1b3nbN/nP6/BBP1P3/q4N2/Pp1P2PP/R2Q1RK1 w kq - 0 1";
        Position? p4 = FEN.PositionFromFEN(fen4);

        if (p4 is null)
            throw new Exception();

        this.position4 = p4;

        string fen5 = "rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8";
        Position? p5 = FEN.PositionFromFEN(fen5);

        if (p5 is null)
            throw new Exception();

        this.position5 = p5;

        string fen6 = "r4rk1/1pp1qppp/p1np1n2/2b1p1B1/2B1P1b1/P1NP1N2/1PP1QPPP/R4RK1 w - - 0 10";
        Position? p6 = FEN.PositionFromFEN(fen6);

        if (p6 is null)
            throw new Exception();

        this.position6 = p6;

        // string fen7 = "2kr1b1r/pb1n1p2/1q2pP2/2pP2B1/1pp5/2N3P1/PP3PBP/R2Q1RK1 w - - 0 16";
        // Position? p7 = FEN.PositionFromFEN(fen7);
        // // 1 - 42, 2 - 1378, 3 - 57321
        //
        // if (p7 is null)
        //     throw new Exception();
        //
        // this.position7 = p7;
        //
        // string fen8 = "r1bqkbnr/1ppp1ppp/p1n5/4p3/B3P3/5N2/PPPP1PPP/RNBQK2R b KQkq - 1 4";
        // Position? p8 = FEN.PositionFromFEN(fen8);
        // // 1 - 32, 2 - 864, 3 - 26625
        //
        // if (p8 is null)
        //     throw new Exception();
        //
        // this.position8 = p8;
        //
        // string fen9 = "8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - 0 1 ";
        // Position? p9 = FEN.PositionFromFEN(fen9);
        //
        // if (p9 is null)
        //     throw new Exception();
        //
        // this.position9 = p9;
        //
        // string fen10 = "r3k2r/Pppp1ppp/1b3nbN/nP6/BBP1P3/q4N2/Pp1P2PP/R2Q1RK1 w kq - 0 1";
        // Position? p10 = FEN.PositionFromFEN(fen10);
        //
        // if (p10 is null)
        //     throw new Exception();
        //
        // this.position10 = p10;
        //
        // string fen11 = "r3k2r/Pppp1ppp/1b3nbN/nPP5/BB2P3/q4N2/Pp1P2PP/R2Q1RK1 b kq - 0 1";
        // Position? p11 = FEN.PositionFromFEN(fen11);
        //
        // if (p11 is null)
        //     throw new Exception();
        //
        // this.position11 = p11;
    }

    [Test]
    public void Position1_Test1()
    {
        int res = this.position1.CountPossibleMoves(1);

        Assert.That(res, Is.EqualTo(20));
    }

    [Test]
    public void Position1_Test2()
    {
        int res = this.position1.CountPossibleMoves(2);

        Assert.That(res, Is.EqualTo(400));
    }

    [Test]
    public void Position1_Test3()
    {
        int res = this.position1.CountPossibleMoves(3);

        Assert.That(res, Is.EqualTo(8902));
    }

    [Test]
    public void Position1_Test4()
    {
        int res = this.position1.CountPossibleMoves(4);

        Assert.That(res, Is.EqualTo(197281));
    }

    [Test]
    public void Position2_Test1()
    {
        int res = this.position2.CountPossibleMoves(1);

        Assert.That(res, Is.EqualTo(48));
    }

    [Test]
    public void Position2_Test2()
    {
        int res = this.position2.CountPossibleMoves(2);

        Assert.That(res, Is.EqualTo(2039));
    }

    [Test]
    public void Position2_Test3()
    {
        int res = this.position2.CountPossibleMoves(3);

        Assert.That(res, Is.EqualTo(97862));
    }

    [Test]
    public void Position3_Test1()
    {
        int res = this.position3.CountPossibleMoves(1);

        Assert.That(res, Is.EqualTo(14));
    }

    [Test]
    public void Position3_Test2()
    {
        int res = this.position3.CountPossibleMoves(2);

        Assert.That(res, Is.EqualTo(191));
    }

    [Test]
    public void Position3_Test3()
    {
        int res = this.position3.CountPossibleMoves(3);

        Assert.That(res, Is.EqualTo(2812));
    }

    [Test]
    public void Position3_Test4()
    {
        int res = this.position3.CountPossibleMoves(4);

        Assert.That(res, Is.EqualTo(43238));
    }

    [Test]
    public void Position3_Test5()
    {
        int res = this.position3.CountPossibleMoves(5);

        Assert.That(res, Is.EqualTo(674624));
    }

    [Test]
    public void Position4_Test1()
    {
        int res = this.position4.CountPossibleMoves(1);

        Assert.That(res, Is.EqualTo(6));
    }

    [Test]
    public void Position4_Test2()
    {
        int res = this.position4.CountPossibleMoves(2);

        Assert.That(res, Is.EqualTo(264));
    }

    [Test]
    public void Position4_Test3()
    {
        int res = this.position4.CountPossibleMoves(3);

        Assert.That(res, Is.EqualTo(9467));
    }

    [Test]
    public void Position4_Test4()
    {
        int res = this.position4.CountPossibleMoves(4);

        Assert.That(res, Is.EqualTo(422333));
    }

    [Test]
    public void Position5_Test1()
    {
        int res = this.position5.CountPossibleMoves(1);

        Assert.That(res, Is.EqualTo(44));
    }

    [Test]
    public void Position5_Test2()
    {
        int res = this.position5.CountPossibleMoves(2);

        Assert.That(res, Is.EqualTo(1486));
    }

    [Test]
    public void Position5_Test3()
    {
        int res = this.position5.CountPossibleMoves(3);

        Assert.That(res, Is.EqualTo(62379));
    }

    [Test]
    public void Position6_Test1()
    {
        int res = this.position6.CountPossibleMoves(1);

        Assert.That(res, Is.EqualTo(46));
    }

    [Test]
    public void Position6_Test2()
    {
        int res = this.position6.CountPossibleMoves(2);

        Assert.That(res, Is.EqualTo(2079));
    }

    [Test]
    public void Position6_Test3()
    {
        int res = this.position6.CountPossibleMoves(3);

        Assert.That(res, Is.EqualTo(89890));
    }
}
