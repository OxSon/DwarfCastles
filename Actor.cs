using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace DwarfFortress
{
    /// <inheritdoc />
    public class Actor : Entity
    {
        //TODO Josh what is this? needs more functionality I assume?
        public IList<Task> Tasks { get; }
        
        public Map map { get; set; } //current map Actor is on
        
        public Actor(string name, Point pos, char ascii,
            ConsoleColor backgroundColor = ConsoleColor.Black,
            ConsoleColor foregroundColor = ConsoleColor.White) :
            base(name, pos, ascii, backgroundColor, foregroundColor) { }
        
        public bool CanMove(bool[,] impassables)
        {
            var adjacents = new[]
            {
                    new Point(Pos.X - 1, Pos.Y),
                    new Point(Pos.X + 1, Pos.Y),
                    new Point(Pos.X, Pos.Y - 1),
                    new Point(Pos.X, Pos.Y + 1)
            };

            return adjacents.Any(s => map.InBounds(s) && !impassables[s.X, s.Y]);
        }

        public void Move(bool[,] impassables, Point destination)
        {
            if (!CanMove(impassables)) return;
            
            var path = new Queue<Point>();
            while (!Equals(path.First(), destination))
            {
                Point next;
                
                foreach (var point in AdjacentPoints(Pos))
                {
                        
                }
            }
        }

        private IList<Point> AdjacentPoints(Point origin)
        {
            var rawAdjacents = new[]
            {
                    new Point(origin.X - 1, origin.Y),
                    new Point(origin.X + 1, origin.Y),
                    new Point(origin.X, origin.Y - 1),
                    new Point(origin.X, origin.Y + 1),
            };

            return rawAdjacents.Where(point => map.InBounds(point)).ToList();
        }

        private int DistToPoint(Point origin, Point dest)
        {
            //TODO
            return 0;
        }
    }
}