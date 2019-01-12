using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SampleUserControlLibrary
{
    public class Scenario
    {
        public string Title
        {
            set;
            get;
        }

        public Type PageClass
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class SampleScenarios : UserControl
    {
        public static DependencyProperty SampleTitleProperty =
            DependencyProperty.Register("SampleTitle", typeof(string), typeof(SampleScenarios));

        public static DependencyProperty SampleScenarioListProperty =
            DependencyProperty.Register("SampleScenarioList", typeof(Scenario[]), typeof(SampleScenarios));

        private SubscriptionKeyPage _subscriptionPage;

        public string SampleTitle
        {
            get { return (string)GetValue(SampleTitleProperty); }
            set { SetValue(SampleTitleProperty, value); }
        }

        public Scenario[] SampleScenarioList
        {
            get { return (Scenario[])GetValue(SampleScenarioListProperty); }
            set
            {
                SetValue(SampleScenarioListProperty, value);
                _scenarioListBox.ItemsSource = SampleScenarioList;
            }
        }

        public string SubscriptionKey
        {
            get;
            set;
        }

        public string SubscriptionEndpoint
        {
            get;
            set;
        }

        public void SetSubscriptionPageEndpoint(string endpoint)
        {
            _subscriptionPage.SetSubscriptionEndpoint(endpoint);
        }

        public SampleScenarios()
        {
            InitializeComponent();
            _subscriptionPage = new SubscriptionKeyPage(this);
            SubscriptionKey = _subscriptionPage.SubscriptionKey;
            SubscriptionEndpoint = _subscriptionPage.SubscriptionEndpoint;

            SampleTitle = "Replace SampleNames with SampleScenarios.SampleTitle property";

            SampleScenarioList = new Scenario[]
            {
                new Scenario { Title = "Scenario 1: Replace items using SampleScenarios.ScenarioList" },
                new Scenario { Title = "Scenario 2: Replace items using SampleScenarios.ScenarioList" }
            };

            _scenarioListBox.ItemsSource = SampleScenarioList;

            _scenarioFrame.Navigate(_subscriptionPage);
        }

        public void Log(string logMessage)
        {
            if (String.IsNullOrEmpty(logMessage) || logMessage == "\n")
            {
                _logTextBox.Text += "\n";
            }
            else
            {
                string timeStr = DateTime.Now.ToString("HH:mm:ss.ffffff");
                string messaage = "[" + timeStr + "]: " + logMessage + "\n";
                _logTextBox.Text += messaage;
            }
            _logTextBox.ScrollToEnd();
        }

        public void ClearLog()
        {
            _logTextBox.Text = "";
        }

        private void ScenarioChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox scenarioListBox = sender as ListBox;
            Scenario scenario = scenarioListBox.SelectedItem as Scenario;
            ClearLog();

            if (scenario != null)
            {
                Page page = Activator.CreateInstance(scenario.PageClass) as Page;
                page.DataContext = this.DataContext;
                _scenarioFrame.Navigate(page);
            }
        }

        private void SubscriptionManagementButton_Click(object sender, RoutedEventArgs e)
        {
            _scenarioFrame.Navigate(_subscriptionPage);
            // Reset the selection so that we can get SelectionChangedEvent later.
            _scenarioListBox.SelectedIndex = -1;
        }
    }

    public class ScenarioBindingConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Scenario s = value as Scenario;
            if (s != null)
            {
                return s.Title;
            }
            return null;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}