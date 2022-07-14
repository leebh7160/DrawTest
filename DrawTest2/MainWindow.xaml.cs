using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DrawTest2.Drawers;
using DrawTest2.Helpers;


namespace DrawTest2
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public Drawer Drawer;

        public MainWindow()
        {
            InitializeComponent();

            Drawer = new Drawer(MainCanvas);

            BtnNone.Click += delegate (object sender, RoutedEventArgs args)
            {
                Drawer.DrawTool = Tool.Selection;
            };

            BtnInk.Click += delegate (object sender, RoutedEventArgs args)
            {
                Drawer.DrawTool = Tool.Ink;
            };
        }
    }
}
