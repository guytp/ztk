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

    // Window Decorations (include maximise / full-screen) - configurable
    // Set window Title / class / etc.
    // use xdg shell
    // Window startup maximise/normal/full screen
    // Resize window

    // Can define fixed Width/Height (NaA means ignore).  Stretch still takes prescedence.  Apply to whole stack including window.
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

    // Mouse events pass through transparent controls to those underneath - concept here of bubble up/down as with WPF.  Bubble up / down keyboard too?

    // Button content presenter?
    // TextBlock wrap support
    // TextBox
    //    - If focussed have I-Bar
    //    - Wrap
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

    // Custom mouse cursors (I bar for text boxces)

    // Animations (Colour / Double)
}