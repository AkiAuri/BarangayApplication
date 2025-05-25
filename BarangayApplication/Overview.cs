using BarangayApplication.Models.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Globalization;

namespace BarangayApplication
{
    public partial class Overview: Form
    {
        private Timer _dateTimeTimer;
        private int _currentChartIndex = 0;
        private List<Action> _chartLoaders;
        private List<string> _chartTopics;

        private string _currentAccountId;
        public Overview(string currentAccountId)
        {
            _currentAccountId = currentAccountId; // Store account ID for greeting
            InitializeComponent();
            SetGreetingMessage();
            InitializeDateTimeTimer();
            InitializeCharts();
            LoadCurrentChart();

            tipCreateNew.SetToolTip(pbCreateNew, "Create New");
            tipSearch.SetToolTip(pbSearch, "Search");

            UpdateResidentAndApplicationCounters();
        }

        private void SetGreetingMessage()
        {
            string greeting;
            int hour = DateTime.Now.Hour;
            if (hour < 12) {
                greeting = "Good Morning";
            } 
            else if (hour < 18) {
                greeting = "Good Afternoon";
            } 
            else {
                greeting = "Good Evening";
            }

            // Use the AccountName that was set at login
            string accountName = LoginMenu.CurrentUser.AccountName;

            if (!string.IsNullOrWhiteSpace(accountName))
                greeting += $" {accountName}";

            lblGreeting.Text = greeting;
        }
        private void InitializeDateTimeTimer()
        {
            _dateTimeTimer = new Timer();
            _dateTimeTimer.Interval = 1000;
            _dateTimeTimer.Tick += UpdateDateTimeLabel;
            _dateTimeTimer.Start();
        }

        private void UpdateDateTimeLabel(object sender, EventArgs e)
        {
            // Update the label with the current date and time
            lblDateTime.Text = DateTime.Now.ToString("dddd, MMMM dd, yyyy hh:mm tt"); // Full date/time pattern
        }
        private void InitializeCharts()
        {
            // Define the chart loaders and their topics
            _chartLoaders = new List<Action>
        {
            LoadAgeGroupChart,
            LoadGenderDistributionChart,
            LoadPurposeDistributionChart
        };

            _chartTopics = new List<string>
        {
            "Age Group Distribution",
            "Gender Distribution",
            "Purpose of Application"
        };
        }

        private void LoadCurrentChart()
        {
            // Clear the chart and load the current chart
            chartOverview.Series.Clear();
            chartOverview.Titles.Clear();

            _chartLoaders[_currentChartIndex]();
            lblChartTopic.Text = _chartTopics[_currentChartIndex];
        }

        private void LoadAgeGroupChart()
        {
            var repo = new ResidentsRepository();
            var ageGroups = repo.GetAgeGroupCounts();

            Series series = new Series
            {
                Name = "AgeGroups",
                ChartType = SeriesChartType.Pie
            };

            foreach (var group in ageGroups)
            {
                // Skip adding data points with a value of 0
                if (group.Value == 0)
                    continue;

                var pointIndex = series.Points.AddXY(group.Key, group.Value);

                var dataPoint = series.Points[pointIndex];
                dataPoint.IsValueShownAsLabel = true; // Show the label when value is not 0
                dataPoint.Label = group.Key.ToString();
            }


            series["PieLabelStyle"] = "Outside";
            series["PieLineColor"] = "Black";
            series.SmartLabelStyle.Enabled = true;
            series.SmartLabelStyle.AllowOutsidePlotArea = LabelOutsidePlotAreaStyle.Yes; // Allow labels outside the chart area
            series.SmartLabelStyle.IsOverlappedHidden = true; // Hide overlapping labels

            chartOverview.Series.Add(series);
            chartOverview.Titles.Add("Age Group Distribution");
            chartOverview.Legends.Clear();

            UpdateLabelsFromChart("AgeGroups", new[] { lblData1, lblData2, lblData3, lblData4, lblData5, lblData6, lblData7, lblData8, lblData9, lblData10, lblData11, lblData12, lblData13 });
        }

        private void LoadGenderDistributionChart()
        {
            var repo = new ResidentsRepository();
            var genderDistribution = repo.GetGenderDistribution();

            Series series = new Series
            {
                Name = "GenderDistribution",
                ChartType = SeriesChartType.Pie
            };

            foreach (var gender in genderDistribution)
            {
                if (gender.Value == 0)
                    continue;

                var pointIndex = series.Points.AddXY(gender.Key, gender.Value);
                var dataPoint = series.Points[pointIndex];
                dataPoint.Label = gender.Key; // Show category name
            }


            // Configure the series
            series["PieLabelStyle"] = "Outside"; // Move labels outside the pie chart
            series["PieLineColor"] = "Black";   // Add leader lines

            // Enable smart labels to avoid overlap
            series.SmartLabelStyle.Enabled = true;

            chartOverview.Series.Add(series);
            chartOverview.Titles.Add("Gender Distribution");

            // Hide the legend
            chartOverview.Legends.Clear();

            UpdateLabelsFromChart("GenderDistribution", new[] { lblData1, lblData2, lblData3, lblData4, lblData5, lblData6, lblData7, lblData8, lblData9, lblData10, lblData11, lblData12, lblData13 });
        }

