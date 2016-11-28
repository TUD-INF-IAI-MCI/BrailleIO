BrailleIO Framework
=========


## Outline
1. BrailleIO
	1. [Intension](#intension)
	2. [Subprojects](#subprojects)
		1. [BrailleIO](#brailleio-1)
		2. [BrailleIO_Interfaces](#brailleio_interfaces)
		3. [BrailleIO_ShowOff](#brailleio_showoff)
		4. [BrailleRenderer](#braillerenderer)
		5. [GestureRecognizer](#gesturerecognizer)
		6. [BrailleIOExample](#brailleioexample)
	3. [How to use:](#how-to-use)
		1. [How to build a hardware abstraction](#how-to-build-a-hardware-abstraction)
			1. [Set up a new Adapter]( #set-up-a-new-adapter)
				1. [Recommendations for event data (raw data)](#recommendations-for-event-data-raw-data)
2. [Examples – still empty yet](#examples)
3. [Deep Dive](#deep-dive)
	1. [Build your own renderer](#build-your-own-renderer)
		1. [Make the renderer hookable](#make-the-renderer-hookable)
		2. [Make a renderer cacheable] (#make-a-renderer-cacheable)
		3. [Make your renderer touchable](#make-your-renderer-touchable)



#BrailleIO

Abstraction Framework for tactile pin-matrix devices' output and user input modelling.

## Intension:

BrailleIO is a small .NET framework for developing two-dimensional tactile applications. It offers general features for displaying tactile information and for interaction. BrailleIO includes hardware abstraction, window and visualization features as well as basic interaction functions, such as panning and zooming on different content types. 

Information visualization can be organized in several independent screens that can be divided into multiple areas, having a full box model. Interaction can be realized via hardware keys of the used device or by basic gestures if the device is touch-sensitive.

BrailleIO is a small framework that should enable application developers to fast and easily build non-visual applications for on a planar two-dimensional tactile pin-matrix display. It is written in C# for .Net and is therefore a framework for windows applications manly. 

More technical details can be found in [Bornschein, Jens. "BrailleIO-a Tactile Display Abstraction Framework." Proceedings of TacTT ’14 Workshop, Nov 16 2014, Dresden, Germany. 2014.](http://ceur-ws.org/Vol-1324/paper_4.pdf)

[[*back to outline* :arrow_up:]](#outline)


## Subprojects

The BrailleIO framework is spitted into several small parts, so the usage and extensibility should be improved. In the following the subprojects will be explained in more detail.

### BrailleIO

This is the main project linking all other projects together. It is used to initialize the main component of the framework (see usage and example) and holds basic class implementations and basic renderers for general content types.

In the following the main components will be named and explained.

[[*back to outline* :arrow_up:]](#outline)

#### BrailleIOMediator

Central instance for the BrailleIO Framework.  It connects the hardware abstraction layers and the GUI/TUI components. It gives you access to the hardware via the IBrailleIOAdapterManager. The GUI/TUI components are available through several methods.

In this component the renderers for the currently active screens and visible views will be called. The rendered content will be sent to all the connected hardware adapters.

![Basic UML structure of the BrailleIO framework showing the BraillIOMediator connecting the UI elements with the Hardware abstraction part](/doc_imgs/UML-Basic-Structure.png)

[[*back to outline* :arrow_up:]](#outline)

#### BrailleIOScreen

This is a container for several views that can contain content. Only one screen can be visible at the same time. You can build an unlimited number of screens and fill them with content. This makes it easy to switch between different views or different applications.

![BrailleIO can build applications with several screens. A screen can contain several viewRanges that can have independent controlled content](/doc_imgs/sreens-and-viewRange.png)

[[*back to outline* :arrow_up:]](#outline)

#### BrailleIOViewRange

This is the component that can hold content. The combination of content with a standard- or user-defined renderer enables to transform abstract content elements into tactile output on a two-dimensional pin device.
Several viewRanges can be combined in a screen to build a complex view. ViewRanges can be independently switched on and off and can be freely placed inside a screen. 

A viewRange has a complex box model with margin padding and boarder. All properties of the box model are independently definable in all four dimensions. 

![BrailleIO view ranges have a full box model consisting of margin, border and padding](/doc_imgs/viewBox.png)

A view Range has a view Box – which defines the position, size and look on the screen – and a content box which holds the rendered content. This Content box can be larger than the view box. Through offsets in x- and y-direction the content can be moved to become visible inside the vie box. So the content box is moved under the view box to show hidden content. Therefore the offsets have to get negative.

![The content of a view range can be larger as the visible area. The content can be moved underneath the viewable area controlled by offset-properties](/doc_imgs/ContentBox.png)

Content can be zoomed if the currently active renderer allows for zoomed rendering.

[[*back to outline* :arrow_up:]](#outline)

#### Adapter

##### AbstractBrailleIOAdapterManagerBase

The adapter manager is an abstract implementation for a component that manages the connection of different specific hardware abstractions. Adapters can be registered and unregistered at this component. The Adapatermanager that is linked to the BrailleIOMediator will be requested for all active hardware displays.

![The abstract tactile displays should contain of a display area and 9 general buttons: ok, esc, gesture, 4 direction buttons, zoom-in and zoom-out](/doc_imgs/general_device.png)

![An hardware abstracting adapter implementation has to implement the interface IBrailleIOAdapter and has to fill his fields to enable to proper usage of the hardware.](/doc_imgs/UML-Adapter.png)

[[*back to outline* :arrow_up:]](#outline)

#### Renderer

The renderers are connected to different kinds of content objects for view ranges. In this part the view standard renderers are defined and described.

In the BrailleIOMediator the renderers are called in the following order to build a combined tactile view of the content:

1.	Content renderer to transform content-object into binary matrix (BrailleIOImageToMatrixRenderer or other connected content renderer)
2.	BrailleIOViewMatixRenderer to place the rendered content inside the view range with respect to the set box model.
3.	BrailleIOBorderRenderer to render the defined borders from the box model on top.

##### BrailleIOBorderRenderer

Renders the in the box model defined borders for a view range as solid lines. 

##### BrailleIOImageToMatrixRenderer

Converts a given image-object to a binary matrix applying a lightness threshold on every image pixel to taxel. In advanced the image will be down- or upscaled given by the set zoom-level of the view range.

##### BrailleIOScrollbarRenderer

Renders scrollbars inside the view rage to indicate the values of the offset positions to the reader. Those will only be rendered if the “ShowScrollbars” field of a view range was set to true.

##### BrailleIOViewMatixRenderer

Places the rendered content matrix inside the view box with respect of panning offsets and the defined box model.

[[*back to outline* :arrow_up:]](#outline)

### BrailleIO_Interfaces

This project is a separated project for interface definitions, abstract basic implementations and base-type definitions to allow for extensibility of the BrailleIO framework by external projects such as the Braille-renderer. Some interesting elements will be explained in the following.

#### Structs

##### BoxModel

The box model struct is used to define margin, padding and borders for view ranges.

##### RenderElement

This struct is used to build rendered element trees for renderer results. This can be used to return the source elements of a renderer result on a certain content position. Can be used as result value for ITouchableRenderer requests.

[[*back to outline* :arrow_up:]](#outline)

#### Interfaces

##### ITouchableRenderer

An Interface for renderers that allow for requesting the basic rendered object before it was transformed into a binary view. This allows for building touchable interactive interfaces if the current used Renderer supports this interface.

The following standard renderers support this interface in a more or less detailed manner:

* MatrixBrailleRenderer (renderer for transforming Strings into Braille)

##### IBraileInterpreter

Interface for converting strings to Braille and back.

##### IBrailleIOAdapter

The basic interface a specific hardware abstraction has to implement. This interface contains all necessary functions, fields and properties that allows for interacting through and displaying on a certain hardware display to be used as an input- or output device.

##### IBrailleIOAdapterManager

Interface for functions a adapter manager has to offer to the BrailleIoMediator. Normally the abstract AbstractBrailleIOAdapterManagerBase can be used.

##### IBrailleIOContentChangedEventSupplier

Allows for listeners to get informed if the content of a view range was changed.

##### IBrailleIOPropertiesChangedEventSupplier

Allows for listeners to get informed if properties of a view rage, such as scrolling, or zooming values, has been changed.

##### IBrailleIORendererInterfaces

The basic function a render must offer to be used as a matrix based renderer for a view range. Implements IBrailleIOContentRenderer as well.

##### IBrailleIOContentRenderer

The most abstract basic function a render must offer to be used as a content renderer for a view range.

##### IBrailleIOHookableRenderer

Some renderers can be hooked by external projects. This means that the renderers’ parameters can be manipulated in advanced of the rendering or the rendering result can be adopted in the end by the hooking process.

The abstract BrailleIOHookableRendererBase can be used to build a hookable renderer in an easy way.

The following standard renderer are hookable:
* BrailleIOImageToMatrixRenderer
* BrailleIOViewMatixRenderer
* MatrixBrailleRenderer

##### IBailleIORendererHook

The interface a process has to implement to be used as a renderer hook.

[[*back to outline* :arrow_up:]](#outline)

### BrailleIO_ShowOff

This project is an implementation of a specialized hardware abstraction of a certain tactile input/output device. It is not build as a real hardware abstraction it is only used as a software emulation for a hardware device. In this case, this software emulates a BrailleDis 7200 Device by Metec AG. It can be used to display the rendering outputs of the BrailleIO framework, as well as simulate button and gesture/touch interaction.

In the following, the usage and some special features of this emulator and debug monitor are explained.

[[*back to outline* :arrow_up:]](#outline)

#### Interaction

The ShowOff adapter is designed to emulate user inputs through its GUI. Button commands as well as single touch inputs can be simulated.

##### Touch (gesture) interaction

The ShowOff adapter can also simulate touch data for the `touchValuesChanged` event. Therefore, you have to move the mouse cursor over the simulated tactile display area in the GUI and press the left mouse button. With every move of the cursor or button interaction, a touch event will be generated. Based on the mouse input metaphor only single-touch interaction is possible to simulate. A simulated touch blob has a diameter of 1.5 pins in each direction.


##### Button interaction

For simulating a single button command, you only have to click one of the 36 designed buttons of the GUI. By clicking a button a button-pressed and a button-released command are fired to listeners of the `keyStateChanged` event.

But not only single button commands are necessary. For non-visual interaction button combinations are essential to support, e.g. in the case of writing Braille with a Braille-keyboard where a single letter is combined out of up to 8 buttons pressed as one! To enable the simulation of multi button command, the ShowOff adapter has the possibility to hold some buttons in the pressed state and release all pressed buttons at once. To do so, you have to hold the Ctrl. keyboard key. While keeping the Ctrl. key pressed, only button pressed events are fired when clicking a new button. If you release the Ctrl. key all collected and currently pressed buttons are send as one single button released event. You can disable a pressed button by clicking them again – the button is removed from the list of buttons to release.

###### Button Codes

To build applications based on the ShowOff adapter, you have to know the key codes used for the button interaction events.

There are 3 kinds of button types available in the BrailleIO framework:

1. `BrailleIO.Interface.BrailleIO_DeviceButton` - 8 general buttons, a two dimensional pin-matrix device have to have to support at least some basic user interaction functionalities. In Addition, a 9th button is defined for devices supplying touch interaction, to switch applications in a gesture-mode to avoid the midas-touch effect. 
2. `BrailleIO.Interface.BrailleIO_BrailleKeyboardButton` - 12 buttons to define an 8-dot Braille keyboard with the addition of 4 function keys for text input interaction.
3. `BrailleIO.Interface.BrailleIO_AdditionalButton` - here an unlimited number of additional function keys can be modeled in stacks of 15 buttons. 

In the ShowOff adapter, the buttons are placed and coded as follow (only one stack of 15 additional buttons is modeled):

Standard buttons (BrailleIO_DeviceButton) are marked with grey, Braille Keyboard buttons (BrailleIO_BrailleKeyboardButton) are marked in yellow and 15 additional function keys (BrailleIO_AdditionalButton) are marked in light blue.

All 36 button codes together:

![The BrailleIO ShowOff Adapter monitor GUI window, with his 36 button definitions based on the modeling system](/doc_imgs/ShowOff.png)

Beyond the generalized button modelling the ShowOff adapter also simulates the proprietary interpretation of buttons and provide them in the raw data filed of the `keyStateChanged` event. Therefore, the buttons are defined by a string uid. The string uids are defined as followed:

TODO: image 

**Hint:**

To make the handling of the several button state Enum-Flag combined fields a bit more handy, in `BrailleIO.Interface.Utils` some functions for transforming the button states into buttons, extract special button states (pressed / released), switch button states to their revers state or get button states for buttons etc. exist. Have a look to the help files for a more detailed overview. Or take a look at the Examples section.


[[*back to outline* :arrow_up:]](#outline)


#### DEBUG and GUI

The ShowOff adapter is designed as a debug tool and to not make the availability of a two-dimensional, dynamic tactile pin matrix device an essential part for software development for such devices. Thereto, often not only the mimic of standard device features, such as displaying a bool matrix or sending touch and button interaction, are necessary. Moreover, some additional functionality for a better debugging and monitoring are desirable for developers. 

In the following, some features for making the ShowOff adapter a powerful debug monitor are presented.

##### Single line debug output console

The ShowOff adapter GUI has a status strip at the very bottom of the window. To this status strip, you can sent text messages, which will be displayed. 

``` C#
// create a ShowOff monitor object
BrailleIO.IBrailleIOShowOffMonitor Monitor = new ShowOff();
// send your debug text to the status strip
Monitor.SetStatusText("Hello World");

// erase the text by simply calling
Monitor.ResetStatusText();
```

##### Mirror touch data from other devices
Let the ShowOff GUI display the touch data from other connected BrailleIO adapters to visualize them to you.

``` C#
// create a ShowOff monitor object
BrailleIO.IBrailleIOShowOffMonitor Monitor = new ShowOff();

...

// in the touch event handler you can mirror the touch data to the ShowOff GUI
void _touchValuesChanged(object sender, BrailleIO.Interface.BrailleIO_TouchValuesChanged_EventArgs e)
{
	if (e != null)
	{
		if (Monitor != null) Monitor.PaintTouchMatrix(e.touches, e.DetailedTouches);
	}
}
```

##### Mirror button interaction states from other devices

TODO: have to be adapted to the new button codes ...

##### Create a visual overlay on top of the display area

For visualizing additional data, you can paint a visual image and lay this over the display area of the ShowOff GUI.

``` C#
// create a ShowOff monitor object
BrailleIO.IBrailleIOShowOffMonitor Monitor = new ShowOff();

...

if (Monitor != null)
{
	// create your overlay picture
	Bitmap img = new Bitmap(
				Monitor.PictureOverlaySize.Width,
				Monitor.PictureOverlaySize.Height);

	using (Graphics g = Graphics.FromImage(img))
	{
		g.DrawString("Hellow World", 
			SystemFonts.DefaultFont, Brushes.Red, 
			new PointF(20, 50));
	}
	// send the image to the ShowOff monitor for displaying
	Monitor.SetPictureOverlay(img);
}
```

##### Change the title of the GUI window

Of cause sometimes, the Debug Monitor is used as a real application’s GUI. Therefore, the title of the GUI window is changeable.

``` C#
// create a ShowOff monitor object
BrailleIO.IBrailleIOShowOffMonitor Monitor = new ShowOff();
// sent your title for the application window
// this feature is not part of the interface, but of the ShowOff object
((ShowOff)Monitor).SetTitle("My tactile application");
```

##### Add a menu to the GUI Window

Of cause sometimes, the Debug Monitor is used as a real application’s GUI. Therefore, the possibility to add a standard menu is provided.

``` C#
// create a ShowOff monitor object
BrailleIO.IBrailleIOShowOffMonitor Monitor = new ShowOff();

// make the menu strip appear in the GUI
((ShowOff)Monitor).ShowMenuStrip();

// hide the menu strip from the GUI
// ((ShowOff)Monitor).HideMenuStrip();

// Build your menu 
var item = new ToolStripMenuItem("&Application");
item.Click += item_Click; // add event handlers

item.DropDown = new ToolStripDropDown();
item.DropDown.Items.Add("&Exit", null, item_Click);

// add your menu to the strip
// this feature is not part of the interface, but of the ShowOff object
((ShowOff)Monitor).AddMenuItem(item);
```


[[*back to outline* :arrow_up:]](#outline)



### BrailleRenderer

This project builds the currently used renderer to transform a given string input into a representation in computer-braille by applying a defined translation table. The translation tables are built by the “Liblouis” project. Currently the German translation tables are used but of course others can be defined and loaded trough the constructor.

This render builds the standard render for String content of the BrailleIo framework. It also implements the ITouchableRenderer interface in a very detailed manner. This allows for highly interactive touchable interface build on this simple text-to-Braille-renderer.

[[*back to outline* :arrow_up:]](#outline)

### GestureRecognizer

This project is a simple gesture recognizer for touch interaction on input devices. It can be used for identifying simple gestures in a series of touch states.

This recognizer currently can identify the following gestures:
* Point (simple touch)
* Line (single line)
* Drag (two to three finger line)
* Pinch
* Circle
* Half circle

The recognizer can be used as following:

``` C#
// set up a new blob tracker for tracking related finger blobs as continuous trajectory
BlobTracker blobTracker = new BlobTracker();
// create new gesture recognizer and hand over the blob tracker
GestureRecognizer gestureRecognizer = new GestureRecognizer(blobTracker);
```

For different types of gestures, you have to give the gesture recognizer a classifier, that can recognize a gesture out of the given touch values.

Two standard classifiers are available. If want to classifier more than the standard gestures or in a more stable or optimized way, you can provide your own classifier here. 

``` C#
// add several classifiers to interpret the tracked blobs
gestureRecognizer.AddClassifier(new TapClassifier());
gestureRecognizer.AddClassifier(new MultitouchClassifier());
```

So the gesture recognition is set up. To start a recognition you have to set the recognizer into evaluation mode and submit frames of touch values, which are analyzed by the blob tracker. After stopping the evaluation of the frames, the gesture recognizer will call the registered classifiers that will return a classification result, which will be finally returned as recognized gesture result.

**ATTENTION:** Because of the so called Midas-Touch effect, you cannot make a continuous touch evaluation on a tactile display. This is because the non-visual reader does use touch interaction for retrieving the content on the display, so every time s/he tries to read an input would be triggered. It is better to use a button that switches the display into a gesture recognition mode. Here the system can rely on the given touch inputs as valid input commands. Therefore, a standard key has been defined you can listen for get pressed to start evaluation and stop evaluation when it is released.


In the event handler function for button commands from an adapter, you can check for the gesture button down command and start sending touch value frames to the gesture recognizer.
``` C#
/// flag for sending touch values to the gesture recognizer or not
bool interpretGesture = false;

/// <summary>
/// Handles the keyStateChanged event of the _bda control.
/// </summary>
/// <param name="sender">The source of the event.</param>
/// <param name="e">The <see cref="BrailleIO.Interface.BrailleIO_KeyStateChanged_EventArgs"/> instance containing the event data.</param>
void _keyStateChanged(object sender, BrailleIO.Interface.BrailleIO_KeyStateChanged_EventArgs e)
{
	if ((e.keyCode & BrailleIO_DeviceButtonStates.GestureDown) == BrailleIO_DeviceButtonStates.GestureDown)
	{
		// start sending touch values to the gesture recognizer
		interpretGesture = true; 
	}
	else if ((e.keyCode & BrailleIO_DeviceButtonStates.GestureUp) == BrailleIO_DeviceButtonStates.GestureUp)
	{ 
		// stop sending touch values to the gesture recognizer
		interpretGesture = false; 
	}
}
```

In the event handler for touch values the >>frames<< of touch values have to be hand over to the gesture recognizer.

``` C#
/// <summary>
/// Handles the touchValuesChanged event of the _bda control.
/// </summary>
/// <param name="sender">The source of the event.</param>
/// <param name="e">The <see cref="BrailleIO.Interface.BrailleIO_TouchValuesChanged_EventArgs"/> instance containing the event data.</param>
void _touchValuesChanged(object sender, BrailleIO.Interface.BrailleIO_TouchValuesChanged_EventArgs e)
{
	if (e != null)
      {
		// add touches to the gesture recognizers
           if (interpretGesture && gestureRecognizer != null)
           {
           		gestureRecognizer.AddFrame(new Frame(e.touches));
		}
	}
}
```

In the end, you can stop the evaluation and request the gesture recognizer for an available classification result of one of the registered classifiers. 

``` C#
IClassificationResult gesture = null;
if (gestureRecognizer != null)
{
      gesture = gestureRecognizer.FinishEvaluation();
}
```

[[*back to outline* :arrow_up:]](#outline)

### BrailleIOExample

This is an example application showing for the usage of several basic functions and objects.

[[*back to outline* :arrow_up:]](#outline)


## How to use:

--	TODO: build a small workflow

![An application using BrailleIO can be set up in a view steps ](/doc_imgs/Usage.png)

[[*back to outline* :arrow_up:]](#outline)


### How to build a hardware abstraction

The abstract tactile displays should contain of a display area and 9 general buttons: ok, esc, gesture, 4 direction buttons, zoom-in and zoom-out

![The abstract tactile displays should contain of a display area and 9 general buttons: ok, esc, gesture, 4 direction buttons, zoom-in and zoom-out](/doc_imgs/general_device.png)

An hardware abstracting adapter implementation has to implement the interface IBrailleIOAdapter and has to fill his fields to enable to proper usage of the hardware.

![An hardware abstracting adapter implementation has to implement the interface IBrailleIOAdapter and has to fill his fields to enable to proper usage of the hardware.](/doc_imgs/UML-Adapter.png)

#### Set up a new Adapter

If you want to create a new hardware abstraction, you have to supply a wrapper for the real hardware interface commands. In this wrapper class, you have to implement the `IBrailleIOApapter` interface. You can also use the abstract implementation of this interface `AbstractBrailleIOAdapterBase` for extension. This abstract class contains an additional property `Synch` that allows the adapter to receive a copy of the content sent to the main adapter – to mirror the display for example.

##### Recommendations for event data (raw data)

In the event arguments, e.g. for the `KeyStateChanged` events, there is an integrated option to submit the raw device data. These are submitted in an `OrderedDictonary`. In my hardware abstractions, I use the dictionary to submit button states for hardware keys that go beyond the generalized 9 standard buttons. Therefore, I fill the dictionary as follows:

Key| Content | Description
------------ | ------------- | -------------
allPressedKeys| `List<String>` | List of stings that represent all currently pressed buttons
allReleasedKeys| `List<String>` | List of stings that represent all last released buttons
newPressedKeys| `List<String>` | List of stings that represent all last newly pressed buttons

[[*back to outline* :arrow_up:]](#outline)

## You want to know more?
For getting a very detailed overview use the [code documentaion section](/Help/index.html) of this project.

[[*back to outline* :arrow_up:]](#outline)

# Examples 

## Small Basic Setup of three regions in one screen

TODO: add a MWE.


## Button interaction handling

Button handling for the general modeled buttons is quite easy. You only have to check, if your buttons you are looking for are set as the sate you want to handle.

In practice it becomes clear that handling released buttons is more stable than handling the pressed events. This is based on the nature of non-visual interaction, where button interaction is often handled as button combinations. This means a user takes time to find and press all the buttons of a certain combination but can release them together in a short period you have to observe. Beside you can make some validations for a correct button handling, e.g. don’t accept the button event if another button is currently pressed, which is an indication for an error or the intension for an extended button combination.

For a button handling first, you need an adapter to observe for button events.

``` C#
// set up a dummy adapter ... use the build in ShowOff Adapter
BrailleIOMediator io = BrailleIOMediator.Instance;

Monitor = new ShowOff();
IBrailleIOAdapter showOffAdptr = Monitor.GetAdapter(io.AdapterManager);
if (showOffAdptr != null)
{
	// add the dummy adapter to the Adapter manager of the framework
	io.AdapterManager.AddAdapter(showOffAdptr);
	// make them the active one! So it will be filled with tactile content.
	io.AdapterManager.ActiveAdapter = showOffAdptr;

	// if don't want to make the dummy adapter the main one 
	// but monitor the tactile output, use the 'Synch' property of its
	// abstract implementation 'AbstractBrailleIOAdapterBase'
	((AbstractBrailleIOAdapterBase)showOffAdptr).Synch = true;

	// register to the key events
	showOffAdptr.keyStateChanged += showOffAdptr_keyStateChanged;
}
```

In the related Event handler, you can check the different button states
 
``` C#
/// <summary>
/// Handles the keyStateChanged event of the showOffAdptr.
/// </summary>
/// <param name="sender">The source of the event.</param>
/// <param name="e">The <see cref="BrailleIO_KeyStateChanged_EventArgs"/> instance containing the event data.</param>
/// <exception cref="System.NotImplementedException"></exception>
void showOffAdptr_keyStateChanged(object sender, BrailleIO_KeyStateChanged_EventArgs e)
{
	// handle the event data
	// in the 'sender' parameter you can check from which device/adapter the interaction is reported

	if (e != null)
	{
		// the event data contains three types of button commands

		// 1. type: general buttons
		BrailleIO_DeviceButtonStates gnrl = e.keyCode;
		// 2. type: BrailleKeyboard buttons
		BrailleIO_BrailleKeyboardButtonStates brlKb = e.keyboardCode;
		// 3. type: unlimited number of additional buttons, packed in stacks of 15
		//      we only use the showOffAdapter, which only have 15 additional buttons
		BrailleIO_AdditionalButtonStates add = BrailleIO_AdditionalButtonStates.None;
		if (e.additionalKeyCode != null && e.additionalKeyCode.Length > 0)
			add = e.additionalKeyCode[0];

		// you can check for released and pressed button changes
		// the BrailleIO.Interface.Utils can help you a lot.
		// first: pressed buttons (see the changed enum type!)
		BrailleIO_DeviceButton pressedGnrl = BrailleIO.Interface.Utils.GetAllDownDeviceButtons(gnrl);
		BrailleIO_BrailleKeyboardButton pressedBrlKb = BrailleIO.Interface.Utils.GetAllDownBrailleKeyboardButtons(brlKb);
		BrailleIO_AdditionalButton pressedAdd = BrailleIO.Interface.Utils.GetAllDownAdditionalButtons(add);

		// second get all released buttons
		BrailleIO_DeviceButton releasedGnrl = BrailleIO.Interface.Utils.GetAllUpDeviceButtons(gnrl);
		BrailleIO_BrailleKeyboardButton releasedBrlKb = BrailleIO.Interface.Utils.GetAllUpBrailleKeyboardButtons(brlKb);
		BrailleIO_AdditionalButton releasedAdd = BrailleIO.Interface.Utils.GetAllUpAdditionalButtons(add);
   
		// TODO: ... do your button handling here

	}
}
```

Now you can handle your buttons you are looking for:


Checking only if one single button was released:

``` C#
if (releasedGnrl.HasFlag(BrailleIO_DeviceButton.Enter))
{
	// do the trick for the general button 'Enter'
}
```

Checking for several released buttons:

``` C#
if (releasedGnrl == (BrailleIO_DeviceButton.Enter | BrailleIO_DeviceButton.Gesture))
{
	// do the trick for 'Enter' and 'Gesture' are released at the same time
}
```

Checking over multiple button types at once:

``` C#
// checking over multiple button types
// ATTENTION: if more than the general buttons are pressed, 
// the 'Unknown' flag in the general buttons should bee set!
if (
	releasedGnrl == (BrailleIO_DeviceButton.Unknown | BrailleIO_DeviceButton.Enter)
	&& releasedBrlKb == BrailleIO_BrailleKeyboardButton.k1
	&& releasedAdd == (BrailleIO_AdditionalButton.fn1 | BrailleIO_AdditionalButton.fn2)
	)
{
	// do the trick for general 'Enter', the Braille keyboard 'k1' and the additional
	// buttons 'fn1' and 'fn2' are all released at the same time
}
```

As mentioned before, you can check if a button is still pressed when handling only complete released combinations:


``` C#
// check if some other button is still pressed
// you have to remove the 'Unknown' key if it was set. Normally only for the general buttons.
// ... does also work for released buttons

if ((pressedGnrl ^ BrailleIO_DeviceButton.Unknown) == BrailleIO_DeviceButton.None
	&& (pressedBrlKb ^ BrailleIO_BrailleKeyboardButton.Unknown) == BrailleIO_BrailleKeyboardButton.None
	&& (pressedAdd ^ BrailleIO_AdditionalButton.Unknown) == BrailleIO_AdditionalButton.None) 
{
	// do the trick if no button was pressed 
}
```

Because the enums are based on Int32 numbers you can also do this

``` C#
if ((int)(pressedGnrl ^ BrailleIO_DeviceButton.Unknown)
	+ (int)(pressedBrlKb ^ BrailleIO_BrailleKeyboardButton.Unknown)
	+ (int)(pressedAdd ^ BrailleIO_AdditionalButton.Unknown) == 0)
{
	// do the trick if no button was pressed 
}
```

Complex button combinations should be collected with released events over a small period of time until no more button is pressed. This is an indication, that all buttons were released and the previously released button events should be related to this one. In practical use a time span of about 500 milliseconds is reasonable.


For getting an easy overview if some buttons are still pressed and the sending device/adapter is implementing the `BrailleIO.Interface.IBrailleIOAdapter2` interface, you can simply ask for the currently pressed buttons. But not for the released ones!

``` C#
if (sender is IBrailleIOAdapter2)
{
   BrailleIO_DeviceButton currentlyPressedGnrl = ((IBrailleIOAdapter2)sender).PressedDeviceButtons;
   BrailleIO_BrailleKeyboardButton currentlyPressedBrlKb = ((IBrailleIOAdapter2)sender).PressedBrailleKeyboardButtons;
   BrailleIO_AdditionalButton[] currentlyPressedAdd = ((IBrailleIOAdapter2)sender).PressedAdditionalButtons;
}
```


[[*back to outline* :arrow_up:]](#outline)

# Deep Dive

Here, very special topics are explained.


## Build your own renderer

If you want to set up your own type of content that is beyond the currently available image and string-to-Braille possibilities, you have to provide your own specialized renderer.

The basic type of content, any other content will be transformed to, is the two-dimensional Boolean matrix (**ATTENTION:** the matrix is a mathematical object (m[i,j]) in which the first dimension is the row – which corresponds to the y-value in an image, and the second dimension is the column – which corresponds to the x-value).
If you have set up a ViewRange, you can add any kind of object as content, as long as you give a renderer that can transform it into a bool matrix.

``` C#
/// <summary>
/// Sets an generic content and a related renderer for this type.
/// </summary>
/// <param name="content">The content.</param>
/// <param name="renderer">The renderer - can not be null.</param>
public void SetOtherContent(Object content, IBrailleIOContentRenderer renderer)
```

The render has to, at least, implement the `BrailleIO.Interface.IBrailleIOContentRenderer`. This only contains of the function:

``` C#
/// <summary>
/// Renders a content object into an boolean matrix;
/// while <c>true</c> values indicating raised pins and <c>false</c> values indicating lowered pins
/// </summary>
/// <param name="view">The frame to render in. This gives access to the space to render and other parameters. Normally this is a <see cref="BrailleIOViewRange"/>.</param>
/// <param name="content">The content to render.</param>
/// <returns>
/// A two dimensional boolean M x N matrix (bool[M,N]) where M is the count of rows (this is height)
/// and N is the count of columns (which is the width). 
/// Positions in the Matrix are of type [i,j] 
/// while i is the index of the row (is the y position) 
/// and j is the index of the column (is the x position). 
/// In the matrix <c>true</c> values indicating raised pins and <c>false</c> values indicating lowered pins</returns>
bool[,] RenderMatrix(IViewBoxModel view, object content);
``` 

In the implementation of the function, the renderer get information about the view (normally this is a `BrailleIOViewRange`) and the content to transform.

### Scrolling
In your renderer implementation, you do not have to care about the functionalities of scrolling! If you build a bool matrix larger than the available display area (`ContentBox`) of the related `BrailleIOViewRange` and the `ShowScrollbars` property of the view range is set to `true`, the framework will handle the creation of scrollbars. 

To handle scrolling, the properties `OffsetPosition.X` and `OffsetPosition.Y` are used to define the content position in relation to the visible area. Therefore, these properties are normally negative values. See section [ViewRange](#brailleioviewrange).

Handling the user interaction manipulating those two properties is not part of the framework and has to be handled by the developer. To help you doing so, in the `BrailleIO.Interface.AbstractViewBoxModelBase` implementation for the view range, several helper function for content movement exist.

**ATTENTION:** If you only want to enable a vertical scrolling, e.g. in the case of displaying long texts etc., you have to provide enough space for the afterwards added scrollbars at the right border of the view range. This have to be at least 3 dots for normal scroll bars and 4 dots for scroll-arrows, which are visualizations for scrollbars for small areas (when a fully visible scrollbar does not have enough space).  

[[*back to outline* :arrow_up:]](#outline)

### Make the renderer hookable

To enable developers to adapt the rendering results, the ability of hooking is implemented for the standard renders (image, text). This you can use as well for your own renderer.

Hooking means that at a/some certain point/s the hooks are called to manipulate either the basic parameters or the result of a function. For renderers two points for hooking exist: 1) before rendering (manipulating the parameters) and 2) after the rendering (adapting the result). To enable your renderer to bee hook, you have to implement the interface:

``` C#
/// <summary>
/// Interface that a renderer has to implement if he wants to allow hooking
/// </summary>
public interface IBrailleIOHookableRenderer
{
	/// <summary>
	/// Register a hook.
	/// </summary>
	/// <param name="hook">The hook.</param>
	void RegisterHook(IBailleIORendererHook hook);
	/// <summary>
	/// Unregisters a hook.
	/// </summary>
	/// <param name="hook">The hook.</param>
	void UnregisterHook(IBailleIORendererHook hook);
}
```

To make it easier to use and to handle all the hooking and hook calling stuff, an abstract implementation for the interface exists with the `BrailleIO.Interface.BrailleIOHookableRendererBase`. The only thing you have to do is to call the functions `protected virtual void callAllPreHooks(ref IViewBoxModel view, ref object content, params object[] additionalParams)` before starting the rendering and `protected virtual void callAllPostHooks(IViewBoxModel view, object content, ref bool[,] result, params object[] additionalParams)` before returning the result.

ATTENTION: Be aware that calling hooks can damage your rendering result and can make the rendering inefficient and slow!

[[*back to outline* :arrow_up:]](#outline)

### Make a renderer cacheable

Starting the rendering of content when it should be displayed, can take a long time and make the application can feel slow. In addition, if no changes in the content or its influencing parameters, such as thresholds, zooming etc., where made, there is no need for a real new rendering of the content. For all this, the framework provides the possibility of some caching rendering result.

To make your renderer cacheable, to the standard `BrailleIO.Interface.IBrailleIOContentRenderer` you have to implement the interface:

``` C#
namespace BrailleIO.Renderer
{
    /// <summary>
    /// Interface for caching rendering results.
    /// </summary>
    public interface ICacheingRenderer
    {
        /// <summary>
        /// Gets or sets a value indicating whether content changed or not to check if a new rendering is necessary.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [content has changed]; otherwise, <c>false</c>.
        /// </value>
        bool ContentChanged { get; set; }
        /// <summary>
        /// Gets the time stamp for the last content rendering.
        /// </summary>
        /// <value>
        /// The last time stamp of content rendering rendered.
        /// </value>
        DateTime LastRendered { get; }
        /// <summary>
        /// Gets a value indicating whether this instance is currently rendering.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is currently rendering; otherwise, <c>false</c>.
        /// </value>
        bool IsRendering { get; }

        /// <summary>
        /// Informs the renderer that the content the or view has changed.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="content">The content.</param>
        void ContentOrViewHasChanged(IViewBoxModel view, object content);

        /// <summary>
        /// Renders the current content
        /// </summary>
        void PrerenderMatrix(IViewBoxModel view, object content);

        /// <summary>
        /// Gets the previously rendered and cached matrix.
        /// </summary>
        /// <returns>The cached rendering result</returns>
        bool[,] GetCachedMatrix();

    }
}
```

In the function void `PrerenderMatrix(IViewBoxModel view, object content)` the rendering of the bool matrix after a content or property change is requested. By calling the `bool[,] GetCachedMatrix()` function, a prerendered rendering result is supplied.

Of cause, you can make your renderer hookable and touchable as well. An abstract implementation of a cached renderer is also available:

``` C#
namespace BrailleIO.Renderer
{
    /// <summary>
    /// Abstract implementation for renderer that allow for a caching. 
    /// This renderer can hold a prerendered result. If the content doesn't change and a 
    /// rendering request force them for a rendering, they will return the cached result 
    /// without any new rendering.
    /// </summary>
    /// <seealso cref="BrailleIO.Interface.BrailleIOHookableRendererBase" />
    /// <seealso cref="BrailleIO.Renderer.ICacheingRenderer" />
    /// <seealso cref="BrailleIO.Interface.IBrailleIORendererInterfaces" />
    public class AbstractCachingRendererBase : BrailleIOHookableRendererBase, ICacheingRenderer, IBrailleIORendererInterfaces
    {
		...
	}
}
```

[[*back to outline* :arrow_up:]](#outline)

### Make your renderer touchable

To enable users to interact with your application via touch, the possibility of accessing the basic content objects out of the rendering result have to be provided. So auditory feedback or direct manipulation gets possible. 
Therefore, the renderer have to keep information about which content object is rendered to which position in the resulting matrix. To make this feature available to the developers your renderer has to implement the interface:

``` C#
namespace BrailleIO.Renderer
{
    public interface ITouchableRenderer
    {
        /// <summary>
        /// Gets the Object at position x,y in the content.
        /// </summary>
        /// <param name="x">The x position in the content matrix.</param>
        /// <param name="y">The y position in the content matrix.</param>
        /// <returns>An object at the requester position in the content or <c>null</c></returns>
        Object GetContentAtPosition(int x, int y);

        /// <summary>
        /// Get all Objects inside (or at least partial) the given area.
        /// </summary>
        /// <param name="left">Left border of the region to test (X).</param>
        /// <param name="right">Right border of the region to test (X + width).</param>
        /// <param name="top">Top border of the region to test (Y).</param>
        /// <param name="bottom">Bottom border of the region to test (Y + heigh).</param>
        /// <returns>A list of elements inside or at least partial inside the requested area.</returns>
        IList GetAllContentInArea(int left, int right, int top, int bottom);
    }
}
```

For content objects, such as texts, a helping structure called `BrailleIO.Renderer.Structs.RenderElement` is available. In this struct several features such as building a content tree. This struct is only usable for rectangular content types because of its bounding-box based metaphor.

[[*back to outline* :arrow_up:]](#outline)
