using System;
using System.Threading.Tasks;

namespace Core;

public class TreeBuilder
{
    private readonly Random random;

    public TreeBuilder(uint nodeCount)
    {
        this.NodeCount = nodeCount;
        this.random = new Random();
    }

    public uint NodeCount { get; set; }
    public uint NullProbabilityPct { get; set; }

    public (Tree<int?>, uint size) Generate()
    {
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

        var tree = new Tree<int?>([.. data]);
        return (tree, NodeCount);
    }
}