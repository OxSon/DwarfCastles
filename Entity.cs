using System;
using System.Collections.Generic;
using System.Drawing;

namespace DwarfCastles
{
    /// <summary>
    /// Authors: Alec Mills & Josh DeMoss
    ///
    /// Represents a physical entity in game world
    /// </summary>
    public class Entity
    {
        private Tag t;
        public IList<Tag> Tags => t.SubTags;

        public string Name { get; set; }
        public string Display { get; set; }
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
            t = new Tag();
        }

        /// </summary>
        /// <param name="name"></param>
        /// <param name="pos"></param>
        /// <param name="ascii"></param>
        /// <param name="backgroundColor"> Optional; default = black </param>
        /// <param name="foregroundColor"> Optional; default = white </param>
        public Entity(string name, Point pos, char ascii, //TODO Remove constructor TESTING
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

        public override string ToString()
        {
            return t.ToString();
        }

        public void Inherit(Entity e)
        {
            Name = e.Name;
            Ascii = e.Ascii;
            BackgroundColor = e.BackgroundColor;
            ForegroundColor = e.ForegroundColor;
            foreach (var t in e.Tags)
            {
                Tags.Add(t.Clone());
            }
        }
    }
}