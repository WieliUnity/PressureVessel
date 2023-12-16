using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace PressureVessel
{
    public partial class PressureVesselControl : UserControl
    {
        public PressureVesselControl()
        {
            InitializeComponent();
        }
        private readonly Dictionary<string, ListSortDirection> _sortDirections = new Dictionary<string, ListSortDirection>();

        private void btnCalculate_Click(object sender, RoutedEventArgs e)
        {
            // Parse input values from text boxes
            if (double.TryParse(txtVesselHeight.Text, out double vesselHeight) &&
                double.TryParse(txtVesselDiameter.Text, out double vesselDiameter) &&
                double.TryParse(txtThickness.Text, out double thickness) &&
                double.TryParse(txtCostPerKg.Text, out double costPerKg) &&
                double.TryParse(txtWeldCostPerHour.Text, out double weldCostPerHour))
            {
                // Create a PressureVesselCalculation object
                var calculation = new PressureVesselCalculation
                {
                    VesselHeight = vesselHeight,
                    VesselDiameter = vesselDiameter,
                    Thickness = thickness,
                    CostPerKg = costPerKg,
                    WeldCostPerHour = weldCostPerHour
                };

                // Call the calculation method
                var results = calculation.CalculateCosts();

                // Display the results in the ListView
                lvResults.ItemsSource = results;
            }
            else
            {
                MessageBox.Show("Please enter valid numbers in all fields.");
            }
        }

        private void GridViewColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is GridViewColumnHeader headerClicked)
            {
                // Check if the DisplayMemberBinding is of type Binding
                if (headerClicked.Column.DisplayMemberBinding is Binding binding)
                {
                    string sortBy = binding.Path.Path;
                    Sort(sortBy);
                }
                else
                {
                    // Fallback to using the header's content as the sort by field
                    string sortBy = headerClicked.Column.Header as string;
                    if (sortBy != null)
                    {
                        Sort(sortBy);
                    }
                }
            }
        }


        private void Sort(string sortBy)
        {
            ICollectionView dataView = CollectionViewSource.GetDefaultView(lvResults.ItemsSource);

            if (!_sortDirections.TryGetValue(sortBy, out var currentDirection) || currentDirection == ListSortDirection.Descending)
            {
                currentDirection = ListSortDirection.Ascending;
            }
            else
            {
                currentDirection = ListSortDirection.Descending;
            }

            _sortDirections[sortBy] = currentDirection;

            dataView.SortDescriptions.Clear();
            dataView.SortDescriptions.Add(new SortDescription(sortBy, currentDirection));
            dataView.Refresh();
        }

    }
}