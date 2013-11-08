using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using ScreenGrab.AsyncTasks;
using ScreenGrab.ClickOnce;
using ScreenGrab.Controls;
using ScreenGrab.Interfaces;
using ScreenGrab.Properties;
using Application = System.Windows.Application;
using DataObject = System.Windows.Forms.DataObject;
using IDataObject = System.Windows.Forms.IDataObject;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using ListBox = System.Windows.Controls.ListBox;
using MessageBox = System.Windows.MessageBox;
using Point = System.Windows.Point;

namespace ScreenGrab.Windows {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged {
        public MainWindow() {
            InitializeComponent();
            Settings.Default.Upgrade();

            RegisterCommandBindings();
            Zoom = 1;
            //InitialiseAvailableHighlights();

            DataContext = this;
            //Title = "ScreenGrab " + ClickOnceHelper.CurrentVersion;
            Title = "ScreenGrab";

            //Properties.Settings.Default.PropertyChanged += new PropertyChangedEventHandler(Default_PropertyChanged);

            Closing += MainWindowClosing;

        }

        protected override void OnSourceInitialized(EventArgs e) {
            base.OnSourceInitialized(e);
            this.SetPlacement(Settings.Default.MainWindowPlacement);
        }

        void MainWindowClosing(object sender, CancelEventArgs e) {
            Settings.Default.MainWindowPlacement = this.GetPlacement();
            Settings.Default.Save();
        }

        void RegisterCommandBindings() {
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Delete, DeleteSelectedGrab));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, SaveSelectedGrab));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, CopySelectedGrab));

            //RoutedUICommand hl = new RoutedUICommand("Highlight", "HighlightCommand", typeof(MainWindow));
            CommandBindings.Add(new CommandBinding(ScreenGrabCommands.HighlightActivateCommand, HighlightToggle));
            CommandBindings.Add(new CommandBinding(ScreenGrabCommands.UserPreferences, UserPreferencesExecuted));
            CommandBindings.Add(new CommandBinding(ScreenGrabCommands.NewScreenGrab, NewScreenGrabExecuted, NewScreenGrabCanExecute));
            CommandBindings.Add(new CommandBinding(ScreenGrabCommands.RedoScreenGrab, RedoScreenGrabExecuted, RedoScreenGrabCanExecute));
            CommandBindings.Add(new CommandBinding(ScreenGrabCommands.ClearHighlights, ClearHighlightsExecuted, ClearHighlightsCanExecute));
            //CommandBindings.Add(new CommandBinding(ClickOnceCommands.CheckForUpdatesCommand, CheckForUpdatesExecuted, CheckForUpdatesCanExecute));
        }

        private void UserPreferencesExecuted(object sender, ExecutedRoutedEventArgs e) {
            Options options = new Options();
            ShowPopup(options, "Options", true, true);
        }

        private void ClearHighlightsCanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = true;
        }

        private void ClearHighlightsExecuted(object sender, ExecutedRoutedEventArgs e) {
            _canvasWithOverlay.RemoveAllHighlights();
            ((ScreenCapture) _images.SelectedItem).Highlights = null;
        }

        private void RedoScreenGrabCanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = _images.SelectedItem != null;
        }

        private void RedoScreenGrabExecuted(object sender, ExecutedRoutedEventArgs e) {
            //throw new NotImplementedException();
            ScreenCapture sel = _images.SelectedItem as ScreenCapture;
            if (sel == null) {
                return;
            }

            if ((Keyboard.Modifiers & ModifierKeys.Alt) == 0) {
                Visibility = Visibility.Hidden;
            }

            GrabDesktopImageTask screenGrabTask = new GrabDesktopImageTask(sel.SourceScreen);
            screenGrabTask.Finished += RedoTaskFinished;
            screenGrabTask.Run();

        }

        private void RedoTaskFinished(object sender, EventArgs e) {

            ScreenCapture imageToClone = _images.SelectedItem as ScreenCapture;
            var grabbedImage = new ScreenCapture(((GrabDesktopImageTask)sender).Image);

            int x = imageToClone.SourcePosition.X;
            int y = imageToClone.SourcePosition.Y;

            Int32Rect r = new Int32Rect(x, y, imageToClone.SourcePosition.Width, imageToClone.SourcePosition.Height);
            CroppedBitmap crop = new CroppedBitmap(grabbedImage.Source as BitmapSource, r);
            var img = new ScreenCapture(crop) { SourcePosition = imageToClone.SourcePosition, SourceScreen = imageToClone.SourceScreen };
            Images.Add(img);
            Visibility = Visibility.Visible;
        }

        private void NewScreenGrabCanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = AvailableScreens.SelectedItem != null;
        }

        private void NewScreenGrabExecuted(object sender, ExecutedRoutedEventArgs e) {
            //if ((Keyboard.Modifiers & ModifierKeys.Alt) == 0) {
            Visibility = Visibility.Hidden;
            //}

            GrabDesktopImageTask screenGrabTask = new GrabDesktopImageTask(AvailableScreens.SelectedItem as Screen);
            screenGrabTask.Finished += ScreenGrabTaskFinished;
            screenGrabTask.Run();

        }

        void ScreenGrabTaskFinished(object sender, EventArgs e) {

            GrabDesktopImageTask task = sender as GrabDesktopImageTask;

            DesktopImage win = new DesktopImage {
                Image = ((GrabDesktopImageTask)sender).Image,
                SelectOnMouseUp = true
            };

            Rectangle bounds = new Rectangle();
            if (KeyboardHelper.IsModifierKeyDown) {
                if (KeyboardHelper.IsCtrlDown) {
                    bounds = task.SelectedScreen.Bounds;
                } else if (KeyboardHelper.IsShiftDown) {
                    bounds = Screen.FromHandle(new WindowInteropHelper(this).Handle).Bounds;
                } else if (KeyboardHelper.IsAltDown) { //primary
                    bounds = Screen.PrimaryScreen.Bounds;
                }

            }
            if (bounds.IsEmpty) {
                if (Settings.Default.ShowGrabOnCapturedScreen) {
                    bounds = task.SelectedScreen.Bounds;
                } else if (Settings.Default.ShowGrabOnActiveScreen) {
                    bounds = Screen.FromHandle(new WindowInteropHelper(this).Handle).Bounds;
                } else { //primary
                    bounds = Screen.PrimaryScreen.Bounds;
                }
            }

            win.Width = bounds.Width;
            win.Height = bounds.Height;
            win.Left = bounds.Left;
            win.Top = bounds.Top;

            win.ShowDialog();

            if (win.ReturnedImage != null) {
                var gi = new ScreenCapture(win.ReturnedImage) {
                    SourcePosition = win.ReturnedImageSource,
                    SourceScreen = AvailableScreens.SelectedItem as Screen
                };
                Images.Add(gi);
            }
            win.Close();
            Visibility = Visibility.Visible;
        }

