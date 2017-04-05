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

    // Focus
    //    - bool Focus() method on each control goes to its parent and asks if it can focus - goes right to top of tree before returning (CanFocus() then SetFocus() called)
    //    - Whole tree updates which single control has focus
    //    - Keyboard events sent to control with focus

    // Hookup in Calculator app to keyboard
    // TextBox
    //    - If focussed have I-Bar

    // use xdg shell
    // Window Decorations (include maximise / full-screen) - configurable
    // Window startup maximise/normal/full screen
    // Resize window

    // Can define fixed Width/Height (NaA means ignore).  Stretch still takes prescedence.  Apply to whole stack including window.
    // Set window Title / class / etc.
    // Correctly destroy all wl_ objects and encapsulate in WaylandObject classes

    // Multi-window support (callback objects not static in C code)

    // Keyboard - support for repeats
    // Keyboard - initial key states on enter nto being set
    // Data binding
    // Styles/themes
    // Templates
    // Buttons can have flat style and can be font configurable
    // BUtton/TB add horizontal/vertical content alignment
    // Button adds on templates for OnHover and OnPressed
    // Grid row/column definitions can have min/max height/widths

    // All UI components implement INotifyPropertyChanged
    // Implement a render is required / layout is required / only re-composite changed areas model - trigger renders required when any changes made to properties - store last used buffers (or just write alpha over?)

    // Mouse events pass through transparent controls to those underneath - concept here of bubble up/down as with WPF

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

    // Custom mouse cursors
}