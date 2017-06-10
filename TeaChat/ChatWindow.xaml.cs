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
using Newtonsoft.Json;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Ink;

namespace TeaChat
{
    /// <summary>
    /// Interaction logic for ChatWindow.xaml
    /// </summary>
    public partial class ChatWindow : Window
    {
        Window1 echoWindow;

        TextBox textBoxInCanvas = new TextBox();
        bool creatingTextBox = false;
        bool editingText = false;

        public string myUserName;
        public List<string> chatFriends;

        public ChatWindow(List<string> chatFriends)
        {
            InitializeComponent();
            this.Hide();
            textBoxInCanvas.Width = 100;
            textBoxInCanvas.Height = 50;
            textBoxInCanvas.Background = Brushes.Transparent;
            textBoxInCanvas.BorderBrush = Brushes.Transparent;
            textBoxInCanvas.AcceptsReturn = true;
            
            this.chatFriends = chatFriends;
            this.Title = "聊天中 - ";
            foreach (string friendName in chatFriends)
            {
                this.Title += " " + friendName;
            }
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            if (echoWindow != null) echoWindow.Close();
        }

        private void receiveFromServer()
        {
            while(true)
            {
                // TODO: 從 socket 接收 string command
                string command = "Add Stroke";
                //

                switch(command)
                {
                    case "Add Stroke":
                        // TODO: 從 socket 接收 string drawingAttributesText, string stylusPointsText
                        string drawingAttributesText, stylusPointsText;
                        drawingAttributesText = File.ReadAllText("../../../da.txt");
                        stylusPointsText = File.ReadAllText("../../../sp.txt");
                        //
                        receiveStroke(drawingAttributesText, stylusPointsText);
                        break;
                    case "Erase All":
                        receiveErase();
                        break;
                    case "Add TextBox":
                        // TODO: 從 socket 接收 string text, string X, string Y
                        string textBoxText = "important!!";
                        string X = "100.23";
                        string Y = "58.2345";
                        //
                        receiveTextBox(textBoxText, Convert.ToDouble(X), Convert.ToDouble(Y));
                        break;
                    case "Text Message":
                        // TODO: 從 socket 接收 string text
                        string messageText = "hello~";
                        //
                        receiveTextMessage(messageText);
                        break;
                    default:
                        MessageBox.Show("Server傳了未知指令");
                        break;
                }
            }
        }

        #region 畫圖及清除
        private void InkCanvas_StrokeCollected(object sender, InkCanvasStrokeCollectedEventArgs e)
        {
            string drawingAttributesText = JsonConvert.SerializeObject(e.Stroke.DrawingAttributes);
            string stylusPointsText = JsonConvert.SerializeObject(e.Stroke.StylusPoints);

            // TODO: 傳送 drawingAttributesText, stylusPointsText 兩段 string 給 server
            echoWindow.receiveStroke(drawingAttributesText, stylusPointsText);
            //
        }

        private void menuItemEraseAll_Click(object sender, RoutedEventArgs e)
        {
            inkCanvas.Strokes.Clear();
            inkCanvas.Children.Clear();

            // TODO: 傳送 Erase 命令給 server
            echoWindow.receiveErase();
            //
        }
        #endregion

        #region 畫筆顏色
        private void menuItemRed_Click(object sender, RoutedEventArgs e)
        {
            inkCanvas.DefaultDrawingAttributes.Color = Colors.Red;
            foreach (MenuItem m in menuItemPenColor.Items)
                if (m != sender) m.IsChecked = false;
        }

        private void menuItemGreen_Click(object sender, RoutedEventArgs e)
        {
            inkCanvas.DefaultDrawingAttributes.Color = Colors.Green;
            foreach (MenuItem m in menuItemPenColor.Items)
                if (m != sender) m.IsChecked = false;
        }

        private void menuItemBlue_Click(object sender, RoutedEventArgs e)
        {
            inkCanvas.DefaultDrawingAttributes.Color = Colors.Blue;
            foreach (MenuItem m in menuItemPenColor.Items)
                if (m != sender) m.IsChecked = false;
        }

        private void menuItemBlack_Click(object sender, RoutedEventArgs e)
        {
            inkCanvas.DefaultDrawingAttributes.Color = Colors.Black;
            foreach (MenuItem m in menuItemPenColor.Items)
                if (m != sender) m.IsChecked = false;
        }
        #endregion

