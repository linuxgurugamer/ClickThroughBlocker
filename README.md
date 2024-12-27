Having clicked one too many times on one window, only to have it click on the parts underneath (in the Editor), 
or click on an unwanted item in flight, I decided to solve the problem with yet another mod

Mods which use the Click Through Blocker would need to be modified, and this would become a hard dependency for that mod.

The changes are very simple:

Replace all calls to GUILayout.Window with ClickThruBlocker.GUILayoutWindow, the parameters are identical
Replace all calls to GUI.Window with ClickThruBlocker.GUIWindow, the parameters are identical

How it works

Each call first calls the original method (ie: ClickThruBlocker.GUILayoutWindow will call GUILayout.Window).  After the call,
the position of the mouse is checked to see if it was on top of the window Rect, if it is, it then locks the controls so that clicks don't
pass through to any other window.

Usage

	Add the following to the top of the source:
		using ClickThroughFix;
	Replace calls to GUILayout.Window with ClickThruBlocker.GUILayoutWindow
	Replace calls to GUI.Window with ClickThruBlocker.GUIWindow

Functions - Identical to the GUI and GUILayout versions
	Rect GUILayoutWindow(int id, Rect screenRect, GUI.WindowFunction func, GUIContent content, GUIStyle style, params GUILayoutOption[] options);
	Rect GUILayoutWindow(int id, Rect screenRect, GUI.WindowFunction func, Texture image, GUIStyle style, params GUILayoutOption[] options);
	Rect GUILayoutWindow(int id, Rect screenRect, GUI.WindowFunction func, string text, GUIStyle style, params GUILayoutOption[] options);
	Rect GUILayoutWindow(int id, Rect screenRect, GUI.WindowFunction func, GUIContent content, params GUILayoutOption[] options);
	Rect GUILayoutWindow(int id, Rect screenRect, GUI.WindowFunction func, Texture image, params GUILayoutOption[] options);
	Rect GUILayoutWindow(int id, Rect screenRect, GUI.WindowFunction func, string text, params GUILayoutOption[] options);

	Rect GUIWindow(int id, Rect clientRect, WindowFunction func, Texture image, GUIStyle style);
	Rect GUIWindow(int id, Rect clientRect, WindowFunction func, string text, GUIStyle style);
	Rect GUIWindow(int id, Rect clientRect, WindowFunction func, GUIContent content);
	Rect GUIWindow(int id, Rect clientRect, WindowFunction func, Texture image);
	Rect GUIWindow(int id, Rect clientRect, WindowFunction func, string text);
	Rect GUIWindow(int id, Rect clientRect, WindowFunction func, GUIContent title, GUIStyle style);

Additional functions

bool MouseIsOverWindow(Rect rect)   Returns true if the mouse is over the specified rectangle


==========================================================================

Having clicked one too many times on one window, only to have it click on the parts underneath (in the Editor),  or click on an unwanted item in flight, I decided to solve the problem with yet another mod.

Major Release Update, 1.10.5

Added Settings page 
Added new setting to specify Focus follows Click
Added code to have the focus follow the click instead of the mouse, for both editor and flight modes
Added initial window to select mode, shows one time only
Modified the CBTMonitor to not run in any scene other than the editor
Fixed window data not getting cleared properly
Removed need to save window rect
Reorganized code a bit
Removed some unnecessary assignments
Added stock settings page to support the FocusFollowsClick mode
Removed need to save window rect
Reorganized code a bit
Removed some unnecessary assignments
Added stock settings page to support the FocusFollowsClick mode
Added cleanup class to cleanup all input locks after a delay, immediately upon changing scenes
Added cleanup delay to settings page
NEW DEPENDENCY

ToolbarController
This mod will do nothing by itself, it will need to be used by other mods (see Precise Node for an example)

Mods which use the Click Through Blocker would need to be modified, and this would become a hard dependency for that mod.

The changes are very simple:

Replace all calls to GUILayout.Window with ClickThruBlocker.GUILayoutWindow, the parameters are identical
Replace all calls to GUI.Window with ClickThruBlocker.GUIWindow, the parameters are identical

 

Most important (for mod authors)
All mods using this should add the following line to the AssemblyInfo.cs file:

[assembly: KSPAssemblyDependency("ClickThroughBlocker", 1, 0)]
This will guarantee the load order.  One benefit is that KSP will output a warning and won't load an assembly if it's dependencies aren't met (which may be better than puking out a bunch of exceptions).  The only other real problem with the forced to the top of the sort list method is that technically there's a couple characters before zero ('~', '!', '@', etc.) and dlls directly in GameData come first too.  Of course someone pretty much has to be trying to break things if you have to worry about this particular case.

 

How it works

Each call first calls the original method (ie: ClickThruBlocker.GUILayoutWindow will call GUILayout.Window).  After the call, the position of the mouse is checked to see if it was on top of the window Rect, if it is, it then locks the controls so that clicks don't pass through to any other window.

Usage

Add the following to the top of the source:
using ClickThroughFix;
Replace calls to GUILayout.Window with ClickThruBlocker.GUILayoutWindow
Replace calls to GUI.Window with ClickThruBlocker.GUIWindow
Functions - Identical to the GUI and GUILayout versions

Rect GUILayoutWindow(int id, Rect screenRect, GUI.WindowFunction func, GUIContent content, GUIStyle style, params GUILayoutOption[] options);
Rect GUILayoutWindow(int id, Rect screenRect, GUI.WindowFunction func, Texture image, GUIStyle style, params GUILayoutOption[] options);
Rect GUILayoutWindow(int id, Rect screenRect, GUI.WindowFunction func, string text, GUIStyle style, params GUILayoutOption[] options);
Rect GUILayoutWindow(int id, Rect screenRect, GUI.WindowFunction func, GUIContent content, params GUILayoutOption[] options);
Rect GUILayoutWindow(int id, Rect screenRect, GUI.WindowFunction func, Texture image, params GUILayoutOption[] options);
Rect GUILayoutWindow(int id, Rect screenRect, GUI.WindowFunction func, string text, params GUILayoutOption[] options);
 
Rect GUIWindow(int id, Rect clientRect, WindowFunction func, Texture image, GUIStyle style);
Rect GUIWindow(int id, Rect clientRect, WindowFunction func, string text, GUIStyle style);
Rect GUIWindow(int id, Rect clientRect, WindowFunction func, GUIContent content);
Rect GUIWindow(int id, Rect clientRect, WindowFunction func, Texture image);
Rect GUIWindow(int id, Rect clientRect, WindowFunction func, string text);
Rect GUIWindow(int id, Rect clientRect, WindowFunction func, GUIContent title, GUIStyle style);
Additional functions

bool MouseIsOverWindow(Rect rect)   Returns true if the mouse is over the specified rectangle
 
Download

Source: https://github.com/linuxgurugamer/ClickThroughBlocker
Spacedock: https://spacedock.info/mod/1689
Github: https://github.com/linuxgurugamer/ClickThroughBlocker/releases
License:  GPLv3
Available via CKAN

