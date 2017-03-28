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

    // Render controls
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
    // Keyboard events
    // Focussed item for keyboard
    // TextBox


    // Set window Title / class / etc.
    // Image (via ImageBrush)
    // Window Decorations (include maximise / full-screen)
    // Window startup maximise/normal/full screen
    // Resize window
    // MessageBox (can be popup)
    // Grid Row/Column Span
    // Mouse events pass through transparent controls to those underneath
    // Scroll Viewer
    // "Click" and "Double Click" events - needs button tracking better as a MouseState object
    // Sort out App.cs into different classes modelling the global boejcts in Wayland (registry, shm, etc.)
    // Correctly destroy all wl_ objects and encapsulate in WaylandObject classes
    // Split to multiple DLLs / namespaces
    // All UI components implement INotifyPropertyChanged
    // Data binding
    // Templates
    // Threading model (STA, multi-thread the Wayland bits, dispatcher model)
    // Implement a render is required / layout is required / only re-composite changed areas model
}