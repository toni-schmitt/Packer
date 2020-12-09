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
using System.Windows.Shapes;

namespace Packer
{
    /// <summary>
    /// Interaktionslogik für MessageBox.xaml
    /// </summary>
    public partial class MessageBox : Window
    {

        /// <summary>
        /// Constructor Methode for MessageBox Window
        /// </summary>
        /// <param name="currentResourceDictionary">Takes current RecourceDictionary to show correct language</param>
        public MessageBox(ResourceDictionary currentResourceDictionary)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            // Sets ResourceDictionary to currentResourceDictionary
            this.Resources.MergedDictionaries.Add(currentResourceDictionary);
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            // Cloeses this Window
            this.Close();
        }

        
    }
}
