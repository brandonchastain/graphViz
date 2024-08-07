namespace Core;

public class Tree<T>
{
    public Tree()
    {
    }
    
    public Tree(T[] data)
    {
        if (data == null || !data.Any())
        {
            throw new InvalidOperationException("data is required");
        }

        this.Root = new TreeNode<T>(data[0]);
        this.BuildTree(this.Root, data);
    }

    public TreeNode<T>? Root { get; set; }

    public uint GetSize()
    {
        uint size = 0;
        this.Root?.Visit(_ => size += 1);
        return size;
    }

    private void BuildTree(TreeNode<T> root, T[] data)
    {
        var q = new Stack<Tuple<TreeNode<T>, int>>(); 

        q.Push(new (root, 0));

        while (q.Any())
        {
            (var parent, var parentIndex) = q.Pop();
            int nextLeft = parentIndex * 2 + 1;
            int nextRight = parentIndex * 2 + 2;

            if (nextLeft < data.Length && data[nextLeft] != null)
            {
                parent.Left = new TreeNode<T>(data[nextLeft]);
                parent.Left.Parent = parent;
                q.Push(new (parent.Left, nextLeft));
            }

            if (nextRight < data.Length && data[nextRight] != null)
            {
                parent.Right = new TreeNode<T>(data[nextRight]);
                parent.Right.Parent = parent;
                q.Push(new (parent.Right, nextRight));
            }
        }
    }
}
