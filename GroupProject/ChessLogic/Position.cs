namespace ChessInternals;

public class Position
{
    Piece?[,] board;
    int moves;
    int halfmovesFromPawnMoveOrCapture;
    Color colorToMove;
    (bool, bool, bool, bool) castling;
    Square? enPassant;

    public Position(
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

    public Position? MakeAMove(Move m)
    {
        return this.MakeAMove(m.start, m.end);
    }

    public Position? MakeAMove(Square start, Square end)
    {
        List<Square> l = GetLegalMoves(start);

        foreach (Square s in l)
            Console.WriteLine(s);

        if (!l.Contains(end))
            return null;

        this.board[end.X, end.Y] = this.board[start.X, start.Y];
        this.board[start.X, start.Y] = null;

        if (this.colorToMove == Color.Black)
            this.moves++;

        this.colorToMove = ColorFuncs.Opposite(this.colorToMove);

        return this;
    }

    public List<Square> GetLegalMoves(Square c)
    {
        Piece? p = GetPieceOnSquare(c);

        if (p is null)
            return new List<Square>();

        if (p.color != this.colorToMove)
            return new List<Square>();

        return p.type switch
        {
            PieceType.King => this.GetKingLegalMoves(c),
            PieceType.Queen => this.GetQueenLegalMoves(c),
            PieceType.Rook => this.GetRookLegalMoves(c),
            PieceType.Bishop => this.GetBishopLegalMoves(c),
            PieceType.Knight => this.GetKnightLegalMoves(c),
            PieceType.Pawn => this.GetPawnLegalMoves(c),
            _ => throw new NotImplementedException(),
        };
    }

    private List<Square> GetKingLegalMoves(Square c)
    {
        Piece? king = GetPieceOnSquare(c);

        if (king is null)
            return new List<Square>();

        List<Square> moves = PieceTypeFuncs.GetTypesPossibleMoves(c, PieceType.King);
        List<Square> res = new List<Square>();

        foreach (Square move in moves)
        {
            Piece? p = GetPieceOnSquare(move);

            if (p is null)
            {
                res.Add(move);
                continue;
            }

            if (p.color != king.color)
                res.Add(move);
        }

        return res;
    }

    private List<Square> GetQueenLegalMoves(Square c)
    {
        List<Square> l = this.GetRookLegalMoves(c);
        return new List<Square>(l.Concat(this.GetBishopLegalMoves(c)));
    }

    private List<Square> GetRookLegalMoves(Square c)
    {
        Piece? rook = GetPieceOnSquare(c);

        if (rook is null)
            return new List<Square>();

        List<Square> moves = PieceTypeFuncs.GetTypesPossibleMoves(c, PieceType.Rook);
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

                Piece? p = this.board[x, y];

                if (p is null)
                    continue;

                if (p.color == rook.color)
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
        Piece? bishop = this.GetPieceOnSquare(c);

        if (bishop is null)
            return new List<Square>();

        List<Square> moves = PieceTypeFuncs.GetTypesPossibleMoves(c, PieceType.Bishop);
        List<Square> res = new List<Square>();

        foreach (Square move in moves)
        {
            int step_x = (move.X - c.X) / Math.Abs(move.X - c.X);
            int step_y = (move.Y - c.Y) / Math.Abs(move.Y - c.Y);
            bool isPossible = true;

            for (int i = c.X + step_x; (move.X - i) * step_x >= 0; i += step_x)
            {
                Piece? p = this.board[i, c.Y + Math.Abs(i - c.X) * step_y];

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
        Piece? knight = GetPieceOnSquare(c);

        if (knight is null)
            return new List<Square>();

        List<Square> moves = PieceTypeFuncs.GetTypesPossibleMoves(c, knight.type);
        List<Square> res = new List<Square>();

        foreach (Square move in moves)
        {
            Piece? p = GetPieceOnSquare(move);

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
        Piece? pawn = GetPieceOnSquare(c);

        if (pawn is null)
            return new List<Square>();

        List<Square> moves = PieceTypeFuncs.GetTypesPossibleMoves(c, PieceType.Pawn, pawn.color);
        List<Square> res = new List<Square>();

        foreach (Square move in moves)
        {
            Piece? p = GetPieceOnSquare(move);

            if (move.X == c.X && p is null)
                res.Add(move);

            if (move.X != c.X && p is not null && p.color != pawn.color)
                res.Add(move);
        }

        return res;
    }

    public Piece? GetPieceOnSquare(Square c)
    {
        return this.board[c.X, c.Y];
    }

    public bool IsWhiteToMove()
    {
        return this.colorToMove == Color.White;
    }

    public override string ToString()
    {
        string res = "";

        for (int j = 7; j >= 0; j--)
        {
            for (int i = 0; i < 8; i++)
            {
                Piece? p = this.board[i, j];

                if (p is null)
                    res += "- ";
                else
                    res += p.ToString() + " ";
            }

            res += "\n";
        }

        res += $"Move - {this.moves}, Color To Move - {this.colorToMove}\n";
        return res;
    }
}

public class Move
{
    public Square start;
    public Square end;

    public Move(Square start, Square end)
    {
        this.start = start;
        this.end = end;
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

    public Square(int i, int j)
    {
        this.coordinates = (i, j);
    }

    public Square((int, int) coords)
    {
        this.coordinates = coords;
    }

    public Square(string input)
    {
        (int, int)? c = ConvertStringToCoordinates(input);

        if (c is null)
            throw new Exception();

        this.coordinates = ((int, int))c;
    }

    public static Square? StringToSquare(string input)
    {
        (int, int)? coords = ConvertStringToCoordinates(input);
        if (coords is null)
            return null;

        return new Square(((int, int))coords);
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

public class PieceTypeFuncs
{
    public static (PieceType, Color)? FromChar(char c)
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

        return ((PieceType)t, color);
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
                res.Add(new Square(new_x, new_y));
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
                res.Add(new Square(new_x, new_y));
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
                res.Add(new Square(i, j));
        }

        return res;
    }

    private static List<Square> GetPossibleRookMoves(Square c)
    {
        List<Square> res = new List<Square>();

        for (int i = 0; i < 8; i++)
        for (int j = 0; j < 8; j++)
            if ((i == c.X || j == c.Y) && !(i == c.X && j == c.Y))
                res.Add(new Square(i, j));

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

        Console.WriteLine($"from square {c} {color}");

        for (int i = 0; i < 3; i++)
        {
            int x = c.X + i - 1;
            int y = c.Y + step;

            Console.WriteLine($"pawn -- {x} {y}");

            if (PieceTypeFuncs.AreCorrectCoords(x, y))
                res.Add(new Square(x, y));
        }

        if (c.Y == rankThatCanGoDouble)
            res.Add(new Square(c.X, c.Y + 2 * step));

        foreach (Square s in res)
            Console.WriteLine(s);

        return res;
    }

    private static bool AreCorrectCoords(int i, int j)
    {
        return !(i >= 8 || i < 0 || j >= 8 || j < 0);
    }
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

        Square? enPassant = GetEnPassantFromFEN(parts[3]);

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

        return new Position(board, moves, halfmoves, (Color)c, castling, enPassant);
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
                    (PieceType, Color)? a = PieceTypeFuncs.FromChar(c);

                    if (a is null)
                        return null;

                    var b = ((PieceType, Color))a;

                    pieces[j, 7 - i] = new Piece(b.Item1, b.Item2);

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

    private static Square? GetEnPassantFromFEN(string input)
    {
        return Square.StringToSquare(input);
    }
}
