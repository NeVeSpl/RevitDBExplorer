using System;
using System.Threading.Tasks;
using RevitDBExplorer.Domain.DataModel.Accessors;
using RevitDBExplorer.Domain.DataModel.ValueViewModels.Base;
using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueViewModels
{
    internal class ExecuteEditor : BaseViewModel, IValueEditor, ICanRead, ICanWrite
    {
        private readonly IAccessor accessor;
        private readonly Action exeFunc;
        private readonly Func<bool> canBeExecutedFunc;
        private Action raiseSnoopableObjectChanged;

        public RelayCommand ExecuteCommand
        {
            get;
        }
        public bool CanBeWritten { get; private set; } = false;


        public ExecuteEditor(IAccessor accessor, Action exeFunc, Func<bool> canBeExecutedFunc)
        {
            this.accessor = accessor;
            this.exeFunc = exeFunc;
            this.canBeExecutedFunc = canBeExecutedFunc;
            ExecuteCommand = new RelayCommand(x => Write(), x => CanBeWritten);
        }

        public void Read(SnoopableContext context, object @object)
        {           
            CanBeWritten = canBeExecutedFunc();
        }

        private void Write()
        {
            ExternalExecutorExt.ExecuteInRevitContextInsideTransactionAsync((x) =>
            {
                exeFunc();
            }, null, $"{accessor.GetType().Name}").Forget();
            raiseSnoopableObjectChanged?.Invoke();
        }

        public void Setup(Action raiseSnoopableObjectChanged)
        {
            this.raiseSnoopableObjectChanged = raiseSnoopableObjectChanged;
        }
    }
}