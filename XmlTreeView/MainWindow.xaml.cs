using System;
using System.Windows;
using System.Windows.Controls;

namespace XmlTreeView {

    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window {

        public MainWindow() {
            InitializeComponent();

            Test01();
        }

        private void Test01() {
            Logger.SimpleLogger log = new Logger.SimpleLogger();
            try {
                SAXWrapper.SettingReader setting = new SAXWrapper.SettingReader();
                setting.SetDirectory(System.IO.Path.GetDirectoryName(@"./Setting.config"));
                setting.SetFileName(System.IO.Path.GetFileName(@"./Setting.config"));
                setting.Parse();
                TreeViewItem root = new TreeViewItem();
                root.Header = setting.GetNode().GetNodeName();
                root.Name = setting.GetNode().GetNodeName();
                Tree(root, setting.GetNode());
                treeView.Items.Add(root);
            }
            catch (Exception ex) {
                log.Error(ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        private void Tree(TreeViewItem item, SAXWrapper.NodeEntity node) {
            node.GetChildren().ForEach(c => {
                TreeViewItem add = new TreeViewItem();
                add.Header = c.GetNodeName();
                add.Name = c.GetNodeName();
                Tree(add, c);
                item.Items.Add(add);
            });
        }
    }
}