using jCubeOS.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace jCubeOS
{
    class InputTextBox : Input
    {
        private TextBox Input { get; set; }
        private bool InputReady { get; set; }

        public InputTextBox(TextBox inputTextBox)
        {
            Input = inputTextBox;

            Input.TextChanged -= TextChanged;
            Input.TextChanged += TextChanged;

            InputReady = false;
        }

        public override string ReadLine()
        {
            Input.Focus(Windows.UI.Xaml.FocusState.Keyboard);
            
            while (!InputReady)
            {
                Debug.WriteLine("InputWait");
                Task.Delay(1000);
            }

            return Input.Text;
        }

        void TextChanged(Object sender, TextChangedEventArgs args)
        {
            InputReady = true;
        }
    }
}
