using System.Collections.Generic;
using System.Drawing;

namespace MazeMasters
{
    public class Location
    {
        public Pixel Center { get; private set; }

        public bool IsDeadEnd
        {
            get
            {
                return exits.Count < 2;
            }
        }

        public bool IsIntersection
        {
            get
            {
                return exits.Count > 2;
            }
        }

        private List<Path> exits = new List<Path>();
        private List<Thing> things = new List<Thing>();
        private Rectangle bounds;

        public Location(Location neighbor, Pixel center, int width)
        {
            Center = center;

            bounds = new Rectangle(
                Center.X - width / 2,
                Center.Y - width / 2,
                width,
                width);

            AddNeighbor(neighbor);
        }

        public void AddNeighbor(Location neighbor)
        {
            if (neighbor != null)
            {
                Path path = new Path(this, neighbor);

                exits.Add(path);
                neighbor.exits.Add(path);
            }
        }

        public delegate bool Visit(int depth, bool stopping, Location location);

        public bool Traverse(Visit onEnter, Path.Visit onEnterPath, Path.Visit onReturnPath, Visit onReturn)
        {
            HashSet<object> visited = new HashSet<object>();
            bool stop = Traverse(visited, onEnter, onEnterPath, onReturnPath, onReturn, 0);
            return stop;
        }

        public bool Traverse(HashSet<object> visited, Visit onEnter, Path.Visit onEnterPath, Path.Visit onReturnPath, Visit onReturn, int depth)
        {
            bool stop = false;

            if (visited.Contains(this))
                return stop;

            visited.Add(this);

            if (onEnter != null)
            {
                stop = onEnter(depth, stop, this);
                if (stop)
                    return stop;
            }

            foreach (var exit in exits)
            {
                bool stopping = exit.Traverse(visited, onEnter, onEnterPath, onReturnPath, onReturn, depth + 1);
                stop = stop || stopping;
                if (stop)
                    break;
            }

            if (onReturn != null)
            {
                bool stopping = onReturn(depth, stop, this);
                stop = stop || stopping;
            }

            return stop;
        }

        public void Draw(Graphics graphics, Brush brush)
        {
#if true
            graphics.FillCircle(brush, bounds);
#else
            graphics.FillRectangle(brush, bounds);
#endif
        }

        public void DrawThings(Graphics graphics)
        {
            foreach (Thing thing in things)
                thing.Draw(graphics, bounds);
        }

        public void Put(Thing thing)
        {
            things.Add(thing);
        }
    }
}
