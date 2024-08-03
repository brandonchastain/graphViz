using System;
using System.Threading.Tasks;
using Core;

namespace GraphViz;

public class VizUI
{
    private readonly Random random;
    private Tree<int?> tree;

    public VizUI(int nodeCount)
    {
        this.NodeCount = nodeCount;
        this.random = new Random();
    }

    public int NodeCount { get; set; }
    public int NullProbabilityPct { get; set; }

    public void Refresh()
    {
        this.tree = null;
    }

    public (Tree<int?>, int size) GetTreeToDisplay(DateTimeOffset timestamp)
    {
        if (this.tree != null)
        {
            return (this.tree, this.NodeCount);
        }

        var data = new List<int?>();

        for (int i = 0; i < NodeCount; i++)
        {
            var log = Math.Ceiling(Math.Log2(i));
            int r = this.random.Next(100);

            if (i > 0 && r < NullProbabilityPct * log)
            {
                data.Add(null);
            }
            else
            {
                data.Add(r);
            }
        }

        this.tree = new Tree<int?>([.. data]);
        return (this.tree, NodeCount);
    }
}