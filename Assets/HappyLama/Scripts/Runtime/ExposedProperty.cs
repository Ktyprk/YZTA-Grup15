namespace HappyLama
{
    [System.Serializable]
    public class ExposedProperty
    {
        public enum PropertyType { String, Integer, Float, Boolean }

        public string PropertyName = "New Property";
        public PropertyType Type = PropertyType.Integer;
        public string StringValue = "";
        public int IntValue = 0;
        public float FloatValue = 0f;
        public bool BoolValue = false;

        public object PropertyValue { get; set; }

        public static ExposedProperty CreateInstance()
        {
            return new ExposedProperty();
        }
    }
}