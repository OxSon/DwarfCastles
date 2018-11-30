namespace DwarfFortress
{
    public class Value
    {
        private double doubleValue;
        private bool boolValue;
        private string stringValue;
        
        // Setters
        public void setValue(double d)
        {
            doubleValue = d;
        }
        public void setValue(string s)
        {
            stringValue = s;
        }
        public void setValue(bool b)
        {
            boolValue = b;
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
        
        // Utility Methods
        public Value Clone()
        {
            Value clone = new Value {boolValue = boolValue, doubleValue = doubleValue, stringValue = stringValue};
            return clone;
        }
    }
}