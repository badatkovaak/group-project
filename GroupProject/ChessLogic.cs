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

        foreach (Square s in l)
            Console.WriteLine(s);

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



        if (!goDeeper){
            Console.WriteLine($"Shallow legal moves from {c}");

            foreach(Square move in moves)
                Console.Write($"{move} ");

            Console.WriteLine();

            return moves;
        }

        List<Square> result = new List<Square>();

        foreach (Square move in moves)
        {
            Position clone = this.CreateACopy();

            List<Move>? res = clone.MakeAMove(c, move, false);

            if (res is null)
                throw new Exception();

            Console.WriteLine(clone);

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

            Console.WriteLine($"kings position is {kingSquare}");

            bool canCaptureKing = false;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Square s = Square.NewUnchecked(i, j);

                    Piece? p1 = clone[s];

                    if (p is null || p.color != clone.colorToMove)
                        continue;

                    List<Square> moves1 = clone.GetLegalMoves(s, false);

                    Console.WriteLine($"checking moves from {s} - {moves.Contains(kingSquare)}");

                    if (moves.Contains(kingSquare))
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

        // Console.WriteLine(this);
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

public class MoveCertain
{
    public Square start;
    public Square end;

    public MoveCertain(Square start, Square end)
    {
        this.start = start;
        this.end = end;
    }

    public static MoveCertain? StringToMove(Position position, string input)
    {
        if (input[0] == '0')
        {
            if (input.Length != 3 && input.Length != 5)
                return null;

            Color c = position.colorToMove;

            int start_y;

            if (c == Color.White)
                start_y = 0;
            else
                start_y = 7;

            int end_x;

            if (input.Length == 3)
                end_x = 6;
            else
                end_x = 2;

            Square start1 = Square.NewUnchecked(4, start_y);
            Square end1 = Square.NewUnchecked(end_x, start_y);

            List<Square> moves = position.GetLegalMoves(start1);

            if (!moves.Contains(end1))
                return null;

            return new MoveCertain(start1, end1);
        }

        PieceType? type = null;
        PieceType? promoteTo = null;

        if (input.Length == 2)
            type = PieceType.Pawn;
        else if (input.Contains('x') && input[0] >= 'a' && input[0] <= 'h')
            type = PieceType.Pawn;
        else if (input.Contains('='))
        {
            type = PieceType.Pawn;
            promoteTo = Piece.FromChar(input[input.Length - 1])?.type;
        }
        else
            type = Piece.FromChar(input[0])?.type;

        if (type is null)
            return null;

        if (promoteTo is not null)
        {
            Square? end1 = Square.New(input.Substring(0, 2));

            if (end1 is null)
                return null;

            int y = 0;
            if (position.colorToMove == Color.White)
                y = 7;

            if (end1.Y != y)
                return null;

            int diff = 1;
            if (position.colorToMove == Color.White)
                diff = -1;

            Square start1 = Square.NewUnchecked(end1.X, end1.Y + diff);

            List<Square> moves = position.GetLegalMoves(start1);

            if (!moves.Contains(end1))
                return null;

            return new MoveCertain(start1, end1);
        }

        Square? end = Square.New(input.Substring(input.Length - 2));

        if (end is null)
            return null;

        Console.WriteLine($"StringToMove\n{position}, end - {end}, type - {type}");

        // Console.WriteLine($"{type} {end} {pos.enPassant}");

        if (input.Contains('x'))
        {
            Piece? p = position[end];

            // Console.WriteLine($"p is - {p}");

            if (p is null)
            {
                if (position.enPassant is null)
                    return null;

                if (position.enPassant != end)
                    return null;
            }
            else if (p.color == position.colorToMove)
                return null;
        }

        // Console.WriteLine("here1");

        Square? start = null;

        for (int i = 0; i < 8; i++)
        for (int j = 0; j < 8; j++)
        {
            Piece? piece = position[Square.NewUnchecked(i, j)];

            if (piece is null)
                continue;

            if (piece.color != position.colorToMove || piece.type != type)
                continue;

            Console.WriteLine($"{Square.NewUnchecked(i, j)} - {piece}, {i}, {j}");

            List<Square> moves = position.GetLegalMoves(Square.NewUnchecked(i, j));

            foreach (Square move in moves)
                Console.WriteLine(move);

            if (!moves.Contains(end))
                continue;

            Console.WriteLine($"Contains {Square.NewUnchecked(i, j)} - {piece}, {start}");

            if (start is null)
            {
                start = Square.NewUnchecked(i, j);
                continue;
            }

            Console.WriteLine("here1111111111111111");

            if (input.Length != 4)
                return null;

            char c;

            if (type == PieceType.Pawn)
                c = input[0];
            else
                c = input[1];

            if (i == start.X && j == start.Y)
                return null;

            if (i == start.X)
            {
                int row = (int)c - (int)'1';

                if (j == row)
                    start = Square.NewUnchecked(i, j);

                break;
            }
            else
            {
                int file = (int)c - (int)'a';

                if (i == file)
                    start = Square.NewUnchecked(i, j);

                break;
            }
        }

        if (start is null)
            return null;

        return new MoveCertain(start, end);
    }
}

public class Piece
{
    public readonly PieceType type;
    public readonly Color color;

    public Piece(PieceType t, Color c)
    {
        this.type = t;
        this.color = c;
    }

    public override string ToString()
    {
        char res = this.type switch
        {
            PieceType.King => 'k',
            PieceType.Queen => 'q',
            PieceType.Rook => 'r',
            PieceType.Bishop => 'b',
            PieceType.Knight => 'n',
            PieceType.Pawn => 'p',
            _ => 'e',
        };

        if (this.color == Color.White)
            res = (char)((int)res & 0b11011111);

        return $"{res}";
    }

    public static Piece? FromChar(char c)
    {
        Color color = Color.White;

        if (((int)c & 32) > 0)
            color = Color.Black;

        c = (char)((int)c | 0b00100000);
        PieceType? t = c switch
        {
            'k' => PieceType.King,
            'q' => PieceType.Queen,
            'r' => PieceType.Rook,
            'b' => PieceType.Bishop,
            'n' => PieceType.Knight,
            'p' => PieceType.Pawn,
            _ => null,
        };

        if (t is null)
            return null;

        return new Piece((PieceType)t, color);
    }
}

public class Square : IEquatable<Square>
{
    (int, int) coordinates;

    public int X
    {
        get => this.coordinates.Item1;
    }
    public int Y
    {
        get => this.coordinates.Item2;
    }

    private Square(int i, int j)
    {
        this.coordinates = (i, j);
    }

    private Square((int, int) coords)
    {
        this.coordinates = coords;
    }

    public static Square NewUnchecked(int i, int j)
    {
        if (i < 0 || i > 7 || j < 0 || j > 7)
            throw new ArgumentException();

        return new Square(i, j);
    }

    public static Square? New(int i, int j)
    {
        if (i < 0 || i > 7 || j < 0 || j > 7)
            return null;

        return new Square(i, j);
    }

    public static Square? New((int, int) coords)
    {
        return Square.New(coords.Item1, coords.Item2);
    }

    public static Square? New(string input)
    {
        (int, int)? c = ConvertStringToCoordinates(input);

        if (c is null)
            return null;

        return Square.New(((int, int))c);
    }

    public static (int, int)? ConvertStringToCoordinates(string input)
    {
        if (input.Length < 2)
            return null;

        bool first_char_condition =
            (input[0] > 'h' || input[0] < 'a') && (input[0] < 'A' || input[0] > 'H');

        if (first_char_condition)
            return null;

        if (input[1] < '1' || input[1] > '8')
            return null;

        int first = ((int)(input[0] - 'A')) & 0xdf;
        int second = (int)(input[1] - '1');

        return (first, second);
    }

    public bool Equals(Square? s)
    {
        if (s is null)
            return false;
        return (s.X == this.X) && (s.Y == this.Y);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;

        Square? s = obj as Square;

        if (s is null)
            return false;

        return this.Equals(s);
    }

    public static bool operator ==(Square s1, Square s2) => s1.Equals(s2);

    public static bool operator !=(Square s1, Square s2) => !s1.Equals(s2);

    public override int GetHashCode()
    {
        return Convert.ToInt32(Math.Pow(2, this.X) * Math.Pow(3, this.Y));
    }

    public override string ToString()
    {
        char first = (char)((int)('a') + this.coordinates.Item1);
        char second = (char)((int)('1') + this.coordinates.Item2);
        string res = "";
        res += first;
        res += second;
        return res;
    }
}

public enum PieceType
{
    King,
    Queen,
    Rook,
    Bishop,
    Knight,
    Pawn,
}

public enum Color
{
    White,
    Black,
}

public class ColorFuncs
{
    public static Color? FromChar(char c)
    {
        return (char)(((int)c) | 0b00100000) switch
        {
            'b' => Color.Black,
            'w' => Color.White,
            _ => null,
        };
    }

    public static Color Opposite(Color c)
    {
        return c switch
        {
            Color.Black => Color.White,
            Color.White => Color.Black,
            _ => throw new Exception(),
        };
    }
}

public class FEN
{
    public static Position? PositionFromFEN(string input)
    {
        string[] parts = input.Split(' ');

        if (parts.Length < 6)
            return null;

        Piece?[,]? board = GetBoardFromFEN(parts[0]);

        if (board is null)
            return null;

        Color? c = GetColorFromFEN(parts[1]);

        if (c is null)
            return null;

        (bool, bool, bool, bool) castling = GetCastlingFromFEN(parts[2]);
        Square? enPassant = Square.New(parts[3]);

        int halfmoves;
        int moves;

        try
        {
            halfmoves = Convert.ToInt32(parts[4]);
            moves = Convert.ToInt32(parts[5]);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }

        return Position.New(board, moves, halfmoves, (Color)c, castling, enPassant);
    }

    private static Piece?[,]? GetBoardFromFEN(string input)
    {
        string[] rows = input.Split('/');

        if (rows.Length != 8)
            return null;

        Piece?[,] pieces = new Piece?[8, 8];

        for (int i = 0; i < rows.Length; i++)
        {
            int j = 0;
            for (int k = 0; k < rows[i].Length; k++)
            {
                char c = rows[i][k];

                if (c >= '1' && c <= '8')
                    j += (int)c - (int)'0';
                else
                {
                    Piece? a = Piece.FromChar(c);

                    if (a is null)
                        return null;

                    pieces[j, 7 - i] = (Piece)a;
                    j += 1;
                }
            }

            if (j < 8)
                return null;
        }

        return pieces;
    }

    private static Color? GetColorFromFEN(string input)
    {
        return ColorFuncs.FromChar(input[0]);
    }

    private static (bool, bool, bool, bool) GetCastlingFromFEN(string input)
    {
        return (
            input.IndexOf('K') >= 0,
            input.IndexOf('Q') >= 0,
            input.IndexOf('k') >= 0,
            input.IndexOf('q') >= 0
        );
    }
}

// class Program
// {
// 	public static void Main(string[] args)
// 	{
// 		string fen1 = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
// 		string fen2 = "rnbqkbnr/pp1p1ppp/8/2pPp3/8/8/PPP1PPPP/RNBQKBNR w KQkq c6 0 3";


// 		Position? p = FEN.PositionFromFEN(fen2);
// 		Console.WriteLine(p);

// 		if (p is null)
// 			return;

// 		while (true)
// 		{
// 			Console.Write(">>>");
// 			string? input = Console.ReadLine();

// 			if (input is null)
// 				continue;

// 			string[] parts = input.Replace('\n', ' ').Trim().Split(' ');

// 			if (parts.Length != 2)
// 				continue;

// 			Square? s1 = Square.StringToSquare(parts[0]);
// 			Square? s2 = Square.StringToSquare(parts[1]);

// 			if (s1 is null || s2 is null)
// 				continue;

// 			Position? res = p.MakeAMove(s1, s2);
// 			Console.WriteLine(res);
// 		}
// 	}
// }
