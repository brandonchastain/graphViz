namespace Core;

public class Program
{
    public static void Main(string[] args)
    {
        var data = new int?[] {1, 2, 3, 4, 5, null, 6, 7, 8};
        var tree = new Tree<int?>(data);
        tree.Root!.Visit(node => 
        {
            if (node == null)
            {
                return;
            }

            Console.Write($"{node.Value}");
            if (node.Left != null || node.Right != null)
            {
                Console.Write($" - L:{node.Left?.Value} R:{node.Right?.Value} ");
            }
            Console.WriteLine();
        });
    }
}