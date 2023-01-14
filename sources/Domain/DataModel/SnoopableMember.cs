using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RevitDBExplorer.Domain.DataModel.MemberAccessors;
using RevitDBExplorer.Domain.DataModel.ValueObjects;
using RevitDBExplorer.UIComponents.List.ValuePresenters;
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

        public DeclaringType DeclaringType => declaringType;
        public Kind MemberKind { get; }
        public string Name { get; }
        public string DeclaringTypeName => declaringType.Name;
        public int DeclaringTypeLevel => declaringType.InheritanceLevel;        
        public bool HasAccessor => memberAccessor is not null;
        public DocXml Documentation => documentation?.Value ?? DocXml.Empty;

        public Label Label { get; private set; } = new Label("", false);
        public string AccessorName { get; private set; }
        public IValuePresenter ValueViewModel { get; private set; } = new DefaultPresenterVM();
        public bool CanBeSnooped { get; private set; }        
        public bool CanBeWritten { get; private set; } = false;
        public RelayCommand WriteCommand { get; }        
        public event Action SnoopableObjectChanged;


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
            if (memberAccessor == null)
            {
                this.declaringType = DeclaringType.NotExposed;
            }
            if (documentationFactoryMethod != null)
            {
                this.documentation = new Lazy<DocXml>(documentationFactoryMethod);
            }
            WriteCommand = new RelayCommand(x => Write(), x => CanBeWritten); 
        }   

        
        public void Read()
        {
            if (isFrozen) return;
            if (!HasAccessor) return;
            Read(parent.Context, parent.Object);            
        }
        private void Read(SnoopableContext context, object @object)
        {
            try
            {
                ValueViewModel = memberAccessor.GetPresenter(context, @object);
                var result = memberAccessor.Read(context, @object);                  
                AccessorName = result.AccessorName;
                CanBeSnooped = result.CanBeSnooped;
                Label = new Label(result.Label, false);
                if (memberAccessor is IMemberAccessorWithWrite writer)
                {
                    CanBeWritten = writer.CanBeWritten(parent.Context, parent.Object);
                }
            }
            catch (Exception valueAccessException)
            {
                ValueViewModel = new ErrorPresenterVM();
                Label = new Label(Labeler.GetLabelForException(valueAccessException), true);
            }
            OnPropertyChanged(nameof(ValueViewModel));
            OnPropertyChanged(nameof(Label));            
            OnPropertyChanged(nameof(AccessorName));
            OnPropertyChanged(nameof(CanBeSnooped));
        }

        public IEnumerable<SnoopableObject> Snooop()
        {
            if (isFrozen) return frozenSnooopResult;           
            if (memberAccessor is IMemberAccessorWithSnoop snooper)
            {
                return snooper.Snoop(parent.Context, parent.Object);
            }
            return Enumerable.Empty<SnoopableObject>();
        }

        public void Write()
        {
            Write(parent.Context, parent.Object);
        }
        private void Write(SnoopableContext context, object @object)
        {
            if (memberAccessor is IMemberAccessorWithWrite writer)
            {
                ExternalExecutorExt.ExecuteInRevitContextInsideTransactionAsync((x) =>
                {                    
                    writer.Write(context, @object);
                }, context.Document, $"{memberAccessor.GetType().Name}").Forget();
                SnoopableObjectChanged?.Invoke();
            }            
        }


        private bool isFrozen = false;
        private IList<SnoopableObject> frozenSnooopResult;
        public void Freeze()
        {            
            if (CanBeSnooped)
            {
                frozenSnooopResult = Snooop().ToList();
                frozenSnooopResult.ForEach(x => x.Freeze());
            }
            isFrozen = true;
        }
    }
}