using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ScreenGrab {
    public static class KeyboardHelper {

        public static bool IsCtrlDown {
            get { return (Keyboard.Modifiers & ModifierKeys.Control) != 0; }
        }

        public static bool IsShiftDown {
            get { return (Keyboard.Modifiers & ModifierKeys.Shift) != 0; }
        }

        public static bool IsAltDown {
            get { return (Keyboard.Modifiers & ModifierKeys.Alt) != 0; }
        }

        public static bool IsModifierKeyDown {
            get { return IsCtrlDown || IsShiftDown || IsAltDown; }
        }

    }
}
