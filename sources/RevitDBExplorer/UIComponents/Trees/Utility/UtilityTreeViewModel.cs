using System;
using System.Collections.ObjectModel;
using System.Windows;
using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.UIComponents.Trees.Base;
using RevitDBExplorer.UIComponents.Trees.Base.Items;
using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.UIComponents.Trees.Utility
{
    internal class UtilityTreeViewModel : BaseTreeViewModel
    {
        private static ObservableCollection<SnoopableObject> cache = new ObservableCollection<SnoopableObject>();
        public static event EventHandler<RemoveItemsEventArgs> RemoveItemsEvent;
        public static event EventHandler<AddObjectEventArgs> AddObjectEvent;
        public static event EventHandler<MoveItemEventArgs> MoveItemEvent;

        private UtilityGroupTreeItem rootItem;

        public RelayCommand RemoveCommand { get; }


        public UtilityTreeViewModel()
        {
            AllowToFrezeeItem = true;
            rootItem = new UtilityGroupTreeItem(TreeItemsCommands, cache) { IsExpanded = true };            
            TreeItems.Add(rootItem);
            RemoveCommand = new RelayCommand(RemoveItems);

            WeakEventManager<UtilityTreeViewModel, RemoveItemsEventArgs>.AddHandler(null, nameof(UtilityTreeViewModel.RemoveItemsEvent), RemoveItemsEventHandler);
            WeakEventManager<UtilityTreeViewModel, AddObjectEventArgs>.AddHandler(null, nameof(UtilityTreeViewModel.AddObjectEvent), AddObjectEventHandler);
            WeakEventManager<UtilityTreeViewModel, MoveItemEventArgs>.AddHandler(null, nameof(UtilityTreeViewModel.MoveItemEvent), MoveItemEventHandler);
        }


        public void AddObject(SnoopableObject inputObject)
        {
            var objectCopy = new SnoopableObject(inputObject.Context.Document, inputObject.Object);
            cache.Add(objectCopy);
            AddObjectEvent?.Invoke(null, new AddObjectEventArgs(objectCopy));
        }
        private void AddObjectEventHandler(object sender, AddObjectEventArgs evntArgs)
        {
            var vm = new SnoopableObjectTreeItem(evntArgs.Object, TreeItemsCommands);
            rootItem.Items.Add(vm);
        }

        private void RemoveItems(object item)
        {
            if (item is SnoopableObjectTreeItem snoopableObjectTreeItem)
            {
                var index = cache.IndexOf(snoopableObjectTreeItem.Object);
                cache.Remove(snoopableObjectTreeItem.Object);
                RemoveItemsEvent?.Invoke(null, new RemoveItemsEventArgs(index));
            }
            else
            {               
                cache.Clear();
                RemoveItemsEvent?.Invoke(null, new RemoveItemsEventArgs(null));
            }            
        }
        private void RemoveItemsEventHandler(object sender, RemoveItemsEventArgs evntArgs)
        {
            if (evntArgs.Index is not null)
            {
                rootItem.Items.RemoveAt(evntArgs.Index.Value);         
            }
            else
            {               
                rootItem.Items.Clear();                
            }
        }
               
        public void MoveItem(SnoopableObjectTreeItem item, SnoopableObjectTreeItem target)
        {
            var o1 = item.Object;
            var o2 = target.Object;

            var oldIndex = cache.IndexOf(o1);
            var newIndex = cache.IndexOf(o2);

            cache.Move(oldIndex, newIndex);

            MoveItemEvent?.Invoke(null, new MoveItemEventArgs(oldIndex, newIndex));
        }
        private void MoveItemEventHandler(object sender, MoveItemEventArgs evntArgs)
        { 
            rootItem.Items.Move(evntArgs.OldIndex, evntArgs.NewIndex);
        }


        public void RemoveSelection()
        {
            RemoveSelection(rootItem);

            void RemoveSelection(TreeItem item)
            {
                item.IsSelected = false;

                if (item.Items == null) return;

                foreach (var child in item.Items)
                {
                    RemoveSelection(child);
                }
            }
        }


        internal class RemoveItemsEventArgs : EventArgs
        {
            public int? Index { get; }

            public RemoveItemsEventArgs(int? index)
            {
                Index = index;
            }            
        }
        internal class AddObjectEventArgs : EventArgs
        {
            public SnoopableObject Object { get; }

            public AddObjectEventArgs(SnoopableObject @object)
            {
                Object = @object;
            }
        }
        internal class MoveItemEventArgs : EventArgs
        {
            public int OldIndex { get; }
            public int NewIndex { get; }


            public MoveItemEventArgs(int item, int target)
            {
                OldIndex = item;
                NewIndex = target;
            }            
        }
    }
}