using System.Collections.Generic;

namespace DwarfFortress
{
    /// <summary>
    /// Authors: Alec Mills and Josh DeMoss
    /// This class is an organizational tool for all objects to be able to
    /// store and retrieve diverse data without changing the class structure.
    /// </summary>
    public class Tag
    {
        public string Name { get; private set; }

        // These are possible values of tags. Only one will be used at a time
        // This fits the JSON format of value, array, or new object
        // TODO possibly make this only use what's needed
        public Value Value { get; set; }
        public IList<Tag> SubTags { get; }
        public IList<Value> ArrayValues { get; }

        #region Constructors

        public Tag()
        {
            Value = new Value();
            SubTags = new List<Tag>();
            ArrayValues = new List<Value>();
        }

        public Tag(string name) : this()
        {
            Name = name.ToLower();
        }

        public Tag(string name, bool value) : this(name)
        {
            Value.setValue(value);
        }

        public Tag(string name, double value) : this(name)
        {
            Value.setValue(value);
        }

        public Tag(string name, string value) : this(name)
        {
            Value.setValue(value);
        }

        #endregion Constructors

        #region Convienience Methods

        public void AddArrayValue(Value v)
        {
            ArrayValues.Add(v);
        }

        public Tag GetTag(string TagName)
        {
            foreach (var tag in SubTags)
            {
                if (tag.Name == TagName)
                {
                    return tag;
                }
            }
        }

        public void AddTag(Tag tag)
        {
            SubTags.Add(tag);
        }

        #endregion Convienience Methods

        public Tag Clone()
        {
            Tag clone = new Tag {Name = Name, Value = Value.Clone()};
            foreach (var subTag in SubTags)
            {
                clone.AddTag(subTag.Clone());
            }

            foreach (var Value in ArrayValues)
            {
                clone.AddArrayValue(Value.Clone());
            }

            return clone;
        }
    }
}