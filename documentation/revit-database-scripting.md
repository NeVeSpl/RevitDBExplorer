## Revit database scripting (RDS)

- [Types of scripts](#types-of-scripts)
    - [SELECT query](#select-query)
    - [UPDATE command](#update-command)
    - [SCRIPT](#script)
- [Input parameters](#input-parameters)
- [Code generation](#code-generation)



### Types of scripts

 RDS scripts are divided into three categories: 
- SELECT query, where the result of the query is displayed in the RDBE UI 
- UPDATE command, where as a result of the command execution, the model is changed
- SCRIPT, behaves the same as UPDATE commands, but the code does not have to be put inside a function

&nbsp; | SELECT query | UPDATE command | SCRIPT
---|--------------|---------------|---------------
format of code  | C# function | C# function  | C# script
returns | something   | `void` | n/a
can change model | **no**, it is read-only | **yes**, it runs inside transaction | **yes**, it runs inside transaction

#### SELECT query

#### UPDATE command

#### SCRIPT 


### Input parameters

Input parameters are available for all types of scripts. Besides a few predefined variables, any object (or group of objects) displayed in the Tree, can be used as input for the script.


### Code generation

RDS aims to facilitate writing scripts by code generation in some areas: 
- it can generate a SELECT query from any RDQ query
- it can generate an UPDATE command for any parameter 