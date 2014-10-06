using System;
using System.Collections.Generic;
using System.Drawing;

namespace MazeMasters
{
    public class Point
    {
        public int X;
        public int Y;

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public class Pixel : Point
    {
        public Pixel(int x, int y) : base(x, y)
        {
        }

        public static implicit operator System.Drawing.Point(Pixel point)
        {
            return new System.Drawing.Point(point.X, point.Y);
        }
    }

    public class Vector : Point
    {
        public Vector(int x, int y) : base(x, y)
        {
        }
    }

    public class Coordinates : Point
    {
        public Coordinates(int x, int y) : base(x, y)
        {
        }

        public Coordinates Add(Vector vector)
        {
            return new Coordinates(X + vector.X, Y + vector.Y);
        }
    }

    public class Maze
    {
        private static readonly Vector Left = new Vector(-1, 0);
        private static readonly Vector Right = new Vector(1, 0);
        private static readonly Vector Up = new Vector(0, 1);
        private static readonly Vector Down = new Vector(0, -1);

        private static List<Thing> things = new List<Thing>();

        static Maze()
        {
            things.AddRange(new Thing[]
            {
                new Trap(Resources.Lava),
                new Trap(Resources.Tower_of_Terror),
                new Treasure(Resources.Castle_of_Gold),
                new Treasure(Resources.Diamond),
                new Treasure(Resources.Staff_of_God),
            });
        }

        private readonly Random random;
        private int size;
        private int pathWidth;
        private Rectangle bounds;
        private Location[] squares;
        private Location start = null;
        private Location end = null;

        private int func = -1;
        private int from1 = 0;
        private int from2 = 0;
        private FillNextFunc[] funcs;

        public int Seed { get; private set; }
        public Size PixelSize { get; private set; }

        public enum Difficulty
        {
            MiniMaze,
            Easy,
            Medium,
            Hard,
            VeryHard,
            SuperHard,
            SuperDuperHard,
            SuperDuperDuperHard,
            SuperDuperDuperDuperHard,
        }

        public Maze(int seed, Difficulty difficulty, int maxPixelWidth)
        {
            this.Seed = seed;
            random = new Random(seed);

            switch (difficulty)
            {
                case Difficulty.MiniMaze:
                    size = 10;
                    break;

                case Difficulty.Easy:
                    size = 23;
                    break;

                case Difficulty.Medium:
                    size = 30;
                    break;

                case Difficulty.Hard:
                    size = 42;
                    break;

                case Difficulty.VeryHard:
                    size = 50;
                    break;

                case Difficulty.SuperHard:
                    size = 55;
                    break;

                case Difficulty.SuperDuperHard:
                    size = 65;
                    break;

                case Difficulty.SuperDuperDuperHard:
                    size = 75;
                    break;

                case Difficulty.SuperDuperDuperDuperHard:
                    size = 100;
                    break;
            }

            this.pathWidth = maxPixelWidth / size;
            this.PixelSize = new Size(size * pathWidth, size * pathWidth);
            this.bounds = new Rectangle(0, 0, PixelSize.Width, PixelSize.Height);

            funcs = new FillNextFunc[2];
            funcs[0] = () =>
            {
                int? empty = FindNext1(from1);
                if (!empty.HasValue)
                    return true;

                from1 = empty.Value;

                return FillEmpty(empty.Value);
            };
            funcs[1] = () =>
            {
                int? empty = FindNext2(from2);
                if (!empty.HasValue)
                    return true;

                from2 = empty.Value;

                return FillEmpty(empty.Value);
            };

            squares = new Location[size * size];
            FillEmpty(0);
            start = squares[0];
            start.Put(new Thing(Resources.Hole));
        }

        private enum State
        {
            Fill,
            Treasures,
            Traps,
            Done,
        }

        private State state = State.Fill;
        private List<Location> treasures = new List<Location>();

