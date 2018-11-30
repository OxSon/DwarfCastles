namespace DwarfFortress
{
    public class Value
    {
        private double doubleValue;
        private bool boolValue;
        private string stringValue;

        public Value(){}
        
        public Value(object o)
        {
            setValue(o);
        }
        
        // Setters
        public void setValue(object o)
        {
            if (o.GetType() == typeof(double))
            {
                doubleValue = (double) o;
            }else if (o.GetType() == typeof(bool))
            {
                boolValue = (bool) o;
            }else if (o.GetType() == typeof(string))
            {
                stringValue = (string) o;
            }
        }
        
        // Getters
        public void getValue(out double outputDouble)
        {
            outputDouble = doubleValue;
        }
        public void getValue(out string outputString)
        {
            outputString = stringValue;
        }
        public void getValue(out bool outputBool)
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
            Value clone = new Value {boolValue = boolValue, doubleValue = doubleValue, stringValue = stringValue};
            return clone;
        }
    }
}