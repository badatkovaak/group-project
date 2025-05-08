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
