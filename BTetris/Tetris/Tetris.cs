using System;
using System.Collections.Generic;

namespace Tetris
{
    public class Tetris
    {
        private TimeSpan tickMs;
        private DateTimeOffset lastTick = default;
        private TetrisState state;
        private TetrisBoard board;
        private PlayerInput playerInput;
        private Piece piece;
        private Piece nextPiece;
        private Piece bankPiece;
        private bool userBankedPiece;
        private int score;

        public Tetris(int width, int height)
        {
            Init(width, height);
        }

        public bool IsOver => state == TetrisState.GameOver;

        public void Update(DateTimeOffset ts)
        {
            this.HandlePlayerInput();

            if (state == TetrisState.Started)
            {
                if (ts - lastTick > tickMs)
                {
                    GameUpdate();
                    lastTick = ts;
                }
            }
        }

        public void Reset()
        {
            Init(this.board.Width, this.board.Height);
        }

        public void SendKeyDown(string keyCode)
        {
            this.playerInput.QueueInput(keyCode);
        }

        public IDrawable GetDrawableBankPiece() => bankPiece;

        public IDrawable GetDrawablePiece() => piece;

        public IDrawable GetDrawableNextPiece() => nextPiece;

        public IDrawable GetDrawableBoard() => board;

        public int GetScore() => score;

        public void Start()
        {
            state = TetrisState.Started;
        }

        public void BankCurrentPiece()
        {
            if (userBankedPiece)
            {
                // only once per piece, can't swap back until piece falls
                return;
            }

            var temp = piece;
            var result = bankPiece;
            bankPiece = piece;
            piece = result;

            if (piece == null)
            {
                piece = Piece.GetNextPiece();
            }

            while (board.CanPieceMoveTo(piece, piece.GetRow() - 1, piece.GetCol()))
            {
                piece.Move(InputButton.Up);
            }

            userBankedPiece = true;
        }

        private void Init(int width, int height)
        {
            board = new TetrisBoard(height, width);
            playerInput = new PlayerInput(this, board);
            piece = Piece.GetNextPiece();
            nextPiece = Piece.GetNextPiece();
            bankPiece = null;
            score = 0;
            tickMs = TimeSpan.FromMilliseconds(300);
        }

        private void HandlePlayerInput()
        {
            this.playerInput.HandlePlayerInput(this.piece);
        }

        private void GameUpdate()
        {
            var rowBelow = piece.GetRow() + 1;
            if (board.CanPieceMoveTo(piece, rowBelow, piece.GetCol()))
            {
                piece.Move(InputButton.Down);
            }
            else
            {
                board.PlacePiece(piece);
                
                var completedRowCount = board.ClearCompleteRows();
                this.score += completedRowCount;
                this.tickMs -= TimeSpan.FromMilliseconds(10 * completedRowCount);

                GenerateNextPiece();
            }
        }

        private void GenerateNextPiece()
        {
            piece = nextPiece;
            nextPiece = Piece.GetNextPiece();
            CheckForGameOver();
            userBankedPiece = false;
        }

        private void CheckForGameOver()
        {
            if (!board.CanPieceMoveTo(piece, piece.GetRow(), piece.GetCol()))
            {
                state = TetrisState.GameOver;
            }
        }

        private enum TetrisState
        {
            Init,
            Started,
            GameOver
        }
    }
}