# Running the code in Visual Studio
* Set the WinUI.DemoApp.Package as the startup project.
* Select either x64 or x86 as the platform
* Right Click the WinUI.DemoApp.Package and select Deploy.  You shouldn't need to do this more than once.
* Run the WinUI.DemoApp.Package using the green play button in Visual Studio, you should now be running.
* Once the project is running, experiment by making your own changes.
* Sometimes Visual Studio puts an AnyCPU platform back into the solution.  Don't use it.

# Current Features and What you can Learn
* Provides a class called WpfWindow that mimics some WPF Windowing features
* Place a shadow around a grid without using the DropShadowPanel. Shadow has rounded corners.
* Two examples of how to create a Behavior
* Remove and Add a Window Border/Titlebar
* Provide Window Drag like the Titlebar normally provides.  Works with the Mouse and Touch, see DragMoveBehavior.cs
* Provides many examples of use the Win32 PInvoke API
* Set the Window transparency between 0 (fully visible) and 100 (fully transparent)
* Provide a SizeToContent feature like WPF provides.
* Maximize, Restore, Minimize capabilities
* Change the size of the Window
* Provide double tap to maximize a Window while disabling a double tap of a button from triggering the maximize

#Requirements
* Uses .Net5.0, initial version was/is preview 6 but check to see if that is the current version
* Uses WinUI Preview 1, again check to make sure that is the current version
* Uses Microsoft's dependency injection container and attempts to fully decouple that app from static refs
* WinUI requires a Desktop app to be packaged and compiled to a specific platform (not AnyCpu). For that reason, I removed AnyCpu from projects but Visual Studio insists on placing it back in the solution.  Do not use AnyCpu.
* I have Nullable enabled on all projects to enforce clean coding practices.

# Known Issues (Work in Progress)
* '<PackageReference Include="Microsoft.Windows.CsWinRT" Version="0.1.0-prerelease.200629.3" />' was added to the DemoApp to avoid a compatibility issue between .Net5 preview 5/6 and WinUI
* When you set the Window transparency all of the Window content becomes transparent too.  I need to figure out how to handle this.
* Currently I have not gone through the trouble to use ICommands for the buttons, or disable buttons that are not applicable to a given state.
* I need to figure out the cleanest way to set the Window icon
* When I resize the Window on startup, it is not a smooth transition because I currently have to call Window.Activate and then resize directly after.  
* The SizeToContent feature works well without a Window border, but if there is a border or titlebar, I do not account for those space requirements (yet).
* 
* The Debug window shows 5 'WinRT transform error' exceptions that are swallowed on app start
* Debug Window shows a 'mincore\com\oleaut32\dispatch\ups.cpp(2122)\OLEAUT32.dll' library not registered error.

## Known Visual Studio or Project Issues
* MSB4181 regarding CompileXaml task returned false but did not log and error warning keeps showing up.
* I often see WMV1006, WMC1007 about not being able to resolve assemblies.
* During the development of this project, Visual Studio has crashed several times.  This is nothing new though.
* Two times in a row, when I double clicked on a Xaml file it crashed Visual Studio.  It works now though.

# To Save you Time
* You cannot create a DependencyProperty on the Window class because it is not a DependencyObject
* Do not try to create a DependencyProperty with a IntPtr type.  It will throw exceptions.

# WinUI 3 preview 2 is out
On July 15th, WinUI preview two was released.  I gave it a really quick test and I am seeing program exceptions that I will need to 
address before upgrading to the newer version.  I hope to work on that in the next day.
```
    <PackageReference Include="Microsoft.WinUI" Version="3.0.0-preview2.200713.0" />
```