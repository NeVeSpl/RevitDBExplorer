using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using RevitDBExplorer.Domain.DataModel.MemberAccessors;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;
using RevitDBExplorer.Domain.DataModel.ValueObjects;
using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel
{
    internal class SnoopableMember : BaseViewModel
    {
        public enum Kind { Property, Method, StaticMethod, Extra, AsArgument }

        private readonly SnoopableObject parent;
        private readonly DeclaringType declaringType;      
        private readonly IMemberAccessor memberAccessor;
        private readonly Lazy<DocXml> documentation;


        public Kind MemberKind { get; }
        public string Name { get; }
        public string DeclaringTypeName => declaringType.Name;
        public int DeclaringTypeLevel => declaringType.InheritanceLevel;
        public bool HasAccessor => memberAccessor is not null;
        public bool HasValue => memberAccessor is IMemberAccessorWithValue reader;
        public bool IsWritable => memberAccessor is IMemberAccessorWithWrite writer;
        public DocXml Documentation => documentation?.Value ?? DocXml.Empty;

        public Label Label { get; private set; }        
        public IValueContainer ValueContainer 
        { 
            get
            {
                return memberAccessor is IMemberAccessorWithValue reader ? reader.Value : null;
            } 
        }
        public string AccessorName { get; private set; }
        public bool CanBeSnooped { get; private set; }
        public RelayCommand WriteCommand { get; }
        public bool CanBeWritten { get; private set; } = false;


        public SnoopableMember(SnoopableObject parent, Kind memberKind, string name, Type declaringType, IMemberAccessor memberAccessor, Func<DocXml> documentationFactoryMethod)
        {
            this.parent = parent;
            this.MemberKind = memberKind;
            this.Name = name;
            if (declaringType != null)
            {
                this.declaringType = DeclaringType.Create(declaringType, parent?.Object?.GetType());
            }
            this.memberAccessor = memberAccessor;
            if (documentationFactoryMethod != null)
            {
                this.documentation = new Lazy<DocXml>(documentationFactoryMethod);
            }
            WriteCommand = new RelayCommand(x => Write(), x => CanBeWritten); 
        }
        public SnoopableMember(SnoopableObject snoopableObject, SnoopableMember source) : this(snoopableObject, source.MemberKind, source.Name, null, source.memberAccessor, null)
        {
            this.declaringType = source.declaringType;
            this.documentation = source.documentation;
        }

        
        public void Read()
        {
            if (isFrozen) return;
            Read(parent.Context, parent.Object);
            if (memberAccessor is IMemberAccessorWithWrite writer)
            {
                CanBeWritten = writer.CanBeWritten(parent.Context, parent.Object);
            }
        }
        private void Read(SnoopableContext document, object @object)
        {            
            try
            {
                var result = memberAccessor.Read(document, @object);                  
                AccessorName = result.AccessorName;
                CanBeSnooped = result.CanBeSnooped;
                Label = new Label(result.Label, false);
            }
            catch (Exception valueAccessException)
            {
                Label = new Label(Labeler.GetLabelForException(valueAccessException), true);
            }
            OnPropertyChanged(nameof(Label));
            OnPropertyChanged(nameof(ValueContainer));
            OnPropertyChanged(nameof(AccessorName));
            OnPropertyChanged(nameof(CanBeSnooped));
        }
        public IEnumerable<SnoopableObject> Snooop(UIApplication app)
        {
            if (isFrozen) return frozenSnooopResult;
            return memberAccessor.Snoop(parent.Context, parent.Object);
        }

        public void Write()
        {
            if (memberAccessor is IMemberAccessorWithWrite writer)
            {
                writer.Write(parent.Context, parent.Object).Forget();
            }            
        }


        private bool isFrozen = false;
        private IList<SnoopableObject> frozenSnooopResult;
        public void Freeze()
        {            
            if (CanBeSnooped)
            {
                frozenSnooopResult = Snooop(null).ToList();
                frozenSnooopResult.ForEach(x => x.Freeze());
            }
            isFrozen = true;
        }
    }
}