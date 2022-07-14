using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using DrawTest2.Helpers;

namespace DrawTest2.Drawers
{
    public class XShape
    {
        protected static object instance;

        internal bool IsDrawing;
        internal Point StartPoint;
        internal FrameworkElement FollowItem;

        public string id;
        public object OwnedControl;
        public Shape OwnedShape;
        public Drawer Drawer;

        private bool _isSelected;
        private DrawerStyle _style;


        public DrawerStyle Style
        {
            get => _style;
            set
            {
                _style = value;

                if (OwnedShape != null)
                {
                    OwnedShape.Stroke = _style.Border;
                    OwnedShape.StrokeThickness = _style.BorderSize;
                    OwnedShape.Opacity = _style.Opacity;
                    OwnedShape.Fill = _style.Background;
                }

                if (OwnedControl != null)
                {
                    if (OwnedControl is RichTextBox txt)
                    {
                        txt.BorderBrush = _style.Border;
                        txt.BorderThickness = new Thickness(_style.BorderSize);
                        txt.Opacity = _style.Opacity;
                        txt.Background = _style.Background;
                        txt.FontSize = _style.FontSize;
                    }
                }
            }
        }

        public XShape(Drawer drawer)
        {
            Drawer = drawer;

            id = Guid.NewGuid().ToString();
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;

                if (value)
                {
                    if (OwnedShape != null)
                    {
                        OwnedShape.Stroke = new SolidColorBrush(Colors.Aqua);
                        OwnedShape.StrokeThickness = 3;
                    }

                    if (OwnedControl != null)
                    {
                        if (OwnedControl is RichTextBox o)
                        {
                            o.BorderBrush = new SolidColorBrush(Colors.Aqua);
                            o.BorderThickness = new Thickness(3);
                        }
                    }
                }
                else
                {
                    if (OwnedShape != null)
                    {
                        OwnedShape.Stroke = _style.Border;
                        OwnedShape.StrokeThickness = _style.BorderSize;
                    }

                    if (OwnedControl != null)
                    {
                        if (OwnedControl is RichTextBox o)
                        {
                            o.BorderBrush = _style.Border;
                            o.BorderThickness = new Thickness(_style.BorderSize);
                        }
                    }
                    //Style = _style;
                }
            }
        }

        public void OnSelect(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is Polygon && IsDrawing)
            {
                e.Handled = false;
            }
            else
            {
                if (sender is RichTextBox)
                {
                    e.Handled = true;
                }
                else
                {
                    if (Drawer.DrawTool != Tool.Selection && Drawer.DrawTool != Tool.MoveResize)
                    {
                        e.Handled = true;
                    }
                }
            }

            if (Drawer.DrawTool != Tool.Selection && Drawer.DrawTool != Tool.MoveResize) return;

            if (Drawer.IsEditMode) return;

            //선택한 객체 태두리로 잡는 기능 지우기
            //Drawer.AdornerHelper.RemoveAllAdorners();
            Drawer.Selector.DeselectAll();

            IsSelected = true;

            /* (FollowItem != null)
            {
                Drawer.AdornerHelper.AddAdorner(sender, this);
            }
            else
            {
                Drawer.AdornerHelper.AddAdorner(sender);
            }*/

            if (OwnedShape != null)
            {
                Drawer.ActiveObject = OwnedShape;
            }

            if (OwnedControl != null)
            {
                if (OwnedControl is List<Border> borders)
                {
                    var c = (FrameworkElement)sender;

                    foreach (var b in borders)
                    {
                        if (b.Uid == c.Uid)
                        {
                            Drawer.ActiveObject = b;
                            break;
                        }
                    }
                }
                else
                {
                    Drawer.ActiveObject = (FrameworkElement)OwnedControl;
                }
            }
        }

        internal void OnErase(object sender, System.Windows.Input.StylusEventArgs e)
        {
            if (e.Inverted)
            {
                Drawer.Selector.DeleteObject(OwnedShape);
            }
        }
    }
}
