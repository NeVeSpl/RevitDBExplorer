using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RevitDBExplorer.WPF;
using RDQCommand = RevitDBExplorer.Domain.RevitDatabaseQuery.Command;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.UIComponents.QueryVisualization
{
    internal class QueryVisualizationVM : BaseViewModel
    {
        private static SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
        private ObservableCollection<CommandVM> commands = new();


        public ObservableCollection<CommandVM> Commands
        {
            get
            {
                return commands;
            }
            set
            {
                commands = value;
                OnPropertyChanged();
            }
        }


        public async Task Update(IEnumerable<RDQCommand> commands)
        {
            var toAdd = new List<CommandVM>();
            var toKeep = new List<CommandVM>();
            var toRemove = new List<CommandVM>();

            try
            {
                await semaphore.WaitAsync();

                foreach (var commandVM in commands.Select(x => new CommandVM(x)))
                {
                    if (Commands.Contains(commandVM))
                    {
                        toKeep.Add(commandVM);
                    }
                    else
                    {
                        //toKeep.Add(commandVM);
                        toAdd.Add(commandVM);
                    }
                }
                
                foreach (var commandVM in Commands)
                {
                    if (toKeep.Contains(commandVM) == false)
                    {
                        if (commandVM.ToRemove == false)
                        {
                            toRemove.Add(commandVM);
                            commandVM.ToRemove = true;
                        }
                    }
                }

                foreach (var commandVM in toAdd)
                {
                    Commands.Add(commandVM);
                }
            }
            finally
            {
                semaphore.Release();
            }

            await Task.Delay(500);

            try
            {
                await semaphore.WaitAsync();

                foreach (var commandVM in toRemove)
                {
                    Commands.Remove(commandVM);
                }
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}