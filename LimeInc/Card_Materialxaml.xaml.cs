using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пользовательский элемент управления" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234236

namespace LimeInc
{
    public sealed partial class Card_Materialxaml : UserControl
    {
        public Card_Materialxaml()
        {
            this.InitializeComponent();
        }

        private bool _check;
        private string _message;
        private string _title;
        private BitmapImage _icon;
        private string _imgLoc;
        [Category("Custom Props")]
        public string ImgLock
        {
            get { return _imgLoc; }
            set { _imgLoc = value; }

        }
        [Category("Custom Props")]
        public string Title
        {
            get { return _title; }
            set { _title = value; NameFile.Text = value; }
        }

        [Category("Custom Props")]
        public string Message
        {
            get { return _message; }
            set { _message = value; TypeFile.Text = value; }
        }
        [Category("Custom Props")]
        public BitmapImage Icon
        {
            get { return _icon; }
            set { _icon = value; Image.Source = value; }
        }

    }
}
