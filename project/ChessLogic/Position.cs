namespace ChessLogic;

public class Position
{
    Piece?[,] board;
    public int moves;
    int halfmovesFromPawnMoveOrCapture;
    public Color colorToMove;
    public (bool, bool, bool, bool) castling;
    public Square? enPassant;
    public PieceType promoteTo = PieceType.Queen;

    private Position(
        Piece?[,] board,
        int moves,
        int halfmoves,
        Color c,
        (bool, bool, bool, bool) castling,
        Square? enPassant
    )
    {
        if (board.Rank != 2 || board.GetLength(0) != 8 || board.GetLength(1) != 8)
            throw new ArgumentException();

        this.board = board;
        this.moves = moves;
        this.halfmovesFromPawnMoveOrCapture = halfmoves;
        this.colorToMove = c;
        this.castling = castling;
        this.enPassant = enPassant;
    }

    public static Position? New(
        Piece?[,] board,
        int moves,
        int halfmoves,
        Color c,
        (bool, bool, bool, bool) castling,
        Square? enPassant
    )
    {
        if (board.Rank != 2 || board.GetLength(0) != 8 || board.GetLength(1) != 8)
            return null;

        return new Position(board, moves, halfmoves, c, castling, enPassant);
    }

    public Piece? this[Square s]
    {
        get => this.board[s.X, s.Y];
        set => this.board[s.X, s.Y] = value;
    }

    public Piece? this[int i, int j]
    {
        get => this.board[i, j];
        set => this.board[i, j] = value;
    }

    public List<Move>? MakeAMove(Square start, Square end, bool goDeeper = true)
    {
        List<Square> l = GetLegalMoves(start, goDeeper);

        if (!l.Contains(end))
            return null;

        List<Move> result = new List<Move>();

        if (this.colorToMove == Color.Black)
            this.moves++;

        this.colorToMove = ColorFuncs.Opposite(this.colorToMove);

        Piece? p = this[start];

        if (end.Equals(this.enPassant) && p is not null && p.type == PieceType.Pawn)
        {
            int diff = 1;

            if (p.color == Color.White)
                diff = -1;

            this[end] = this[start];
            this[start] = null;
            this[end.X, end.Y + diff] = null;

            this.enPassant = null;

            result.Add(new Move(start, end));
            // result.Add(new Move(start, null));
            result.Add(new Move(Square.NewUnchecked(end.X, end.Y + diff), null));

            return result;
        }

        this.enPassant = null;

        if (p is not null && p.type == PieceType.Pawn)
        {
            this.halfmovesFromPawnMoveOrCapture = 0;

            if (Math.Abs(start.Y - end.Y) == 2)
            {
                this.enPassant = Square.NewUnchecked(start.X, start.Y + (end.Y - start.Y) / 2);
            }

            int max = 7;

            if (p.color == Color.Black)
                max = 0;

            if (end.Y == max)
            {
                this[end] = new Piece(this.promoteTo, p.color);
                this[start] = null;

                result.Add(new Move(null, end));
                result.Add(new Move(start, null));

                return result;
            }
        }

        Piece? target = this[end];

        if (target is not null)
            this.halfmovesFromPawnMoveOrCapture = 0;
        else
            this.halfmovesFromPawnMoveOrCapture++;

        if (p is not null && p.type == PieceType.King)
        {
            if (p.color == Color.White)
            {
                if (castling.Item1 || castling.Item2)
                {
                    castling.Item1 = false;
                    castling.Item2 = false;
                }
            }
            else
            {
                if (castling.Item3 || castling.Item4)
                {
                    castling.Item3 = false;
                    castling.Item4 = false;
                }
            }

            if (Math.Abs(end.X - start.X) >= 2)
            {
                Square rook_start;
                Square rook_end;

                if (Math.Abs(end.X - start.X) == 2)
                {
                    rook_start = Square.NewUnchecked(7, start.Y);
                    rook_end = Square.NewUnchecked(5, start.Y);
                }
                else
                {
                    rook_start = Square.NewUnchecked(0, start.Y);
                    rook_end = Square.NewUnchecked(3, start.Y);
                }

                this[rook_end] = this[rook_start];
                this[rook_start] = null;
                this[end] = this[start];
                this[start] = null;

                result.Add(new Move(rook_start, rook_end));
                // result.Add(new Move(rook_start, null));
                result.Add(new Move(start, end));
                // result.Add(new Move(start, null));

                return result;
            }
        }

        Square s1 = Square.NewUnchecked(0, 0);
        if (end == s1 || start == s1)
            this.castling.Item1 = false;

        s1 = Square.NewUnchecked(7, 0);
        if (end == s1 || start == s1)
            this.castling.Item2 = false;

        s1 = Square.NewUnchecked(0, 7);
        if (end == s1 || start == s1)
            this.castling.Item1 = false;

        s1 = Square.NewUnchecked(7, 7);
        if (end == s1 || start == s1)
            this.castling.Item2 = false;

        this[end] = this[start];
        this[start] = null;

        result.Add(new Move(start, end));
        // result.Add(new Move(start, null));

        return result;
    }

