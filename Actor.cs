using System;
using System.Collections.Generic;

namespace DwarfFortress
{
    /// <inheritdoc />
    public class Actor : Entity
    {
        //TODO Josh what is this? needs more functionality I assume?
        public IList<Task> Tasks { get; }

        public Actor(string name, Point pos, char ascii,
            ConsoleColor backgroundColor = ConsoleColor.Black,
            ConsoleColor foregroundColor = ConsoleColor.White) :
            base(name, pos, ascii, backgroundColor, foregroundColor) { }
    }
}