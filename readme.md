# Revit database explorer (RDBE)

Yet another [RevitLookup](https://github.com/jeremytammik/RevitLookup) clone. RevitLookup is a mature and indispensable tool to work with Revit. But its code had many authors and architectural changes(reflection, modeless) through time which led to quite significant technical debt and lack of consistency. For worse, its UI is still based on WinForms ... When I was adding support for [modeless windows #93](https://github.com/jeremytammik/RevitLookup/pull/93) to RevitLookup , I knew that at some point in time I will rewrite it whole. So here we are, let me introduce you to RDBE, a completely rewritten RevitLookup with WPF UI and a few small improvements:

- [query Revit database from UI](#possibility-to-query-Revit-database)
- [filterable tree of elements and list of properties and metohds](#filterable-tree-of-elements-and-list-of-properties-and-metohds)
- [tooltips with Revit documentation](#tooltips-with-revit-documentation)
- [extensive support for ForgeTypeId](#extensive-support-for-forgetypeid)
- [better support for Revit Extensible Storage](#better-support-for-revit-extensible-storage)
- [easier work with Element.Geometry](#easier-work-with-geometry)
- [more data exposed](#more-data-exposed)
- [elements of Family, FamilySymbol, FamilyInstance are grouped by category in tree](#grouping)

## Installation

- Download and install [RevitDBExplorer.msi](https://github.com/NeVeSpl/RevitDBExplorer/releases/latest/download/RevitDBExplorer.msi). Setup will instal RDBE for Revit 2022 and 2023.

## Features

### <a name="possibility-to-query-Revit-database"></a>query Revit database (RDQ) from UI with RQL - Revit query language

It is a very early version of this feature, but it can interpret words separated by `,` as element ids, Revit classes, and categories. It builds from them FilteredElementCollector (which syntax is available in a tooltip) and use it to query Revit database. The table with all available options/grammar is below the example.

![possibility-to-query-Revit-database-from-UI](documentation/examples/rdq-revit-database-query-with-rql-revit-query-language.gif)
 
keywords/text | Interpretation | translates to in Revit Api
----------|------------| ----
`,`, `;` | seperates phrases/commands
`:` | reserved, not used right now
`active`, `active view` | select elements from active view | FilteredElementCollector(doc, doc.ActiveView.Id)
`type`, `element type`  | select only types | .WhereElementIsElementType()
`element`, `not element type` | select only elements | .WhereElementIsNotElementType()
e.g. `123456` - number | select elements with given ids  | var ids = new [] {new ElementId(123456)} </br>FilteredElementCollector(document, ids)
e.g. `Wall` - revit class | select elements of given class | .OfClass(typeof(Wall))
e.g. `OST_Windows` - revit category | select elements of given category | .OfCategory(BuiltInCategory.OST_Windows)
`foo` - any not recognized text | serach for given text in parameters : Name, Mark |BuiltInParameter.ALL_MODEL_TYPE_NAME, BuiltInParameter.ALL_MODEL_MARK, BuiltInParameter.ELEM_FAMILY_AND_TYPE_PARAM

Queries are case-insensitive, you can query for many ids, but only for one category/class at a time. Matching for categories/classes is done in a fuzzy way, you do not have to be very precise with names, but this may lead to some false positive results.


### filterable tree of elements and list of properties and metohds

![filterable-list-of-properties-and-metohds](documentation/examples/filterable-tree-of-elements-and-list-of-properties-and-metohds.gif)


### tooltips with Revit documentation

They are problems with loading some of tooltips, but most of them should work.

![tooltips-with-Revit-documentations](documentation/examples/tooltips-with-Revit-documentation.png)


### extensive support for ForgeTypeId

We all love (or hate) the ForgeTypeId, RDBE exposes all data related to a given ForgeTypeId scattered through many utils. You can also snoop all ForgeTypeIds returned from: 
- ParameterUtils.GetAllBuiltInGroups
- ParameterUtils.GetAllBuiltInParameters
- UnitUtils.GetAllMeasurableSpecs
- UnitUtils.GetAllDisciplines
- SpecUtils.GetAllSpecs
-  UnitUtils.GetAllUnits

![extensive-support-for-ForgeTypeId](documentation/examples/extensive-support-for-ForgeTypeId.gif)


### better support for Revit Extensible Storage

RDBE allows you to snoop all schemas that are loaded into Revit memory, and you can easily get all elements that have an entity of a given schema. You get access to an Extensible Storage data exactly like through RevitApi, by invoking: Element.GetEntity(). In contrast to Revit Lookup, you will only see entities that you can read and really exist in a given element. (Revit Lookup shows an exception when cannot access an entity even when an entity does not exist in a given element....)

![better-support-for-revit-extensible-storage](documentation/examples/better-support-for-revit-extensible-storage.gif)


### <a name="easier-work-with-geometry"></a>easier work with Element.Geometry

Not only you have faster access to a geometry of an element, but you can also select an instance of GeometryObject in Revit if it has a valid reference.  

![easier-work-with-geometry](documentation/examples/easier-work-with-geometry.gif)


### <a name="more-data-exposed"></a>more data exposed from Revit database

In comparison to RevitLookup, RDBE in addition gives access to:

- Category
    - IsBuiltInCategory
    - GetBuiltInCategory
    - GetBuiltInCategoryTypeId
- Document
    - GetTypeOfStorage
- FormatOptions
    - GetValidSymbols
    - CanHaveSymbol
- HostObject
    - **FindInserts**
- JoinGeometryUtils
    - **GetJoinedElements**
    - **IsCuttingElementInJoin** 
- LabelUtils
    - GetLabelForBuiltInParameter
    - GetLabelForGroup
    - GetLabelForUnit
    - GetLabelForSpec
    - GetLabelForSymbol
    - GetLabelForDiscipline
- ParameterUtils
    - IsBuiltInParameter
    - GetBuiltInParameter
    - IsBuiltInGroup
    - GetBuiltInParameterGroup
- Rebar
    - DoesBarExistAtPosition 
    - **GetCenterlineCurves** 
    - GetCouplerId 
    - GetEndTreatmentTypeId 
    - **GetFullGeometryForView**
    - GetHookOrientation
    - GetHookRotationAngle
    - GetHookTypeId
    - **GetMovedBarTransform**
    - **GetTransformedCenterlineCurves**
    - IsBarHidden
- UnitFormatUtils
    - Format
- UnitUtils
    - IsMeasurableSpec
    - IsSymbol
    - IsUnit
    - GetDiscipline
    - GetTypeCatalogStringForSpec
    - GetTypeCatalogStringForUnit
    - GetValidUnits
- SpecUtils
    - IsValidDataType
    - IsSpec


### <a name="grouping"></a>elements of Family, FamilySymbol, FamilyInstance are grouped by category in the tree

![grouping](documentation/examples/grouping.png)




