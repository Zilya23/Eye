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
using Microsoft.Win32;

namespace Eye.Pages
{
    /// <summary>
    /// Логика взаимодействия для AgentPage.xaml
    /// </summary>
    public partial class AgentPage : Page
    {
        public Agent Agent { get; set; }
        public List<AgentType> AgentTypes { get; set; }
        public List<Product> Products { get; set; }
        public AgentPage(Agent agent, bool isNew = false)
        {
            InitializeComponent();
            Agent = agent;
            AgentTypes = BDConnection.connection.AgentType.ToList();
            Products = BDConnection.connection.Product.ToList();

            if (isNew)
            {
                Title = $"Новый {Title}";
                btnDelete.Visibility = Visibility.Hidden;
            }
            else
                Title = $"{Title} №{Agent.ID}";

            this.DataContext = this;

            if (agent.ProductSale != null)
                btnDelete.Visibility = Visibility.Hidden;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BDConnection.connection.Agent.Add(agent);
                BDConnection.connection.SaveChanges();
            }
            catch
            {
                MessageBox.Show("Введены некорректные значения", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            NavigationService.Navigate(new MainPage());

        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы точно хотите удалить данного агента?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.No)
                return;
            BDConnection.connection.Agent.Remove(agent);
            NavigationService.Navigate(new MainPage());
        }

        private void btnAddImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog
            {
                Filter = "*.png|*.png|*.jpeg|*.jpeg|*.jpg|*.jpg"
            };

            if (fileDialog.ShowDialog().Value)
            {
                var image = File.ReadAllBytes(fileDialog.FileName);
                Agent.Logo = image;

                imgLogo.Source = new BitmapImage(new Uri(fileDialog.FileName));
            }
        }

        private void lvProductSales_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var productSale = lvProductSales.SelectedItem as ProductSale;
                if (productSale == null)
                    return;
                var result = MessageBox.Show("Вы точно хотите удалить данную продажу?", 
                    "Предупреждение", 
                    MessageBoxButton.YesNoCancel, 
                    MessageBoxImage.Warning);
                if (result != MessageBoxResult.Yes)
                    return;
                Agent.ProductSales.Remove(productSale);

                lvProductSales.ItemsSource = Agent.ProductSales;
                lvProductSales.Items.Refresh();
            }
            catch { }
        }

        private void cbProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var product = cbProducts.SelectedItem as Product;
            if (Agent.ProductSales.Any(x => x.Product.ID == product.ID))
                return;

            Agent.ProductSales.Add(new ProductSale
            {
                Agent = Agent,
                Product = product,
                SaleDate = DateTime.Now
            });

            lvProductSales.ItemsSource = Agent.ProductSales;
            lvProductSales.Items.Refresh();
        }
    }
}
