namespace BarangayApplication.Models
{
    // Generic ComboBox item for value/text
    public class ComboBoxItem<T>
    {
        public string Text { get; }
        public T Value { get; }
        public ComboBoxItem(string text, T value)
        {
            Text = text;
            Value = value;
        }
        public override string ToString() => Text;
    }
}