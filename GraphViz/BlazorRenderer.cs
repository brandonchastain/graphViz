namespace GraphViz;

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Blazor.Extensions.Canvas.Canvas2D;
using Core;
using Microsoft.AspNetCore.Components.Forms;

public class BlazorRenderer : IRenderer
{
    private const uint TileSize = 20;
    private Canvas2DContext canvas;
    private long width;
    private long height;

    /// <summary>
    /// Map of XY coordinates to the corresponding TreeNode at that location.
    /// </summary>
    private Dictionary<(long, long), TreeNode<int?>> coordsToNode;

    /// <summary>
    /// Map of TreeNodes to their XY coordinates.
    /// </summary>
    private Dictionary<TreeNode<int?>, (long, long)> nodeToCoords;

    public BlazorRenderer(Canvas2DContext canvas, long width, long height)
    {
        this.canvas = canvas ?? throw new ArgumentNullException(nameof(canvas));
        this.width = width;
        this.height = height;
        this.coordsToNode = new Dictionary<(long, long), TreeNode<int?>>();
        this.nodeToCoords = new Dictionary<TreeNode<int?>, (long, long)>();
    }

    public bool IsArtModeEnabled { get; private set; }
    
    public async ValueTask SetArtMode(bool isArtModeEnabled)
    {
        this.IsArtModeEnabled = isArtModeEnabled;
        await this.canvas.ClearRectAsync(0, 0, 10000, 10000);
    }

    public async Task DrawAsync(Tree<int?> tree, uint size)
    {
        await this.canvas.BeginBatchAsync();

        if (!IsArtModeEnabled)
        {
            await this.canvas.ClearRectAsync(0, 0, 10000, 10000);
            // await this.DrawGrid();
        }

        await this.canvas.SetFontAsync("bold 14pt Arial");
        await DrawNode(tree.Root!, size, 600, 50, 1);

        await this.canvas.EndBatchAsync();
    }

    private async Task DrawGrid()
    {
        //await this.RenderError($"width: {this.width} height: {this.height}");
        uint tileSize = TileSize;

        try
        {
            for (uint i = 0; i <= this.width; i += tileSize)
            {
                // await this.RenderError($"i <= this.width {i <= this.width}");
                await this.canvas.SetStrokeStyleAsync("white");
                await this.canvas.BeginPathAsync();
                await this.canvas.MoveToAsync(i, 0);
                await this.canvas.LineToAsync(i, this.height);
                await this.canvas.StrokeAsync();
            }

            for (uint i = 0; i <= this.height; i += tileSize)
            {
                // await this.RenderError($"i <= this.width {i <= this.width}");
                await this.canvas.SetStrokeStyleAsync("white");
                await this.canvas.BeginPathAsync();
                await this.canvas.MoveToAsync(0, i);
                await this.canvas.LineToAsync(this.width, i);
                await this.canvas.StrokeAsync();
            }
        }
        catch (Exception ex)
        {
            await this.RenderError(ex.ToString());
        }
    }

    private async Task DrawNode(TreeNode<int?> node, uint size, long x, long y, uint depth)
    {
        double log = Math.Log2(size);
        uint tileSize = TileSize;
        uint yOffset = 2 * TileSize;
        uint xOffset = (uint)(tileSize * 3 * Math.Ceiling(log) / depth);

        if (node == null)
        {
            await this.canvas.SetFillStyleAsync("red");
            await this.canvas.FillTextAsync("*", x - 6, y + 10);

            return;
        }
            
        // snap to nearest multiple of TileSize
        long nodeX = x - (x % TileSize);
        long nodeY = y - (y % TileSize);
        this.coordsToNode.TryAdd((nodeX, nodeY), node);
        this.nodeToCoords.TryAdd(node, (nodeX, nodeY));

        // this node
        if (!IsArtModeEnabled)
        {
            await this.canvas.SetFillStyleAsync("#FFFFFF");
            await this.canvas.FillRectAsync(nodeX, nodeY, tileSize, tileSize);
        }

        await this.canvas.SetFillStyleAsync("#000000");
        await this.canvas.FillTextAsync($"{node.Value}", x - tileSize / 2, y + tileSize / 3);

        if (node.Left == null && node.Right == null)
        {
            return;
        }

        // left
        await this.canvas.SetStrokeStyleAsync("red");
        await this.canvas.BeginPathAsync();
        await this.canvas.MoveToAsync(x, y + tileSize / 2);
        await this.canvas.LineToAsync(x - xOffset, y + yOffset);
        await this.canvas.StrokeAsync();

        // right
        await this.canvas.BeginPathAsync();
        await this.canvas.MoveToAsync(x, y + tileSize / 2);
        await this.canvas.LineToAsync(x + xOffset, y + yOffset);
        await this.canvas.StrokeAsync();

        // recurse
        await DrawNode(node.Left, size, x - xOffset, y + yOffset, depth + 1);
        await DrawNode(node.Right, size, x + xOffset, y + yOffset, depth + 1);
    }

    public async ValueTask DrawPathToRoot(long x, long y)
    {
        long nodeX = x - (x % TileSize);
        long nodeY = y - (y % TileSize);
        var key = (nodeX, nodeY);

        if (!this.coordsToNode.ContainsKey(key))
        {
            return;
        }

        var node = this.coordsToNode[key];
        var path = node.GetPathToRoot();
        foreach (var pnode in path)
        {
            (var px, var py) = this.nodeToCoords[pnode];
            await this.canvas.SetFillStyleAsync("#00FFFF");
            await this.canvas.FillRectAsync(px, py, TileSize, TileSize);
        }
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