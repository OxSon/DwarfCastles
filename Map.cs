using System.Collections.Generic;
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
        public IEnumerable<Entity> Entities { get; }

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