        #region 畫筆粗細
        private void menuItem1_Click(object sender, RoutedEventArgs e)
        {
            inkCanvas.DefaultDrawingAttributes.Height = 1;
            inkCanvas.DefaultDrawingAttributes.Width = 1;
            foreach (MenuItem m in menuItemPenSize.Items)
                if (m != sender) m.IsChecked = false;
        }
        private void menuItem2_Click(object sender, RoutedEventArgs e)
        {
            inkCanvas.DefaultDrawingAttributes.Height = 2;
            inkCanvas.DefaultDrawingAttributes.Width = 2;
            foreach (MenuItem m in menuItemPenSize.Items)
                if (m != sender) m.IsChecked = false;
        }
        private void menuItem3_Click(object sender, RoutedEventArgs e)
        {
            inkCanvas.DefaultDrawingAttributes.Height = 3;
            inkCanvas.DefaultDrawingAttributes.Width = 3;
            foreach (MenuItem m in menuItemPenSize.Items)
                if (m != sender) m.IsChecked = false;
        }
        private void menuItem4_Click(object sender, RoutedEventArgs e)
        {
            inkCanvas.DefaultDrawingAttributes.Height = 4;
            inkCanvas.DefaultDrawingAttributes.Width = 4;
            foreach (MenuItem m in menuItemPenSize.Items)
                if (m != sender) m.IsChecked = false;
        }
        private void menuItem5_Click(object sender, RoutedEventArgs e)
        {
            inkCanvas.DefaultDrawingAttributes.Height = 5;
            inkCanvas.DefaultDrawingAttributes.Width = 5;
            foreach (MenuItem m in menuItemPenSize.Items)
                if (m != sender) m.IsChecked = false;
        }
        #endregion

        #region 文字方塊
        private void menuItemAddText_Checked(object sender, RoutedEventArgs e)
        {
            creatingTextBox = true;
            inkCanvas.EditingMode = InkCanvasEditingMode.None;
        }

        private void menuItemAddText_Unchecked(object sender, RoutedEventArgs e)
        {
            creatingTextBox = false;
            inkCanvas.EditingMode = InkCanvasEditingMode.Ink;
        }

        private void inkCanvas_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (creatingTextBox)
            {
                e.Handled = true;
                inkCanvas.Children.Add(textBoxInCanvas);
                InkCanvas.SetLeft(textBoxInCanvas, e.GetPosition((IInputElement)sender).X);
                InkCanvas.SetTop(textBoxInCanvas, e.GetPosition((IInputElement)sender).Y);
                textBoxInCanvas.Focus();
                editingText = true;
                creatingTextBox = false;
            }
            else if (editingText && !textBoxInCanvas.IsMouseOver)
            {
                e.Handled = true;

                string text = textBoxInCanvas.Text;
                double X = InkCanvas.GetLeft(textBoxInCanvas);
                double Y = InkCanvas.GetTop(textBoxInCanvas);
                textBoxInCanvas.Text = "";
                inkCanvas.Children.Remove(textBoxInCanvas);

                Label label = new Label();
                label.Content = text;
                inkCanvas.Children.Add(label);
                InkCanvas.SetLeft(label, X);
                InkCanvas.SetTop(label, Y);

                menuItemAddText.IsChecked = false;
                editingText = false;

                // TODO: 傳送文字方塊 string text 和座標 double X, Y給 server
                echoWindow.receiveTextBox(text, X, Y);
                //
            }
        }
        #endregion

        #region 文字聊天
        private void buttonSendText_Click(object sender, RoutedEventArgs e)
        {
            string text = textBox.Text;
            textBox.Text = "";
            textBlock.Text += "我： " + text + "\n";

            // TODO: 傳送文字給 server
            echoWindow.receiveTextMessage(text);
            //
        }

        private void textBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string text = textBox.Text;
                textBox.Text = "";
                textBlock.Text += "我： " + text + "\n";

                // TODO: 傳送文字給 server
                echoWindow.receiveTextMessage(text);
                //
            }
        }
        #endregion


        #region 從server收到的命令
        public void receiveStroke(string drawingAttributesText, string stylusPointsText)
        {
            DrawingAttributes drawingAttributes = JsonConvert.DeserializeObject<DrawingAttributes>(drawingAttributesText);
            StylusPointCollection strokeCollection = JsonConvert.DeserializeObject<StylusPointCollection>(stylusPointsText);
            inkCanvas.Strokes.Add(new Stroke(strokeCollection, drawingAttributes));
        }

        public void receiveErase()
        {
            inkCanvas.Strokes.Clear();
            inkCanvas.Children.Clear();
        }

        public void receiveTextBox(string text, double X, double Y)
        {
            Label label = new Label();
            label.Content = text;
            inkCanvas.Children.Add(label);
            InkCanvas.SetLeft(label, X);
            InkCanvas.SetTop(label, Y);
        }

        public void receiveTextMessage(string text)
        {
            textBlock.Text += "他： " + text + "\n";
        }
        #endregion
    }
}
