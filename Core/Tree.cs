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
        this.BuildTree(this.Root, 0, data);
    }

    public TreeNode<T>? Root { get; set; }

    private void BuildTree(TreeNode<T> parent, int parentIndex, T[] data)
    {
        int nextLeft = parentIndex * 2 + 1;
        int nextRight = parentIndex * 2 + 2;

        if (nextLeft < data.Length && data[nextLeft] != null)
        {
            parent.Left = new TreeNode<T>(data[nextLeft]);
            BuildTree(parent.Left, nextLeft, data);
        }

        if (nextRight < data.Length && data[nextRight] != null)
        {
            parent.Right = new TreeNode<T>(data[nextRight]);
            BuildTree(parent.Right, nextRight, data);
        }
    }
}
