using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.UI;
using RevitDBExplorer.Domain.DataModel.Streams.Base;
using RevitDBExplorer.Domain.DataModel.ValueViewModels;
using RevitDBExplorer.Domain.DataModel.ValueViewModels.Base;
using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel
{
    internal class SnoopableMember : BaseViewModel, IAmSourceOfEverything
    {
        private readonly SnoopableObject parent;
        private readonly MemberDescriptor memberDescriptor;
        private IValueViewModel memberValue;       
        

        public DeclaringType DeclaringType => memberDescriptor.DeclaringType;
        public MemberKind MemberKind => memberDescriptor.Kind;
        public string Name => memberDescriptor.Name;               
        private bool HasAccessor => memberDescriptor.MemberAccessor is not null;
        public DocXml Documentation => memberDescriptor.Documentation;
        public event Action SnoopableObjectChanged;
        public string AccessorName { get; private set; }
        public IValueViewModel ValueViewModel { get; private set; } = EmptyPresenter.Instance;
        public bool CanBeSnooped { get; private set; }        
                       

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
                memberValue ??= memberDescriptor.MemberAccessor.CreatePresenter(context, @object);
                if (memberValue is ICanRead canRead)
                {
                    canRead.Read(context, @object);
                }
                if (memberValue is ICanWrite canWrite)
                {
                    canWrite.Setup(RaiseSnoopableObjectChanged);
                }
                if (memberValue is ICanSnoop canSnoop)
                {
                    CanBeSnooped = canSnoop.CanBeSnooped;
                }
                ValueViewModel = memberValue;
            }
            catch (Exception valueAccessException)
            {
                ValueViewModel = new ErrorPresenter(Labeler.GetLabelForException(valueAccessException));              
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
            if (memberValue is ICanSnoop snooper)
            {
                return snooper.Snoop(parent.Context, parent.Object);
            }
            return Enumerable.Empty<SnoopableObject>();
        }


        private void RaiseSnoopableObjectChanged()
        {
            SnoopableObjectChanged?.Invoke();
        }


        private bool isFrozen = false;
        private IList<SnoopableObject> frozenSnooopResult;
        public void Freeze(int candies)
        {
            if (CanBeSnooped)
            {
                frozenSnooopResult = Snoop(null).ToList();
                frozenSnooopResult.ForEach(x => x.Freeze(candies + 1));
            }
            isFrozen = true;
        }
    }
}