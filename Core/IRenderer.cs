namespace Core;

public interface IRenderer
{
    Task DrawAsync(Tree<int?> tree, int size);
}