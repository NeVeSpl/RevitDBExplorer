# Revit database explorer

**The fastest, most advanced, asynchronous Revit database exploration tool for Revit 2021+.**

Yet another [RevitLookup](https://github.com/jeremytammik/RevitLookup) like tool. RevitLookup was an indispensable tool to work with Revit API for many years. But now, there is a better tool for the job. Let me introduce you to Revit database explorer and its capabilities. It not only allows us to explore database in a more efficient way thanks to querying, but also to modify Revit database through ad hoc scripts written in C#. 

- [query Revit database](#query-revit-database-with-rdq-revit-database-querying)
- [script Revit database](#script-revit-database-with-rds-revit-database-scripting)
  - [ad hoc SELECT query](#ad-hoc-select-query)
  - [ad hoc UPDATE command](#ad-hoc-update-command)
- [easy access to Revit API documentation](#easy-access-to-revit-api-documentation)
- [edit parameter value](#edit-parameter-value)
- [extensive support for ForgeTypeId](#extensive-support-for-forgetypeid)
- [better support for Revit Extensible Storage](#better-support-for-revit-extensible-storage)
- [easier work with Element.Geometry](#easier-work-with-elementgeometry)
- [dark and light UI themes](#dark-and-light-ui-themes)
- [snoop Revit events](#snoop-revit-events-with-rem-revit-event-monitor)
- [snoop updaters](#snoop-updaters)



## Installation

Download and install [RevitDBExplorer.msi](https://github.com/NeVeSpl/RevitDBExplorer/releases/latest/download/RevitDBExplorer.msi). Setup will install Revit database explorer for Revit 2021, 2022, 2023, 2024.

## YouTube tutorials

- [How to select elements that pass Rule-based Filter defined in Revit?](https://www.youtube.com/watch?v=9Uup4Qe8csI)
- [How to find an element using its IfcGuid in Revit?](https://www.youtube.com/watch?v=oT6bxfKc2lg)

## Features

### query Revit database with RDQ (Revit database querying)

RDQ is able to interpret words separated by `,` as element ids, Revit classes, categories, parameters and many more. RDQ builds from them FilteredElementCollector and uses it to query Revit database. 

[Learn more about Revit database querying (RDQ).](documentation/revit-database-querying.md)

![possibility-to-query-Revit-database-from-UI](documentation/examples/rdq-revit-database-query-with-rql-revit-query-language.v2.gif)

### script Revit database with RDS (Revit database scripting)

RDS is intended to compile and run C# code that is too small or ephemeral to make macro/dynamo/addon for it. RDS offers the quickest way to run C# code generated with generative AI chats (such as ChatGPT). 

[Learn more about Revit database scripting (RDS).](documentation/revit-database-scripting.md) 

#### ad hoc SELECT query

![revit-database-scripting-select-query](documentation/examples/revit-database-scripting-select-query.gif)

#### ad hoc UPDATE command

An example shows how to add a prefix to `Mark` parameter for many selected elements as inputs for the script. 

![possibility-to-query-Revit-database-from-UI](documentation/examples/revit-database-scripting-update-command.gif)


### easy access to Revit API documentation

 Tooltips work out-of-box, RevitApi.chm file is part of [Revit SDK](https://www.autodesk.com/developer-network/platform-technologies/revit) and the path to it needs to be set manually.

![tooltips-with-Revit-documentations](documentation/examples/easy-access-to-revit-api-documentation.gif)

### edit parameter value

![edit-parameter-value](documentation/examples/set.parameter.value.gif)

### extensive support for ForgeTypeId

We all love (or hate) the ForgeTypeId, Revit database explorer exposes all data related to a given ForgeTypeId scattered through many utils. You can also snoop all ForgeTypeIds returned from: 
- ParameterUtils.GetAllBuiltInGroups
- ParameterUtils.GetAllBuiltInParameters
- UnitUtils.GetAllMeasurableSpecs
- UnitUtils.GetAllDisciplines
- SpecUtils.GetAllSpecs
- UnitUtils.GetAllUnits

![extensive-support-for-ForgeTypeId](documentation/examples/extensive-support-for-ForgeTypeId.gif)


### better support for Revit Extensible Storage

Revit database explorer allows you to snoop all schemas that are loaded into Revit memory, and you can easily get all elements that have an entity of a given schema. You get access to Extensible Storage data exactly like through RevitApi, by invoking: Element.GetEntity(). In contrast to Revit Lookup, you will only see entities that you can read and really exist in a given element. (Revit Lookup shows an exception when cannot access an entity even when an entity does not exist in a given element....)

![better-support-for-revit-extensible-storage](documentation/examples/better-support-for-revit-extensible-storage.gif)


### easier work with Element.Geometry

Not only you have faster access to a geometry of an element, but you can also select an instance of GeometryObject in Revit if it has a valid reference.  

![easier-work-with-geometry](documentation/examples/easier-work-with-element-geometry.gif)


### dark and light UI themes

![dark-and-light-ui-themes](documentation/examples/dark-and-light-ui-themes.gif)

### snoop Revit events with REM (Revit Event Monitor)

A new take on [EventsMonitor from  RevitSdkSamples
](https://github.com/jeremytammik/RevitSdkSamples/tree/master/SDK/Samples/Events/EventsMonitor/CS). Revit database explorer stores the latest 30 events that occurred during Revit session and allows to snoop them. UIControlledApplication.Idling event and ControlledApplication.ProgressChanged events are not stored because they are too noisy - they happen too often. In order to use this feature, you need to enable event monitor, which by default is disabled.

![snooping-events](documentation/examples/snooping-events.gif)

### snoop updaters

Revit database explorer allows to look deeper into UpdaterRegistry.GetRegisteredUpdaterInfos(). What is special about this feature is that, with a bit of luck, it is able to get UpdaterId, as the first publicly available tool.

![snooping-updaters](documentation/examples/snooping-updaters.gif)
