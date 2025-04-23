namespace ChessInternals;

public class Position
{
    public Position() { }
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
        return $"({this.coordinates.Item1}, {this.coordinates.Item2})";
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
        throw new NotImplementedException();
    }

    private static List<Square> GetPossibleKingMoves(Square c)
    {
        return new List<Square>();
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
