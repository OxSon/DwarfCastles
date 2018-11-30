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
        public IList<Tag> Tags { get; }

        public string Name { get; set; }
        public Point Pos { get; protected set; }
        public int Id { get; }
        private static int id;

        //console 'drawing' related values
        public char Ascii { get; set; } //ascii character used to 'draw' entity
        public ConsoleColor BackgroundColor { get; set; }
        public ConsoleColor ForegroundColor { get; set; }

        public Entity()
        {
            Name = "N/A";
            Pos = new Point();
            Ascii = '?';
            BackgroundColor = ConsoleColor.Black;
            ForegroundColor = ConsoleColor.White;
            Id = id++;
        }

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
            foreach (var tag in Tags)
            {
                if (tag.Name == tagName)
                {
                    return tag;
                }
            }

            return null;
        }

        public Entity Clone()
        {
            Entity e = new Entity
            {
                Name = Name, Ascii = Ascii, BackgroundColor = BackgroundColor, ForegroundColor = ForegroundColor
            };
            foreach (Tag t in Tags)
            {
                e.Tags.Add(t.Clone());
            }

            return e;
        }

        public void Inherit(Entity e)
        {
        }
    }
}