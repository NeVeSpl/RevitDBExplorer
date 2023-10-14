using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.UI;
using RevitDBExplorer.Domain.DataModel.Accessors;
using RevitDBExplorer.Domain.DataModel.ValueViewModels;
using RevitDBExplorer.Domain.DataModel.ValueViewModels.Base;
using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel
{
    internal abstract class SnoopableItem : BaseViewModel, IAmSourceOfEverything, IComparable<SnoopableItem>, IEquatable<SnoopableItem>
    {
        protected readonly SnoopableObject parent;
        private readonly IAccessor accessor;
        private readonly IValueViewModel itemValueViewModel;

        public event Action ParentObjectChanged;
        public string AccessorName { get; private set; }
        public virtual string Name { get;}
        public IValueViewModel ValueViewModel { get; private set; } = EmptyPresenter.Instance;
        public bool CanBeSnooped
        {
            get
            {
                if (itemValueViewModel is ICanSnoop snoopable)
                {
                    return snoopable.CanBeSnooped;
                }
                return false;
            }
        }


        public SnoopableItem(SnoopableObject parent, IAccessor accessor)
        {
            this.parent = parent;
            this.accessor = accessor;
            this.itemValueViewModel = accessor?.CreatePresenter(parent.Context, parent.Object);
        }


        public void Read()
        {
            if (isFrozen) return;
            if (!HasAccessor()) return;
            Read(parent.Context, parent.Object);
        }
        private bool HasAccessor() => accessor is not null;
        private void Read(SnoopableContext context, object @object)
        {
            try
            {                
                if (itemValueViewModel is ICanRead canRead)
                {
                    canRead.Read(context, @object);
                }
                if (itemValueViewModel is ICanWrite canWrite)
                {
                    canWrite.Setup(RaiseParentObjectChanged);
                }
                ValueViewModel = itemValueViewModel;
            }
            catch (Exception valueAccessException)
            {
                ValueViewModel = new ErrorPresenter(Labeler.GetLabelForException(valueAccessException));
            }
            OnPropertyChanged(nameof(ValueViewModel));
            OnPropertyChanged(nameof(AccessorName));
            OnPropertyChanged(nameof(CanBeSnooped));
        }        
        private void RaiseParentObjectChanged()
        {
            ParentObjectChanged?.Invoke();
        }



        public abstract SourceOfObjects Snoop();
        IEnumerable<SnoopableObject> IAmSourceOfEverything.Snoop(UIApplication app)
        {
            if (isFrozen) return frozenSnooopResult;
            if (itemValueViewModel is ICanSnoop snooper)
            {
                return snooper.Snoop(parent.Context, parent.Object);
            }
            return Enumerable.Empty<SnoopableObject>();
        }
                


        private bool isFrozen = false;
        private IList<SnoopableObject> frozenSnooopResult;
        public void Freeze(int candies)
        {
            if (CanBeSnooped)
            {
                frozenSnooopResult = (this as IAmSourceOfEverything).Snoop(null).ToList();
                frozenSnooopResult.ForEach(x => x.Freeze(candies + 1));
            }
            isFrozen = true;
        }


        public abstract int CompareTo(SnoopableItem other);
        public abstract bool Equals(SnoopableItem other);
    }
}