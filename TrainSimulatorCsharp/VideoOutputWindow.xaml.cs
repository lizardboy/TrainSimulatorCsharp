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
using System.Windows.Shapes;

namespace TrainSimulatorCsharp
{

    public partial class VideoOutputWindow : Window
    {
        public VideoOutputWindow()
        {
            InitializeComponent();
        }

        private void testMediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            CabFootageVideo.Position = new TimeSpan(0);
            CabFootageVideo.Play();
        }

    }
}
