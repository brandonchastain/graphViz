using System;

namespace Tetris
{
    public class TetrisBoard : IDrawable
    {
        private bool[][] tiles;

        public TetrisBoard(int rows, int cols)
        {
            tiles = new bool[rows][];
            for (int i = 0; i < rows; i++)
            {
                tiles[i] = new bool[cols];
            }
        }

        public int Height => tiles.Length;
        public int Width => tiles[0].Length;

        public bool CanPieceMoveTo(Piece piece, int row, int col)
        {
            var pieceTiles = piece.GetTiles();
            for (int r = 0; r < pieceTiles.Length; r++)
            {
                for (int c = 0; c < pieceTiles[r].Length; c++)
                {
                    if (pieceTiles[r][c])
                    {
                        var rowOutOfBounds = (row + r) >= tiles.Length || (row + r) < 0;
                        if (rowOutOfBounds)
                        {
                            return false;
                        }

                        var colOutOfBounds = (col + c) >= tiles[row + r].Length || (col + c) < 0;
                        if (colOutOfBounds)
                        {
                            return false;
                        }

                        var overlap = tiles[row + r][col + c];
                        if (overlap)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public void PlacePiece(Piece piece)
        {
            var pieceTiles = piece.GetTiles();
            for (int r = 0; r < pieceTiles.Length; r++)
            {
                for (int c = 0; c < pieceTiles[r].Length; c++)
                {
                    int boardRow = r + piece.GetRow();
                    int boardCol = c + piece.GetCol();

                    if (pieceTiles[r][c] == true)
                    {
                        if (!tiles[boardRow][boardCol])
                        {
                            tiles[boardRow][boardCol] = pieceTiles[r][c];
                        }
                        else
                        {
                            throw new InvalidOperationException("Tried to place a piece on top of another piece.");
                        }
                    }
                }
            }
        }

        public int ClearCompleteRows()
        {
            int count = 0;
            for (int r = 0; r < tiles.Length; r++)
            {
                bool complete = true;
                for (int c = 0; c < tiles[r].Length; c++)
                {
                    if (!tiles[r][c])
                    {
                        complete = false;
                        break;
                    }
                }

                if (complete)
                {
                    ClearCompleteRow(r);
                    count++;
                }
            }

            return count;
        }

        private void ClearCompleteRow(int row)
        {
            for (int c = 0; c < tiles[row].Length; c++)
            {
                tiles[row][c] = false;
            }

            //shift all tiles above down
            for (int i = row; i > 0; i--)
            {
                for (int c = 0; c < tiles[i].Length; c++)
                {
                    this.tiles[i][c] = this.tiles[i - 1][c];
                }
            }
        }

        public bool[][] GetTiles()
        {
            return tiles;
        }

        public int GetCol() => 0;
        public int GetRow() => 0;
    }
}