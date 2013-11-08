using System.Threading;
using System.Windows.Forms;

namespace ScreenGrab.AsyncTasks{
    class IdScreenTask {

        public Screen TheScreen { get; private set; }

        public IdScreenTask(Screen theScreen) {
            TheScreen = theScreen;
        }

        public void Run() {
            Thread t = new Thread(DoTask);
            t.SetApartmentState(ApartmentState.STA);
            t.IsBackground = true;
            t.Start();
        }

        private void DoTask() {
            Windows.IdentifyScreen win = new Windows.IdentifyScreen();
            //win.Activate();
            //win.Top = (TheScreen.Bounds.Height - win.Height)/2;
            //win.Left =  ((TheScreen.Bounds.Width - win.ActualWidth) / 2) + TheScreen.Bounds.Left;
            win.Width = TheScreen.Bounds.Width;
            win.Height = TheScreen.Bounds.Height;
            win.Left = TheScreen.Bounds.Left;
            win.Top = TheScreen.Bounds.Top;
            win.Show();
            Thread.Sleep(1000);
            //win.Close();
        }
    }
}