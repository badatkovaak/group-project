namespace ChessLogic;

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
