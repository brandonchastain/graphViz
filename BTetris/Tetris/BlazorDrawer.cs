using Blazor.Extensions.Canvas.Canvas2D;

namespace Tetris
{
    public class BlazorDrawer : ITetrisDrawer
    {
        private Canvas2DContext context;
        private Tetris game;
        private bool[][] erasePiece;
        public static int TileSize = 25;
        public static int[] BankButtonPos = new int[2]{270, 315};
        public static int[] BankButtonSize = new int[2]{100, 100};

        public BlazorDrawer(Canvas2DContext context, Tetris game)
        {
            this.context = context;
            this.game = game;
            this.erasePiece = new bool[5][];
            for (int i = 0; i < 5; i++)
            {
                this.erasePiece[i] = new bool[5];
            }
        }

        public async ValueTask Draw()
        {
            await DrawTitle();
            await DrawScore();

            IDrawable board = game.GetDrawableBoard();
            await Draw(board, "green", force: true);
            await Draw(game.GetDrawablePiece(), "red");
            await DrawNextPiece(game.GetDrawableNextPiece(), board);
            await DrawBankPiece(game.GetDrawableBankPiece(), board);
            await DrawBankButton();
            await DrawBorders(board, "white");
        }

        private async ValueTask DrawTitle()
        {
            await context.SetFillStyleAsync("black");
            await context.FillRectAsync(260, 300, 200, 200);
            await context.SetFillStyleAsync("white");
            await context.SetFontAsync("bold 18pt Helvetica");
            await context.FillTextAsync("B TETRIS", 260, 490);
        }

        private async ValueTask DrawScore()
        {
            var score = game.GetScore();
            await context.SetFillStyleAsync("white");
            await context.SetFontAsync("bold 18pt Helvetica");
            await context.FillTextAsync($"Score: {score}", 260, 460);
        }

        private async ValueTask DrawNextPiece(IDrawable p, IDrawable board)
        {
            var w = board.GetTiles()[0].Length;
            await DrawTiles(erasePiece, 1, w, "black", force: true);
            await DrawTiles(p.GetTiles(), 1, w, "blue");
            //await context.SetStrokeStyleAsync("white");
            //await context.StrokeRectAsync(w * TileSize, 0, 5 * TileSize, 5 * TileSize);
        }

        private async ValueTask DrawBankPiece(IDrawable p, IDrawable board)
        {
            var w = board.GetTiles()[0].Length;
            await DrawTiles(erasePiece, 7, w, "black", force: true);

            if (p == null)
            {
                return;
            }

            await DrawTiles(p.GetTiles(), 7, w, "yellow");
        }

        private async ValueTask DrawBankButton()
        {
            await context.SetFillStyleAsync("green");
            await context.FillRectAsync(BankButtonPos[0], BankButtonPos[1], BankButtonSize[0], BankButtonSize[1]);
            await context.SetFillStyleAsync("white");
            await context.SetFontAsync("18pt Helvetica");
            await context.FillTextAsync("BANK", BankButtonPos[0] + 15, BankButtonPos[1] + 65);
        }

        private async ValueTask Draw(IDrawable p, string color, bool force = false)
        {
            await DrawTiles(p.GetTiles(), p.GetRow(), p.GetCol(), color, force);
        }

        private async ValueTask DrawTiles(bool[][] tiles, int row, int col, string color, bool force = false)
        {
            for (int r = 0; r < tiles.Length; r++)
            {
                for (int c = 0; c < tiles[r].Length; c++)
                {
                    if (tiles[r][c])
                    {
                        await context.SetFillStyleAsync(color);
                        await context.FillRectAsync((col + c) * TileSize, (row + r) * TileSize, TileSize, TileSize);
                    }
                    else if (force)
                    {
                        await context.SetFillStyleAsync("gray");
                        await context.StrokeRectAsync((col + c) * TileSize, (row + r) * TileSize, TileSize, TileSize);
                        await context.SetFillStyleAsync("black");
                        await context.FillRectAsync((col + c) * TileSize, (row + r) * TileSize, TileSize, TileSize);
                    }
                }
            }
        }

        private async ValueTask DrawBorders(IDrawable board, string color)
        {
            var tiles = board.GetTiles();
            var height = tiles.Length * TileSize;
            var width = tiles[0].Length * TileSize;
            await context.SetStrokeStyleAsync(color);
            await context.StrokeRectAsync(1, 1, width - 1, height - 1);
        }
    }
}
