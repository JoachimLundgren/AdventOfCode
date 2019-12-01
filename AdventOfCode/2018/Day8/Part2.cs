using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using System.Drawing;

namespace AdventOfCode2018.Day8
{
    public class Part2
    {
        private static int index;
        public static void Run()
        {
            var input = File.ReadAllText("2018/Day8/Input.txt");

            var items = input.Split(' ').Select(int.Parse);
            var root = ReadNode(items);

            Console.WriteLine(root.GetMetadataSum2());
        }

        private static Node ReadNode(IEnumerable<int> input)
        {
            var node = new Node();
            var children = input.ElementAt(index++);
            var metadata = input.ElementAt(index++);

            for (int i = 0; i < children; i++)
            {
                node.AddChild(ReadNode(input));
            }

            for (int i = 0; i < metadata; i++)
            {
                node.AddMetadata(input.ElementAt(index++));
            }

            return node;
        }


        private class Node
        {
            public List<Node> Children { get; } = new List<Node>();
            public List<int> Metadata { get; } = new List<int>();

            public void AddChild(Node node)
            {
                Children.Add(node);
            }

            public void AddMetadata(int metadata)
            {
                Metadata.Add(metadata);
            }

            public int GetMetadataSum()
            {
                return Metadata.Sum() + Children.Sum(s => s.GetMetadataSum());
            }

            public int GetMetadataSum2()
            {
                if (Children.Any())
                {
                    var sum = 0;
                    foreach (var metadata in Metadata)
                    {
                        if (Children.Count >= metadata)
                            sum += Children[metadata - 1].GetMetadataSum2();
                    }
                    return sum;
                }
                else
                {
                    return Metadata.Sum();
                }
            }
        }
    }
}
