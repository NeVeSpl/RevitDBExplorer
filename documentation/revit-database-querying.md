## Revit database querying

### RQL - Revit query language
 
input/keywords | interpretation | translates to in Revit Api
----------|------------| ----
`,`, `;` | seperates phrases/commands
`:` | [classifier](#classifiers)
`active`, <br/>`active view` | select elements from active view | new VisibleInViewFilter()
`type`, <br/>`element type`, <br/>`not element`  | select only element types | .WhereElementIsElementType()
`element`, <br/>`not element type`, <br/>`not type` | select only elements | .WhereElementIsNotElementType()
e.g. `123456` - number | select elements with given id  | new ElementIdSetFilter(new [] {new ElementId(123456)})
e.g. `Wall` - revit class | select elements of given class | .OfClass(typeof(Wall)) or <br/>new ElementMulticlassFilter()
e.g. `OST_Windows` - revit category | select elements of given category | .OfCategory(BuiltInCategory.OST_Windows) or <br/>new ElementMulticategoryFilter()
e.g. `Level 13` - level name | select elements from given level | new ElementLevelFilter()
e.g. `Room 44` - room name | select elements from given room | new ElementIntersectsSolidFilter()
e.g. `Approved Connections` - filter name | select elements that pass rule-based filter | ParameterFilterElement.GetElementFilter()
`param = value` | a phrase that uses [any of the operators](#operators) is recognised as a search for a parameter (value)| new ElementParameterFilter()
`foo` - any not recognized text | wildcard search for a given text in parameters:<br/> Name,<br/> Mark,<br/> Type Name,<br/> Family and Type | ParameterFilterRuleFactory.CreateContainsRule(),  <br/>BuiltInParameter.ALL_MODEL_TYPE_NAME, <br/>BuiltInParameter.ALL_MODEL_MARK, <br/>BuiltInParameter.ELEM_FAMILY_AND_TYPE_PARAM, <br/>BuiltInParameter.DATUM_TEXT
`s:column`|select elements matching a structural type|new ElementStructuralTypeFilter()

Queries are case-insensitive. Matching for categories/classes/parameters is done in a fuzzy way, you do not have to be very precise with names, but this may lead to some false positive results. 

A value you are searching for is not parsed/interpreted (yet), which means that it uses internal Revit storage units/form, not Revit UI units. For parameters that have StorageType.String, you can do a wildcard search by using `%` or  `*` at the beginning and/or end of searching text e.g. `Mark = *foo%`

<a name="classifiers"></a>

classifier | meaning
-----------|-------
`i:[text]`,`id:[text]`, `ids:[text]` | interpret `[text]` as an ElementId
`c:[text]`, `cat:[text]`, `category:[text]` | interpret `[text]` as a BuiltInCategory
`t:[text]`, `type:[text]`, `class:[text]`, `typeof:[text]` | interpret `[text]` as an element type/class
`n:[text]`, `name:[text]` | 
`s:[text]`, `stru:[text]`,  `structual:[text]`| interpret `[text]` as a structural type
`l:[text]`, `lvl:[text]`,  `level:[text]`| interpret `[text]` as a level 
`r:[text]`, `room:[text]`| interpret `[text]` as a room
`f:[text]`, `filter:[text]`, | interpret `[text]` as a rule-based filter

<a name="operators"></a>

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