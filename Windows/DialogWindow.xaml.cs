using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace ScreenGrab.Windows {
    /// <summary>
    /// Interaction logic for DialogWindow.xaml
    /// </summary>
    public partial class DialogWindow : INotifyPropertyChanged {
        public DialogWindow() {
            _closeCommand = new CommandBinding(ApplicationCommands.Close, Executed);
            InitializeComponent();
            CommandBindings.Add(_closeCommand);
            DataContext = this;
        }

        private CommandBinding _closeCommand;
        private void Executed(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs) {
            Close();
        }

        public bool AnimateToCenterOnSizeChanged { get; set; }

/*
        public DialogWindow(string regionToShow)
            : this() {
            _regionToShow = regionToShow;
        }

        private string _regionToShow;
        public string RegionToShow {
            get { return _regionToShow; }
            set {
                _regionToShow = value;
                PropertyChanged(this, new PropertyChangedEventArgs("RegionToShow"));
            }
        }
*/


        private void Window_SizeChanged(object sender, SizeChangedEventArgs e) {

            if (!AnimateToCenterOnSizeChanged) { return; }

            var ctrl = (Window)sender;
            var ds = ctrl.DesiredSize;
            var mw = Application.Current.MainWindow;
            double newX = mw.Left + ((mw.ActualWidth - ds.Width) / 2);
            double newY = (mw.ActualHeight - ds.Height) / 2;
            AnimateSizeChanged((Window)sender, newX, newY);
        }

        private void AnimateClear(Window wdw) {
            wdw.BeginAnimation(TopProperty, null);
        }

        private DoubleAnimation anim;
        private void AnimateSizeChanged(Window wdw, double newX, double newY) {
            AnimateClear(wdw);
            wdw.Left = newX;

            Storyboard sb = new Storyboard();
            TimeSpan duration = TimeSpan.FromMilliseconds(50);

            anim = new DoubleAnimation { From = wdw.Top, To = newY, Duration = new Duration(duration) };
            anim.Completed += anim_Completed;
            Storyboard.SetTargetProperty(anim, new PropertyPath("Top", newY));
            sb.Children.Add(anim);
            sb.Begin(wdw);
        }

        void anim_Completed(object sender, EventArgs e) {
            anim.Completed -= anim_Completed;
        }

        private void Window_Closing(object sender, CancelEventArgs e) {
            CommandBindings.Remove(_closeCommand);
        }

        public void DragWindow(object sender, MouseButtonEventArgs args) {
            DragMove();
        }

        protected override void OnPreviewKeyUp(KeyEventArgs e) {
            if (e.Key == Key.Escape) {
                Close();
            } else {
                base.OnPreviewKeyUp(e);
            }
        }

/*
        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged = delegate { };


        #endregion
*/
    }
}
