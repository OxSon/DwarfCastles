using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace DwarfFortress
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
            Size = new Point(50, 50);
        }

        private void GenerateSampleMap()
        {
            Random rand = new Random();
            // Generate Trees
            for (int i = rand.Next(20, 50); i != 0; i--)
            {
                Entity e = new Entity("Tree", new Point(rand.Next(0,40), rand.Next(0, 40)), 'T', ConsoleColor.Black, ConsoleColor.Green);
                //e.Tags.Add(new Tag("Passable", false)); // TODO Implement Tags
                Impassables[e.Pos.X, e.Pos.Y] = true; //TODO will this work for testing?
                AddEntity(e);
            }
            // Generate Rocks
            for (int i = rand.Next(20, 50); i != 0; i--)
            {
                Entity e = new Entity("Rock", new Point(rand.Next(0,40), rand.Next(0, 40)), 'r', ConsoleColor.Black, ConsoleColor.Gray);
                AddEntity(e);
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
            return from ent in Entities where ent.Tags.ContainsKey(tagName) select ent;
        }

        /// <summary>
        /// get all entities that have a tag EQUAL to a certain tag, i.e. same name AND value
        /// </summary>
        /// <param name="tag">tag to search for</param>
        /// <returns>matching entities</returns>
        public IEnumerable<Entity> LocateEntitiesByTag(Tag tag)
        {
            string name = tag.Name;
            return from ent in Entities
                where ent.Tags.ContainsKey(name) &&
                      ent.GetTag(name).Value == tag.Value
                select ent;
        }
    }
}