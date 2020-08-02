# Requirements
Note that the WinUI team has indicated a version of the .Net 5.0 prelease that is lower than what I use in these projects.  Right now I am at preview 7 and that's why the global.json tries to use that version.

I prefer to live on the bleeding edge of the latest .Net preview builds and I have a lot of internal code that does this.  Since I need the WinUI libraries I write to work with the latest previews, that's what I have used here.

If you add more features to what I have done and you come upon something that doesn't quite work, consider if you are using the recommended .Net version prior to reporting a WinUI bug.

* Uses .Net5.0, preview 7 but check to see if that is the current version.  See global.json.  I didn't pin it to a specific version because projects won't load for you if you do not have it.
* Uses WinUI Preview 2, again check to make sure that is the current version
* Uses Microsoft's dependency injection container and attempts to fully decouple that app from static refs
* WinUI requires a Desktop app to be packaged and compiled to a specific platform (not AnyCpu). For that reason, I removed AnyCpu from projects but Visual Studio insists on placing it back in the solution.  Do not use AnyCpu.
* I have Nullable enabled on all projects to enforce clean coding practices.  See Directory.Build.props, this is where it is enabled for all projects.

# Running the code in Visual Studio
* Set the WinUI.DemoApp.Package as the startup project.
* Select either x64 or x86 as the platform
* Right Click the WinUI.DemoApp.Package and select Deploy.  You shouldn't need to do this more than once.
* Run the WinUI.DemoApp.Package using the green play button in Visual Studio, you should now be running.
* Once the project is running, experiment by making your own changes.
* Sometimes Visual Studio puts an AnyCPU platform back into the solution.  Don't use it.

# Current Features and What you can Learn
I will try to take more time in the future to document why some of these features took a bit of work.

* Provides a class called ExtWindow that mimics some WPF Windowing features.
* Provide double tap to maximize and Minimize a Window, while ignoring DoubleTaps on Buttons
* Provide a behavior that with one line of code will place a shadow around a control.  Unlike the UWP DropShadowPanel, it can have rounded corners.
* A behavior that Swallows double clicks so they are not triggered by buttons. Please see SwallowButtonDoubleTapBehavior.cs
* Provide Window DragMove Behavior like the TitleBar normally provides.  Works with the Mouse and Touch, see DragMoveBehavior.cs
* Remove and Add the Win32 Border/TitleBar and Add or Remove a Custom XAML Version that is not normally available in WinUI Desktop apps.
* Provides many examples of using Win32 PInvoke API calls directly or via the NuGet package PInvoke.User32.
* Set the Window transparency between 0 (fully visible) and 100 (fully transparent).
* Provide a SizeToContent feature like WPF provides.  Capable of sizing up and down to fit client space requirements; great for border-less windows but still works with border.
* Maximize, Restore, Minimize, change Window size and move it to a new location.
* Ability to automatically scale the ExtWindow content upon a DPI change.
* Hook into WndProc to handle native window messages

# Known Issues
* `<PackageReference Include=\"Microsoft.Windows.CsWinRT Version=\"0.1.0-prerelease.200629.3\" />` was added to the DemoApp to avoid a compatibility issue between .Net5 preview 6 and WinUI
* "Warning	WMC9999	Type universe cannot resolve assembly: WinRT.Runtime, Version=0.1.0.2153, Culture=neutral, PublicKeyToken=null.	WinUI.DemoApp"
* The Debug window shows 5 'WinRT transform error' exceptions that are swallowed on app start
* Debug Window shows a 'mincore\com\oleaut32\dispatch\ups.cpp(2122)\OLEAUT32.dll' library not registered error.
* The Debug Window warns that ActualWidthProperty is not found on Canvas.  I thought that maybe creating a property path, I should switch to just "ActualWidth"; even though the error goes away, it breaks the drop shadow and can't be correct.
* The Debug Window warns that ActualHeightProperty is not found on Canvas.  Same as above.

## DPI Changed Issue
Once in a while I would see during startup that if I forced a DPI change to occur, my design was not properly scaling.  I thought
at first that this was related to timing in calculating the Window size needed to fully display its content, but I was wrong.
It happened again moments ago and I found that the DPI changed event was not firing.  

So I decided to cut out the middleman for that DPI Changed event and process it by directly sniffing for the message on 
the WndProc loop.  I will keep an eye on this to see if I ever see that problem again.  Its going to be a tough
one to intentionally cause so I doubt I will log an issue unless it also happens with the Win32 msg loop.

## Mouse Drag Capture Issue
On rare occasions the relationship between the mouse pointer and the Window seems to indicate that the Window has 
mouse capture because the DragMove behavior is causing the Window to move even when the left mouse button is not
pressed.  Oddly I don't even capture the mouse in my own code, I didn't find it to be necessary so I figured that
I would remove it so that there would be zero risk that somehow it would remain captured and cause a problem
like I am seeing infrequently.  Maybe I need to capture and release?  I have a feeling that it is related to 
using the debugger.  

# TODO
* NAMESPACES, a recent refactor left a mismatch between namespaces and assembly/folder names and Analyzers are not catching this.  CodeRush can't fix it either.'
* Change Cursor on DragMove operation
* When you set the Window transparency all of the Window content becomes transparent too.  I need to figure out how to handle this.
* Modify Window and related Views/View Models to make it mimic an Adorner
* When I resize the Window on startup, it is not a smooth transition because I currently have to call Window.Activate and then resize directly after.
* The WindowHookManager has a static dictionary ApplicationsHookedWindows that needs to be converted to a thread safe concurrent type.

## Known Visual Studio or Project Issues
* MSB4181 regarding CompileXaml task returned false but did not log and error warning keeps showing up.
* I often see WMV1006, WMC1007 about not being able to resolve assemblies.
* During the development of this project, Visual Studio has crashed several times.  This is nothing new though.
* Two times in a row, when I double clicked on a Xaml file it crashed Visual Studio.  It works now though.

# To Save you Time
* You cannot create a DependencyProperty on the Window class because it is not a DependencyObject
* Do not try to create a DependencyProperty with a IntPtr type.  It will throw exceptions.
* Including a resource dictionary that has a backing C# file is included like ```<namespace:YouDictionaryName />``` and not how one is normally included. 

# Notes to Self
* Figure out or log a potential issue where custom controls that inherit from other controls such as a grid, only work in XAML, not code behind.  Code behind throws exceptions.  See the WindowRoot control.
* Determine if I should open a discussion or issue regarding the infinite DPI change events firing.  If I do I need to create a small project that can most easily reproduce the problem.