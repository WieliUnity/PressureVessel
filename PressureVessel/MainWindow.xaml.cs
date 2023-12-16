using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace PressureVessel
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void BtnPressureVessel_Click(object sender, RoutedEventArgs e)
        {
            var pressureVesselControl = new PressureVesselControl();
            this.Content = pressureVesselControl; // Replace the entire window content
        }
    }
}
