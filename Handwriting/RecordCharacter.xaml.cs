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
    /// Interaction logic for RecordCharacter.xaml
    /// </summary>
    public partial class RecordCharacter : Window
    {
        private char character;
        private List<RelativeRoute> routes;
        public RecordCharacter(char c)
        {
            InitializeComponent();

            this.CharacterShower.Content = c.ToString();
            this.character = c;
            this.StartRequest();
        }
        public void StartRequest()
        {
            var thread = new Thread(new ThreadStart(() =>
            {
                routes = Server.GetInstance().RequestCharacterWithTcp(character);
                var action = new Action(() => {
                    this.Close();
                });
                this.Dispatcher.Invoke(action);
            }));
            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = true;
            thread.Start();
        }
        

        public static List<RelativeRoute> RequestCharacterRoute(char c)
        {
            var win = new RecordCharacter(c);
            win.ShowDialog();
            return optimizationRoute(win.routes);
        }

        private static List<RelativeRoute> optimizationRoute(List<RelativeRoute> list)
        {
            int minY = int.MaxValue;
            int minX = int.MaxValue;
            foreach(var route in list)
            {
                if (route.StartX < minX && route.StartX >= 0) minX = route.StartX;
                if (route.StartY < minY && route.StartY >= 0) minY = route.StartY;
                if (route.EndX < minX && route.EndX >= 0) minX = route.EndX;
                if (route.EndY < minY && route.EndY >= 0) minY = route.EndY;
            }
            return list.Select((RelativeRoute route) =>
            {
                return new RelativeRoute(
                    route.StartX - minX,
                    route.StartY - minY,
                    route.EndX - minX,
                    route.EndY - minY);

            }).ToList();
        }
    }
}
