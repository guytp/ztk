using System;

namespace Ztk.Demos.SampleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                using (App app = new App(new SampleWindow()))
                    app.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }

    // Keyboard events
    // Focussed item for keyboard - controls can opt to not have keyboard focus
    // TextBox

    // use xdg shell
    // Window Decorations (include maximise / full-screen) - configurable
    // Window startup maximise/normal/full screen
    // Resize window

    // Buttons can have flat style and can be font configurable
    // Can define fixed Width/Height (NaA means ignore).  Stretch still takes prescedence.  Apply to whole stack including window.
    // calculator sample app
    // Set window Title / class / etc.
    // Correctly destroy all wl_ objects and encapsulate in WaylandObject classes

    // Multi-window support (callback objects not static in C code)

    // Data binding
    // Styles/themes
    // Templates
    // Button adds on templates for OnHover and OnPressed

    // All UI components implement INotifyPropertyChanged
    // Implement a render is required / layout is required / only re-composite changed areas model - trigger renders required when any changes made to properties - store last used buffers (or just write alpha over?)

    // BUtton/TB add horizontal/vertical content alignment
    // Button content presenter?
    // Image (via ImageBrush)
    // Radio Button
    // Check Box
    // Combo Box
    // Scroll Viewer
    // Items Control
    // StackPanel
    // WrapPanel

    // MessageBox (can be popup)
    // Grid Row/Column Span
    // Mouse events pass through transparent controls to those underneath
}