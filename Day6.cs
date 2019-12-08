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
        return string.Empty;
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
    }
}