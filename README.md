BrailleIO
=========

## Intension:

BrailleIO is a small .NET framework for developing two-dimensional tactile applications. It offers general features for displaying tactile information and for interaction. BrailleIO includes hardware abstraction, window and visualization features as well as basic interaction functions, such as panning and zooming on different content types. 

Information visualization can be organized in several independent screens that can be divided into multiple areas, having a full box model. Interaction can be realized via hardware keys of the used device or by basic gestures if the device is touch-sensitive.

BrailleIO is a small framework that should enable application developers to fast and easily build non-visual applications for on a planar two-dimensional tactile pin-matrix display. It is written in C# for .Net and is therefore a framework for windows applications manly. 

More technical details can be found in [Bornschein, Jens. "BrailleIO-a Tactile Display Abstraction Framework." Proceedings of TacTT ’14 Workshop, Nov 16 2014, Dresden, Germany. 2014.](http://ceur-ws.org/Vol-1324/paper_4.pdf)

## Subprojects

The BrailleIO framework is spitted into several small parts, so the usage and extensibility should be improved. In the following the subprojects will be explained in more detail.

### BrailleIO

This is the main project linking all other projects together. It is used to initialize the main component of the framework (see usage and example) and holds basic class implementations and basic renderers for general content types.

In the following the main components will be named and explained.

#### BrailleIOMediator

Central instance for the BrailleIO Framework.  It connects the hardware abstraction layers and the GUI/TUI components. It gives you access to the hardware via the IBrailleIOAdapterManager. The GUI/TUI components are available through several methods.

In this component the renderers for the currently active screens and visible views will be called. The rendered content will be sent to all the connected hardware adapters.

![Basic UML structure of the BrailleIO framework showing the BraillIOMediator connecting the UI elements with the Hardware abstraction part](/doc_imgs/UML-Basic-Structure.png)

#### BrailleIOScreen

This is a container for several views that can contain content. Only one screen can be visible at the same time. You can build an unlimited number of screens and fill them with content. This makes it easy to switch between different views or different applications.

![BrailleIO can build applications with several screens. A screen can contain several viewRanges that can have independent controlled content](/doc_imgs/sreens-and-viewRange.png)

#### BrailleIOViewRange

This is the component that can hold content. The combination of content with a standard- or user-defined renderer enables to transform abstract content elements into tactile output on a two-dimensional pin device.
Several viewRanges can be combined in a screen to build a complex view. ViewRanges can be independently switched on and off and can be freely placed inside a screen. 

A viewRange has a complex box model with margin padding and boarder. All properties of the box model are independently definable in all four dimensions. 

![BrailleIO view ranges have a full box model consisting of margin, border and padding](/doc_imgs/viewBox.png)

A view Range has a view Box – which defines the position, size and look on the screen – and a content box which holds the rendered content. This Content box can be larger than the view box. Through offsets in x- and y-direction the content can be moved to become visible inside the vie box. So the content box is moved under the view box to show hidden content. Therefore the offsets have to get negative.

![The content of a view range can be larger as the visible area. The content can be moved underneath the viewable area controlled by offset-properties](/doc_imgs/ContentBox.png)

Content can be zoomed if the currently active renderer allows for zoomed rendering.

#### Adapter

##### AbstractBrailleIOAdapterManagerBase

The adapter manager is an abstract implementation for a component that manages the connection of different specific hardware abstractions. Adapters can be registered and unregistered at this component. The Adapatermanager that is linked to the BrailleIOMediator will be requested for all active hardware displays.

![The abstract tactile displays should contain of a display area and 9 general buttons: ok, esc, gesture, 4 direction buttons, zoom-in and zoom-out](/doc_imgs/general_device.png)

![An hardware abstracting adapter implementation has to implement the interface IBrailleIOAdapter and has to fill his fields to enable to proper usage of the hardware.](/doc_imgs/UML-Adapter.png)

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

### BrailleIO_Interfaces

This project is a separated project for interface definitions, abstract basic implementations and base-type definitions to allow for extensibility of the BrailleIO framework by external projects such as the Braille-renderer. Some interesting elements will be explained in the following.

#### Structs

##### BoxModel

The box model struct is used to define margin, padding and borders for view ranges.

##### RenderElement

This struct is used to build rendered element trees for renderer results. This can be used to return the source elements of a renderer result on a certain content position. Can be used as result value for ITouchableRenderer requests.

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

The abstract BrailleIOHookableRendererBase can be used to build a hookable renderer in a easy way.

The following standard renderer are hookable:
* BrailleIOImageToMatrixRenderer
* BrailleIOViewMatixRenderer
* MatrixBrailleRenderer

##### IBailleIORendererHook

The interface a process has to implement to be used as a renderer hook.

### BrailleIO_ShowOff

This project is an implementation of a specialized hardware abstraction of a certain tactile input/output device. It is not build as a real hardware abstraction it is only used as a software emulation for a hardware device. In this case this software emulates a BrailleDis 7200 Device by Metec AG. It can be used to display the rendering outputs of the BrailleIO framework, as well as simulate button and gesture/touch interaction.

### BrailleRenderer

This project builds the currently used renderer to transform a given string input into a representation in computer-braille by applying a defined translation table. The translation tables are built by the “Liblouis” project. Currently the German translation tables are used but of course others can be defined and loaded trough the constructor.

This render builds the standard render for String content of the BrailleIo framework. It also implements the ITouchableRenderer interface in a very detailed manner. This allows for highly interactive touchable interface build on this simple text-to-Braille-renderer.

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


### BrailleIOExample

This is an example application showing for the usage of several basic functions and objects.




## How to use:

--	TODO: build a small workflow

![An application using BrailleIO can be set up in a view steps ](/doc_imgs/Usage.png)




### How to build a hardware abstraction

![The abstract tactile displays should contain of a display area and 9 general buttons: ok, esc, gesture, 4 direction buttons, zoom-in and zoom-out](/doc_imgs/general_device.png)

![An hardware abstracting adapter implementation has to implement the interface IBrailleIOAdapter and has to fill his fields to enable to proper usage of the hardware.](/doc_imgs/UML-Adapter.png)


## You want to know more?
For getting a very detailed overview use the [code documentaion section](/Help/index.html) of this project.

# Examples



## Set up a new Adapter

If you want to create a new hardware abstraction, you have to supply a wrapper for the real hardware interface commands. In this wrapper class, you have to implement the `IBrailleIOApapter` interface. You can also use the abstract implementation of this interface `AbstractBrailleIOAdapterBase` for extension. This abstract class contains an additional property `Synch` that allows the adapter to receive a copy of the content sent to the main adapter – to mirror the display for example.

### Recommendations for event data (raw data)

In the event arguments, e.g. for the `KeyStateChanged` events, there is an integrated option to submit the raw device data. These are submitted in an `OrderedDictonary`. In my hardware abstractions, I use the dictionary to submit button states for hardware keys that go beyond the generalized 9 standard buttons. Therefore, I fill the dictionary as follows:

Key| Content | Description
------------ | ------------- | -------------
allPressedKeys| `List<String>` | List of stings that represent all currently pressed buttons
allReleasedKeys| `List<String>` | List of stings that represent all last released buttons
newPressedKeys| `List<String>` | List of stings that represent all last newly pressed buttons