/*
        private void CheckForUpdatesCanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = ClickOnceHelper.CanCheckForUpdates();
        }

        private void CheckForUpdatesExecuted(object sender, ExecutedRoutedEventArgs e) {

            DoubleAnimation da = new DoubleAnimation {
                From = 0,
                To = 360,
                Duration = new Duration(TimeSpan.FromSeconds(2)),
                RepeatBehavior = RepeatBehavior.Forever
            };
            RotateTransform rt = new RotateTransform();
            CheckForUpdatesButton.RenderTransformOrigin = new Point(0.5, 0.5);
            CheckForUpdatesButton.RenderTransform = rt;
            rt.BeginAnimation(RotateTransform.AngleProperty, da);

            ClickOnceHelper.Finished += ClickOnceHelperFinished;
            ClickOnceHelper.CheckForUpdates();
        }

        void ClickOnceHelperFinished(object sender, UpdateCompleteEventArgs args) {
            CheckForUpdatesButton.RenderTransform = null;
            if (!args.NeedsRestart) {
                return;
            }
            var dr = MessageBox.Show("The application has been upgraded. You need to restart to use the updated version. Restart now?", "Update complete", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (dr == MessageBoxResult.Yes) {
                //restart = true;
                System.Windows.Forms.Application.Restart();
                Application.Current.Shutdown();
            }

        }
*/

        private void HighlightToggle(object sender, ExecutedRoutedEventArgs e) {
            //Console.WriteLine("hereiam");
        }

        private void CopySelectedGrab(object sender, ExecutedRoutedEventArgs e) {
            var imgdata = SelectedCapture.ImageAsJpg();
            Image img = Image.FromStream(imgdata);
            IDataObject dataObj = new DataObject();
            dataObj.SetData(img);
            WpfClipboard.SetClipboardDataObject(dataObj);
        }

        private void SaveSelectedGrab(object sender, ExecutedRoutedEventArgs e) {
            SaveFileDialog diag = new SaveFileDialog { Filter = @"JPG Image (*.jpg)|*.jpg" };
            diag.ShowDialog();
            if (String.IsNullOrEmpty(diag.FileName)) {
                return;
            }

            var imgdata = SelectedCapture.ImageAsJpg();

            using (Stream file = File.OpenWrite(diag.FileName)) {
                imgdata.Position = 0;
                CopyStream(imgdata, file);
                file.Close();
            }

        }

        ScreenCapture SelectedCapture {
            get { return _images.SelectedItem as ScreenCapture; }
        }

        public static void CopyStream(Stream input, Stream output) {
            byte[] buffer = new byte[8 * 1024];
            int len;
            while ((len = input.Read(buffer, 0, buffer.Length)) > 0) {
                output.Write(buffer, 0, len);
            }
        }

        private void DeleteSelectedGrab(object sender, ExecutedRoutedEventArgs e) {
            var item = SelectedCapture;
            if (item != null) {
                Images.Remove(item);
            }
        }

        #region bindings

        private ObservableCollection<ScreenCapture> _images1;
        public ObservableCollection<ScreenCapture> Images {
            get { return _images1 ?? (_images1 = new ObservableCollection<ScreenCapture>()); }
            set { _images1 = value; }
        }

        private ObservableCollection<Screen> _monitors;
        public ObservableCollection<Screen> Monitors {
            get {
                if (_monitors == null) {
                    _monitors = new ObservableCollection<Screen>();
                    foreach (Screen screen in Screen.AllScreens) {
                        _monitors.Add(screen);
                    }
                }
                return _monitors;
            }
            set { _monitors = value; }
        }

        private ObservableCollection<string> _availableHighlights;
        public ObservableCollection<string> AvailableHighlights {
            get {
                if (_availableHighlights == null) {
                    _availableHighlights = new ObservableCollection<string> { "Yellow", "Aqua", "Firebrick", "DarkSlateBlue", "DarkMagenta" };
                }
                return _availableHighlights;
            }
            set { _availableHighlights = value; }
        }

        public object AppSettings {
            get { return Settings.Default; }
        }

        private string _selectedHighlight;
        public string SelectedHighlight {
            get {
                if (string.IsNullOrWhiteSpace(_selectedHighlight)) {
                    _selectedHighlight = Settings.Default.LastSelectedHighlight;
                }
                return _selectedHighlight;
            }
            set {
                _canvasWithOverlay.HighlightColor = value;
                _selectedHighlight = value;
                Settings.Default.LastSelectedHighlight = _selectedHighlight;
                Settings.Default.Save();

            }
        }

        #endregion

        private void ImagesPreviewKeyUp(object sender, KeyEventArgs e) {
            if (e.Key == Key.Delete) {
                var item = ((ListBox)sender).SelectedItem as ScreenCapture;
                if (item != null) {
                    Images.Remove(item);
                }
            }
        }

        private double _zoom;
        public double Zoom {
            get { return _zoom; }
            private set {
                _zoom = value;
                NotifyChanged("Zoom");
                NotifyChanged("ZoomPercent");
            }
        }

        public int ZoomPercent {
            get { return (int)(Zoom * 100.0); }
        }

        private void WindowPreviewMouseWheel(object sender, MouseWheelEventArgs e) {
            //if (!Keyboard.IsKeyDown(Key.LeftCtrl)) {
            //    return;
            //}
            if (e.Delta < 0) {
                if (Zoom > .5) {
                    Zoom -= .01;
                }
            } else if (e.Delta > 0) {
                if (Zoom < 2) {
                    Zoom += .01;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        void NotifyChanged(string name) {
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        private void AvailableScreensMouseUp(object sender, MouseButtonEventArgs e) {
            IdScreenTask id = new IdScreenTask(((ListBox)sender).SelectedItem as Screen);
            id.Run();
        }

        private PopupContainer _popupContainer;

        public void ShowPopup(IHostedControl control, string title, bool moveable, bool animate) {

            _popupContainer = new PopupContainer(control, title);

            var overlay = new WindowOverlay(DockPanelMainWindow);
            AdornerLayer parentAdorner = AdornerLayer.GetAdornerLayer(DockPanelMainWindow);

            if (parentAdorner == null) {
                return;
            }

            parentAdorner.Add(overlay);

            _popupContainer.Owner = this;
            _popupContainer.AnimateToCenterOnSizeChanged = animate;

            _popupContainer.ShowDialog();
            _popupContainer.Close();
            parentAdorner.Remove(overlay);

            _popupContainer = null;

        }

        private void _canvasWithOverlay_HighlightAdded(object sender, AddHighlightEventArgs args) {
            SelectedCapture.Highlights.Add(args.TheHighlight);
        }

    }

}
