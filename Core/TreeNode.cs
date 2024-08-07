namespace Core;

public class TreeNode<T>
{
    public TreeNode(T val)
    {
        this.Value = val;
    }

    public T Value { get; set; }
    public TreeNode<T>? Left { get; set; }
    public TreeNode<T>? Right { get; set; }
    public TreeNode<T>? Parent { get; set; }

    public TreeNode<T>[] GetPathToRoot()
    {
        var res = new List<TreeNode<T>>();
        TreeNode<T>? cur = this;
        while (cur != null)
        {
            res.Add(cur);
            cur = cur.Parent;
        }

        return res.ToArray();
    }

    public void Visit(Action<TreeNode<T>> act)
    {
        act(this);

        if (this.Left != null)
        {
            this.Left.Visit(act);
        }

        if (this.Right != null)
        {
            this.Right.Visit(act);
        }
    }
}