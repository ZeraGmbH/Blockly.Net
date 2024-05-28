# Blockly.NET

Currently highly experimental, please wait for version 1ff for documentation. The repository is currently private but if you are interested in more details please contact J.Manns@Zera.De. The following is the README of the repository just for information - no link can be used!

# Blockly as a scripting environment for .NET core 8ff based web server application

The goal of this project is to create a library to use [Google Blockly](https://developers.google.com/blockly) as a basic scripting language to design customizable operations of a web server to form scripts. In contrast to other similiar approaches the major aspect of this project is to execute the Blockly scrip inside the server. The execution should be initiated either interactivly or using a server API and it must especially not depend on any client application to be active for the full lifetime of the execution.

We decided to start with the .NET implementation [IronBlock](https://github.com/richorama/IronBlock) which especially provides excution of all standard Blockly blocks - which mostly is the coding infrastructure. In addition this package provides some concepts to extend the runtime enviroment - the most important aspect for this project.

This project is in an early state just ready for discussion. Although it currently is a standalone repository with no direct (fork) connection to IronBlock it may develop into a future version of IronBlock.

The rest of this document will briefly present the changes and extension we made to achieve our goal. All is of discussion and for details please refer to the code itself - currently this is far away from being a documentation.

# The core

Actually you can find what's left of IronBlock in the [Core](Library/Core) folder. The [standard block implementations](Library/Core/Blocks) are mostly untouched - beside some refacturings.

The most important point to notice is that (at least for now) we removed the `Expression` based code generation, so blocks can only be interpreted - which could be some performance issue but in our special case server operations are often in the order of hundreths of milliseconds so a bit interpretation takes no harm. Since we expect users of this library to strongly rely on custom blocks forcing to implement both interpretation and generation seems to be a too expensive double task to solve.

A bit for this reason as well currently the project includes none of the unit tests provided in the IronBlock repository. As an example how these we added a single [sample](Tests/Core) in the Testproject. Actually the test settings are copied from IronBlock but the type of execution is somewhat different.

A refacturing of IronBlock allows easier and a bit faster access to runtime entities like `Fields`, `Values` or `Statements` - one reason which might make it hard to merge back our adaptions back to IronBlock. The IronBlock [execution engine](Library/Core/Model) has been extended as follows:

- the heart of the engine is [asynchronous](Library/Core/Model/IFragment.cs) using `Task` as a result of each block execution
- execution can be [cancelled](Library/Core/Model/Context.cs) at well defined intended break points - see the [test](Tests/Engine/CancellationTests.cs)
- [dependency injection](Library/Core/Model/Context.cs) as used in typical .NET core web servers is provided in the runtime
- a concept of [variable presets](Library/Core/Parser.cs) when starting a script as been added

The parsing concept has been abstracted a bit. We experiemented with the [JSON serialization format](Library/Core/JsonParser.cs) of Blockly but stopped it due some undocumented fields in the area of mutions. To continue here it might be necessary to change the standard block implementations - e.g. the `if` should not rely on `elseIf` and `else` and just use the `Statements` to execute the block.

# Script definitions

In the real world scenario scripts will be defined somewhere in a database and recalled for execution. In [this approach](Library/Scripting) script is some kind of function but external to the currently executing script:

- executing a script definition will get it's own IronBlock execution context
- parameters can be passed into the execution
- the execution may produce a `result`
- during the execution a [progress](Tests/Engine/ProgressTests.cs) can be generated
- multiple script definitions can be executed in [parallel](Tests/Engine/ParallelTests.cs)

# Customization

The most important aspect of this project is the ability to [extend the runtime](Library/Extensions/Builder) with custom blocks. Actually this includes some [infrastructure](Tests/Customization) to communication the corresponding definitions with an appropriate client as well, but currently this is not part of this package.

The customizations allows for [easy creation](Library/Extensions/Delay.cs) of blocks:

- the definitions for the client including toolbox entries are made [declarativly](Library/Extensions/Builder/CustomBlockAttribute.cs)
- only a interpretation has to be implemented
- access to runtime entities is simplified using a couple of extension method helpes
- each execution is asynchrous, but can simply be ported from a synchrous implementation using `Task.FromResult`
- and additional `script engine site` provides more real life functionality, esp. accessing the dependency injection manager

The definition builer supports:

- auto block generation for enumerations blocks
- auto model generation for models which some minor restrictions, e.g. if a model has a property with the type of another model this other model must be built first

To achive this customization in a .NET core environment the [builder](Library/Extensions/Builder/BlocklyExtensions.cs) itself allows to inject specific interfaces. For proper integration with the dependency injection a [helper extension](Library/BlocklyDIExtensions.cs) is provided. The use of these technics are demonstrated in the [unit test base class](Tests/TestEnvironment.cs) and the [customization tests](Tests/Customization/ModelAndEnumTests.cs).

In addition to make all customization available to a client e.g. using a `WebApi` controller the helper extension makes a [dedicated service](Library/Extensions/Builder/IConfigurationService.cs) available in the dependency injection. This includes structuring toolbox entries in categories and using special names to allow for internationalization.

# Blockly blocks provided in the library

In addition to the standard blocks as provided by IronBlock there are a couple of blocks added to support real life applications - actually these are an extract of the blocks we are currently using in the project which initiates this package. There are only very few [unit tests](Tests/CoreEx) for these.

- get the [current time](Library/Extensions/Now.cs) formatted, e.g. for reporting
- [delay](Library/Extensions/Delay.cs) execution
- make [HTTP requests](Library/Extensions/HttpRequest.cs), including auuthorization
- report execution [progress](Library/Extensions/SetProgress.cs)
- execute one or more [script definition](Library/Extensions/RunScript.cs), possibly [in parallel](Library/Extensions/RunParallel.cs)
- [read](Library/Extensions/ReadFromModel.cs) and [write](Library/Extensions/UpdateModel.cs) fields in models, included nesting and array fields
- exception [handling](Library/Extensions/TryCatchFinally.cs) and [generation](Library/Extensions/Throw.cs)

# Script engine

On top of the IronBlock executing engine we added a [script engine](Library/Scripting/Engine).

- it will [execute](Library/Scripting/Engine/ScriptEngine.cs) script definitions
- during execution there will be a progress management
- nested execution will be coordinated
- a simple state machine will allow clients to observe the execution
- there is a infrastructure to request user input - from inside the server

The most detailed example can be found in the [unit test](Tests/Engine/ProgressTests.cs) on parallel script execution.
