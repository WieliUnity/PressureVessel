using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
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
        private NozzleDataLoader _nozzleDataLoader = new NozzleDataLoader();

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
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            // Add header labels
            headerGrid.Children.Add(CreateHeaderLabel("Nozzle Type", 0));
            headerGrid.Children.Add(CreateHeaderLabel("Amount", 1));
            headerGrid.Children.Add(CreateHeaderLabel("Lenght Nozzle", 2));
            headerGrid.Children.Add(CreateHeaderLabel("Nozzle Size", 3));
            headerGrid.Children.Add(CreateHeaderLabel("Pressure Class", 4));
            headerGrid.Children.Add(CreateHeaderLabel("Price Pipe + Flange", 5));
            headerGrid.Children.Add(CreateHeaderLabel("Blindflange?", 6));
            headerGrid.Children.Add(CreateHeaderLabel("Davit?", 7));
            headerGrid.Children.Add(CreateHeaderLabel("Weld Hours", 8));
            headerGrid.Children.Add(CreateHeaderLabel("Cost", 9));
            headerGrid.Children.Add(CreateHeaderLabel("Weight", 10));
            headerGrid.Children.Add(CreateHeaderLabel("Build Hours", 11));
            headerGrid.Children.Add(CreateHeaderLabel("Amount Of Bolts", 12));
            headerGrid.Children.Add(CreateHeaderLabel("Size Of Bolts", 13));

            // Add the header grid to the stack panel
            nozzleDynamicContent.Children.Add(headerGrid);

            var nozzleSizes = new List<string> { "DN15", "DN20", "DN25", "DN32", "DN40", "DN50", "DN65", "DN80", "DN100", "DN150", "DN200", "DN250", "DN300", "DN350", "DN400", "DN450", "DN500", "DN600" };
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
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                Label label = new Label { Content = $"NozzleType {i}" };
                Grid.SetColumn(label, 0);
                grid.Children.Add(label);

                TextBox txtAmount = new TextBox { Name = $"txtAmountofSize{i}" };
                txtAmount.TextChanged += ControlValueChanged;
                Grid.SetColumn(txtAmount, 1);
                grid.Children.Add(txtAmount);

                TextBox txtLenghtNozzle = new TextBox { Name = $"txtLenghtNozzle{i}", Text = "200" };
                txtLenghtNozzle.TextChanged += ControlValueChanged;
                Grid.SetColumn(txtLenghtNozzle, 2);
                grid.Children.Add(txtLenghtNozzle);

                ComboBox cmbNozzleSize = new ComboBox { Name = $"nozzleSize{i}" };
                nozzleSizes.ForEach(size => cmbNozzleSize.Items.Add(size));
                cmbNozzleSize.SelectionChanged += ControlValueChanged;
                Grid.SetColumn(cmbNozzleSize, 3);
                grid.Children.Add(cmbNozzleSize);

                ComboBox cmbFlangeClass = new ComboBox { Name = $"flangeClass{i}" };
                flangeClasses.ForEach(flange => cmbFlangeClass.Items.Add(flange));
                cmbFlangeClass.SelectionChanged += ControlValueChanged;
                Grid.SetColumn(cmbFlangeClass, 4);
                grid.Children.Add(cmbFlangeClass);

                TextBox txtPrice = new TextBox { Name = $"txtPrice{i}" };
                txtPrice.TextChanged += ControlValueChanged;
                Grid.SetColumn(txtPrice, 5);
                grid.Children.Add(txtPrice);

                CheckBox chkBlind = new CheckBox { Name = $"chkBlind{i}", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
                chkBlind.Click += ControlValueChanged;
                Grid.SetColumn(chkBlind, 6);
                grid.Children.Add(chkBlind);

                CheckBox chkDavit = new CheckBox { Name = $"chkDavit{i}", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
                chkDavit.Click += ControlValueChanged;
                Grid.SetColumn(chkDavit, 7);
                grid.Children.Add(chkDavit);

                TextBox txtWeldHours = new TextBox { Name = $"txtWeldHours{i}", IsReadOnly = true };
                Grid.SetColumn(txtWeldHours, 8);
                grid.Children.Add(txtWeldHours);

                TextBox txtCost = new TextBox { Name = $"txtCost{i}", IsReadOnly = true };
                Grid.SetColumn(txtCost, 9);
                grid.Children.Add(txtCost);

                TextBox txtWeight = new TextBox { Name = $"txtWeight{i}" , IsReadOnly = true};
                Grid.SetColumn(txtWeight, 10);
                grid.Children.Add(txtWeight);

                TextBox txtBuildHours = new TextBox { Name = $"txtBuildHours{i}", IsReadOnly = true };
                Grid.SetColumn(txtBuildHours, 11);
                grid.Children.Add(txtBuildHours);
                
                TextBox txtAmountBolts = new TextBox { Name = $"txtAmountBolts{i}", IsReadOnly = true };
                Grid.SetColumn(txtAmountBolts, 12);
                grid.Children.Add(txtAmountBolts);
                
                TextBox txtSizeBolts = new TextBox { Name = $"txtSizeBolts{i}", IsReadOnly = true };
                Grid.SetColumn(txtSizeBolts, 13);
                grid.Children.Add(txtSizeBolts);


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

        private void ControlValueChanged(object sender, EventArgs e)
        {
            if (sender is Control control && int.TryParse(Regex.Match(control.Name, @"\d+").Value, out int index))
            {

                // Find the parent grid of the changed control
                var parentGrid = control.Parent as Grid;
                if (parentGrid == null) return;

                // Initialize variables to hold the found controls
                TextBox txtAmount = null, txtLengthNozzle = null, txtPrice = null, txtWeldHours = null, txtCost = null, txtWeight = null, txtBuildHours = null, txtAmountBolts = null, txtSizeBolts = null;
                ComboBox cmbNozzleSize = null, cmbFlangeClass = null;
                CheckBox chkBlind = null, chkDavit = null;

                // Traverse the children of the parent grid to find the controls
                foreach (var child in parentGrid.Children)
                {
                    if (child is TextBox textBox)
                    {
                        if (textBox.Name == $"txtAmountofSize{index}") txtAmount = textBox;
                        else if (textBox.Name == $"txtLenghtNozzle{index}") txtLengthNozzle = textBox;
                        else if (textBox.Name == $"txtPrice{index}") txtPrice = textBox;
                        else if (textBox.Name == $"txtWeldHours{index}") txtWeldHours = textBox;
                        else if (textBox.Name == $"txtCost{index}") txtCost = textBox;
                        else if (textBox.Name == $"txtWeight{index}") txtWeight = textBox;
                        else if (textBox.Name == $"txtBuildHours{index}") txtBuildHours = textBox;
                        else if (textBox.Name == $"txtAmountBolts{index}") txtAmountBolts = textBox;
                        else if (textBox.Name == $"txtSizeBolts{index}") txtSizeBolts = textBox;

                    }
                    else if (child is ComboBox comboBox)
                    {
                        if (comboBox.Name == $"nozzleSize{index}") cmbNozzleSize = comboBox;
                        else if (comboBox.Name == $"flangeClass{index}") cmbFlangeClass = comboBox;
                    }
                    else if (child is CheckBox checkBox)
                    {
                        if (checkBox.Name == $"chkBlind{index}") chkBlind = checkBox;
                        else if (checkBox.Name == $"chkDavit{index}") chkDavit = checkBox;
                    }
                }

                // Now you can use these controls as before
                UpdateWeight(cmbNozzleSize, cmbFlangeClass, txtWeight, txtAmount,chkBlind, chkDavit, txtWeldHours, txtBuildHours, txtAmountBolts, txtSizeBolts, txtCost, txtPrice);

                // Add your logic for other calculations, if needed
                // UpdateHoursAndCost(txtAmount, txtLengthNozzle, txtPrice, txtHours, txtCost, chkBlind, chkDavit);
            }
        }



        private void UpdateWeight(ComboBox cmbNozzleSize, ComboBox cmbFlangeClass, TextBox txtWeight, TextBox txtAmount, CheckBox chkBlind, CheckBox chkDavit, TextBox txtWeldHours, TextBox txtBuildHours, TextBox txtAmountBolts, TextBox txtSizeBolts, TextBox txtCost, TextBox txtPrice)
        {
            if (cmbNozzleSize != null && cmbFlangeClass != null && txtWeight != null && txtAmount != null)
            {
                string nozzleSize = cmbNozzleSize.SelectedItem?.ToString();
                string flangeClass = cmbFlangeClass.SelectedItem?.ToString();
                bool hasDavit = chkDavit.IsChecked.HasValue && chkDavit.IsChecked.Value;
                bool hasBlind = chkBlind.IsChecked.HasValue && chkBlind.IsChecked.Value;

                double? weight;
                double? weldHours;
                double? buildHours;
                int? numberBolts;
                string? sizeBolts;
                double.TryParse(txtWeldCostPerHour.Text, out double weldCostPerHour);
                double.TryParse(txtPrice.Text, out double pricePerFlange);
                if (!string.IsNullOrEmpty(nozzleSize) && !string.IsNullOrEmpty(flangeClass) && double.TryParse(txtAmount.Text, out double amount))
                {
                    if (hasBlind && hasDavit)
                    {
                        //weight = _nozzleDataLoader.GetWeightAndHoursWithAll(nozzleSize, flangeClass);
                        (weight, weldHours,buildHours,numberBolts,sizeBolts) = _nozzleDataLoader.GetWeightAndHoursWithAll(nozzleSize, flangeClass);
                    }
                    else if (hasBlind)
                    {
                        //weight = _nozzleDataLoader.GetWeightAndHoursWithBlind(nozzleSize, flangeClass);
                        (weight, weldHours, buildHours, numberBolts, sizeBolts) = _nozzleDataLoader.GetWeightAndHoursWithBlind(nozzleSize, flangeClass);
                    }
                    else
                    {
                        //weight = _nozzleDataLoader.GetWeightAndHours(nozzleSize, flangeClass);
                        (weight, weldHours, buildHours, numberBolts, sizeBolts) = _nozzleDataLoader.GetWeightAndHours(nozzleSize, flangeClass);
                    }

                    if (weight.HasValue && weldHours.HasValue && buildHours.HasValue)
                    {
                        double totalWeldHours = weldHours.Value * amount;
                        double totalWeight = weight.Value * amount;
                        double totalBuildHours = buildHours.Value * amount;
                        double totalCost = (((totalWeldHours + totalBuildHours) * weldCostPerHour) + pricePerFlange) * amount;

                        txtWeight.Text = totalWeight.ToString("N2");
                        txtWeldHours.Text = totalWeldHours.ToString("N2");
                        txtBuildHours.Text = totalBuildHours.ToString("N2");
                        txtCost.Text = totalCost.ToString("N2");


                        if (hasBlind)
                        {
                            int totalNumberBolts = numberBolts.Value * (int)amount;
                            txtAmountBolts.Text = totalNumberBolts.ToString();
                            txtSizeBolts.Text = sizeBolts.ToString();
                        }
                        else
                        {
                            txtAmountBolts.Text = "N/A";
                            txtSizeBolts.Text = "N/A";
                        }

                        
                        

                    }
                    else
                    {
                        txtWeight.Text = "N/A";
                    }
                }
                else
                {
                    txtWeight.Text = "N/A"; // In case one or both selections are null or empty.
                }
            }
        }

        
        private void CmbEnd_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CalculateAndUpdateResults();
        }

        private void txtCalculatedThickness_TextChanged(object sender, TextChangedEventArgs e)
        {
            CalculateAndUpdateResults();
        }


        //private void CmbEnd_SelectionChanged(object sender, SelectionChangedEventArgs e)
        private void CalculateAndUpdateResults()
        {
            // Check if both combo boxes have selected items
            if (cmbTopEnd.SelectedItem == null || cmbBottomEnd.SelectedItem == null)
                return;

            // Assuming methods to get diameter and thickness from another tab
            double diameter = double.Parse(txtDiameter.Text);
            double thickness = int.Parse(txtCalculatedThickness.Text);

            // Get the price for Top End, default to 0 if not provided
            double.TryParse(txtTopEndCapPrice.Text, out double topEndPrice);

            // Get the price for Bottom End, default to 0 if not provided
            double.TryParse(txtBottomEndCapPrice.Text, out double bottomEndPrice);

            if (cmbTopEnd.SelectedItem is ComboBoxItem topEndSelectedItem)
            {
                string topEndSelection = topEndSelectedItem.Content.ToString();
                DishedEndCalculator calculator = new DishedEndCalculator();
                var topEndResult = calculator.Calculate(topEndSelection, diameter, thickness, topEndPrice);
                // Update UI or perform other actions with topEndResult
                txtVolumeTopEnd.Text = topEndResult.Volume.ToString("N2");
                txtWeightTopEnd.Text = topEndResult.Weight.ToString("N2");
                txtTopEndWeldHours.Text = topEndResult.WeldHours.ToString("N2");
                txtTopEndBuildHours.Text = topEndResult.BuildHours.ToString("N2");
                
            }

            if (cmbBottomEnd.SelectedItem is ComboBoxItem bottomEndSelectedItem)
            {
                string bottomEndSelection = bottomEndSelectedItem.Content.ToString();
                DishedEndCalculator calculator = new DishedEndCalculator();
                var bottomEndResult = calculator.Calculate(bottomEndSelection, diameter, thickness, bottomEndPrice);
                // Update UI or perform other actions with bottomEndResult
                txtVolumeBottomEnd.Text = bottomEndResult.Volume.ToString("N2");
                txtWeightBottomEnd.Text = bottomEndResult.Weight.ToString("N2");
                txtBottomEndWeldHours.Text = bottomEndResult.WeldHours.ToString("N2");
                txtBottomEndBuildHours.Text = bottomEndResult.BuildHours.ToString("N2");
            }
        }



    }
}