using System;
using System.Collections.Generic;

namespace DwarfFortress
{
    /// <summary>
    /// Authors: Alec Mills and Josh DeMoss
    ///
    /// TODO josh can you write a better doc description for this class?
    /// </summary>
    public class Tag
    {
        private string val;

        public string Value
        {
            get => val;
            private set
            {
                if (value != null) val = value;
                else throw new ArgumentException();
            }
        }

        public string Name { get; }
        public Dictionary<string, Tag> SubTags { get; }
    }
}