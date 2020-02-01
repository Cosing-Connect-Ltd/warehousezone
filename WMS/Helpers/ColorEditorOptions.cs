namespace WMS.Helpers
{
    public enum Palette { Default, Gray, Red, Green, Blue }

    public class ColorEditorOptions
    {
        public ColorEditorOptions()
        {
            EnableCustomColors = true;
            ColumnCount = 10;
            Palettes = Palette.Default;
        }

        public bool EnableCustomColors { get; set; }
        public int ColumnCount { get; set; }
        public Palette Palettes { get; set; }
    }
}