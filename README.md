WinUI pre v1 with .Net5 pre v6 to demonstrate work-arounds required to achieve
Window features that are available in WPF but not WinUI.  See below for a more
detailed explanation.

In WinUI, the Window class is actually not the responsibility of the WinUI team.
Further, if your migrating from WPF its worth noting that the Window class is
not a framework element so creating DependencyProperty props on a Window is
not possible since the Window class is not a DependencyObject.

To achieve the WPF Window features you might be craving as a WPF developer, you
need to freshen up on your PInvoke skills.  I have enjoyed interacting with
WinRT via PInvoke calls, but it is a time consuming process.  You must find the
proper calls to make, add the extern declarations, and then experiment until you
see the results your chasing.

Much of the work in this initial check-in achieves features like removing the
Window border/titlebar/menubar and implementing DragMove so that you can still
relocate the Window using both the mouse and the touch screen.

Disclaimer : I began this project when I needed to further evaluate how
challenging it would be to migrate a large (as in the number and size of XAML
files) WPF application over to WinUI before WinUI was ready for public adoption.
At the time of this check-in, WinUI preview 1 is available as is .Net5.0 preview
6.  Since some [breaking changes](https://docs.microsoft.com/en-us/dotnet/core/compatibility/interop) were made
to NetCore, you must include a specific version of Microsoft.Windows.CsWinRT so
that you do not hit a missing method exception that is caused by those breaking
changes.  Including that package directly forces WinUI to use a package version
other than what it was linked against.  Although this work-around has allowed me
to continue to use the latest .Net5 v6 pre-release, its worth noting that this
could lead to side effects.  Side effects that you need to be considerate about
before reporting any issues to the WinUI team, or to teams that use WinUI in
their packages (Windows Community Toolkit).  For that reason, when I hit
exceptions I do my best to evaluate older versions of the code before reporting
those exceptions to the proper teams.  One example of this would be the bug I
reported [here](https://github.com/windows-toolkit/WindowsCommunityToolkit/issues/3384) .  The
DropShadowPanel stretches vertically but not horizontally.  I tested this out
with a production release of the toolkit to see if the bug was in the older 6.x
versions of the code and it was.  Point being, if you see issues in my project,
blame me before you blame the WinUI team or the toolkit team since I force the
use of a newer version of Microsoft.Windows.CsWinRT than either was linked
against.
