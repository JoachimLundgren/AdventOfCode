using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode.Utils
{
    public class Coordinate
    {
        public string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public Coordinate(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Coordinate(string name, int x, int y)
            : this(x,y)
        {
            Name = name;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as Coordinate;
            if (other == null)
                return false;

            return X == other.X && Y == other.Y;
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Name))
                return Name;
            return $"{X},{Y}";
        }
    }
}
