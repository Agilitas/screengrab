using System.Windows;

namespace ScreenGrab.Interfaces {
    public interface IHostedControl {

        /// <summary>
        /// Gets or sets the parent window.
        /// </summary>
        /// <value>The parent window.</value>
        Window ParentWindow { get; set; }

    }
}
