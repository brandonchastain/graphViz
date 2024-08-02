using System;
using System.Threading.Tasks;
using Core;

namespace GraphViz;

public class VizUI
{
    private const int nodes = 50;
    private const int nullProbPct = 30;
    private readonly Random random;
    private Tree<int?> tree;

    public VizUI()
    {
        this.random = new Random();
    }

    public void Refresh()
    {
        this.tree = null;
    }

    public Tree<int?> GetTreeToDisplay(DateTimeOffset timestamp)
    {
        if (this.tree != null)
        {
            return this.tree;
        }

        var data = new List<int?>();

        for (int i = 0; i < nodes; i++)
        {
            int r = this.random.Next(100);
            if (r < nullProbPct && i > 0)
            {
                data.Add(null);
            }
            else
            {
                data.Add(r);
            }
        }

        this.tree = new Tree<int?>(data.ToArray());
        return this.tree;
    }
}