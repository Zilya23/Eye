using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Eye.DataBase;

namespace Eye.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public List<Agent> Agents { get; set; }
        public List<Agent> FilteredAgents { get; set; }
        public List<AgentType> AgentTypes { get; set; }
        public Dictionary<string, int> Sortings { get; set; }
        private int _page = 0;
        private int _countOnPage = 10;
        public MainPage()
        {
            InitializeComponent();
            DataContext = this;
            Agents = BDConnection.connection.Agent.ToList();
            FilteredAgents = Agents.ToList();
            AgentTypes = BDConnection.connection.AgentType.ToList();
            AgentTypes.Insert(0, new AgentType() { Title = "Все типы" });

            Sortings = new Dictionary<string, int>
            {
                { "Без сортировки", 1},
                { "Наименование по возрастанию", 2},
                { "Наименование по убыванию", 3 },
                { "Приоритет по возрастанию", 4 },
                { "Приоритет по убыванию", 5 },
                { "Размер скидки по возрастанию", 6},
                { "Размер скидки по убыванию", 7 }
            };


        }

        private void btnEditPriority_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnAddAgent_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AgentPage());
        }

        private void lvAgents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void lvAgents_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void cbAgentTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cbSorting_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        public void Filter(bool filtersChanged)
        {
            var searchingText = tbSearch.Text.ToLower();
            var sorting = Sortings[cbSorting.SelectedItem as string];
            var agentType = cbAgentTypes.SelectedItem as AgentType;

            if (sorting == null || agentType == null)
                return;

            FilteredAgents = Agents.FindAll(agent => agent.Title.ToLower().Contains(searchingText));

            if (agentType.ID != 0)
                FilteredAgents = FilteredAgents.FindAll(agent => agent.AgentType == agentType);

            switch (sorting)
            {
                case 1:
                    FilteredAgents = FilteredAgents.OrderBy(agent => agent.ID).ToList();
                    break;

                case 2:
                    FilteredAgents = FilteredAgents.OrderBy(agent => agent.Title).ToList();
                    break;

                case 3:
                    FilteredAgents = FilteredAgents.OrderByDescending(agent => agent.Title).ToList();
                    break;

                case 4:
                    FilteredAgents = FilteredAgents.OrderBy(agent => agent.Priority).ToList();
                    break;

                case 5:
                    FilteredAgents = FilteredAgents.OrderByDescending(agent => agent.Priority).ToList();
                    break;

                case 6:
                    FilteredAgents = FilteredAgents.OrderBy(agent => agent.Discount).ToList();
                    break;

                case 7:
                    FilteredAgents = FilteredAgents.OrderByDescending(agent => agent.Discount).ToList();
                    break;

                default:
                    break;
            }


            lvAgents.ItemsSource = FilteredAgents;
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            _page = Convert.ToInt32((sender as Button).Content) - 1;

            Filter();
        }
        private void CreatePagingList()
        {
            
            PagingPanel.Children.RemoveRange(0, PagingPanel.Children.Count);

            for (var i = 1; i <= (int)Math.Round((double)Agents.Count / 20); i++)
            {
                var button = new Button
                {
                    Width = 30,
                    Height = 30,
                    Content = i.ToString()
                };
                button.Click += OnButtonClick;

                PagingPanel.Children.Add(button);
            }
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            Filter();
        }
    }
}
