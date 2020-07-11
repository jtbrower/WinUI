# Running the code in Visual Studio
* Set the WinUI.DemoApp.Package as the startup project.
* Select either x64 or x86 as the platform
* Right Click the WinUI.DemoApp.Package and select Deploy.  You shouldn't need to do this more than once.
* Run the WinUI.DemoApp.Package using the green play button in Visual Studio, you should now be running.
* Once the project is running, experiment by making your own changes.
* Sometimes Visual Studio puts an AnyCPU platform back into the solution.  Don't use it.

# Current Features
* Remove and Add a Window Border/Titlebar
* Provide Window Drag like the Titlebar normally provides.  Works with the Mouse and Touch (See WinUI.CustomControls.DragMoveFeature.cs)
* Set the Window transparency between 0 (fully visible) and 100 (fully transparent)
* Change the size of the Window (see App.xaml.cs OnLaunched)
* Uses .Net5.0, initial version was preview 6 but check to see if that is the current version
* Uses WinUI Preview 1, again check to make sure that is the current version

# Known Issues (Work in Progress)
* When you remove the Window Border, the lower part of the Window will not act as a drag area until you maximize the Window to force it to repaint.
* '<PackageReference Include="Microsoft.Windows.CsWinRT" Version="0.1.0-prerelease.200623.5" />' was added to the DemoApp to avoid a compatibility issue between .Net5 preview 5/6 and WinUI
* When you set the Window transparency all of the Window content becomes transparent too.  I need to figure out how to handle this.
* Although the project uses DI and View Models, I did not go through the trouble to disable buttons that are not applicable.  For example, if you remove the Window border, the button to remove is still enabled.
* Currenlty I have not gone through the trouble to use ICommands for the buttons.
* I need to figure out the cleanest way to set the Window icon
* DropShadowPanel control stretches vertically, but not horizontally. See [this issue](https://github.com/windows-toolkit/WindowsCommunityToolkit/issues/3384) I filed for details.
* When I resize the Window on startup, it is not a smooth transition because I currently have to call Window.Activate and then resize directly after.  

# To Save you Time
* You cannot create a DependencyProperty on the Window class because it is not a DependencyObject
* Do not try to create a DependencyProperty with a IntPtr type.  It will throw exceptions.

