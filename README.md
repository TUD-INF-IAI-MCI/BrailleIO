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

In the following the main components will be named and explained

#### BrailleIOMediator

Central instance for the BrailleIO Framework.  It connects the hardware abstraction layers and the GUI/TUI components. It gives you access to the hardware via the IBrailleIOAdapterManager. The GUI/TUI components are available through several methods.

In this component the renderers for the currently active screens and visible views will be called. The rendered content will be sent to all the connected hardware adapters.

[Figure for basic structure]

#### BrailleIOScreen

This is a container for several views that can contain content. Only one screen can be visible at the same time. You can build an unlimited number of screens and fill them with content. This makes it easy to switch between different views or different applications.

#### BrailleIOViewRange

This is the component that can hold content. The combination of content with a standard- or user-defined renderer enables to transform abstract content elements into tactile output on a two-dimensional pin device.
Several viewRanges can be combined in a screen to build a complex view. ViewRanges can be independently switched on and off and can be freely placed inside a screen. 

A viewRange has a complex box model with margin padding and boarder. All properties of the box model are independently definable in all four dimensions. 

[Figure box model]

A view Range has a view Box – which defines the position, size and look on the screen – and a content box which holds the rendered content. This Content box can be larger than the view box. Through offsets in x- and y-direction the content can be moved to become visible inside the vie box. So the content box is moved under the view box to show hidden content. Therefore the offsets have to get negative.

[figure relation between content and view]

Content can be zoomed if the currently active renderer allows for zoomed rendering.

#### Adapter

##### AbstractBrailleIOAdapterManagerBase

The adapter manager is an abstract implementation for a component that manages the connection of different specific hardware abstractions. Adapters can be registered and unregistered at this component. The Adapatermanager that is linked to the BrailleIOMediator will be requested for all active hardware displays.

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

This project builds the currently used renderer to transform a given string input into a representation in computer-braille by applying a defined translation table. The translation tables are built by the “XXX” project. Currently the German translation tables are used but of course others can be defined and loaded trough the constructor.

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

The recognizer can be used as followed:

--	TODO: build example


### BrailleIOExample

This is an example application showing for the usage of several basic functions and objects.




## How to use:

--	TODO: build a small workflow

### How to build a hardware abstraction
