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
        private readonly IMemberAccessorWithWrite memberAccessor;
        private readonly Func<T> readFunc;
        private readonly Action<T> writeFunc;
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


        public ValueEditor(IMemberAccessorWithWrite memberAccessor, Func<T> readFunc, Action<T> writeFunc)
        {
            this.memberAccessor = memberAccessor;
            this.readFunc = readFunc;
            this.writeFunc = writeFunc;
            WriteCommand = new RelayCommand(x => Write(), x => CanBeWritten);
            this.writeFunc = writeFunc;

        }

        public void Read(SnoopableContext context, object @object)
        {
            Value = readFunc();
            CanBeWritten = memberAccessor.CanBeWritten(context, @object);
        }

       
        private void Write()
        {            
            ExternalExecutorExt.ExecuteInRevitContextInsideTransactionAsync((x) =>
            {
                writeFunc(value);
            }, null, $"{memberAccessor.GetType().Name}").Forget();
            raiseSnoopableObjectChanged?.Invoke();            
        }

        public void Setup(Action raiseSnoopableObjectChanged)
        {
            this.raiseSnoopableObjectChanged = raiseSnoopableObjectChanged;
        }
    }

    internal class DoubleEditor : ValueEditor<double>
    { 
        public DoubleEditor(IMemberAccessorWithWrite memberAccessorWithWrite, Func<double> readFunc, Action<double> writeFunc) : base(memberAccessorWithWrite, readFunc, writeFunc)
        {
            
        }
    }

    internal class IntegerEditor : ValueEditor<int>
    {
        public IntegerEditor(IMemberAccessorWithWrite memberAccessorWithWrite, Func<int> readFunc, Action<int> writeFunc) : base(memberAccessorWithWrite, readFunc, writeFunc)
        {
           
        }       
    }

    internal class StringEditor : ValueEditor<string>
    {       
        public StringEditor(IMemberAccessorWithWrite memberAccessorWithWrite, Func<string> readFunc, Action<string> writeFunc) : base(memberAccessorWithWrite, readFunc, writeFunc)
        {
        
        }     
    }
}