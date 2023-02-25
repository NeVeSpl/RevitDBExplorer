using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Autocompletion.Internals;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Parser
{
    internal interface ICommandDefinition : IAmCommandFactory, IOfferCommandAutocompletion
    {
        bool CanRecognizeArgument(string argument);
        bool CanParticipateInGenericSearch();
        IEnumerable<string> GetClassifiers();
        IEnumerable<string> GetKeywords();
    }

    internal interface INeedInitialization
    {
        void Init();
    }
    internal interface INeedInitializationWithDocument
    {
        void Init(Document document);
    }

    internal interface IAmCommandFactory 
    {
        ICommand Create(string cmdText, string argument);
    }

    internal interface IOfferCommandAutocompletion
    {
        IAutocompleteItem GetCommandAutocompleteItem();        
    }
    internal interface IOfferArgumentAutocompletion
    {
        IEnumerable<IAutocompleteItem> GetAutocompleteItems(string prefix);
    }   
}