using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace DwarfCastles
{
    /// <summary>
    /// Authors: Alec Mills and Josh DeMoss
    ///
    /// Map of game world, all locations of entities, etc
    /// </summary>
    public class Map
    {
        // Represents the max Size of the map
        public Point Size;
        
        public List<Entity> Entities { get; }
        public readonly bool[,] Impassables;
        
        public Map(Point size)
        {
            Size = size;
            Entities = new List<Entity>();
            Impassables = new bool[Size.X,Size.Y];
        }

        public void RemoveEntityById(int id)
        {
            var entity = from e in Entities where e.Id == id select e;

            Entities.Remove(entity.First());
        }

        public void AddEntity(Entity e)
        {
            Entities.Add(e);
            if (e.GetTag("attributes.passable") != null)
            {
                if (!e.GetTag("attributes.passable").Value.GetBool())
                {
                    Impassables[e.Pos.X, e.Pos.Y] = true;
                }
            }
        }

        public void AddEntity(Entity e, Point pos)
        {
            e.Pos = pos;
            AddEntity(e);
        }

        public bool InBounds(Point pos)
        {
            var result = pos.X > 0 && pos.Y > 0 && pos.X < Size.X && pos.Y < Size.Y;
//            Logger.Log($"Map.InBounds for {pos} == {result}");
            return result;
        }

        private bool Passable(Point pos)
        {
//            Logger.Log("Found impassables: ");
            
//            foreach (var x in Impassables)
//            {
//                Logger.Log(x.ToString());
//            }
            return InBounds(pos) && !Impassables[pos.X, pos.Y];
        }
        
        public IEnumerable<Point> AdjacentPoints(Point origin)
        {
            var rawAdjacents = new[]
            {
                new Point(origin.X - 1, origin.Y),
                new Point(origin.X + 1, origin.Y),
                new Point(origin.X, origin.Y - 1),
                new Point(origin.X, origin.Y + 1)
            };

//            foreach (var p in rawAdjacents)
//            {
//                Logger.Log($"{p} passable? : {Passable(p)}");
//            }
            
            var result = rawAdjacents.Where(Passable).ToList();

            if (result.Count > 0)
            {
//                foreach (var el in result)
//                {
////                    Logger.Log($"Found passable point: {el}");
//                }
            }
            else
                Logger.Log("Found no passable points");

            return result;
        }
        
        /// <summary>
        /// Get all entites that have a certain tag, ignoring the value of that tag
        /// </summary>
        /// <param name="tagName">tag to search for</param>
        /// <returns>matching values</returns>
        public IEnumerable<Entity> LocateEntitiesByTag(string tagName)
        {
            return from ent in Entities where ent.GetTag(tagName) != null select ent;
        }

        /// <summary>
        /// get all entities that have a tag EQUAL to a certain tag, i.e. same name AND value
        /// </summary>
        /// <param name="tag">tag to search for</param>
        /// <returns>matching entities</returns>
        public IEnumerable<Entity> LocateEntitiesByTag(Tag tag) // TODO Needs reworking
        {
            string name = tag.Name;
            return from ent in Entities
                where ent.GetTag(name) != null &&
                      ent.GetTag(name).Value == tag.Value
                select ent;
        }

        public IEnumerable<Entity> LocateEntitiesByType(string Type)
        {
            IList<Entity> MatchingEntities = new List<Entity>();
            foreach (var e in Entities)
            {
                var Types = e.GetTag("Types");
                foreach (var v in Types.ArrayValues)
                {
                    if (v.GetString() == Type)
                    {
                        MatchingEntities.Add(e);
                    }
                }
            }

            return MatchingEntities;
        }
    }
}