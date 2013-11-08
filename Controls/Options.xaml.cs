using System.Windows;
using System.Windows.Controls;
using ScreenGrab.Interfaces;

namespace ScreenGrab.Controls {
    /// <summary>
    /// Interaction logic for Options.xaml
    /// </summary>
    public partial class Options : IHostedControl {
        public Options() {
            InitializeComponent();
            DataContext = Properties.Settings.Default;
        }

        public Window ParentWindow { get; set; }
    }
}
