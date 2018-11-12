﻿using System;
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

namespace MHW_Shop_Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public static readonly string[] DEFAULT_ITEMS = { "0001", "0005", "000D", "0011", "0046", "0047", "0048", "004A", "004B", "004F", "0058", "0059", "005A", "005C", "0055", "0056", "0057", "0066", "0067", "008A", "008C", "008D", "008F", "0090", "0092", "0093", "0095", "0096", "0098", "0099", "009A", "009B", "009D", "009E", "009F", "00A0", "00A1", "00A2", "00A3", "00A4", "00A5", "00A7", "00A8", "00A9", "00AE", "00AF", "00B0", "00B1", "00B2", "00B4" };

        public MainWindow()
        {
            InitializeComponent();
            Clear();

        }

        private void Populate_Boxes(List<string> items)
        {
            foreach (ComboBox cb in listview.Items)
            {
                if (items.Count == 0)
                {
                    break;
                }
                cb.SelectedIndex = int.Parse(items[0], System.Globalization.NumberStyles.HexNumber);
                items.RemoveAt(0);
            }
        }

        private void openFile(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
                FileName = "shopList",
                DefaultExt = ".slt",
                Filter = "Shop List file | *.slt",
                InitialDirectory = System.AppDomain.CurrentDomain.BaseDirectory
            };

            if (dlg.ShowDialog() == true)
            {
                string filename = dlg.FileName;
                byte[] input = System.IO.File.ReadAllBytes(filename);
                byte[] buffer = new byte[2];
                List<string> items = new List<string>();
                for (int i = 10; i < input.Length-1; i+=12)
                {
                    buffer[0] = input[i+1];
                    buffer[1] = input[i];
                    items.Add(BitConverter.ToString(buffer).Replace("-", ""));
                }
                Populate_Boxes(items);
            }
        }

        private async void saveFile(object sender, RoutedEventArgs e)
        {
            System.IO.Stream fs;
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog()
            {
                FileName = "shopList",
                DefaultExt = ".slt",
                Filter = "Shop List file | *.slt",
                InitialDirectory = System.AppDomain.CurrentDomain.BaseDirectory
            };


            if (dlg.ShowDialog() == true)
            {
                if ((fs = dlg.OpenFile()) != null)
                {
                    byte[] header = new byte[] { 0x18, 0x00, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00};
                    byte[] buffer = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00};
                    List<byte> items = header.ToList();
                    foreach (ComboBox cb in listview.Items)
                    {
                        Item item = (Item) cb.SelectedItem;
                        string hex = item.Key.Substring(4);
                        if (hex.Equals("0000"))
                        {
                            continue;
                        } else
                        {
                            items.Add(Convert.ToByte(hex.Substring(2)));
                            items.Add(Convert.ToByte(hex.Substring(0, 2)));
                            items.AddRange(buffer);
                        }                        
                    }
                    byte[] output = items.ToArray();
                    await fs.WriteAsync(output, 0, output.Length);
                    fs.Close();
                }
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ResourceDictionary dict = new ResourceDictionary();

            switch (((sender as ComboBox).SelectedItem as ComboBoxItem).Tag.ToString())
            {
                case "en-US":
                    dict.Source = new Uri("Lang.en-US.xaml", UriKind.Relative);
                    break;
                case "es-ES":
                    dict.Source = new Uri("Lang.es-ES.xaml", UriKind.Relative);
                    break;
                default:
                    break;
            }
            this.Resources.MergedDictionaries.Add(dict);
        }

        private void Default_Items(object sender, RoutedEventArgs e)
        {
            Populate_Boxes(DEFAULT_ITEMS.ToList());
        }

        private void Clear_Button(object sender, RoutedEventArgs e)
        {
            Clear();
        }

        private void Clear()
        {
            foreach (ComboBox cb in listview.Items)
            {
                cb.SelectedIndex = 0;
            }
        }
    }

    public class Item
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public override string ToString()
        {
            return Value;
        }
    }

}