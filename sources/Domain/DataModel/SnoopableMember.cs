using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using RevitDBExplorer.Domain.DataModel.MemberAccessors;
using RevitDBExplorer.Domain.DataModel.Streams.Base;
using RevitDBExplorer.Domain.DataModel.ValueContainers.Base;
using RevitDBExplorer.Domain.DataModel.ViewModels;
using RevitDBExplorer.Domain.DataModel.ViewModels.Base;
using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel
{
    internal class SnoopableMember : BaseViewModel, IAmSourceOfEverything
    {
        private readonly SnoopableObject parent;
        private readonly MemberDescriptor memberDescriptor;      
        private IValueContainer state;
        private IValueViewModel valueVM;


        public DeclaringType DeclaringType => memberDescriptor.DeclaringType;
        public string DeclaringTypeName => DeclaringType.Name;
        public int DeclaringTypeLevel => DeclaringType.InheritanceLevel;
        public MemberKind MemberKind => memberDescriptor.Kind;
        public string Name => memberDescriptor.Name;               
        public bool HasAccessor => memberDescriptor.MemberAccessor is not null;
        public DocXml Documentation => memberDescriptor.Documentation;


        public event Action SnoopableObjectChanged;
        public string AccessorName { get; private set; }
        public IValueViewModel ValueViewModel { get; private set; } = new DefaultPresenterVM();

        public bool CanBeSnooped { get; private set; }        
        public bool CanBeWritten { get; private set; } = false;
               

        public SnoopableMember(SnoopableObject parent, MemberDescriptor memberDescriptor)
        {
            this.parent = parent;
            this.memberDescriptor = memberDescriptor;            
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
                valueVM ??= memberDescriptor.MemberAccessor.CreatePresenter(context, @object);
                ValueViewModel = valueVM;

                if (ValueViewModel is ValueEditorVM { WriteCommand: null } editor )
                {
                    editor.WriteCommand = new RelayCommand(x => Write(), x => CanBeWritten);
                }

                var result = memberDescriptor.MemberAccessor.Read(context, @object, valueVM);
                if (ValueViewModel is DefaultPresenterVM pres)
                {
                    pres.ValueContainer = result.State;
                    pres.Label = result.Label;
                }
                state = result.State;
              
                AccessorName = result.AccessorName;
                CanBeSnooped = result.CanBeSnooped;
                
                if (memberDescriptor.MemberAccessor is IMemberAccessorWithWrite writer)
                {
                    CanBeWritten = writer.CanBeWritten(parent.Context, parent.Object);
                }
            }
            catch (Exception valueAccessException)
            {
                ValueViewModel = new ErrorPresenterVM(Labeler.GetLabelForException(valueAccessException));              
            }
            OnPropertyChanged(nameof(ValueViewModel));                
            OnPropertyChanged(nameof(AccessorName));
            OnPropertyChanged(nameof(CanBeSnooped));
        }


        public SourceOfObjects Snoop()
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
        public IEnumerable<SnoopableObject> Snoop(UIApplication app)
        {
            if (isFrozen) return frozenSnooopResult;
            if (memberDescriptor.MemberAccessor is IMemberAccessorWithSnoop snooper)
            {
                return snooper.Snoop(parent.Context, parent.Object, state);
            }
            return Enumerable.Empty<SnoopableObject>();
        }


        public void Write()
        {
            Write(parent.Context, parent.Object);
        }
        private void Write(SnoopableContext context, object @object)
        {
            if (memberDescriptor.MemberAccessor is IMemberAccessorWithWrite writer)
            {
                ExternalExecutorExt.ExecuteInRevitContextInsideTransactionAsync((x) =>
                {                    
                    writer.Write(context, @object, valueVM);
                }, context.Document, $"{memberDescriptor.MemberAccessor.GetType().Name}").Forget();
                SnoopableObjectChanged?.Invoke();
            }            
        }


        private bool isFrozen = false;
        private IList<SnoopableObject> frozenSnooopResult;
        public void Freeze()
        {
            if (CanBeSnooped)
            {
                frozenSnooopResult = Snoop(null).ToList();
                frozenSnooopResult.ForEach(x => x.Freeze());
            }
            isFrozen = true;
        }
    }
}