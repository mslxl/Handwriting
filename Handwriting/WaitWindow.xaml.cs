using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Threading;
namespace Handwriting
{
    /// <summary>
    /// Interaction logic for WaitWindow.xaml
    /// </summary>
    public partial class WaitWindow : Window
    {
        Thread UiThread;

        public WaitWindow()
        {
            InitializeComponent();
            this.StartWait();
        } 

        private void StartWait()
        {
            this.Show();
            UiThread = new Thread(new ThreadStart(()=> {
                Server.GetInstance();
                var action = new Action(()=> {
                    this.Hide();
                    new MainWindow().ShowDialog();
                    this.Close();
                    
                });
                this.Dispatcher.Invoke(action);
            }));
            UiThread.SetApartmentState(ApartmentState.STA);
            UiThread.IsBackground = true;
            UiThread.Start();
        }
    }
}
