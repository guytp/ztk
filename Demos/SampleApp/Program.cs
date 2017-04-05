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
    //   - Keyboard Focus for controls - get lost/gained keyboard focus events
    //   - Parents can disable keyboard focus down a tree
    //   - Keys that are down on focus - capture these to reset state (need for numlock etc)
    //   - Deal with repeat ourself throughout app - send a KeyRepeat event if XKB agrees

    // Hookup in Calculator app to keyboard
    // TextBox

    // Control focus concept (Sets IsFocussed and then tries to get kbd focus unless control expressly doesnt focus kbd, works down tree to disable focusable)

    // use xdg shell
    // Window Decorations (include maximise / full-screen) - configurable
    // Window startup maximise/normal/full screen
    // Resize window

    // Can define fixed Width/Height (NaA means ignore).  Stretch still takes prescedence.  Apply to whole stack including window.
    // Set window Title / class / etc.
    // Correctly destroy all wl_ objects and encapsulate in WaylandObject classes

    // Multi-window support (callback objects not static in C code)

    // Data binding
    // Styles/themes
    // Templates
    // Buttons can have flat style and can be font configurable
    // BUtton/TB add horizontal/vertical content alignment
    // Button adds on templates for OnHover and OnPressed
    // Grid row/column definitions can have min/max height/widths

    // All UI components implement INotifyPropertyChanged
    // Implement a render is required / layout is required / only re-composite changed areas model - trigger renders required when any changes made to properties - store last used buffers (or just write alpha over?)

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