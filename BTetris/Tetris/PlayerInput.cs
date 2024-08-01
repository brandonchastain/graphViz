using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Tetris
{
    public class PlayerInput
    {
        private Tetris game;
        private TetrisBoard board;
        private InputButton lastInput = InputButton.None;

        public PlayerInput(Tetris game, TetrisBoard board)
        {
            this.game = game ?? throw new ArgumentNullException(nameof(game));
            this.board = board ?? throw new ArgumentNullException(nameof(board));
        }

        public void QueueInput(string keyCode)
        {
            InputButton direction;
            switch (keyCode)
            {
                case "ArrowUp":
                    direction = InputButton.Up;
                    break;
                case "ArrowRight":
                    direction = InputButton.Right;
                    break;
                case "ArrowDown":
                    direction = InputButton.Down;
                    break;
                case "ArrowLeft":
                    direction = InputButton.Left;
                    break;
                case "Space":
                    direction = InputButton.Space;
                    break;
                default:
                    direction = InputButton.None;
                    break;
            }

            this.lastInput = direction;
        }

        public void HandlePlayerInput(Piece currentPlayerPiece)
        {
            var inputDir = this.lastInput;
            this.lastInput = InputButton.None;

            var row = currentPlayerPiece.GetRow();
            var col = currentPlayerPiece.GetCol();

            switch (inputDir)
            {
                case InputButton.Space:
                    this.game.BankCurrentPiece();
                    break;
                case InputButton.Up:
                    currentPlayerPiece.Rotate();

                    if (!board.CanPieceMoveTo(currentPlayerPiece, row, col))
                    {
                        currentPlayerPiece.Rotate();
                        currentPlayerPiece.Rotate();
                        currentPlayerPiece.Rotate();
                    }
                    break;
                case InputButton.Down:
                    var piece = currentPlayerPiece;
                    row = piece.GetRow() + 1;
                    while (board.CanPieceMoveTo(piece, row, col))
                    {
                        piece.Move(InputButton.Down);
                        row++;
                    }
                    break;
                default:
                    TryMovePiece(currentPlayerPiece, inputDir);
                    break;
            }
        }

        private void TryMovePiece(Piece piece, InputButton dir)
        {
            var row = piece.GetRow();
            var col = piece.GetCol();

            switch (dir)
            {
                case InputButton.Right:
                    col++;
                    break;
                case InputButton.Left:
                    col--;
                    break;
                case InputButton.Down:
                    row++;
                    break;
                case InputButton.Up:
                default:
                    break;
            }

            if (dir != InputButton.None && board.CanPieceMoveTo(piece, row, col))
            {
                piece.Move(dir);
            }
        }
    }
}
