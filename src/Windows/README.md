# Running the code in Visual Studio
* Set the WinUI.DemoApp.Package as the startup project.
* Select either x64 or x86 as the platform
* Right Click the WinUI.DemoApp.Package and select Deploy.  You shouldn't need to do this more than once.
* Run the WinUI.DemoApp.Package using the green play button in Visual Studio, you should now be running.
* Once the project is running, experiment by making your own changes.
* Sometimes Visual Studio puts an AnyCPU platform back into the solution.  Don't use it.

# Current Features and What you can Learn
* Provides a class called WpfWindow that mimics some WPF Windowing features.  My choice of name for this class is questionable and I plan to change it.
* Provide double tap to maximize a Window
* Provide a behavior that with one line of code will place a shadow around a control.  Unlike the UWP DropShadowPanel, it can have rounded corners.
* A behavior that Swallows double clicks so they are not triggered by buttons. Please see SwallowButtonDoubleTapBehavior.cs
* Provide Window DragMove Behavior like the Titlebar normally provides.  Works with the Mouse and Touch, see DragMoveBehavior.cs
* Remove and Add a Window Border/Titlebar
* Provides many examples of using Win32 PInvoke API calls directly or via the NuGet package PInvoke.User32.
* Set the Window transparency between 0 (fully visible) and 100 (fully transparent).
* Provide a SizeToContent feature like WPF provides.  Capable of sizing up and down to fit client space requirements; great for border-less windows but still works with border.
* Maximize, Restore, Minimize, change Window size and move it to a new location.
* Ability to automatically scale the WpfWindow content upon a DPI change.

# Requirements
* Uses .Net5.0, initial version was/is preview 6 but check to see if that is the current version
* Uses WinUI Preview 2, again check to make sure that is the current version
* Uses Microsoft's dependency injection container and attempts to fully decouple that app from static refs
* WinUI requires a Desktop app to be packaged and compiled to a specific platform (not AnyCpu). For that reason, I removed AnyCpu from projects but Visual Studio insists on placing it back in the solution.  Do not use AnyCpu.
* I have Nullable enabled on all projects to enforce clean coding practices.  See Directory.Build.props, this is where it is enabled for all projects.

# Known Issues (Work in Progress)
* `<PackageReference Include=\"Microsoft.Windows.CsWinRT Version=\"0.1.0-prerelease.200629.3\" />` was added to the DemoApp to avoid a compatibility issue between .Net5 preview 6 and WinUI
* When you set the Window transparency all of the Window content becomes transparent too.  I need to figure out how to handle this.
* Currently I have not gone through the trouble to use ICommands for the buttons, or disable buttons that are not applicable to a given state.
* I need to figure out the cleanest way to set the Window icon
* When I resize the Window on startup, it is not a smooth transition because I currently have to call Window.Activate and then resize directly after.
* The Debug window shows 5 'WinRT transform error' exceptions that are swallowed on app start
* Debug Window shows a 'mincore\com\oleaut32\dispatch\ups.cpp(2122)\OLEAUT32.dll' library not registered error.
* The Debug Window warns that ActualWidthProperty is not found on Canvas.  I thought that maybe creating a property path, I should switch to just "ActualWidth"; even though the error goes away, it breaks the drop shadow and can't be correct.

## Known Visual Studio or Project Issues
* MSB4181 regarding CompileXaml task returned false but did not log and error warning keeps showing up.
* I often see WMV1006, WMC1007 about not being able to resolve assemblies.
* During the development of this project, Visual Studio has crashed several times.  This is nothing new though.
* Two times in a row, when I double clicked on a Xaml file it crashed Visual Studio.  It works now though.

# To Save you Time
* You cannot create a DependencyProperty on the Window class because it is not a DependencyObject
* Do not try to create a DependencyProperty with a IntPtr type.  It will throw exceptions.