using System.Windows.Input;

namespace ScreenGrab{
    public static class ScreenGrabCommands{
        private static RoutedUICommand _highlightActivateCommand = new RoutedUICommand("Activate the highlight ability", "HighlightActivateCommand ", typeof(ScreenGrabCommands));        

        private static readonly RoutedUICommand _newScreenGrab = new RoutedUICommand("Get a new screen grab", "NewScreenGrab", typeof(ScreenGrabCommands));
        private static readonly RoutedUICommand _redoScreenGrab = new RoutedUICommand("Redo the selected screen grab", "RedoScreenGrab", typeof(ScreenGrabCommands));
        private static readonly RoutedUICommand _clearHighlights = new RoutedUICommand("Clear the highlights", "ClearHighlights", typeof(ScreenGrabCommands));
        private static readonly RoutedUICommand _userPreferences = new RoutedUICommand("User Preferences", "UserPreferences", typeof(ScreenGrabCommands));


        public static RoutedUICommand ClearHighlights{
            get { return _clearHighlights; }
        }
        public static RoutedUICommand NewScreenGrab {
            get { return _newScreenGrab; }
        }

        public static RoutedUICommand RedoScreenGrab {
            get { return _redoScreenGrab; }
        }

        public static RoutedUICommand HighlightActivateCommand{
            get { return _highlightActivateCommand; }
        }
        public static RoutedUICommand UserPreferences {
            get { return _userPreferences; }
        }

    
    }
}