using jCubeOS.Classes;
using System;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using System.Diagnostics;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace jCubeOS
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private RealMachine RealMachine { get; set; };

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            StartButton.IsEnabled = false;
            PathTextBox.IsEnabled = false;
            StartRealMachine();
        }

        private void StartRealMachine()
        {
            RealMachine = new RealMachine();

            string filePath = PathTextBox.Text;
            Input inputHandler = new InputTextBox(InputTextBox);

            RealMachine.LoadVirtualMachine(filePath, inputHandler);
        }

        private void StepButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            RealMachine.GetProcessor().Execute();
        }
    }
}
