using System.Collections.Generic;
using System.Windows.Documents;
using RevitDBExplorer.Domain.DataModel.Members.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel
{
    internal sealed class SnoopableMember : SnoopableItem
    {        
        private readonly MemberDescriptor memberDescriptor;
        

        public DeclaringType DeclaringType => memberDescriptor.DeclaringType;
        public MemberKind MemberKind => memberDescriptor.Kind;
        public override string Name => memberDescriptor.Name; 
        public DocXml Documentation => memberDescriptor.Documentation;
        public override bool CanGenerateCode => memberDescriptor.Kind != MemberKind.None;
        public string IntroducedInRevitVersion => memberDescriptor.IntroducedInRevitVersion;


        public SnoopableMember(SnoopableObject parent, MemberDescriptor memberDescriptor) : base(parent, memberDescriptor.MemberAccessor)
        {            
            this.memberDescriptor = memberDescriptor;            
        }


        public override SourceOfObjects Snoop()
        {
            var fullTitle = Name;
            IEnumerable<Inline> fullTitleColored = new[] { new Run(Name) };
            if (!string.IsNullOrEmpty(Documentation.Name))
            {
                fullTitle = Documentation.Title;
                fullTitleColored = Documentation.TitleCollored;
            }

            var title = $"{parent.Name}.{this.Name}";


            return new SourceOfObjects(this) { Info = new InfoAboutSource(title) { FullTitleColored = fullTitleColored } };
        }
      

        public override int CompareTo(SnoopableItem other)
        {
            if (other is SnoopableMember snoopableMember)
            {
                return memberDescriptor.CompareTo(snoopableMember.memberDescriptor);
            }
            return 1;
           
        }
        public override bool Equals(SnoopableItem other)
        {
            if (other is SnoopableMember snoopableMember)
            {
                return memberDescriptor.Equals(snoopableMember.memberDescriptor);
            }
            return false;
        }       
    }
}