    public List<Square> GetLegalMoves(Square c, bool goDeeper = true)
    {
        // Console.WriteLine($"GetLegalMoves called with args - {c}, {goDeeper}");

        Piece? p = this[c];

        if (p is null)
            return new List<Square>();

        if (p.color != this.colorToMove)
            return new List<Square>();

        List<Square> moves = p.type switch
        {
            PieceType.King => this.GetKingLegalMoves(c),
            PieceType.Queen => this.GetQueenLegalMoves(c),
            PieceType.Rook => this.GetRookLegalMoves(c),
            PieceType.Bishop => this.GetBishopLegalMoves(c),
            PieceType.Knight => this.GetKnightLegalMoves(c),
            PieceType.Pawn => this.GetPawnLegalMoves(c),
            _ => throw new Exception(),
        };

        if (!goDeeper)
        {
            // Console.WriteLine($"Shallow legal moves from {c}");
            //
            // foreach (Square move in moves)
            //     Console.Write($"{move} ");
            //
            // Console.WriteLine();

            return moves;
        }

        // Console.WriteLine("moves ");
        //
        // foreach (Square move in moves)
        //     Console.Write($"{move} ");
        //
        // Console.WriteLine();

        List<Square> result = new List<Square>();

        foreach (Square move in moves)
        {
            Position clone = this.CreateACopy();

            List<Move>? res = clone.MakeAMove(c, move, false);

            // Console.WriteLine($"made a move {c} to {move}");

            if (res is null)
                throw new Exception();

            // Console.WriteLine("----------------");
            // Console.WriteLine(clone);
            // Console.WriteLine("----------------");

            Square? kingSquare = null;

            for (int i = 0; i < 8; i++)
            for (int j = 0; j < 8; j++)
            {
                Piece? k = clone[i, j];

                if (k is null || k.type != PieceType.King || k.color != this.colorToMove)
                    continue;

                kingSquare = Square.NewUnchecked(i, j);
                break;
            }

            if (kingSquare is null)
                throw new Exception();

            // Console.WriteLine($"kings position is {kingSquare}");

            bool canCaptureKing = false;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Square s = Square.NewUnchecked(i, j);

                    Piece? opponentPiece = clone[s];

                    if (opponentPiece is null || opponentPiece.color != clone.colorToMove)
                        continue;

                    List<Square> opponentMoves = clone.GetLegalMoves(s, false);

                    // Console.WriteLine($"checking moves from {s} - {moves1.Contains(kingSquare)}");

                    if (opponentMoves.Contains(kingSquare))
                    {
                        canCaptureKing = true;
                        break;
                    }
                }

                if (canCaptureKing)
                    break;
            }

