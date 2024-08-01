using System;

namespace Tetris
{
    public interface ITetrisDrawer
    {
        ValueTask Draw();
    }
}