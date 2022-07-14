using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using DrawTest2.Drawers;
using Point = System.Windows.Point;
//using XText = DrawTest2.Drawers.XText;


namespace DrawTest2.Helpers
{
    public class Selector
    {
        public Canvas Canvas;
        public bool IsDrawing;
        public Drawer Drawer;

        private static Rectangle _rect;
        private static Point _startPoint;

        public Selector(Drawer drawer)
        {
            Drawer = drawer;
        }

        public void StartSelect(Point e)
        {
            if (Drawer.DrawTool == Tool.MoveResize) return;

            DeselectAll();

            _startPoint     = e;
            IsDrawing       = true;
            _rect           = new Rectangle();
            _rect.Width     = 0;
            _rect.Height    = 0;
            _rect.Fill      = StyleHelper.SelectionStyle.Background;
            _rect.Opacity   = StyleHelper.SelectionStyle.Opacity;
            _rect.Stroke    = StyleHelper.SelectionStyle.Border;
            _rect.Uid       = "x_selector";

            Canvas.SetLeft(_rect, e.X);
            Canvas.SetTop(_rect, e.Y);
            Canvas.Children.Add(_rect);

        }


        public void DeselectAll()
        {
            foreach (var item in Drawer.ObjectsDic.Values)
            {
                if (item.OwnedShape != null || item.OwnedControl != null)
                {
                    item.IsSelected = false;
                }
            }
        }

        public void EndEditForObject()
        {
            foreach (var item in Drawer.ObjectsDic.Values)
            {
                if (item.OwnedControl != null && item.OwnedControl is RichTextBox txt)
                {
                    //txt.Tag.ToType<XText>().EndEdit();
                }
            }
        }


        //도형이 왼쪽, 아래로 넘어갔을 때의 크기를 계산
        public void UpdateSelect(Point e)
        {
            if (!IsDrawing) return;

            var diffX = e.X - _startPoint.X;
            var diffY = e.Y - _startPoint.Y;
            var scaleX = 1;
            var scaleY = 1;

            if (diffX < 0)
            {
                scaleX = -1;
            }

            if (diffY < 0)
            {
                scaleY = -1;
            }

            _rect.RenderTransform = new ScaleTransform(scaleX, scaleY);

            _rect.Width = Math.Abs(diffX);
            _rect.Height = Math.Abs(diffY);
        }

        public void FinishSelect()
        {
            IsDrawing = false;
            //선택 시 확인하는 부분 같음
            //FindContainsObjects();
            Canvas.Children.Remove(_rect);

            _rect = null;
        }



        public void ArrangeObjects()
        {
            Drawer.ObjectsDic = new Dictionary<string, XShape>();
            foreach (FrameworkElement c in Drawer.Page.Children)
            {
                var obj = c.Tag;
                if (c.Tag != null)
                {
                    if (c.Tag.ToType<XShape>().OwnedControl is List<Border> borders)
                    {
                        foreach (var b in borders)
                        {
                            if (b.Uid == c.Uid)
                            {
                                Drawer.ObjectsDic.Add(b.Uid, c.Tag.ToType<XShape>());
                                break;
                            }
                        }
                    }
                    else
                    {
                        Drawer.ObjectsDic.Add(c.Tag.ToType<XShape>().id, c.Tag.ToType<XShape>());
                    }
                }
            }
        }

        /*private void FindContainsObjects()
        {
            DeselectAll();

            foreach (var item in Drawer.ObjectsDic.Values)
            {
                if (item.OwnedShape != null)
                {
                    var s = IsContains(item.OwnedShape);
                    if (s)
                    {
                        Identify(item);
                    }
                }
                else if (item.OwnedControl != null)
                {
                    if (item.OwnedControl is List<Border> inks)
                    {
                        foreach (var ink in inks)
                        {
                            var s = IsContains(ink);
                            if (s)
                            {
                                Identify(ink);
                            }
                        }
                    }
                    else
                    {
                        var s = IsContains((FrameworkElement)item.OwnedControl);
                        if (s)
                        {
                            Identify(item);
                        }
                    }
                }
            }
        }*/

        /*private bool IsContains(FrameworkElement item)
        {
            if (item == null) return false;
            if (_rect == null) return false;

            if(item.Tag is XLine line)
            {
                var xScale = (double)_rect.RenderTransform.GetValue(ScaleTransform.ScaleXProperty);
                var yScale = (double)_rect.RenderTransform.GetValue(ScaleTransform.ScaleYProperty);

                var x1 = Canvas.GetLeft(_rect);
                var y1 = Canvas.GetTop(_rect);


            }
        }*/



        public void DeleteObject(FrameworkElement ctrl)
        {
            var o = ctrl.Tag.ToType<XShape>();

            if (o.OwnedShape != null)
            {
                var a = (UIElement)ctrl;
                Drawer.Page.Children.Remove(a);
            }

            if (o.OwnedControl != null)
            {
                if (o.OwnedControl is List<Border> borders)
                {
                    foreach (var b in borders)
                    {
                        if (ctrl.Uid == b.Uid)
                        {
                            Drawer.Page.Children.Remove(b);
                            Drawer.ObjectsDic.Remove(b.Uid);
                            break;
                        }
                    }
                }
                else
                {
                    Drawer.Page.Children.Remove((UIElement)o.OwnedControl);
                    Drawer.ObjectsDic.Remove(o.id);
                }
            }

            if (o.FollowItem != null)
            {
                Drawer.Page.Children.Remove(o.FollowItem);
            }

            ArrangeObjects();
        }



        public void FinishDraw()
        {
            var lst = Drawer.ObjectsDic.ToList();

            foreach(KeyValuePair<string, XShape> item in lst)
            {
                if (item.Value.OwnedControl == null)
                    continue;

                if (item.Value.OwnedControl is List<Border> b)
                    (item.Value as XInk)?.Finish();
            }
        }

    }
}
