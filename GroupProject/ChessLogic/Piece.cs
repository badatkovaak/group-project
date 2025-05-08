namespace ChessLogic;

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
