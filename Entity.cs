using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace DwarfCastles
{
    /// <summary>
    /// Authors: Alec Mills & Josh DeMoss
    ///
    /// Represents a physical entity in game world
    /// </summary>
    public class Entity
    {
        protected readonly Tag t;
        protected IEnumerable<Tag> Tags => t.SubTags;

        public string Name { get; set; }
        public string Display { get; set; }
        public Point Pos { get; set; }
        public int Id { get; }
        protected static int id;

        //console 'drawing' related values
        public char Ascii { get; set; } //ascii character used to 'draw' entity
        public ConsoleColor BackgroundColor { get; set; }
        public ConsoleColor ForegroundColor { get; set; }

        public bool Locked;

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

        public void AddTag(Tag tag)
        {
            t.AddTag(tag);
        }

        /// <summary>
        /// gets value of tag queried if applicable
        /// </summary>
        /// <param name="tagName">tag to query</param>
        /// <returns>value of Tag, or null if Tag is not present in Entity's Tag-list</returns>
        public Tag GetTag(string tagName)
        {
            return t.GetTag(tagName);
        }

        public virtual Entity Clone()
        {
            var e = new Entity
            {
                Name = Name, Ascii = Ascii, BackgroundColor = BackgroundColor, ForegroundColor = ForegroundColor
            };
            foreach (var tag in Tags)
            {
                e.t.AddTag(tag.Clone());
            }

            return e;
        }

        public bool HasType(string s)
        {
            var typeTag = GetTag("types");
            return typeTag != null && typeTag.ArrayValues.Any(t => t.GetString() == s);
        }

        public override string ToString()
        {
            return t.ToString();
        }

        public string ToString(params string[] properties)
        {
            var results = properties.Select(prop => GetType().GetProperty(prop)?.GetValue(this, null)).ToList();

            var sb = new StringBuilder();
            var i = 0;
            
            foreach (var prop in results)
            {
                sb.Append($"{properties[i]}: {results[i]}; ");
                i++;
            }
            return sb.ToString();
        }

        public void Inherit(Entity e)
        {
            Name = e.Name;
            Ascii = e.Ascii;
            BackgroundColor = e.BackgroundColor;
            ForegroundColor = e.ForegroundColor;
            foreach (var tag in e.Tags)
            {
                t.AddTag(tag.Clone());
            }
        }
    }
}