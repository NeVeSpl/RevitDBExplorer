## Revit database querying (RDQ)

- [Revit query language](#revit-query-language-rql)
    - [classifiers](#classifiers)
    - [operators](#operators)
    - [search for decimal paramater value (StorageType.Double)](search-for-decimal-paramater-value-storagetypedouble)
    - [wildcard search for string paramater value (StorageType.String)](wildcard-search-for-string-paramater-value-storagetype-string)
- [Code generation](#code-generation)
- [Examples](#examples)
    - [find all elements that have given shared parameter](#find-all-elements-that-have-given-shared-parameter)
    - [find an element using its IfcGuid](#find-an-element-using-its-ifcguid)



### Revit query language (RQL)

- queries are case-insensitive
- matching is done in a fuzzy way, you do not have to be very precise with names, but this may lead to some false positive results
- by using a [classifier](#classifiers) you can narrow search space and get better results
- autocompletion is only available when used with classifier
 
input/keywords | interpretation | translates to in Revit API
----------|------------| ----
`,`, `;` | seperates phrases/commands
`:` | symbol of [classifier](#classifiers) 
`visible`, <br/>`visible in view`, </br> `visible in active view` | select elements from the active view | new VisibleInViewFilter()
`selection` | select elements from the current selection in Revit | Selection.GetElementIds()
`type`, <br/>`element type`, <br/>`not element`  | select only element types | .WhereElementIsElementType()
`element`, <br/>`not element type`, <br/>`not type` | select only elements | .WhereElementIsNotElementType()
`owned`, <br/> `owned by view`, <br/> `owned by active view` | select elemenents owned by the active view| new OwnerViewFilter()
e.g. `123456` - number | select elements with a given  id  | new ElementIdSetFilter(new [] {new ElementId(123456)})
e.g. `Wall` - revit class | select elements of a given class | .OfClass(typeof(Wall)) or <br/>new ElementMulticlassFilter()
e.g. `OST_Windows` - revit category | select elements of a given category | .OfCategory(BuiltInCategory.OST_Windows) or <br/>new ElementMulticategoryFilter()
e.g. `Level 13` - level name | select elements from a given level | new ElementLevelFilter()
e.g. `Room 44` - room name | select elements from a given room | new ElementIntersectsSolidFilter()
e.g. `Approved Connections` - filter name | select elements that pass a rule-based filter defined in Revit | ParameterFilterElement.GetElementFilter()
e.g. `StructuralType.Beam`|select elements matching a structural type|new ElementStructuralTypeFilter()
e.g. `workset name`|select elements from a given workset |new ElementWorksetFilter()
`param = value` | a phrase that uses [any of the operators](#operators) is recognised as a search for a parameter (value)| new ElementParameterFilter()
`foo` - any not recognized text | wildcard search for a given text in parameters:<br/> Name,<br/> Mark,<br/> Type Name,<br/> Family and Type | ParameterFilterRuleFactory.CreateContainsRule(),  <br/>BuiltInParameter.ALL_MODEL_TYPE_NAME, <br/>BuiltInParameter.ALL_MODEL_MARK, <br/>BuiltInParameter.ELEM_FAMILY_AND_TYPE_PARAM, <br/>BuiltInParameter.DATUM_TEXT




#### Search for decimal paramater value (StorageType.Double)

A decimal value you are searching for is parsed/interpreted in Revit 2022+, which means that RDQ uses Revit UI units for given document. If that is not feasible, as fallback, internal Revit storage units/form is used. In Revit 2021, RDQ always uses internal units in queries.

#### Wildcard search for string paramater value (StorageType.String)

For parameters that have StorageType.String, you can do a wildcard search by using `%` or  `*` at the beginning and/or end of searching text e.g. `Mark = *foo%`

#### Classifiers

classifier | meaning
-----------|-------
`c:[text]`, `cat:[text]`, `category:[text]` | interpret `[text]` as a BuiltInCategory
`f:[text]`, `filter:[text]` | interpret `[text]` as a rule-based filter
`i:[number]`,`id:[number]`, `ids:[number]` | interpret `[number]` as an ElementId
`l:[text]`, `lvl:[text]`,  `level:[text]`| interpret `[text]` as a level 
`n:[text]`, `name:[text]` | 
`p:[text]` | interpret `[text]` as a parameter name
`r:[text]`, `room:[text]`| interpret `[text]` as a room
`s:[text]`, `stru:[text]`,  `structual:[text]`| interpret `[text]` as a structural type
`t:[text]`, `type:[text]`, `class:[text]`, `typeof:[text]` | interpret `[text]` as an element type/class
`u:[guid]`, `uid:[guid]`, `unique:[guid]` | interpret `[guid]` as an UniqueId
`w:[text]`, `wrk:[text]`, `workset:[text]` | interpret `[text]` as a workset name


#### Operators

operator | meaning | example
-|-|-
`!=`, `<>` | NotEquals | `Length != 0`
`>=` | GreaterOrEqual | `Length >= 0`
`<=` | LessOrEqual | `Length <= 0`
`??` | HasNoValue, parameter exists but has no value | `Length ??`
`!!` | HasValue, paramater exists and has value | `Length !!`
**`?!`** | **Exists, element has given parameter**| `Length ?!`
`=` | Equals | `Length = 0`
`>` | Greater | `Length > 0`
`<` | Less | `Length < 0`

### Code generation

For (almost) every RQL query you have access to the generated corresponding C# code that you can use in your app.

### Examples

#### Find all elements that have given shared parameter

```
YourSharedParamName?!
```
#### Find an element using its IfcGuid

```
IFC_GUID = 0$ySXavSvEv9X0_Nhxg76d
```