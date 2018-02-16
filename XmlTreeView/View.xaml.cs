/*
*
* View.cs
*
* Copyright 2017 Yuichi Yoshii
*     吉井雄一 @ 吉井産業  you.65535.kir@gmail.com
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*     http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*
*/

using Grid;
using OpenFile;
using SaveFile;
using SAXWrapper;
using System;
using System.Windows;
using System.Windows.Controls;
using Tree;

namespace XmlTreeView {

    /// <summary>
    /// View.xaml の相互作用ロジック
    /// </summary>
    public partial class View : Window {

        public View() {
            InitializeComponent();

            Prepare();
        }

        private OperatorEx gop;

        private XMLTreeEx top;

        private void Prepare() {
            gop = new OperatorEx();
            gop.Prepare(grid);
            top = new XMLTreeEx();
            top.Prepare(tree);
            top.SetGrid(gop);

            DropHere.AllowDrop = true;
            DropHere.PreviewDragOver += OnPreviewDragOver;
            DropHere.Drop += OnDrop;

            menu.Click += Menu;
            write.Click += Write;
        }

        private void OnDrop(object sender, DragEventArgs e) {
            try {
                string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
                if (files != null) {
                    OpenFile(files[0]);
                }
            } catch (Exception ex) {
                Console.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        private void OnPreviewDragOver(object sender, DragEventArgs e) {
            try {
                if (e.Data.GetDataPresent(DataFormats.FileDrop, true)) {
                    e.Effects = DragDropEffects.Copy;
                } else {
                    e.Effects = DragDropEffects.None;
                }
                e.Handled = true;
            } catch (Exception ex) {
                Console.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        private void Menu(object sender, RoutedEventArgs e) {
            try {
                OpenFile();
            } catch (Exception ex) {
                Console.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        private void Write(object sender, RoutedEventArgs e) {
            try {
                WriteFile();
            } catch (Exception ex) {
                Console.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        #region -- Private --

        private void OpenFile() {
            OFWindow w = new OFWindow();
            w.ShowDialog();
            string path = w.GetPath();
            XReader reader = new XReader();
            reader.SetDirectory(System.IO.Path.GetDirectoryName(path));
            reader.SetFileName(System.IO.Path.GetFileName(path));
            reader.Parse();
            top.Tree(reader.GetNode());
        }

        private void OpenFile(string arg) {
            XReader reader = new XReader();
            reader.SetDirectory(System.IO.Path.GetDirectoryName(arg));
            reader.SetFileName(System.IO.Path.GetFileName(arg));
            reader.Parse();
            top.Tree(reader.GetNode());
        }

        private void WriteFile() {
            SFWindow pw = new SFWindow();
            pw.ShowDialog();
            string path = pw.GetPath();
            OFWindow sw = new OFWindow();
            sw.ShowDialog();
            string setting = sw.GetPath();
            XWriter writer = new XWriter();
            writer.SetNode((top.GetTree().Items[0] as XMLNode).Restore());
            writer.SetDirectory(System.IO.Path.GetDirectoryName(path));
            writer.SetFileName(System.IO.Path.GetFileName(path));
            writer.SetWriterSetting(setting);
            writer.Write();
        }

        #endregion -- Private --

        #region -- Private Class : OperatorEx --

        private class OperatorEx : Operator {
            private NodeEntity current = null;

            public override void Prepare(DataGrid arg) {
                arg.CellEditEnding += EndEdit;

                base.Prepare(arg);
                PrepareContextMenu(arg);
                AddColumn(@"Name", @"属性", 1.0D, true);
                AddColumn(@"Value", @"値", 2.0D, true);
                CreateColumns();
            }

            private void EndEdit(object sender, DataGridCellEditEndingEventArgs e) {
                if (current == null) { return; }
                if (e.EditAction == DataGridEditAction.Cancel) { return; }
                if (e.Column.DisplayIndex == 0) { return; }
                object title = Value(@"Name");
                object v = Value(@"Value");
                if (title == null || v == null) { return; }
                if ((title as string).Equals(@"Node Value")) {
                    current.SetNodeValue(v as string);
                    return;
                }
                foreach (AttributeEntity a in current.GetAttrList()) {
                    if (a.NameEquals(title as string)) {
                        a.SetAttrValue(v as string);
                        return;
                    }
                }
            }

            public void DisplayNode(NodeEntity arg) {
                Blank();
                current = arg;
                foreach (AttributeEntity a in arg.GetAttrList()) {
                    RowEntity add = new RowEntity();
                    add.TrySetMember(Column(0).GetBindName(), a.GetAttrName());
                    add.TrySetMember(Column(1).GetBindName(), a.GetAttrValue());
                    AddRow(add);
                }
                RowEntity nodeValue = new RowEntity();
                nodeValue.TrySetMember(Column(0).GetBindName(), @"Node Value");
                nodeValue.TrySetMember(Column(1).GetBindName(), arg.GetNodeValue());
                AddRow(nodeValue);
                Refresh();
            }

            private void PrepareContextMenu(DataGrid arg) {
                ContextMenu m = new ContextMenu();

                MenuItem add1 = new MenuItem();
                add1.Name = @"Context_Add";
                add1.Header = @"Add attribute";
                add1.Click += Add_Click;
                m.Items.Add(add1);

                MenuItem add2 = new MenuItem();
                add2.Name = @"Context_Delete";
                add2.Header = @"Delete attribute";
                add2.Click += Delete_Click;
                m.Items.Add(add2);

                arg.ContextMenu = m;
            }

            private void Add_Click(object sender, RoutedEventArgs e) {
                try {
                    AddNewAttribute();
                } catch (Exception ex) {
                    Console.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
                }
            }

            private void AddNewAttribute() {
                NewAttribute n = new NewAttribute();
                n.ShowDialog();
                if (n.AddAttribute()) {
                    RowEntity addRow = new RowEntity();
                    addRow.TrySetMember(Column(0).GetBindName(), n.GetAttrName());
                    addRow.TrySetMember(Column(1).GetBindName(), n.GetAttrValue());
                    AddRow(addRow);
                    AttributeEntity addAttr = new AttributeEntity();
                    addAttr.SetAttrName(n.GetAttrName());
                    addAttr.SetAttrValue(n.GetAttrValue());
                    current.AddAttr(addAttr);
                }
            }

            private void Delete_Click(object sender, RoutedEventArgs e) {
                try {
                    DeleteAttribute();
                } catch (Exception ex) {
                    Console.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
                }
            }

            private void DeleteAttribute() {
                object attrName = Value(@"Name");
                if (attrName == null) { return; }
                current.RemoveAttrByName(attrName as string);
                RemoveRow((int)Value(@"index"));
            }
        }

        #endregion -- Private Class : OperatorEx --

        #region -- Private Class : XMLTreeEx --

        private class XMLTreeEx : XMLTree {
            private OperatorEx grid;

            public void SetGrid(OperatorEx arg) {
                grid = arg;
            }

            public override void Tree(NodeEntity arg) {
                base.Tree(arg);
                GetTree().SelectedItemChanged += OnSelect;
            }

            private void OnSelect(object sender, RoutedEventArgs e) {
                try {
                    TreeView senderObj = sender as TreeView;
                    XMLNode senderNode = senderObj.SelectedItem as XMLNode;
                    if (senderNode == null) { return; }
                    NodeEntity arg = senderNode.Tag as NodeEntity;
                    if (arg == null) { return; }
                    grid.DisplayNode(arg);
                } catch (Exception ex) {
                    Console.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
                }
            }
        }

        #endregion -- Private Class : XMLTreeEx --
    }
}