using System;
using RevitDBExplorer.Domain.DataModel.Streams.Base;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel
{
    internal sealed class SnoopableMember : SnoopableItem, IComparable<SnoopableMember>, IEquatable<SnoopableMember>
    {        
        private readonly MemberDescriptor memberDescriptor;
        

        public DeclaringType DeclaringType => memberDescriptor.DeclaringType;
        public MemberKind MemberKind => memberDescriptor.Kind;
        public string Name => memberDescriptor.Name; 
        public DocXml Documentation => memberDescriptor.Documentation;  


        public SnoopableMember(SnoopableObject parent, MemberDescriptor memberDescriptor) : base(parent, memberDescriptor.MemberAccessor)
        {            
            this.memberDescriptor = memberDescriptor;            
        }


        public override SourceOfObjects Snoop()
        {
            var title = Name;
            if (!string.IsNullOrEmpty(Documentation.Name))
            {
                title = $"{Documentation.ReturnType} {Documentation.Name}{Documentation.Invocation}";
            }
            if (MemberKind == MemberKind.Property)
            {
                title = null;
            }
            return new SourceOfObjects(this) { Title = title };
        }
      


        public int CompareTo(SnoopableMember other)
        {
            return memberDescriptor.CompareTo(other.memberDescriptor);
        }
        public bool Equals(SnoopableMember other)
        {
            return memberDescriptor.Equals(other.memberDescriptor);
        }
    }
}