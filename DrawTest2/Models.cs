using System.Windows.Input;
using System.Windows.Media;

namespace DrawTest2
{
    public enum Tool
    {
        None,
        Selection,
        Ink,
        Line,
        Rectangle,
        Ellipse,
        Text,
        MoveResize,
        Triangle,
        Arrow,
        Custom,
        Highlight,
        Pan
    }

    public class DrawerStyle
    {
        public Brush Background { get; set; }
        public Brush Border { get; set; }
        public double BorderSize { get; set; }
        public double FontSize { get; set; }
        public double Opacity { get; set; }

        public DrawerStyle(DrawerStyle style)
        {
            this.Background = style.Background;
            this.Border = style.Border;
            this.BorderSize = style.BorderSize;
            this.FontSize = style.FontSize;
            this.Opacity = style.Opacity;
        }

        public DrawerStyle()
        {

        }
    }

}