            if (!canCaptureKing)
                result.Add(move);
        }

        Console.WriteLine($"Deep legal moves from {c}: ");

        foreach (Square move in moves)
            Console.Write($"{move} ");

        Console.WriteLine();

        return result;
    }

    private List<Square> GetKingLegalMoves(Square c)
    {
        Piece? king = this[c];

        if (king is null)
            return new List<Square>();

        List<Square> moves = GetTypesPossibleMoves(c, PieceType.King);
        List<Square> result = new List<Square>();

        foreach (Square move in moves)
        {
            Piece? p = this[move];

            if (p is null)
            {
                result.Add(move);
                continue;
            }

            if (p.color != king.color)
                result.Add(move);
        }

        bool canCastleQueenSide;
        bool canCastleKingSide;
        (canCastleKingSide, canCastleQueenSide) = this.GetCastling(king.color);

        if (canCastleKingSide)
        {
            bool hasObstacles = false;

            for (int i = 0; i < 2; i++)
            {
                Square s = Square.NewUnchecked(c.X + i + 1, c.Y);
                Piece? p = this[s];

                if (p is null)
                    continue;

                hasObstacles = true;
            }

            if (!hasObstacles)
                result.Add(Square.NewUnchecked(c.X + 2, c.Y));
        }

        if (canCastleQueenSide)
        {
            bool hasObstacles = false;

            for (int i = 0; i < 3; i++)
            {
                Square s = Square.NewUnchecked(c.X + i + 1, c.Y);
                Piece? p = this[s];

                if (p is null)
                    continue;

                hasObstacles = true;
            }

            if (!hasObstacles)
                result.Add(Square.NewUnchecked(c.X + 2, c.Y));
        }

        return result;
    }

    private List<Square> GetQueenLegalMoves(Square c)
    {
        List<Square> l = this.GetRookLegalMoves(c);
        return new List<Square>(l.Concat(this.GetBishopLegalMoves(c)));
    }

    private List<Square> GetRookLegalMoves(Square c)
    {
        Piece? rook = this[c];

        if (rook is null)
            return new List<Square>();

        List<Square> moves = GetTypesPossibleMoves(c, PieceType.Rook);
        List<Square> res = new List<Square>();

        foreach (Square move in moves)
        {
            int x = c.X;
            int y = c.Y;
            int step_x;
            int step_y;

            if (c.X == move.X)
            {
                step_x = 0;
                step_y = (move.Y - c.Y) / Math.Abs(move.Y - c.Y);
            }
            else
            {
                step_x = (move.X - c.X) / Math.Abs(move.X - c.X);
                step_y = 0;
            }

            bool isPossible = true;

            for (int i = 1; Math.Abs(x - move.X) + Math.Abs(y - move.Y) > 0; i++)
            {
                x += step_x;
                y += step_y;
                Piece? p = this[x, y];

                if (p is null)
                    continue;

                if (p.color == rook.color)
                {
                    isPossible = false;
                    break;
                }

                if (Math.Abs(x - move.X) + Math.Abs(y - move.Y) != 0)
                {
                    isPossible = false;
                    break;
                }
            }

            if (isPossible)
                res.Add(move);
        }

        return res;
    }

    private List<Square> GetBishopLegalMoves(Square c)
    {
        Piece? bishop = this[c];

        if (bishop is null)
            return new List<Square>();

        List<Square> moves = GetTypesPossibleMoves(c, PieceType.Bishop);
        List<Square> res = new List<Square>();

        foreach (Square move in moves)
        {
            int step_x = (move.X - c.X) / Math.Abs(move.X - c.X);
            int step_y = (move.Y - c.Y) / Math.Abs(move.Y - c.Y);
            bool isPossible = true;

            for (int i = c.X + step_x; (move.X - i) * step_x >= 0; i += step_x)
            {
                Piece? p = this[i, c.Y + Math.Abs(i - c.X) * step_y];

                if (p is null)
                    continue;

                if (p.color == bishop.color)
                {
                    isPossible = false;
                    break;
                }

                if ((move.X - i) * step_x > 0)
                {
                    isPossible = false;
                    break;
                }
            }

            if (isPossible)
                res.Add(move);
        }

        return res;
    }

    private List<Square> GetKnightLegalMoves(Square c)
    {
        Piece? knight = this[c];

        if (knight is null)
            return new List<Square>();

        List<Square> moves = Position.GetTypesPossibleMoves(c, knight.type);
        List<Square> res = new List<Square>();

        foreach (Square move in moves)
        {
            Piece? p = this[move];

            if (p is null || p.color != knight.color)
            {
                res.Add(move);
                continue;
            }
        }

        return res;
    }

    private List<Square> GetPawnLegalMoves(Square c)
    {
        Piece? pawn = this[c];
        if (pawn is null)
            return new List<Square>();

        List<Square> moves = GetTypesPossibleMoves(c, PieceType.Pawn, pawn.color);
        List<Square> res = new List<Square>();

        foreach (Square move in moves)
        {
            Piece? p = this[move];

            if (move.X == c.X && p is null)
                res.Add(move);

            if (move.X != c.X && p is not null && p.color != pawn.color)
                res.Add(move);
        }

        if (this.enPassant is null)
            return res;

        if (Math.Abs(this.enPassant.X - c.X) == 1 && Math.Abs(this.enPassant.Y - c.Y) == 1)
        {
            if (this.enPassant.Y == 2 && pawn.color == Color.Black)
                res.Add(this.enPassant);
            else if (this.enPassant.Y == 5 && pawn.color == Color.White)
                res.Add(this.enPassant);
        }

        return res;
    }

    public static List<Square> GetTypesPossibleMoves(
        Square c,
        PieceType t,
        Color color = Color.White
    )
    {
        return t switch
        {
            PieceType.King => GetPossibleKingMoves(c),
            PieceType.Queen => GetPossibleQueenMoves(c),
            PieceType.Rook => GetPossibleRookMoves(c),
            PieceType.Bishop => GetPossibleBishopMoves(c),
            PieceType.Knight => GetPossibleKnightMoves(c),
            PieceType.Pawn => GetPossiblePawnMoves(c, color),
            _ => throw new Exception(),
        };
    }

    private static List<Square> GetPossibleKingMoves(Square c)
    {
        List<Square> res = new List<Square>();
        for (int i = 0; i <= 2; i++)
        for (int j = 0; j <= 2; j++)
        {
            if (i == 1 && j == 1)
                continue;

            int new_x = c.X + i - 1;
            int new_y = c.Y + j - 1;

            if (AreCorrectCoords(new_x, new_y))
                res.Add(Square.NewUnchecked(new_x, new_y));
        }

        return res;
    }

    private static List<Square> GetPossibleBishopMoves(Square c)
    {
        List<Square> res = new List<Square>();
        for (int i = 0; i <= 8; i++)
        for (int j = 0; j < 2; j++)
        {
            if (i == c.X)
                continue;

            int new_x = i;
            int new_y = c.Y + (c.X - i) * (1 - 2 * (j % 2));

            if (AreCorrectCoords(new_x, new_y))
                res.Add(Square.NewUnchecked(new_x, new_y));
        }

        return res;
    }

    private static List<Square> GetPossibleKnightMoves(Square c)
    {
        List<Square> res = new List<Square>();
        for (int i = 0; i < 8; i++)
        for (int j = 0; j < 8; j++)
        {
            int x_diff = Math.Abs(c.X - i);
            int y_diff = Math.Abs(c.Y - j);

            if (Math.Abs(x_diff - y_diff) == 1 && Math.Min(x_diff, y_diff) == 1)
                res.Add(Square.NewUnchecked(i, j));
        }

        return res;
    }

    private static List<Square> GetPossibleRookMoves(Square c)
    {
        List<Square> res = new List<Square>();
        for (int i = 0; i < 8; i++)
        for (int j = 0; j < 8; j++)
            if ((i == c.X || j == c.Y) && !(i == c.X && j == c.Y))
                res.Add(Square.NewUnchecked(i, j));
        return res;
    }

    private static List<Square> GetPossibleQueenMoves(Square c)
    {
        List<Square> res = GetPossibleBishopMoves(c);
        return new List<Square>(res.Concat(GetPossibleRookMoves(c)));
    }

    private static List<Square> GetPossiblePawnMoves(Square c, Color color)
    {
        List<Square> res = new List<Square>();
        int step = 1;
        int rankThatCanGoDouble = 1;

        if (color == Color.Black)
        {
            step = -1;
            rankThatCanGoDouble = 6;
        }

        for (int i = 0; i < 3; i++)
        {
            int x = c.X + i - 1;
            int y = c.Y + step;

            if (AreCorrectCoords(x, y))
                res.Add(Square.NewUnchecked(x, y));
        }

        if (c.Y == rankThatCanGoDouble)
            res.Add(Square.NewUnchecked(c.X, c.Y + 2 * step));

        return res;
    }

    private static bool AreCorrectCoords(int i, int j)
    {
        return !(i >= 8 || i < 0 || j >= 8 || j < 0);
    }

    private (bool, bool) GetCastling(Color c)
    {
        if (c == Color.White)
            return (this.castling.Item1, this.castling.Item2);
        else
            return (this.castling.Item3, this.castling.Item4);
    }

    public Piece? GetPieceOnSquare(Square c)
    {
        return this[c];
    }

    public Position CreateACopy()
    {
        Piece?[,] b = new Piece?[8, 8];

        for (int i = 0; i < 8; i++)
        for (int j = 0; j < 8; j++)
            b[i, j] = this[i, j];

        int halfmoves = this.halfmovesFromPawnMoveOrCapture;
        Color c = this.colorToMove;
        var castle = this.castling;
        Square? s = this.enPassant;
        Position? p = Position.New(b, this.moves, halfmoves, c, castle, s);

        if (p is null)
            throw new Exception();

        return (Position)p;
    }

    public override string ToString()
    {
        string res = "";
        for (int j = 7; j >= 0; j--)
        {
            for (int i = 0; i < 8; i++)
            {
                Piece? p = this[i, j];
                if (p is null)
                    res += "- ";
                else
                    res += p.ToString() + " ";
            }

            res += "\n";
        }

        (bool, bool, bool, bool) c = this.castling;
        res += $"Move - {this.moves}, Color To Move - {this.colorToMove}\n";
        res += $"Castling - {c.Item1}, {c.Item2}, {c.Item3}, {c.Item4}\n";
        return res;
    }

    public int CountPossibleMoves(int depth)
    {
        List<MoveCertain> possibleMoves = new List<MoveCertain>();

        for (int i = 0; i < 8; i++)
        for (int j = 0; j < 8; j++)
        {
            Square start = Square.NewUnchecked(i, j);

            List<Square> moves = this.GetLegalMoves(start);

            foreach (Square end in moves)
            {
                MoveCertain move = new MoveCertain(start, end);

                possibleMoves.Add(move);
            }
        }

        if (depth <= 1)
            return possibleMoves.Count;

        int result = 0;

        foreach (MoveCertain move in possibleMoves)
        {
            Position newpos = this.CreateACopy();

            List<Move>? m = newpos.MakeAMove(move.start, move.end);

            if (m is null)
                throw new Exception();

            result += newpos.CountPossibleMoves(depth - 1);
        }

        return result;
    }
}

public struct Move
{
    public readonly Square? start;
    public readonly Square? end;

    public Move(Square? s, Square? e)
    {
        start = s;
        end = e;
    }
}
