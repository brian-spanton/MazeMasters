using System.Collections.Generic;
using System.Drawing;

namespace MazeMasters
{
    public class Path
    {
        private Location[] exits = new Location[2];

        public Path(Location location1, Location location2)
        {
            exits = new Location[] { location1, location2 };
        }

        public void Draw(Graphics graphics, Pen pen)
        {
            graphics.DrawLine(pen, exits[0].Center, exits[1].Center);
        }

        public delegate bool Visit(int depth, bool stopping, Path path);

        public bool Traverse(HashSet<object> visited, Location.Visit onEnterLocation, Visit onEnter, Visit onReturn, Location.Visit onReturnLocation, int depth)
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
                bool stopping = exit.Traverse(visited, onEnterLocation, onEnter, onReturn, onReturnLocation, depth + 1);
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
    }
}
