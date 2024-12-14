using System.Collections.Generic;

public class UnionFind
{
    private Dictionary<int, int> parent = new Dictionary<int, int>();

    public void Add(int element)
    {
        if (!parent.ContainsKey(element))
            parent[element] = element;
    }

    public int Find(int element)
    {
        if (parent[element] != element)
            parent[element] = Find(parent[element]); 
        return parent[element];
    }

    public void Union(int set1, int set2)
    {
        int root1 = Find(set1);
        int root2 = Find(set2);

        if (root1 != root2)
            parent[root1] = root2;
    }
}
