using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain.DataModel;
using RevitDBExplorer.WPF;
using RevitExplorer.Visualizations;
using RevitExplorer.Visualizations.DrawingVisuals;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.UIComponents.Visualizations
{
    internal class VisualizationsManagerVM : BaseViewModel
    {
        private readonly IRV3DController rdvController;
        private bool isPanelOpen = false;
        private GridLength height;
        private ObservableCollection<VisualizationItem> items;


        public bool IsOpen
        {
            get
            {
                return isPanelOpen;
            }
            set
            {
                isPanelOpen = value;
                OnPropertyChanged();
            }
        }
        public GridLength Height
        {
            get
            {
                return height;
            }
            set
            {
                height = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<VisualizationItem> Items
        {
            get
            {
                return items;
            }
            set
            {
                items = value;
                OnPropertyChanged();
            }
        }


        public VisualizationsManagerVM(IRV3DController rdvController)
        {
            this.rdvController = rdvController;
        }


        private static readonly VisualizationItem axisX = new VisualizationItem("Global Coordinate System", "x-axis", new AxisDrawingVisual() { Orgin = XYZ.Zero, Direction = XYZ.BasisX, Color = VisualizationItem.XAxisColor, LineLength = 50 });
        private static readonly VisualizationItem axisY = new VisualizationItem("Global Coordinate System", "y-axis", new AxisDrawingVisual() { Orgin = XYZ.Zero, Direction = XYZ.BasisY, Color = VisualizationItem.YAxisColor, LineLength = 50 });
        private static readonly VisualizationItem axisZ = new VisualizationItem("Global Coordinate System", "z-axis", new AxisDrawingVisual() { Orgin = XYZ.Zero, Direction = XYZ.BasisZ, Color = VisualizationItem.ZAxisColor, LineLength = 50 });

        public void Populate(List<VisualizationItem> visualizationItems)
        {
            if (IsOpen == false)
            {
                IsOpen = true;
                Height = new GridLength(Math.Max(Height.Value, 123));
            }

            var extendedList = new List<VisualizationItem>(visualizationItems)
            {
                axisX,
                axisY,
                axisZ
            };

            Items = new ObservableCollection<VisualizationItem>(extendedList);
            SetupListView(Items);
            rdvController.SetDrawingVisuals(extendedList.Select(x => x.DrawingVisual).ToArray());
        }

        private void SetupListView(ObservableCollection<VisualizationItem> visualizationItems)
        {
            var lcv = (ListCollectionView)CollectionViewSource.GetDefaultView(visualizationItems);
            
            lcv.GroupDescriptions.Add(new PropertyGroupDescription(nameof(VisualizationItem.Group)));
            //lcv.SortDescriptions.Add(new SortDescription(nameof(IListItem.SortingKey), ListSortDirection.Ascending));           
            //lcv.Filter = ListViewFilter;
        }

        public void Close()
        {
            IsOpen = false;
            Height = new GridLength(0, GridUnitType.Auto);
        }
    }
}