using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using NSourceGenerators;
using RevitDBExplorer.Domain.RevitDatabaseScripting.Dummies;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseScripting
{
    internal class MemberInvocation_SelectMultiple_Template
    {
        [CodeToString(nameof(MemberInvocation_SelectMultiple_Template))]
        IEnumerable<object> Select(Document document, IEnumerable<TypePlaceholder> inputs)
        {
            foreach (var item in inputs)
            {
                yield return item.MethodPlaceholder();
            }
        }
    }


    internal class MemberInvocation_SelectSingle_Template
    {
        [CodeToString(nameof(MemberInvocation_SelectSingle_Template))]
        object Select(Document document, IEnumerable<TypePlaceholder> inputs)
        {
            var item = inputs.FirstOrDefault();
            return item.MethodPlaceholder();            
        }
    }


    internal class MemberInvocation_UpdateMultiple_Template
    {
        [CodeToString(nameof(MemberInvocation_UpdateMultiple_Template))]
        void Update(Document document, IEnumerable<TypePlaceholder> inputs)
        {
            foreach (var item in inputs)
            {
                item.MethodPlaceholder();
            }
        }
    }


    internal class MemberInvocation_UpdateSingle_Template
    {
        [CodeToString(nameof(MemberInvocation_UpdateSingle_Template))]
        void Update(Document document, IEnumerable<TypePlaceholder> inputs)
        {
            var item = inputs.FirstOrDefault();
            item.MethodPlaceholder();            
        }
    }
}