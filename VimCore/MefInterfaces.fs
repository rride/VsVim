﻿#light

namespace Vim
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.Text.Editor
open Microsoft.VisualStudio.Text.Operations
open Microsoft.VisualStudio.Text.Tagging

/// Used to determine if a completion window is active for a given view
type IDisplayWindowBroker =

    /// TextView this broker is associated with
    abstract TextView : ITextView 

    /// Is there currently a completion window active on the given ITextView
    abstract IsCompletionActive : bool

    /// Is signature help currently active
    abstract IsSignatureHelpActive : bool

    // Is Quick Info active
    abstract IsQuickInfoActive : bool 

    /// Is there a smart tip window active
    abstract IsSmartTagSessionActive : bool

    /// Dismiss any completion windows on the given ITextView
    abstract DismissDisplayWindows : unit -> unit

type IDisplayWindowBrokerFactoryService  =

    abstract CreateDisplayWindowBroker : ITextView -> IDisplayWindowBroker

type ITrackingLineColumn =
    abstract TextBuffer : ITextBuffer

    /// Get the point as it relates to current Snapshot.  Returns None
    /// in the case that the line and column cannot be matched
    abstract Point : SnapshotPoint option 

    /// Get the point as it relates the current Snapshot.  If the current
    /// length of the line is not long enough to support the column, it will be 
    /// truncated to the last non-linebreak character of the line
    abstract PointTruncating: SnapshotPoint option

    /// Get the point as a VirtualSnapshot point on the current ITextSnapshot
    abstract VirtualPoint : VirtualSnapshotPoint option

    /// Needs to be called when you are done with the ITrackingLineColumn
    abstract Close : unit -> unit

type ITrackingLineColumnService = 

    /// Create an ITrackingLineColumn at the given position in the buffer.  
    abstract Create : ITextBuffer -> line:int -> column: int -> ITrackingLineColumn

    /// Creates a disconnected ITrackingLineColumn instance.  ITrackingLineColumn 
    /// instances can only be created against the current snapshot of an ITextBuffer.  This
    /// method is useful when a valid one can't be supplied so instead we provide 
    /// a ITrackingLineColumn which satisifies the interface but produces no values
    abstract CreateDisconnected : ITextBuffer -> ITrackingLineColumn

    /// Creates an ITrackingLineColumn for the given SnapshotPoint.  If the point does
    /// not point to the current snapshot of ITextBuffer, a disconnected ITrackingLineColumn
    /// will be created
    abstract CreateForPoint : SnapshotPoint -> ITrackingLineColumn

    /// Close all of the outstanding ITrackingLineColumn instances
    abstract CloseAll : unit -> unit

type IVimBufferFactory =
    
    /// Create a IVimBuffer for the given parameters
    abstract CreateBuffer : IVim -> ITextView -> IVimBuffer

type IVimBufferCreationListener =

    /// Called whenever an IVimBuffer is created
    abstract VimBufferCreated : IVimBuffer -> unit

/// Supports the creation and deletion of folds within a ITextBuffer
type IFoldManager = 

    /// Associated ITextBuffer
    abstract TextBuffer : ITextBuffer

    /// Gets snapshot spans for all of the currently existing folds
    abstract Folds : SnapshotSpan seq

    /// Create a fold for the given Span.  The fold will actually be for the
    /// entire lines at the start and end of the span
    abstract CreateFold : SnapshotSpan -> unit 

    /// Delete a fold which crosses the given SnapshotPoint.  Returns false if 
    /// there was no fold to be deleted
    abstract DeleteFold : SnapshotPoint -> bool

    /// Delete all of the folds in the buffer
    abstract DeleteAllFolds : unit -> unit

    /// Raised when the collection of folds are updated
    [<CLIEvent>]
    abstract FoldsUpdated: IEvent<System.EventArgs>

/// Supports the get and creation of IFoldManager for a given ITextBuffer
type IFoldManagerFactory =
    
    abstract GetFoldManager : ITextBuffer -> IFoldManager

/// Abstract representation of the mouse
type IMouseDevice = 
    
    /// Is the left button pressed
    abstract IsLeftButtonPressed : bool

/// Abstract representation of the keyboard 
type IKeyboardDevice = 

    /// Is the given key pressed
    abstract IsKeyDown : VimKey -> bool

/// Tracks changes to the IVimBuffer
type ITextChangeTracker =

    /// Associated IVimBuffer
    abstract VimBuffer : IVimBuffer

    /// Current change
    abstract CurrentChange : TextChange option

    /// Raised when a change is completed
    [<CLIEvent>]
    abstract ChangeCompleted : IEvent<TextChange>

/// Manages the ITextChangeTracker instances
type ITextChangeTrackerFactory =

    abstract GetTextChangeTracker : IVimBuffer -> ITextChangeTracker

/// Provides access to the system clipboard 
type IClipboardDevice =

    abstract Text : string with get,set
