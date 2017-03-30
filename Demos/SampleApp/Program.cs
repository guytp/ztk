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

    // TextBlock - sort out measure/render to cope with Stretch in new model - test all combinations in current SampleWindow
    // Layouts totally broken for various alignments (see Window sample for nested borders where auto isnt'w roking by default and 0 values being used)
    // Border
    //   Stretching width/height overlaps entire parent grid somehow
    //   Mask child when rounded corners
    // Grid
    //    margins not being handled correctly?
    //    * widths not perfect if * + an auto (some unused space) - or is this margins?
    //    Mouse events passed to children
    //    Nested grids test
    // Add back mouse events and structure to propogate through all nested controls

    // Button
    // Keyboard events
    // Focussed item for keyboard - controls can opt to not have keyboard focus
    // TextBox

    // use xdg shell
    // calculator sample app
    // Set window Title / class / etc.
    // Correctly destroy all wl_ objects and encapsulate in WaylandObject classes

    // Window Decorations (include maximise / full-screen) - configurable
    // Window startup maximise/normal/full screen
    // Resize window
    
    // Multi-window support (callback objects not static in C code)

    // "Click" and "Double Click" events - needs button tracking better as a MouseState object

    // Data binding
    // Styles/themes
    // Templates
    // Button adds on templates for OnHover and OnPressed

    // All UI components implement INotifyPropertyChanged
    // Implement a render is required / layout is required / only re-composite changed areas model - trigger renders required when any changes made to properties - store last used buffers (or just write alpha over?)

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