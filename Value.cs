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
            switch (o)
            {
                case double d:
                    doubleValue = d;
                    break;
                case bool b:
                    boolValue = b;
                    break;
                case string s:
                    stringValue = s;
                    break;
            }
        }

        // Getters
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