using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Text;
using System.Threading.Tasks;
using DrawTest2.Drawers;
using System.Windows.Controls;
using System.Windows.Input;

namespace DrawTest2.Helpers
{
    public class Drawer
    {
        public Selector Selector;

        public Dictionary<string, XShape> ObjectsDic;

        private Tool _drawTool;

        #region PROPERTIES
        public Tool DrawTool
        {
            get { return _drawTool; }
            set
            {
                _drawTool = value;

                if(value != Tool.Ink)
                    Selector.FinishDraw();

                if (value == Tool.Selection || value == Tool.None)
                    Selector.EndEditForObject();

                if(value == Tool.Ink)
                {
                    var o = new XInk(this);
                    ObjectsDic.Add(o.id, o);
                    //확장함수 참조
                    ObjectsDic.Last().Value.ToType<XInk>().Create(new Point());
                }
            }
        }

        private Canvas _page;
        public Canvas Page
        {
            get { return _page; }
        }
        #endregion


        public bool IsEditMode;
        public bool IsObjectCreating;
        public bool IsDrawEnded = true;

        public FrameworkElement ActiveObject;
        public Drawer instance;

        public Drawer() { }

        public Drawer(Canvas canvas)
        {
            instance = this;

            Selector = new Selector(this);

            Initialize(canvas);
        }

        private void Initialize(Canvas canvas)
        {
            Selector.Canvas = canvas;
            ObjectsDic = new Dictionary<string, XShape>();

            _page = canvas;
            _page.PreviewMouseLeftButtonDown    += _canvas_PreviewMouseLeftButtonDown;
            _page.MouseMove                     += _canvas_PreviewMouseMove;
            _page.PreviewMouseLeftButtonUp      += _canvas_PreviewMouseLeftButtonUp;
            var window = Application.Current.MainWindow;
            /*if (window != null)
            {
                window.KeyDown += P_PreviewKeyDown;
            }*/
        }


        private void P_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //HotKeyHelper.ExecuteShortcut();
        }


        //마우스 왼쪽 버튼 클릭 시
        private void _canvas_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ActiveObject = null;

            if (DrawTool == Tool.None) return;

            if (e.StylusDevice != null && e.StylusDevice.Inverted)
            {
                return;
            }

            if (DrawTool == Tool.Selection)
            {
                Selector.StartSelect(e.GetPosition(Page));
                Selector.EndEditForObject();
            }
            else if (DrawTool == Tool.MoveResize)
            {
                DrawTool = Tool.Selection;
                //AdornerHelper.RemoveAllAdorners();
                Selector.EndEditForObject();
            }
            else
            {
                IsEditMode = false;
                if (DrawTool != Tool.Text)
                {
                    Selector.EndEditForObject();
                }

                //StartDraw(e.GetPosition(Page));
            }
        }

        //마우스로 캔버스에 그리기 할 때
        private void _canvas_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (DrawTool == Tool.None) return;

            if (DrawTool != Tool.Selection && DrawTool != Tool.MoveResize)
            {
                //UpdateDraw(e.GetPosition(Page));
            }
            else
            {
                if (DrawTool == Tool.Selection)
                {
                    Selector.UpdateSelect(e.GetPosition(Page));
                }
            }
        }

        private void _canvas_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            FinishDraw();
        }

        public void FinishDraw()
        {
            //if (IsEditMode) return;
            if (!IsObjectCreating && Selector.IsDrawing == false) return;

            if (DrawTool == Tool.Selection)
            {
                Selector.FinishSelect();
            }
            else if (DrawTool == Tool.Rectangle)
            {
                //Objects.Last().Value.Finish();
            }
            else if (DrawTool == Tool.Ellipse)
            {
                //Objects.Last().Value.Finish();
            }
            else if (DrawTool == Tool.Triangle)
            {

            }
            else if (DrawTool == Tool.Line)
            {
                //Objects.Last().Value.OwnedShape.Tag.ToType<XLine>().Finish();
            }
            else if (DrawTool == Tool.Text)
            {
                /*if (!(Objects.Last().Value is XText txt)) return;

                if (IsEditMode == false)
                {
                    txt.Edit();
                    txt.Finish();
                }
                else
                {
                    DrawTool = Tool.Selection;
                }*/
            }
            else if (DrawTool == Tool.Arrow)
            {
                //Objects.Last().Value.OwnedShape.Tag.ToType<XArrow>().Finish();
            }
            else if (DrawTool == Tool.Custom)
            {
                //Objects.Last().Value.Finish();
            }
        }
    }
}
