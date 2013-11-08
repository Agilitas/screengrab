using System;
using System.Windows.Controls;
using System.Windows.Input;
using ScreenGrab.Interfaces;

namespace ScreenGrab.Windows {
    /// <summary>
    /// Interaction logic for PopupContainer.xaml
    /// </summary>
    public partial class PopupContainer : IDisposable {



        private IHostedControl _hostedControl;

        private UserControl _control;
        //private ScrollViewer _scroll;

        /// <summary>
        /// Initializes a new instance of the <see cref="PopupContainer"/> class.
        /// </summary>
        /// <param name="hostedControl">The hosted control.</param>
        /// <param name="title">The title.</param>
        public PopupContainer(IHostedControl hostedControl, string title) : this() {
            HostedControl = hostedControl;
            HostedControl.ParentWindow = this;
            Title = title;
        }

        public bool AnimateToCenterOnSizeChanged {get; set;}

        /// <summary>
        /// Initializes a new instance of the <see cref="PopupContainer"/> class.
        /// </summary>
        public PopupContainer() {
            InitializeComponent();

            CommandBindings.Add(new CommandBinding(ApplicationCommands.Close,
                 delegate { Close(); }));

        }

        /// <summary>
        /// Gets or sets the hosted control.
        /// </summary>
        /// <value>The hosted control.</value>
        public IHostedControl HostedControl {
            get { return _hostedControl; }
            set {
                _hostedControl = value;
                if (value != null) {
                    _control = (UserControl)value;

                    Grid.SetRow(_control, 1);
                    Grid.SetColumn(_control, 0);
                    Host.Children.Add(_control);
                }
            }
        }

        /// <summary>
        /// Drag window.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        public void DragWindow(object sender, MouseButtonEventArgs args) {
            DragMove();
        }

        #region Implementation of IDisposable

        public void Dispose() {
            Host.Children.Remove(_control);
            HostedControl.ParentWindow = null; //need to release the ref
            HostedControl = null;
            _control = null;
        }

        #endregion

        //private bool _silentClose;
        public void Close(bool silentClose) {
            //_silentClose = silentClose;
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            //if (_silentClose) { return; }

            //if (_scroll.Content is IClosableControl) {
            //    e.Cancel = ((IClosableControl)_scroll.Content).CloseUserControl();
            //}

        }

    }
}