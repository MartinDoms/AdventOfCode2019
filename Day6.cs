using System;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

public static class Day6 {

    public static string Problem1() {
        var sample = @"COM)B
B)C
C)D
D)E
E)F
B)G
G)H
D)I
E)J
J)K
K)L";
        var sampleTotal = TotalOrbits(sample.Split("\r\n"));
        Debug.Assert(sampleTotal == 42, $"Expected 42 but got {sampleTotal}");

        var file = ".\\data\\day6.txt";
        var input = File.ReadAllText(file).Split("\r\n");

        return TotalOrbits(input).ToString();
    }

    public static string Problem2() {
        var sample = @"COM)B
B)C
C)D
D)E
E)F
B)G
G)H
D)I
E)J
J)K
K)L
K)YOU
I)SAN";

        var sampleTransfers = GetMinimumTransfers(sample.Split("\r\n"), "YOU", "SAN");
        Debug.Assert(sampleTransfers == 4, $"Expected 4 but found {sampleTransfers}");

        var file = ".\\data\\day6.txt";
        var input = File.ReadAllText(file).Split("\r\n");

        var orbitalTransfers = GetMinimumTransfers(input, "YOU", "SAN");
        return orbitalTransfers.ToString();
    }

    static int GetMinimumTransfers(IEnumerable<string> nodes, string from, string to) {
        var tree = ToTree(nodes.Select(ToNodePair));
        var node1 = tree.FindNode(from);
        var node2 = tree.FindNode(to);
        var commonParent = GetCommonParent(tree, node1, node2);
        
        var parent = node1.Parent;
        int pathLength = 0;
        while (parent != commonParent) {
            pathLength++;
            parent = parent.Parent;
        }

        parent = node2.Parent;
        while (parent != commonParent) {
            pathLength++;
            parent = parent.Parent;
        }

        return pathLength;
    }

    static Node GetCommonParent(Node tree, Node node1, Node node2) {
        var node1Path = new List<Node>();
        var parent = node1.Parent;
        Node found = null;
        while (parent != null) {
            node1Path.Add(parent);
            parent = parent.Parent;
        }

        parent = node2.Parent;
        while (found == null && parent != null) {
            found = node1Path.FirstOrDefault(n => n.Name == parent.Name);
            parent = parent.Parent;
        }

        return found;
    }

    // nodePairs is in format AAA)BBB
    private static int TotalOrbits(IEnumerable<string> nodePairs) {
        var tree = ToTree(nodePairs.Select(ToNodePair));

        var orbits = 0;
        
        var currentNode = tree;
        var nextNodes = new Queue<Node>();

        while (currentNode != null) {
            foreach (var child in currentNode.Children) {
                nextNodes.Enqueue(child);
            }
            orbits += Level(currentNode);
            
            var next = nextNodes.TryDequeue(out currentNode);
            if (!next) currentNode = null;
        }

        return orbits;
    }

    // this is insanely innefficient, but it works and it finishes quickly so.... whatever
    static int Level(Node node) {
        var level = 0;
        while (node.Parent != null) {
            level++;
            node = node.Parent;
        }

        return level;
    }

    private static Node ToTree(IEnumerable<NodePair> pairs) {
        var lookup = new Dictionary<string, Node>();

        foreach (var pair in pairs) {
            Node parentNode;
            Node childNode;

            if (lookup.ContainsKey(pair.Child)) {
                childNode = lookup[pair.Child];
            }
            else {
                childNode = new Node(pair.Child);
                lookup.Add(pair.Child, childNode);
            }

            if (lookup.ContainsKey(pair.Parent)) {
                parentNode = lookup[pair.Parent];
                parentNode.Children.Add(childNode);
            }
            else {
                parentNode = new Node(pair.Parent, childNode);
                lookup.Add(pair.Parent, parentNode);
            }
            childNode.Parent = parentNode;
        }

        return lookup.Values.Single(n => n.Parent == null);
    }

    private static NodePair ToNodePair(string nodePair) {
        var nodes = SplitNodePair(nodePair);

        return new NodePair(nodes.node1, nodes.node2);
    }

    private static (string node1, string node2) SplitNodePair(string nodePair) {
        var nodes = nodePair.Split(')');
        return (nodes[0], nodes[1]);
    }

    class NodePair {
        public string Parent { get; set; }
        public string Child { get; set; }

        public NodePair(string parent, string child)
        {
            Parent = parent;
            Child = child;
        }
    }

    class Node {
        public string Name { get; set; }
        public List<Node> Children { get; set; }

        public Node Parent { get; set; }

        public Node(string name, params Node[] children)
        {
            Name = name;
            Children = new List<Node>();
            Children.AddRange(children);
        }

        internal Node FindNode(string n1)
        {
            if (Name == n1) return this;

            foreach (var child in Children) {
                var found = child.FindNode(n1);
                if (found != null) {
                    return found;
                }
            }

            return null;
        }
    }
}