        private void LoadPurposeDistributionChart()
        {
            var repo = new ResidentsRepository();
            var purposeDistribution = repo.GetPurposeDistribution();

            Series series = new Series
            {
                Name = "PurposeDistribution",
                ChartType = SeriesChartType.Pie
            };

            foreach (var purpose in purposeDistribution)
            {
                if (purpose.Value == 0)
                    continue;

                var pointIndex = series.Points.AddXY(purpose.Key, purpose.Value);
                var dataPoint = series.Points[pointIndex];
                dataPoint.Label = purpose.Key; // Show category name
            }

            // Configure the series
            series["PieLabelStyle"] = "Outside"; // Move labels outside the pie chart
            series["PieLineColor"] = "Black";   // Add leader lines

            // Enable smart labels to avoid overlap
            series.SmartLabelStyle.Enabled = true;

            chartOverview.Series.Add(series);
            chartOverview.Titles.Add("Purpose of Application");

            // Hide the legend
            chartOverview.Legends.Clear();

            UpdateLabelsFromChart("PurposeDistribution", new[] { lblData1, lblData2, lblData3, lblData4, lblData5, lblData6, lblData7, lblData8, lblData9, lblData10, lblData11, lblData12, lblData13 });
        }
        
        /// <summary>
        /// Updates the total counters for residents and applications.
        /// - lblTotalResApplied: unique residents (no duplicate full names)
        /// - lblTotalAppMonth: total applications submitted this month (including duplicates)
        /// - label2: e.g. "TOTAL APPLICATIONS THIS MAY"
        /// </summary>
        private void UpdateResidentAndApplicationCounters()
        {
            var repo = new ResidentsRepository();

            var residents = repo.GetApplicants();

            // 1. Unique residents by full name (case-insensitive)
            var uniqueNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var r in residents)
            {
                string fullName = $"{r.LastName ?? ""},{r.FirstName ?? ""},{r.MiddleName ?? ""}".Trim();
                if (!string.IsNullOrWhiteSpace(fullName))
                    uniqueNames.Add(fullName);
            }
            lblTotalResApplied.Text = uniqueNames.Count.ToString();

            // 2. Applications added this month, using logbook (not DateOfBirth)
            int totalThisMonth = repo.GetMonthlyResidentAddCount();
            lblTotalAppMonth.Text = totalThisMonth.ToString();

            // 3. Update label2 with the actual month name
            int month = DateTime.Now.Month;
            label2.Text = $"TOTAL APPLICATIONS THIS {CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month).ToUpper()}";

            // (Optional) If you want a separate label for "residents added this month" via logs
            // lblMonthlyResidentAddCount.Text = totalThisMonth.ToString();
            // lblMonthlyResidentAddTitle.Text = "RESIDENTS ADDED THIS " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month).ToUpper();
        }
        
        private void picNext_Click(object sender, EventArgs e)
        {
            // Navigate to the next chart
            _currentChartIndex = (_currentChartIndex + 1) % _chartLoaders.Count;
            LoadCurrentChart();
        }

        private void picPrevious_Click(object sender, EventArgs e)
        {
            // Navigate to the previous chart
            _currentChartIndex = (_currentChartIndex - 1 + _chartLoaders.Count) % _chartLoaders.Count;
            LoadCurrentChart();
        }
        private void UpdateLabelsFromChart(string seriesName, Label[] labels)
        {
            // Retrieve the series from the chart
            var series = chartOverview.Series[seriesName];

            // Loop through the labels and update them
            for (int i = 0; i < labels.Length; i++)
            {
                if (i < series.Points.Count)
                {
                    // Update label text with legend and value
                    labels[i].Text = $"{series.Points[i].AxisLabel}: {series.Points[i].YValues[0]}";
                    labels[i].Visible = true;
                }
                else
                {
                    //hide excess labels
                    labels[i].Visible = false;
                }
            }
        }

        private void Overview_Load(object sender, EventArgs e)
        {
            UpdateDateTimeLabel(null, null);

            // Apply rounded corners to panels
            SetRoundedCorners(Box1, 20); // 20 = corner radius
            SetRoundedCorners(Box2, 20);
            SetRoundedCorners(pnlTotalResidentsApplied, 20);
            SetRoundedCorners(panel2, 20);
        }

        /// <summary>
        /// Sets rounded corners to a given Panel.
        /// </summary>
        /// <param name="panel">The panel to modify.</param>
        /// <param name="radius">The radius of the corner rounding.</param>
        private void SetRoundedCorners(Panel panel, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.StartFigure();
            path.AddArc(new Rectangle(0, 0, radius, radius), 180, 90);
            path.AddArc(new Rectangle(panel.Width - radius, 0, radius, radius), 270, 90);
            path.AddArc(new Rectangle(panel.Width - radius, panel.Height - radius, radius, radius), 0, 90);
            path.AddArc(new Rectangle(0, panel.Height - radius, radius, radius), 90, 90);
            path.CloseFigure();
            panel.Region = new Region(path);
        }



        private void chartOverview_Click(object sender, EventArgs e)
        {

        }

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {

        }

        private void pbCreateNew_Click(object sender, EventArgs e)
        {
            // Open the MainViewForm as a modal dialog for adding a new resident
            using (MainViewForm form = new MainViewForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    // Refresh overview stats and charts
                    UpdateResidentAndApplicationCounters();
                    LoadCurrentChart();
                }
            }
        }

        private void lblTotalResApplied_Click(object sender, EventArgs e)
        {

        }

        private void pbSearch_Click(object sender, EventArgs e)
        {
            FormApplication Search = new FormApplication();
            Search.ShowDialog();
        }
    }
}
