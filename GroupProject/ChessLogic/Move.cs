namespace ChessLogic;

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
