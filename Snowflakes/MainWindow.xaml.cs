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
namespace Snowflakes
{
    public partial class MainWindow : Window
    {
        static int count = 50;// count
        System.Windows.Controls.Image[] snow = new System.Windows.Controls.Image[count];
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

        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Content = grid;
            int[] i_size = { 5,10,6, 8,7, 9, 11, 12};
            int j = 0;
            for (int i = 0; i < count; i++)
            {
                snow[i] = new System.Windows.Controls.Image();

                snow[i].Source = new BitmapImage(new Uri("snowflake.png", UriKind.Relative));
                snow[i].Width = i_size[j];
                if (j++ == i_size.Count()-1)
                    j = 0;
                grid.Children.Add(snow[i]);
            }
            await Move();
        }

        public async Task Move()
        {
            for (int i = 0; i < count; i++)
            {
                int left = random.Next(limitW_Left, limitW_Right);
                int top = random.Next(limitH_Top + limitH_Top + limitH_Top, limitH_Bottom + limitH_Top + limitH_Top);
                snow[i].Margin = new Thickness(left, top, 0, 0);
            }

            while (true)
            {
                for (int i = 0; i < count; i++)
                {
                    var left = snow[i].Margin.Left;
                    var top = snow[i].Margin.Top;
                    top += 2;
                    if (top > limitH_Bottom)
                    {
                        top = limitH_Top;
                        left = random.Next(limitW_Left, limitW_Right);
                    }
                    snow[i].Margin = new Thickness(left, top, 0, 0);
                }
                await Task.Delay(40);//speed
            }
        }
        private void Show(object sender, EventArgs e)
        {
            this.Visibility = System.Windows.Visibility.Visible;
            this.Activate();
        }

        private void Hide(object sender, EventArgs e)
        {
            this.Visibility = System.Windows.Visibility.Hidden;
        }

        private void Close(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
