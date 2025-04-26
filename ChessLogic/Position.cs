namespace ChessInternals;

public class Position
{
    int halfmove;
    Piece?[,] board;

    public Position(int halfmove, Piece?[,] board)
    {
        this.halfmove = halfmove;
        this.board = board;
    }

    public List<Square> GetLegalMoves(Square c)
    {
        throw new NotImplementedException();
    }
}

public class Piece
{
    PieceType type;
    Color color;

    public Piece(PieceType t, Color c)
    {
        this.type = t;
        this.color = c;
    }
}

public class Square
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

    public Square(string input)
    {
        (int, int)? c = ConvertStringToCoordinates(input);

        if (c is null)
            throw new Exception();

        this.coordinates = ((int, int))c;
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
    public static PieceType? FromChar(char c)
    {
        return c switch
        {
            'k' => PieceType.King,
            'q' => PieceType.Queen,
            'r' => PieceType.Rook,
            'b' => PieceType.Bishop,
            'n' => PieceType.Knight,
            _ => null,
        };
    }

    public static List<Square> GetTypesPossibleMoves(Square c, PieceType t)
    {
        return t switch
        {
            PieceType.King => GetPossibleKingMoves(c),
            PieceType.Queen => GetPossibleQueenMoves(c),
            PieceType.Rook => GetPossibleRookMoves(c),
            PieceType.Bishop => GetPossibleBishopMoves(c),
            PieceType.Knight => GetPossibleKnightMoves(c),
            PieceType.Pawn => GetPossiblePawnMoves(c),
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

    private static List<Square> GetPossiblePawnMoves(Square c)
    {
        List<Square> res = new List<Square>();
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
        return (char)(((int)c) & 0xdf) switch
        {
            'b' => Color.Black,
            'w' => Color.White,
            _ => null,
        };
    }
}
