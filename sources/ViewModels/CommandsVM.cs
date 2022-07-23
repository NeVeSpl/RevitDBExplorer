using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using RevitDBExplorer.WPF;
using RDQCommand = RevitDBExplorer.Domain.RevitDatabaseQuery.Command;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.ViewModels
{
    internal class CommandsVM : BaseViewModel
    {
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

            foreach (var commandVM in commands.Select(x => new CommandVM(x)))
            {
                if (Commands.Contains(commandVM))
                {
                    toKeep.Add(commandVM);
                }else
                {
                    toKeep.Add(commandVM);
                    toAdd.Add(commandVM);
                }
            }

            var toRemove = new List<CommandVM>();
            foreach (var commandVM in Commands.ToList())
            {
                if (toKeep.Contains(commandVM) == false)
                {
                    toRemove.Add(commandVM);
                    commandVM.ToRemove = true;
                }
            }

            foreach (var commandVM in toAdd)
            {
                Commands.Add(commandVM);
            }

            await Task.Delay(500);

            foreach (var commandVM in toRemove)
            {
                Commands.Remove(commandVM);
            }
        }
    }
}