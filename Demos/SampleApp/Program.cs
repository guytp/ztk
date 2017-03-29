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

    // Sort out App.cs into different classes modelling the global boejcts in Wayland (registry, shm, etc.)
    //      Once this is all split out go through again checking for OO consistency (store objects ot others never store just their pointers) - and manage dispose cycles
    //      ShellSurface should use inheritance rather than .ConvertToSHell()
    // Split to multiple DLLs / namespaces
    // Correctly destroy all wl_ objects and encapsulate in WaylandObject classes
    // Window descends from container control and separates out Surface code so layout/measure same as other controls
    // Layout also gets X/Y for children and stores all this, along with bounding rects, in a base class rather than bespoke implementations
    // Render controls
    //    Layouts totally broken for various alignments (see Window sample for nested borders where auto isnt'w roking by default and 0 values being used)
    //    Border
    //      Corner Radius
    //      Mouse events - in base class
    //      Stretching width/height overlaps entire parent grid somehow
    //      Mask child when rounded corners
    //    Grid
    //          margins not being handled correctly?
    //          * widths not perfect if * + an auto (some unused space) - or is this margins?
    //          Mouse events passed to children
    //          Nested grids test
    //    Button
    //          On Hover / Pressed states and templates
    // Keyboard events
    // Focussed item for keyboard
    // TextBox


    // use xdg shell
    // calculator sample app
    // Set window Title / class / etc.
    // Image (via ImageBrush)
    // Radio Button
    // Check Box
    // Combo Box
    // Multi-window support (callback objects not static in C code)

    // Window Decorations (include maximise / full-screen)
    // Window startup maximise/normal/full screen
    // Resize window
    // MessageBox (can be popup)
    // Grid Row/Column Span
    // Mouse events pass through transparent controls to those underneath
    // Scroll Viewer
    // "Click" and "Double Click" events - needs button tracking better as a MouseState object
    // All UI components implement INotifyPropertyChanged
    // Data binding
    // Templates
    // Threading model (STA, multi-thread the Wayland bits, dispatcher model)
    // Implement a render is required / layout is required / only re-composite changed areas model
}