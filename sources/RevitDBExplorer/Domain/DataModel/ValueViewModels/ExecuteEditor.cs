using System;
using System.Threading.Tasks;
using RevitDBExplorer.Domain.DataModel.MemberAccessors;
using RevitDBExplorer.Domain.DataModel.ValueViewModels.Base;
using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.DataModel.ValueViewModels
{
    internal class ExecuteEditor : BaseViewModel, IValueEditor, ICanRead, ICanWrite
    {
        private readonly IMemberAccessorWithWrite memberAccessor;
        private readonly Action exeFunc;
        private Action raiseSnoopableObjectChanged;

        public RelayCommand ExecuteCommand
        {
            get;
        }
        public bool CanBeWritten { get; private set; } = false;


        public ExecuteEditor(IMemberAccessorWithWrite memberAccessor, Action exeFunc)
        {
            this.memberAccessor = memberAccessor;
            this.exeFunc = exeFunc;
            ExecuteCommand = new RelayCommand(x => Write(), x => CanBeWritten);
        }

        public void Read(SnoopableContext context, object @object)
        {           
            CanBeWritten = memberAccessor.CanBeWritten(context, @object);
        }

        private void Write()
        {
            ExternalExecutorExt.ExecuteInRevitContextInsideTransactionAsync((x) =>
            {
                exeFunc();
            }, null, $"{memberAccessor.GetType().Name}").Forget();
            raiseSnoopableObjectChanged?.Invoke();
        }

        public void Setup(Action raiseSnoopableObjectChanged)
        {
            this.raiseSnoopableObjectChanged = raiseSnoopableObjectChanged;
        }
    }
}