        public bool FillNext()
        {
            switch (state)
            {
                case State.Fill:
                    while (true)
                    {
                        func = (func + 1) % funcs.Length;

                        bool done = funcs[func]();
                        if (done)
                            break;
                    }

                    if (from1 == squares.Length - 1)
                    {
                        end = squares[from1];
                        end.Put(new Thing(Resources.Bright_Door));
                        state = State.Treasures;
                    }

                    return true;

                case State.Treasures:
                    List<Location> deadEnds = new List<Location>();

                    start.Traverse(
                        (depth, stopping, location) =>
                        {
                            if (location.IsDeadEnd && location != start && location != end)
                                deadEnds.Add(location);

                            return stopping;
                        },
                        null,
                        null,
                        null);

                    for (int i = 0; i < size / 2; i++)
                    {
                        int rand = random.Next(deadEnds.Count);
                        Location location = deadEnds[rand];
                        location.Put(RandomThing(typeof(Treasure)));

                        treasures.Add(location);

                        deadEnds.RemoveAt(rand);
                    }

                    state = State.Traps;
                    return true;

                case State.Traps:
                    {
                        int distance;
                        bool trapSet;

                        foreach (var treasure in treasures)
                        {
                            distance = 0;
                            trapSet = false;

                            start.Traverse(
                                (depth, stopping, location) =>
                                {
                                    return location == treasure;
                                },
                                null,
                                null,
                                (depth, stopping, location) =>
                                {
                                    if (stopping)
                                    {
                                        distance++;

                                        if (!trapSet && depth < distance && location != start && location != end)
                                        {
                                            location.Put(RandomThing(typeof(Trap)));
                                            trapSet = true;
                                        }
                                    }

                                    return stopping;
                                });
                        }

                        distance = 0;
                        trapSet = false;

                        start.Traverse(
                            (depth, stopping, location) =>
                            {
                                return location == end;
                            },
                            null,
                            null,
                            (depth, stopping, location) =>
                            {
                                if (stopping)
                                {
                                    distance++;

                                    if (!trapSet && depth < distance && location != start && location != end)
                                    {
                                        location.Put(RandomThing(typeof(Trap)));
                                        trapSet = true;
                                    }
                                }

                                return stopping;
                            });

                        state = State.Done;
                        return true;
                    }

                default:
                    return false;
            }
        }

        private delegate bool FillNextFunc();

        private int? FindNext1(int fromIndex)
        {
            Coordinates from = GetLocation(fromIndex);

            Coordinates next = from.Add(Up);
            int? tempIndex = GetIndex(next);
            if (tempIndex.HasValue)
                return tempIndex;

            next = new Coordinates(from.X + 1, 0);
            return GetIndex(next);
        }

        private int? FindNext2(int fromIndex)
        {
            Coordinates from = GetLocation(fromIndex);

            Coordinates next = from.Add(Right);
            int? tempIndex = GetIndex(next);
            if (tempIndex.HasValue)
                return tempIndex;

            next = new Coordinates(0, from.Y + 1);
            return GetIndex(next);
        }

        private bool FillEmpty(int index)
        {
            Location location = squares[index];
            if (location != null)
                return false;

            Location neighbor = null;

            List<int> possibilities = new List<int>();

            Coordinates coordinates = GetLocation(index);
            AddIfNotEmpty(possibilities, coordinates, Left);
            AddIfNotEmpty(possibilities, coordinates, Right);
            AddIfNotEmpty(possibilities, coordinates, Up);
            AddIfNotEmpty(possibilities, coordinates, Down);

            if (possibilities.Count > 0)
            {
                int rand = random.Next(possibilities.Count);
                int neighborIndex = possibilities[rand];
                neighbor = squares[neighborIndex];
            }

            Pixel pixel = GetPixel(coordinates);

            CreateNode(index, neighbor, pixel);

            Wander(index);

            return true;
        }

        private void CreateNode(int index, Location neighbor, Pixel pixel)
        {
            Location location = new Location(neighbor, pixel, pathWidth - 2);
            squares[index] = location;
        }

