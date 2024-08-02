namespace GraphViz;

using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Blazor.Extensions.Canvas.Canvas2D;
using Core;

public class BlazorRenderer : IRenderer
{
    private Canvas2DContext canvas;

    public BlazorRenderer(Canvas2DContext canvas)
    {
        this.canvas = canvas ?? throw new ArgumentNullException(nameof(canvas));
    }

    public async Task Draw(Tree<int?> tree)
    {
        int size = 0;
        tree.Root?.Visit(_ => size += 1);

        await this.canvas.ClearRectAsync(0, 0, 10000, 10000);
        await this.canvas.SetFontAsync("bold 14pt Arial");
        await DrawNode(tree.Root!, size, 500, 50, 1);
    }

    private async Task DrawNode(TreeNode<int?> node, int size, int x, int y, int depth)
    {
        double log = Math.Log2(size);

        int tileSize = 20;
        int yOffset = 50;
        int xOffset = (int)(tileSize * 3 * Math.Ceiling(log) / depth);

        if (node == null)
        {
            await this.canvas.SetFillStyleAsync("red");
            await this.canvas.FillTextAsync("*", x - 6, y + 10);
            return;
        }

        // this node
        await this.canvas.SetFillStyleAsync("#FFFFFF");
        await this.canvas.FillRectAsync(x - tileSize / 2, y - tileSize / 2, tileSize, tileSize);
        await this.canvas.SetFillStyleAsync("#000000");
        await this.canvas.FillTextAsync($"{node.Value}", x - tileSize / 2, y + tileSize / 3);

        // left child line
        await this.canvas.SetStrokeStyleAsync("red");
        await this.canvas.BeginPathAsync();
        await this.canvas.MoveToAsync(x, y + tileSize / 2);
        await this.canvas.LineToAsync(x - xOffset, y + yOffset);
        await this.canvas.StrokeAsync();

        // right child line
        await this.canvas.BeginPathAsync();
        await this.canvas.MoveToAsync(x, y + tileSize / 2);
        await this.canvas.LineToAsync(x + xOffset, y + yOffset);
        await this.canvas.StrokeAsync();

        // recurse
        await DrawNode(node.Left, size, x - xOffset, y + yOffset, depth + 1);
        await DrawNode(node.Right, size, x + xOffset, y + yOffset, depth + 1);
    }

    public async ValueTask RenderError(string msg)
    {
        await this.canvas.BeginBatchAsync();
        await this.canvas.SetFillStyleAsync("black");
        await this.canvas.FillRectAsync(10, 550, 200, 200);
        await this.canvas.SetFontAsync("bold 12px Helvetica");
        await this.canvas.SetFillStyleAsync("red");
        var parts = msg.Split("\n");
        int curRow = 620;
        foreach (var part in parts)
        {
            await this.canvas.FillTextAsync(part, 10, curRow);
            curRow += 20;
        }
        await this.canvas.EndBatchAsync();
    }
}