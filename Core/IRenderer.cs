namespace Core;

public interface IRenderer
{
    Task Draw(Tree<int?> tree);
}