        private void Wander(int index)
        {
            List<int> possibilities = new List<int>();

            Coordinates coordinates = GetLocation(index);
            AddIfEmpty(possibilities, coordinates, Left);
            AddIfEmpty(possibilities, coordinates, Right);
            AddIfEmpty(possibilities, coordinates, Up);
            AddIfEmpty(possibilities, coordinates, Down);

            if (possibilities.Count > 0)
            {
                Location location = squares[index];

                int rand = random.Next(possibilities.Count);
                int neighborIndex = possibilities[rand];
                Coordinates neighborLocation = GetLocation(neighborIndex);
                Pixel pixel = GetPixel(neighborLocation);

                CreateNode(neighborIndex, location, pixel);

                Wander(neighborIndex);
            }
        }

        public Pixel GetPixel(Coordinates location)
        {
            int halfWidth = pathWidth / 2;
            int pixelHeight = size * pathWidth;
            return new Pixel(location.X * pathWidth + halfWidth, pixelHeight - (location.Y * pathWidth + halfWidth));
        }

        private int? GetIndex(Coordinates location)
        {
            if (location.X < 0)
                return null;

            if (location.X >= size)
                return null;

            if (location.Y < 0)
                return null;

            if (location.Y >= size)
                return null;

            return location.Y * size + location.X;
        }

        public Coordinates GetLocation(int index)
        {
            return new Coordinates(index % size, index / size);
        }

        private void AddIfEmpty(List<int> list, Coordinates location, Vector direction)
        {
            Coordinates neighborLocation = location.Add(direction);
            int? neighborIndex = GetIndex(neighborLocation);
            if (!neighborIndex.HasValue)
                return;

            Location neighbor = squares[neighborIndex.Value];
            if (neighbor != null)
                return;

            list.Add(neighborIndex.Value);
        }

        private void AddIfNotEmpty(List<int> list, Coordinates location, Vector direction)
        {
            Coordinates neighborLocation = location.Add(direction);

            int? neighborIndex = GetIndex(neighborLocation);
            if (!neighborIndex.HasValue)
                return;

            Location neighbor = squares[neighborIndex.Value];
            if (neighbor == null)
                return;

            list.Add(neighborIndex.Value);
        }

        public void Draw(Graphics graphics, bool debug)
        {
            // start with a black background
            using (Brush brush = new SolidBrush(Color.Green))
                graphics.FillRectangle(brush, bounds);

            // make sure we even have a maze to draw
            if (start == null)
                return;

            // draw the white paths
            using (Brush brush = new SolidBrush(Color.DarkSlateGray))
            using (Pen pen = new Pen(Color.DarkSlateGray, pathWidth - 2))
            {
                start.Traverse(
                    null,
                    null,
                    (depth, stopping, path) =>
                    {
                        path.Draw(graphics, pen);

                        return stopping;
                    },
                    (depth, stopping, location) =>
                    {
                        location.Draw(graphics, brush);

                        return stopping;
                    });
            }

            if (debug)
            {
                // draw the red solution line
                using (Pen pen = new Pen(Color.Red, 1))
                {
                    start.Traverse(
                        (depth, stopping, location) =>
                        {
                            if (location == end)
                                return true;

                            return stopping;
                        },
                        null,
                        (depth, stopping, path) =>
                        {
                            if(stopping)
                                path.Draw(graphics, pen);

                            return stopping;
                        },
                        null);
                }
            }

            // draw the traps and treasures and entrance and exit
            start.Traverse(
                null,
                null,
                null,
                (depth, stopping, location) =>
                {
                    location.DrawThings(graphics);

                    return stopping;
                });
        }

        private T Random<T>(List<T> list)
        {
            int i = random.Next(list.Count);
            return list[i];
        }

        private Thing RandomThing(Type type)
        {
            Predicate<Thing> match = (t) => { return t.GetType() == type; };
            var matches = things.FindAll(match);
            return Random(matches);
        }
    }
}
