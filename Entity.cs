using System;
using System.Collections.Generic;
using System.Drawing;

namespace DwarfFortress
{
    /// <summary>
    /// Authors: Alec Mills & Josh DeMoss
    ///
    /// Represents a physical entity in game world
    /// </summary>
    public class Entity
    {
        public IDictionary<string, Tag> Tags { get; }

        public string Name { get; }
        public Point Pos { get; }
        public int Id { get; } //TODO Josh what is this for?
        private static int id;

        //console 'drawing' related values
        public char Ascii { get; } //ascii character used to 'draw' entity
        public ConsoleColor BackgroundColor { get; }
        public ConsoleColor ForegroundColor { get; }

        //TODO
        /*
         * Seems to me that we shouldn't have to specify ascii everytime we create an entity. Are there
         * a limited number of possible varieties of Entity? If so, why not an enum for 'types' of entities,
         * and then the ascii will be based on that enum value? Or a struct, like EntityType, if we need
         * to be more flexible with what EntityType gets what ascii char?
         */

        /// <summary>
        /// constructor allowing for color specification
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pos"></param>
        /// <param name="ascii"></param>
        /// <param name="backgroundColor"> Optional; default = black </param>
        /// <param name="foregroundColor"> Optional; default = white </param>
        public Entity(string name, Point pos, char ascii,
            ConsoleColor backgroundColor = ConsoleColor.Black, ConsoleColor foregroundColor = ConsoleColor.White)
        {
            Name = name;
            Pos = pos;
            Ascii = ascii;
            BackgroundColor = backgroundColor;
            ForegroundColor = foregroundColor;

            Id = id++;
        }

        /// <summary>
        /// gets value of tag queried if applicable
        /// </summary>
        /// <param name="tagName">tag to query</param>
        /// <returns>value of Tag, or null if Tag is not present in Entity's Tag-list</returns>
        public Tag GetTag(string tagName)
        {
            return Tags.TryGetValue(tagName, out var result) ? result : null;
        }

        public void SetTag(string value, Tag tag)
        {
            //TODO * input validation for different tag types?
            //TODO * are we allowing a tag to be changed? if so, if it's present do we remove it first then readd?
            //TODO * how should this work?
            //TODO * by default the IDictionary.Add() method will throw an exception if tag is already present
            //TODO * this is how it currently functions
            Tags.Add(value, tag);
        }
    }
}