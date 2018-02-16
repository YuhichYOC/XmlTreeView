using System;
using System.Windows;

namespace XmlTreeView {

    /// <summary>
    /// NewAttribute.xaml の相互作用ロジック
    /// </summary>
    public partial class NewAttribute : Window {
        private string attrName;

        private string attrValue;

        private bool addAttribute;

        public string GetAttrName() {
            return attrName;
        }

        public string GetAttrValue() {
            return attrValue;
        }

        public bool AddAttribute() {
            return addAttribute;
        }

        public NewAttribute() {
            InitializeComponent();

            attrName = string.Empty;
            attrValue = string.Empty;
            addAttribute = false;

            Cue.Click += Cue_Click;
        }

        private void Cue_Click(object sender, RoutedEventArgs e) {
            try {
                attrName = AttrName.Text;
                attrValue = AttrValue.Text;
                addAttribute = true;
                Close();
            } catch (Exception ex) {
                Console.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
            }
        }
    }
}