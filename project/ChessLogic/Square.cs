namespace ChessLogic;

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

    public static Square NewUnchecked(string input)
    {
        Square? res = Square.New(input);

        if (res is null)
            throw new Exception();

        return (Square)res;
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
