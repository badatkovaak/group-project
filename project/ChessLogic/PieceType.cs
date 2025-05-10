public enum PieceType
{
    King,
    Queen,
    Rook,
    Bishop,
    Knight,
    Pawn,
}

class PieceTypeUtils
{
    public static PieceType[] GetPromotableTypes()
    {
        return new PieceType[4]
        {
            PieceType.Queen,
            PieceType.Rook,
            PieceType.Bishop,
            PieceType.Knight,
        };
    }
}
