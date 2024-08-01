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