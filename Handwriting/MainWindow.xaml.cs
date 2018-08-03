using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WF = System.Windows.Forms;
namespace Handwriting
{
   
    public partial class MainWindow : Window
    {
        private String OpenFile = null;
        private String OpenSettings = null;
        private Drawer drawer = new Drawer();

        private Dictionary<String, TextBox> SettingsNameMapping = new Dictionary<String, TextBox>();
        public MainWindow()
        {
            InitializeComponent();
            {
                void m(String name,TextBox box)
                {
                    SettingsNameMapping[name] = box;
                }

                m("PenSize", this.PenSize);
                m("FontSize", this.FontSize);
                m("FontColor", this.FontColor);

                m("WordSpacing", this.WordSpacing);
                m("LineSpacing", this.LineSpacing);

                m("WordOffsets", this.WordOffset);
                m("LineOffsets", this.LineOffset);
                m("SpacingOffsets", this.SpacingOffset);
                m("PenSizeOffsets", this.PenSizeOffset);

                m("PaperWidth", this.PaperWidth);
                m("PaperHeight", this.PaperHeight);
                m("PaperTop", this.PaperTop);
                m("PaperLeft", this.PaperLeft);
                m("PaperRight", this.PaperRight);
                m("PaperBottom", this.PaperBottom);

                m("WrongChar", this.WrongCharProbability);

            
            }
        }

        private void ReadSettings(String path)
        {
            var doc = new XmlDocument();
            doc.Load(path);
            var nodes = doc.SelectSingleNode("Handwriting").ChildNodes;
            foreach(XmlNode node in nodes)
            {
                if(node.Name == "EnabledWrong")
                {
                    this.WrongCharCheckBox.IsChecked = node.InnerText == "True";
                } else
                {
                    if (SettingsNameMapping.ContainsKey(node.Name))
                    {
                        SettingsNameMapping[node.Name].Text = node.InnerText;
                    }
                }
            }
        }

        /// <summary>
        /// 保存设置
        /// </summary>
        /// <param name="path">XML 路径</param>
        private void SaveSettings(String path)
        {
            var doc = new XmlDocument();
            var root = doc.CreateElement("Handwriting");
            doc.AppendChild(root);
            void ele(String name,String value)
            {
                var element = doc.CreateElement(name);
                element.InnerText = value;
                root.AppendChild(element);
            }
         
            
            ele("EnabledWrong", this.WrongCharCheckBox.IsChecked.ToString());
            foreach(var node in SettingsNameMapping)
            {
                ele(node.Key, node.Value.Text);
            }

            doc.Save(path);
        }
        /// <summary>
        /// 菜单事件 - 读取文本
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_OpenTextFile(object sender, RoutedEventArgs e)
        {
            var dialog = new WF.OpenFileDialog();
            dialog.Filter = "Text Documents (*.txt)|*.txt";
            dialog.RestoreDirectory = true;
            if(dialog.ShowDialog() == WF.DialogResult.OK)
            {
                var path = dialog.FileName;
                TextInputBox.Text = File.ReadAllText(path);
                this.OpenFile = path;
            }
        }
        /// <summary>
        /// 菜单事件 - 保存文本
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_SaveTextFile(object sender, RoutedEventArgs e)
        {
            if(this.OpenFile == null)
            {
                var dialog = new WF.SaveFileDialog();
                dialog.Filter = "Text Documents (*.txt)|*.txt";
                dialog.RestoreDirectory = true;
                if (dialog.ShowDialog() == WF.DialogResult.OK)
                {
                    this.OpenFile = dialog.FileName;
                }
                else return;
            }
            File.WriteAllText(this.OpenFile, this.TextInputBox.Text);
        }
        /// <summary>
        /// 菜单事件 - 文本另存为
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_SaveTextFileAs(object sender, RoutedEventArgs e)
        {
            this.OpenFile = null;
            this.MenuItem_SaveTextFile(sender, e);
        }
        /// <summary>
        /// 菜单事件 - 打开设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_OpenSettings(object sender, RoutedEventArgs e)
        {
            var dialog = new WF.OpenFileDialog();
            dialog.Filter = "XML File (*.xml)|*.xml";
            dialog.RestoreDirectory = true;
            if (dialog.ShowDialog() == WF.DialogResult.OK)
            {
                var path = dialog.FileName;
                this.ReadSettings(path);
                this.OpenSettings = path;
            }
        }
        /// <summary>
        /// 菜单事件 - 保存设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_SaveSettings(object sender, RoutedEventArgs e)
        {
            if (this.OpenSettings == null)
            {
                var dialog = new WF.SaveFileDialog();
                dialog.Filter = "XML file (*.xml)|*.xml";
                dialog.RestoreDirectory = true;
                if (dialog.ShowDialog() == WF.DialogResult.OK)
                {
                    this.OpenSettings = dialog.FileName;
                }
                else return;
            }
            this.SaveSettings(this.OpenSettings);
        }
        /// <summary>
        /// 菜单事件 - 设置另存为
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_SaveSettingsAs(object sender, RoutedEventArgs e)
        {
            this.OpenSettings = null;
            this.MenuItem_SaveSettings(sender, e);
        }
        /// <summary>
        /// 菜单事件 - 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// 菜单事件 - 关于
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Feedback(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/mslxl/Handwriting/issues/new");
        }
        /// <summary>
        /// 菜单事件 - 构建图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_BuildImage(object sender, RoutedEventArgs e)
        {
            var drawer = new Drawer();
            drawer.SetMargin(int.Parse(PaperLeft.Text),int.Parse(PaperRight.Text),int.Parse(PaperTop.Text),int.Parse(PaperBottom.Text));
            drawer.SetSpacing(int.Parse(LineSpacing.Text), int.Parse(WordSpacing.Text));
            drawer.SetPen(int.Parse(FontSize.Text), int.Parse(PenSize.Text));
            drawer.CreatePaperTemplate(int.Parse(PaperWidth.Text), int.Parse(PaperHeight.Text));
            drawer.InitAllRoutes(TextInputBox.Text);
            var canvas = drawer.Draw();
            int index = 0;
            foreach(var item in canvas)
            {
                var tab = new TabItem();
                tab.Header = string.Format("Page {0}", ++index);
                tab.Content = item;

                TabPanel.Items.Add(tab);
            }
        }
    }
}
