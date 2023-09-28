using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Forms;
using System.Drawing;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Media.Effects;
using System.Collections;

namespace Snowflakes
{
    public class SnowNode {
        public System.Windows.Controls.Image snowflake;
        public bool on_the_ground;
        public bool is_valid;

        public SnowNode(System.Windows.Controls.Image img, bool ground, bool isvalid)
        {
            snowflake = img;
            on_the_ground = ground;
            is_valid = isvalid;
        }
    }
      

    public partial class MainWindow : Window
    {
        static int snow_count = 200;
        int snow_size_min = 5;
        int snow_size_max = 12;
        
        SnowNode[] snow_list = new SnowNode[snow_count];

        // System.Windows.Controls.Image[] snow = new System.Windows.Controls.Image[snow_count];
        // bool[] on_the_ground = new bool[snow_count];
        Random random = new Random();
        Grid grid = new Grid();

        int screenH;
        int screenW;

        int limitH_Top;
        int limitH_Bottom;
        int limitW_Left;
        int limitW_Right;

        private NotifyIcon notifyIcon;

        public MainWindow()
        {
            InitializeComponent();

            screenH = (int)SystemParameters.PrimaryScreenHeight;
            screenW = (int)SystemParameters.PrimaryScreenWidth;
            limitH_Top = 0 - screenH - 20;
            limitH_Bottom = screenH + 20;
            limitW_Left = 0 - screenW + 20;
            limitW_Right = screenW - 20;
            WindowState ws = WindowState;

            this.notifyIcon = new NotifyIcon();
            this.notifyIcon.BalloonTipText = "wow,snowing... ...";
            this.notifyIcon.ShowBalloonTip(2000);
            this.notifyIcon.Text = "wow,snowing... ...";
            //this.notifyIcon.Icon = new System.Drawing.Icon(@"AppIcon.ico");
            this.notifyIcon.Visible = true;
            this.notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);

            System.Windows.Forms.MenuItem open = new System.Windows.Forms.MenuItem("show");
            open.Click += new EventHandler(Show);
            System.Windows.Forms.MenuItem hide = new System.Windows.Forms.MenuItem("hide");
            hide.Click += new EventHandler(Hide);
            System.Windows.Forms.MenuItem exit = new System.Windows.Forms.MenuItem("exit");
            exit.Click += new EventHandler(Close);

            System.Windows.Forms.MenuItem[] childen = new System.Windows.Forms.MenuItem[] { open, hide, exit };//reg function
            notifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu(childen);

            this.notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler((o, e) =>
            {
                if (e.Button == MouseButtons.Left) this.Show(o, e);
            });
            this.ShowInTaskbar = false;

            // TODO: hide snowflakes when fullscreen_app exist
            //System.Threading.Timer timer = new System.Threading.Timer(CheckFullScreenApp, null, 100, 3 * 1000);
            //timer.Change(100, 3 * 1000);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Content = grid;

            for (int i = 0; i < snow_count; i++)
            {
                System.Windows.Controls.Image imgnode = new System.Windows.Controls.Image
                {
                    Source = new BitmapImage(new Uri("snowflake.png", UriKind.Relative)),
                    Width = random.Next(snow_size_min, snow_size_max),
                    Margin = new Thickness(
                            random.Next(limitW_Left, limitW_Right),
                            random.Next(limitH_Top + limitH_Top + limitH_Top, limitH_Bottom + limitH_Top + limitH_Top),
                            0,
                            0),
                    //Effect = new BlurEffect // high GPU usage, i suggest retouch the PNG instead
                    //{
                    //    KernelType = KernelType.Gaussian,
                    //    Radius = GetRandomDoubleNum(2, 4),
                    //    RenderingBias = RenderingBias.Performance
                    //},
                };
                bool valid = false;
                if (i < snow_count / 2)
                {
                    grid.Children.Add(imgnode);
                    valid = true;
                }
                snow_list[i] = new SnowNode(imgnode, false, valid);
            }
            await Move();
        }

