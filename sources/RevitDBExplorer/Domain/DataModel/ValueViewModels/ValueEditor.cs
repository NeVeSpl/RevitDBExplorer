using System;
using System.Threading.Tasks;
using RevitDBExplorer.Domain.DataModel.MemberAccessors;
using RevitDBExplorer.Domain.DataModel.ValueViewModels.Base;
using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueViewModels
{
    internal abstract class ValueEditor<T> : BaseViewModel, IValueEditor, ICanRead, ICanWrite
    {
        private readonly IMemberAccessor memberAccessor;
        private readonly Func<T> readFunc;
        private readonly Action<T> writeFunc;
        private readonly Func<bool> canBeWrittenFunc;
        private SnoopableContext context;
        private T value;        
        private Action raiseSnoopableObjectChanged;

        public T Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
                OnPropertyChanged();
            }
        }
        public RelayCommand WriteCommand
        {
            get; 
        }
        public bool CanBeWritten { get; private set; } = false;


        public ValueEditor(IMemberAccessor memberAccessor, Func<T> readFunc, Action<T> writeFunc, Func<bool> canBeWrittenFunc)
        {
            this.memberAccessor = memberAccessor;
            this.readFunc = readFunc;
            this.writeFunc = writeFunc;
            this.canBeWrittenFunc = canBeWrittenFunc;
            WriteCommand = new RelayCommand(x => Write(), x => CanBeWritten);
        }

        public void Read(SnoopableContext context, object @object)
        {
            this.context = context;
            Value = readFunc();
            CanBeWritten = canBeWrittenFunc();
        }

       
        private void Write()
        {            
            ExternalExecutorExt.ExecuteInRevitContextInsideTransactionAsync((x) =>
            {
                writeFunc(value);
            }, context.Document, $"{memberAccessor.GetType().Name}").Forget();
            raiseSnoopableObjectChanged?.Invoke();            
        }

        public void Setup(Action raiseSnoopableObjectChanged)
        {
            this.raiseSnoopableObjectChanged = raiseSnoopableObjectChanged;
        }
    }

    internal class DoubleEditor : ValueEditor<double>
    { 
        public DoubleEditor(IMemberAccessor memberAccessorWithWrite, Func<double> readFunc, Action<double> writeFunc, Func<bool> canBeWrittenFunc) : base(memberAccessorWithWrite, readFunc, writeFunc, canBeWrittenFunc)
        {
            
        }
    }

    internal class IntegerEditor : ValueEditor<int>
    {
        public IntegerEditor(IMemberAccessor memberAccessorWithWrite, Func<int> readFunc, Action<int> writeFunc, Func<bool> canBeWrittenFunc) : base(memberAccessorWithWrite, readFunc, writeFunc, canBeWrittenFunc)
        {
           
        }       
    }

    internal class StringEditor : ValueEditor<string>
    {       
        public StringEditor(IMemberAccessor memberAccessorWithWrite, Func<string> readFunc, Action<string> writeFunc, Func<bool> canBeWrittenFunc) : base(memberAccessorWithWrite, readFunc, writeFunc, canBeWrittenFunc)
        {
        
        }     
    }
}