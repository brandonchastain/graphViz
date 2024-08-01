using System;

namespace Tetris
{
    public interface IDrawable
    {
        bool[][] GetTiles();
        int GetCol();
        int GetRow();
    }
}