        public async Task Move()
        {
            while (true)
            {
                foreach (SnowNode snownode in snow_list)
                {
                    if (snownode.on_the_ground == true || snownode.is_valid == false)
                    {
                        continue;
                    }
                    var left = snownode.snowflake.Margin.Left;
                    var top = snownode.snowflake.Margin.Top;
                    if ( top + random.Next(10, 50) > limitH_Bottom)
                    {
                       
                        DelayAction(60*1000, snownode);
                    }
                    else
                    {
                        snownode.snowflake.Margin = new Thickness(left, top+2, 0, 0);
                    }
                }
                await Task.Delay(40); // speed
            }
        }
        private void Show(object sender, EventArgs e)
        {
            this.Visibility = System.Windows.Visibility.Visible;
            this.Activate();
        }

        private void Hide(object sender, EventArgs e)
        {
            this.Hide();
            //this.Visibility = System.Windows.Visibility.Hidden;
        }

        private void Close(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        public void DelayAction(int millisecond, SnowNode s)
        {
            s.on_the_ground = true;
            AddSnowflake(true);
          
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Tick += delegate
            {
                RemoveSnowflake(s);
                timer.Stop();
            };

            timer.Interval = TimeSpan.FromMilliseconds(millisecond);
            timer.Start();
        }

        public void AddSnowflake(bool valid ) {

            foreach (SnowNode n in snow_list)
            {
                if (n.is_valid == false && n.on_the_ground == false)
                {
                    var left = random.Next(limitW_Left, limitW_Right);
                    n.snowflake.Margin = new Thickness(left, limitH_Top, 0, 0);
                    grid.Children.Add(n.snowflake);
                    n.is_valid = true;
                    return;
                }
            }


        }

        public void RemoveSnowflake(SnowNode s)
        {
            grid.Children.Remove(s.snowflake);
            s.is_valid = false;
            s.on_the_ground = false;
        }

        //public double GetRandomDoubleNum(double minimum, double maximum)
        //{
        //    return random.NextDouble() * (maximum - minimum) + minimum;
        //}


        // TODO: hide snowflakes when fullscreen_app exist
        //[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        //private struct RECT
        //{
        //    public int left;
        //    public int top;
        //    public int right;
        //    public int bottom;
        //}

        //[System.Runtime.InteropServices.DllImport("user32.dll")]
        //private static extern bool GetWindowRect(System.Runtime.InteropServices.HandleRef hWnd, [System.Runtime.InteropServices.In, System.Runtime.InteropServices.Out] ref RECT rect);

        //[System.Runtime.InteropServices.DllImport("user32.dll")]
        //private static extern IntPtr GetForegroundWindow();

        //public static bool IsForegroundFullScreen()
        //{
        //    return IsForegroundFullScreen(null);
        //}

        //public static bool IsForegroundFullScreen(Screen screen)
        //{
        //    if (screen == null)
        //    {
        //        screen = Screen.PrimaryScreen;
        //    }
        //    RECT rect = new RECT();
        //    GetWindowRect(new System.Runtime.InteropServices.HandleRef(null, GetForegroundWindow()), ref rect);
        //    return new System.Drawing.Rectangle(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top).Contains(screen.Bounds);
        //}


        //public void CheckFullScreenApp(object state)
        //{
        //    var xx = IsForegroundFullScreen(null);
        //    var aa = this.Visibility;
        //    int i = 2;
        //    if (aa != System.Windows.Visibility.Hidden )
        //    {
        //        this.Visibility = System.Windows.Visibility.Hidden;
        //        //this.Hide();
        //    }
        //    //else if (this.Visibility == System.Windows.Visibility.Hidden)
        //    //{
        //    //    this.Visibility = System.Windows.Visibility.Visible;
        //    //    this.Activate();
        //    //}
        //    return;
        //}

    }

}


