# Script life cycle events

Currently only a single script can be executed by the script engine. While executing the script engine can fire various notifications through the optional `IScriptEngineNotifySink` service. When created a script engine instance will be connected to a notification sink which is expected to forward events to all clients currently interested in the activity of the running script. In addition the `Reconnect` method can be used to report all events to a newly connected client using the same interface.

## ScriptStarted [ScriptEngineNotifyMethods.Started](ScriptInformation.cs)

Fired as soon as a new script started execution. When a new client used `Reconnect` the **ActiveScript** [ScriptEngineNotifyMethods.Current](ScriptInformation.cs) is used to indicate that there is already an active script.

## ScriptProgress [ScriptEngineNotifyMethods.Progress](ScriptProgress.cs)

A script can use the [`set_progress`](../../Extensions/SetProgress.cs) block to create a progress information - using `Reconnect` only the latest progress observed is reported. Progress information is a rather complicated structure depending on the script implementation.

### object Info

Some opaque structure which the script can build as needed. Clients should be able to interpret the contents depending on the configuration of the script definition. Although in principle there are no restrictions on the contents of `Info` the script should keep the structure as small as possible.

### List<[ProgressDetails](ProgressDetails.cs)> AllProgress

Simply a list of tuples with display name and progress counter for each level of script execution. The counter must be between 0 and 1 - both inclusive. The list may contain more than one entry if a script starts nested scripts - there is no limit on the depth of nesting.

### [ScriptGroupStatus](ScriptGroupStatus.cs) GroupStatus

Information on finished execution groups if the script is using such. This data can be used for restarting a script and keep specific results from the former run without recalculating it.

## InputRequest [ScriptEngineNotifyMethods.InputRequest](UserInputRequest.cs)

A script can use the [request_user_input](../../Extensions/RequestUserInput.cs) block to stall execution and request some user interaction - which can be provided by any connected client. Especially the `ValueType` can be used to create the corresponding form element - e.g. for a string other than for a number. There is no restriction for the format of the type so the clients must be able to interpret it.

If the script has stalled itself on a user interaction the `Reconnect` will report the event as well.

In a poorly implemented script where multiple parallel nested scripts require user interaction at the same time only the latest request will be considered.

## ScriptError [ScriptEngineNotifyMethods.Error](ScriptError.cs)

This event indicates that the active script has been terminated due to an exception observed. The `ErrorMessage` field will contain the description of the exception as a single string. The script is still considered active and must be manually terminated. `Reconnect` will report error events as well.

## ScriptDone [ScriptEngineNotifyMethods.Done](ScriptDone.cs)

The script finished without any exception. The script engine still blocks the next script to be started after a client has requested the result. On `Reconnect` the same event is reported.

## ScriptFinished [ScriptEngineNotifyMethods.Finished](ScriptFinished.cs)

The script has manually be finished after is has been successfully completed. A new script can be started after that. A `Reconnect` will never report this notification.
