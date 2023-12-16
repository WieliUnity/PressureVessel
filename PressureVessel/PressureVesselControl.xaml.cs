using System;
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
        private DesignCalculations designCalculations = new DesignCalculations();

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

        private void BtnCalculateThk_Click(object sender, RoutedEventArgs e)
        {
            if (cmbMaterial.SelectedItem is ComboBoxItem selectedMaterial)
            {
                string material = selectedMaterial.Content.ToString();
                double diameter = double.Parse(txtDiameter.Text);
                double pressure = double.Parse(txtDesignPressure.Text);
                int temperature = int.Parse(txtTemperature.Text);
                temperature = (int)Math.Ceiling(temperature / 10.0) * 10;
                double corrosionAllowance = double.Parse(txtCorrosionAllowance.Text);
                double extraThickness = double.Parse(txtExtraThickness.Text);

                double fbValue = designCalculations.FindFbValue(material, temperature);

                if (fbValue == -1)
                {
                    MessageBox.Show("No data found for the selected material and temperature.");
                    return;
                }

                double minThickness = designCalculations.CalculateMinThickness(pressure, diameter, fbValue);
                double minThicknessIncSafety = designCalculations.CalculateMinThicknessIncSafety(minThickness, corrosionAllowance);
                int finalThickness = designCalculations.CalculateFinalThickness(minThicknessIncSafety, extraThickness);
                double currentUsage = designCalculations.CalculateCurrentUsage(minThicknessIncSafety, finalThickness);

                txtCalculatedThickness.Text = finalThickness.ToString();
                txtUtnyttjandegrad.Text = currentUsage.ToString("P");
                UpdateMantletTabFields();
            }
            else
            {
                MessageBox.Show("Please select a material.");
            }
        }


        private void UpdateMantletTabFields()
        {
            // Assuming the Mantlet tab's TextBoxes have been named as follows:
            // txtVesselHeight, txtVesselDiameter, and txtThickness
            txtVesselDiameter.Text = txtDiameter.Text; // Update Diameter
            txtVesselHeight.Text = txtHeight.Text;     // Update Height
            txtThickness.Text = txtCalculatedThickness.Text; // Update Thickness
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

        private void TxtNozzleAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(txtNozzleAmount.Text, out int amount))
            {
                CreateDynamicControls(amount);
            }
        }

        private void CreateDynamicControls(int amount)
        {
            nozzleDynamicContent.Children.Clear();

            // Create the header grid first
            Grid headerGrid = new Grid();
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            // Add header labels
            headerGrid.Children.Add(CreateHeaderLabel("Nozzle Type", 0));
            headerGrid.Children.Add(CreateHeaderLabel("Amount", 1));
            headerGrid.Children.Add(CreateHeaderLabel("Nozzle Size", 2));
            headerGrid.Children.Add(CreateHeaderLabel("Pressure Class", 3));
            headerGrid.Children.Add(CreateHeaderLabel("Price", 4));
            headerGrid.Children.Add(CreateHeaderLabel("Davit", 5));

            // Add the header grid to the stack panel
            nozzleDynamicContent.Children.Add(headerGrid);

            var nozzleSizes = new List<string> { "DN15", "DN20", "DN25", "DN32", "DN40", "DN50" };
            var flangeClasses = new List<string> { "PN10", "PN16", "PN40", "150#", "300#", "600#" };

            for (int i = 1; i <= amount; i++)
            {
                Grid grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                Label label = new Label { Content = $"NozzleType {i}" };
                Grid.SetColumn(label, 0);
                grid.Children.Add(label);

                TextBox txtAmount = new TextBox { Name = $"txtAmountofSize{i}" };
                Grid.SetColumn(txtAmount, 1);
                grid.Children.Add(txtAmount);

                ComboBox cmbNozzleSize = new ComboBox { Name = $"nozzleSize{i}" };
                nozzleSizes.ForEach(size => cmbNozzleSize.Items.Add(size));
                Grid.SetColumn(cmbNozzleSize, 2);
                grid.Children.Add(cmbNozzleSize);

                ComboBox cmbFlangeClass = new ComboBox { Name = $"flangeClass{i}" };
                flangeClasses.ForEach(flange => cmbFlangeClass.Items.Add(flange));
                Grid.SetColumn(cmbFlangeClass, 3);
                grid.Children.Add(cmbFlangeClass);

                TextBox txtPrice = new TextBox { Name = $"txtPrice{i}" };
                Grid.SetColumn(txtPrice, 4);
                grid.Children.Add(txtPrice);

                CheckBox chkDavit = new CheckBox { Name = $"chkDavit{i}", Content = "Davit" };
                Grid.SetColumn(chkDavit, 5);
                grid.Children.Add(chkDavit);

                nozzleDynamicContent.Children.Add(grid);
            }
        }

        private Label CreateHeaderLabel(string content, int column)
        {
            Label headerLabel = new Label
            {
                Content = content,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            Grid.SetColumn(headerLabel, column);
            return headerLabel;
        }








    }
}