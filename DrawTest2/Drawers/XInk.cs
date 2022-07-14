using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Shapes;
using DrawTest2.Helpers;

namespace DrawTest2.Drawers
{
    public class XInk : XShape, IShape
    {
        public InkCanvas Drawing;
        public List<Stroke> StrokesList;

        private int _currentIndex = 0;
        private bool IsHighlight = false;


        public XInk(Drawer drawer) : base(drawer)
        {
            Drawer = drawer;
        }

        //그리기위한 inkCanvas를 생성
        public void Create(Point e)
        {
            IsDrawing = true;
            StartPoint = e;

            Drawing             = new InkCanvas();
            Drawing.Width       = Drawer.Page.ActualWidth;
            Drawing.Height      = Drawer.Page.ActualHeight;
            Drawing.Tag         = this;
            Drawing.Background  = new SolidColorBrush(Colors.Transparent);
            StrokesList         = new List<Stroke>();

            var drawingAttributes = new DrawingAttributes();

            //============================이 부분 설정 부분으로 빼기
            if(Drawer.DrawTool == Tool.Ink)
            {
                drawingAttributes.Color             = Colors.Black;
                drawingAttributes.IgnorePressure    = false;
                drawingAttributes.FitToCurve        = true;
                drawingAttributes.StylusTip         = StylusTip.Ellipse;
                drawingAttributes.Width             = 4;
            }
            //============================이 부분 설정 부분으로 빼기**

            Drawing.DefaultDrawingAttributes = drawingAttributes;
            Drawing.StrokeCollected += Drawing_StrokeCollected;
            Drawing.StrokeErased += Drawing_StrokeErased;
            OwnedControl = new List<Border>();

            Style = new DrawerStyle();

            Drawer.Page.Children.Add(Drawing);
            Drawer.IsObjectCreating = true;
        }

        public void Update(Point e)
        {
            throw new NotImplementedException();
        }

        private void Drawing_StrokeCollected(object sender, InkCanvasStrokeCollectedEventArgs e)
        {
            StrokesList.Add(e.Stroke);
            _currentIndex = StrokesList.Count;
            Console.WriteLine("collected");
        }

        private void Drawing_StrokeErased(object sender, RoutedEventArgs e)
        {
            ArrangeStrokes();
        }

        private void ArrangeStrokes()
        {
            StrokesList.Clear();
            foreach (var s in Drawing.Strokes)
            {
                StrokesList.Add(s);
            }
            _currentIndex = StrokesList.Count;
        }

        //다른 메뉴를 선택 시
        public void Finish()
        {
            if (!IsDrawing || Drawing == null) return;

            IsDrawing = false;

            Drawer.IsObjectCreating = false;
            Drawer.IsDrawEnded = true;
            Drawer.Page.Children.Remove(Drawing);
            Drawer.ObjectsDic.Remove(id);

            var lst = Drawing.Strokes.ToList();

            //사실상 다시그리기
            foreach (var stroke in lst)
            {
                var geometry = stroke.GetGeometry(stroke.DrawingAttributes).GetOutlinedPathGeometry();

                var border = new Border();
                border.Background = new SolidColorBrush(Colors.Transparent); //그리기 끝났을 때 확인용도
                border.Width = stroke.GetBounds().Width;
                border.Height = stroke.GetBounds().Height;
                border.MouseLeftButtonDown += OnSelect;
                border.StylusDown += OnErase;
                border.Uid = Guid.NewGuid().ToString();

                var path = new Path();
                path.Data = geometry;
                path.VerticalAlignment = VerticalAlignment.Stretch;
                path.HorizontalAlignment = HorizontalAlignment.Stretch;

                if (!IsHighlight)
                    path.Fill = new SolidColorBrush(stroke.DrawingAttributes.Color);
                else
                    path.Fill = new SolidColorBrush(Color.FromArgb(127, 255, 255, 0));

                path.Stretch = Stretch.Fill;
                border.Tag = this;
                border.Child = path;
                Canvas.SetLeft(border, stroke.GetBounds().Left);
                Canvas.SetTop(border, stroke.GetBounds().Top);

                //형식이 is List<Border>인가 확인하고 oLst라는 이름으로 대체
                if(OwnedControl is List<Border> oLst)
                    oLst.Add(border);

                Drawer.ObjectsDic.Add(border.Uid, this);
                Drawer.Page.Children.Add(border);

                //Drawer.UndoHelper.AddStep(UndoHelper.ActionType.Create, border);
            }

        }
    }
}
