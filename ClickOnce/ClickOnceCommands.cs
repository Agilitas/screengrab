using System.Windows.Input;

namespace ScreenGrab.ClickOnce {
    public static class ClickOnceCommands {
        private static readonly RoutedUICommand _checkForUpdatesCommand = new RoutedUICommand("Check for updates", "CheckForUpdatesCommand", typeof(ClickOnceCommands));

        public static RoutedUICommand CheckForUpdatesCommand {
            get { return _checkForUpdatesCommand; }
        }
    }
}