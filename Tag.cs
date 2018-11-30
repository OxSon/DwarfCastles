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

        public Tag GetTag(string TagChain)
        {
            string[] Tags = TagChain.Split('.');
            Tag CurrentTag = this;
            foreach (var s in Tags)
            {
                bool Found = false;
                foreach (var tag in CurrentTag.SubTags)
                {
                    if (tag.Name != s)
                        continue;
                    CurrentTag = tag;
                    Found = true;
                    break;
                }

                if (!Found)
                {
                    return null;
                }
            }

            return CurrentTag;
        }
        

        public void AddTag(Tag tag)
        {
            if (GetTag(tag.Name) != null)
            {
                for (int i = 0; i < SubTags.Count; i++)
                {
                    if (SubTags[i].Name == tag.Name)
                    {
                        SubTags.RemoveAt(i);
                    }
                }
            }
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

        public override string ToString()
        {
            string builtString = Name + "\n";
            foreach (var t in SubTags)
            {
                builtString += t.ToString() + "\n";
            }

            foreach (var v in ArrayValues)
            {
                builtString += v + "\n";
            }

            builtString += Value + "\n";
            return builtString;
        }
    }
}