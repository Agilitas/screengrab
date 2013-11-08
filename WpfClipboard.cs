using System;
using System.Windows;

namespace ScreenGrab {
    public static class WpfClipboard {
        public static IDataObject GetClipboardDataObject() {
            for (int i = 0; i < 10; i++) {
                try {
                    return Clipboard.GetDataObject();
                } catch { }
                System.Threading.Thread.Sleep(100);
            }

            //try one last time and allow exception to be thrown this time
            return Clipboard.GetDataObject();
        }

        public static void SetClipboardDataObject(object data) {
            for (int i = 0; i < 10; i++) {
                try {
                    Clipboard.SetDataObject(data);
                    return;
                } catch { }
                System.Threading.Thread.Sleep(100);
            }

            //Try one last time and allow exception to be thrown this time.
            Clipboard.SetDataObject(data);
        }


        public static void SetText(String value) {
            SetClipboardDataObject(value);
        }

        public static string GetText() {
            IDataObject dataObject = GetClipboardDataObject();
            if (dataObject == null) {
                return string.Empty;
            }

            return dataObject.GetData(DataFormats.Text) as string;

        }
    }
}
