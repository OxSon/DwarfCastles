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
        
        public IList<Entity> Entities { get; }
        public bool[,] Impassables;
        

        public Map()
        {
            Entities = new List<Entity>();
            Size = new Point(25,25);
            Impassables = new bool[Size.X, Size.Y];
            GenerateSampleMap();
        }

        private void GenerateSampleMap()
        {
            Random rand = new Random();
            // Generate Trees
            for (int i = rand.Next(20, 50); i != 0; i--)
            {
                Entity e = new Entity("Tree", new Point(rand.Next(0,Size.X), rand.Next(0, Size.Y)), 'T', ConsoleColor.Black, ConsoleColor.Green);
                //e.Tags.Add(new Tag("Passable", false)); // TODO Implement Tags
                Impassables[e.Pos.X, e.Pos.Y] = true; //TODO will this work for testing?
                AddEntity(e);
            }
            // Generate Rocks
            for (int i = rand.Next(20, 50); i != 0; i--)
            {
                Entity e = new Entity("Rock", new Point(rand.Next(0,Size.X), rand.Next(0, Size.Y)), 'r', ConsoleColor.Black, ConsoleColor.Gray);
                AddEntity(e);
            }
            // Generate Dwarves
            for (int i = rand.Next(2,7); i != 0; i--)
            {
                Actor a = new Actor("Dwarf", new Point(rand.Next(0,Size.X), rand.Next(0, Size.Y)), 'D', this, ConsoleColor.Black, ConsoleColor.Blue);
                AddEntity(a);
            }
            
        }

        public void AddEntity(Entity e)
        {
            Entities.Add(e);
        }

        public bool InBounds(Point pos)
        {
            return pos.X > 0 && pos.Y > 0 && pos.X < Size.X && pos.Y < Size.Y;
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
                if (e != null)
                {
                    foreach (var v in Types.ArrayValues)
                    {
                        if (v.GetString() == Type)
                        {
                            MatchingEntities.Add(e);
                        }
                    }
                }
            }

            return MatchingEntities;
        }
    }
}