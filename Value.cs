namespace DwarfCastles
{
    public class Value
    {
        private double doubleValue;
        private bool boolValue;
        private string stringValue;

        public Value()
        {
        }

        public Value(object o)
        {
            SetValue(o);
        }

        // Setters
        public void SetValue(object o)
        {
            if (o is double d)
            {
                doubleValue = d;
            }
            else if (o is bool b)
            {
                boolValue = b;
            }
            else if (o is string s)
            {
                stringValue = s;
            }
        }

        // Getters
        public void GetValue(out double outputDouble)
        {
            outputDouble = doubleValue;
        }

        public void GetValue(out string outputString)
        {
            outputString = stringValue;
        }

        public void GetValue(out bool outputBool)
        {
            outputBool = boolValue;
        }

        public bool GetBool()
        {
            return boolValue;
        }

        public string GetString()
        {
            return stringValue;
        }

        public double GetDouble()
        {
            return doubleValue;
        }

        public override string ToString()
        {
            return $"Value ({boolValue}, {stringValue}, {doubleValue})";
        }

        // Utility Methods
        public Value Clone()
        {
            var clone = new Value {boolValue = boolValue, doubleValue = doubleValue, stringValue = stringValue};
            return clone;
        }
    }
}