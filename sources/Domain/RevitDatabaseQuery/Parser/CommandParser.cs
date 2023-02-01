using System;
using System.Collections.Generic;
using System.Linq;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Filters;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Parser.Commands;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Parser
{
    internal static class CommandParser
    {
        private static readonly List<ICommandFactory> factories = new List<ICommandFactory>()
        {
            new CategoryCmdFactory(),
            new ClassCmdFactory(),
            new ElementIdCmdFactory(),
            new ElementTypeCmdFactory(), 
            new LevelCmdFactory(),
            new NameCmdFactory(),
            new NotElementTypeCmdFactory(),
            new ParameterCmdFactory(),
            new RoomCmdFactory(),
            new RuleBasedFilterCmdFactory(),
            new StructuralTypeCmdFactory(),
            new VisibleInViewCmdFactory(),
        };
        private static readonly Dictionary<string, ICommandFactory> classifierToFactoryMap = new Dictionary<string, ICommandFactory>();
        private static readonly Dictionary<string, ICommandFactory> keywordToFactoryMap = new Dictionary<string, ICommandFactory>();
        private static readonly Dictionary<Type, ICommandFactory> matchTypeToFactoryMap = new Dictionary<Type, ICommandFactory>();

        static CommandParser()
        {
            RegisterClassifiers();
            RegisterKeywords();
            RegisterMatchTypes();
        }
        private static void RegisterClassifiers()
        {
            foreach (var factory in factories)
            {
                foreach (var classifier in factory.GetClassifiers())
                {
                    string key = classifier.NormalizeForLookup();
                    if (classifierToFactoryMap.ContainsKey(key))
                    {
                        throw new Exception("Should it not be possible to be here, but here we are...");
                    }
                    classifierToFactoryMap[key] = factory;
                }
            }
        }
        private static void RegisterKeywords()
        {
            foreach (var factory in factories)
            {
                foreach (var keyword in factory.GetKeywords())
                {
                    string key = keyword.NormalizeForLookup();
                    if (keywordToFactoryMap.ContainsKey(key))
                    {
                        throw new Exception("Should it not be possible to be here, but here we are...");
                    }
                    keywordToFactoryMap[key] = factory;
                }
            }
        }
        private static void RegisterMatchTypes()
        {
            foreach (var factory in factories)
            {
                if(factory.MatchType != null)
                {                    
                    if (matchTypeToFactoryMap.ContainsKey(factory.MatchType))
                    {
                        throw new Exception("Should it not be possible to be here, but here we are...");
                    }
                    matchTypeToFactoryMap[factory.MatchType] = factory;
                }
            }
        }


        public static IEnumerable<ICommand> Parse(string cmdText)
        {
            var splittedByClassifier = cmdText.Split(new[] { ':' }, 2, System.StringSplitOptions.RemoveEmptyEntries);
           
            string argument = null;
            ICommandFactory selectedFactory = null;

            if (splittedByClassifier.Length == 1)
            {
                var keyword = cmdText.NormalizeForLookup();
                keywordToFactoryMap.TryGetValue(keyword, out selectedFactory);
                argument = splittedByClassifier[0];                
            }
            if (splittedByClassifier.Length == 2)
            {
                var classifier = splittedByClassifier[0].NormalizeForLookup();
                classifierToFactoryMap.TryGetValue(classifier, out selectedFactory);
                argument = splittedByClassifier[1];
            }

            if (selectedFactory == null)
            {
                foreach (var factory in factories)
                {
                    if (factory.CanRecognizeArgument(argument))
                    {
                        selectedFactory = factory;
                        break;
                    }
                }
            }

            if (selectedFactory != null)
            {     
                var args = selectedFactory.ParseArgument(argument);
                yield return selectedFactory.Create(cmdText, args.ToArray());
                yield break;
            }

            if (string.IsNullOrEmpty(argument)) yield break;

            var matchedArguments = FuzzySearchEngine.Lookup(argument, FuzzySearchEngine.LookFor.Category |
                                                                      FuzzySearchEngine.LookFor.Class |
                                                                      FuzzySearchEngine.LookFor.Level |
                                                                      FuzzySearchEngine.LookFor.Room |
                                                                      FuzzySearchEngine.LookFor.RuleBasedFilter
                                                           ).ToArray();

            if (!matchedArguments.IsEmpty())
            {
                var groupedByType = matchedArguments.GroupBy(x => x.CmdType);
                foreach (var group in groupedByType)
                {
                    var type = MapCmdToType(group.Key);
                    var factory = matchTypeToFactoryMap[type];
                    yield return factory.Create(cmdText, group.ToArray());
                }
                yield break;
            }

            var parsedArgs = NameCmdFactory.Instance.ParseArgument(argument);
            yield return NameCmdFactory.Instance.Create(cmdText, parsedArgs.ToArray());                         
        }

        private static Type MapCmdToType(RevitDBExplorer.Domain.RevitDatabaseQuery.CmdType cmdType)
        {
            switch (cmdType)
            {   
                case CmdType.Category:
                    return typeof(CategoryMatch);
                case CmdType.Class:
                    return typeof(ClassMatch);              
                case CmdType.Parameter:
                    return typeof(ParameterMatch);
                case CmdType.StructuralType:
                    return typeof(StructuralTypeMatch);                  
                case CmdType.Level:
                    return typeof(LevelMatch);
                case CmdType.Room:
                    return typeof(RoomMatch);
                case CmdType.RuleBasedFilter:
                    return typeof(RuleMatch);              
            }
            return null;
        }

        private static string NormalizeForLookup(this String text)
        {
            return text.ToLower().RemoveWhitespace();
        }
    }
}