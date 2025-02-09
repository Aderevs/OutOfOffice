namespace OutOfOffice.Attributes
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    sealed class DisplayJsonAttribute : Attribute
    {
        public string DisplayedJson { get; }
        public DisplayJsonAttribute(string displayedJson)
        {
            DisplayedJson = displayedJson;
        }

        public string GetValue()
        {
            return DisplayedJson;
        }
    }
}
