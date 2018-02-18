/*
*
* OFWindow.cs
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Tree;

namespace OpenFile {

    /// <summary>
    /// OFWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class OFWindow : Window {

        #region -- Private Fields --

        private string path;

        private bool selected;

        private OperatorEx gop;

        private FileSystemTreeEx top;

        #endregion -- Private Fields --

        #region -- Getter --

        public string GetPath() {
            return path;
        }

        public bool Selected() {
            return selected;
        }

        #endregion -- Getter --

        public OFWindow() {
            InitializeComponent();

            Prepare();
        }

        #region -- Events --

        private void DriveChanged(object sender, SelectionChangedEventArgs e) {
            try {
                top = new FileSystemTreeEx();
                top.Prepare(tree);
                top.SetGrid(gop);
                top.Tree((sender as ComboBox).SelectedValue.ToString());
            } catch (Exception ex) {
                Console.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        private void CellChanged(object sender, SelectedCellsChangedEventArgs e) {
            try {
                DataGrid senderObj = sender as DataGrid;
                RowEntity senderRow = senderObj.SelectedItem as RowEntity;
                object path = null;
                if (senderRow.TryGetMember(@"FileName", out path))
                    text.Text = path as string;
            } catch (Exception ex) {
                Console.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        private void ButtonClick(object sender, RoutedEventArgs e) {
            try {
                path = text.Text;
                selected = true;
                Close();
            } catch (Exception ex) {
                Console.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        #endregion -- Events --

        #region -- Private Methods --

        private void Prepare() {
            ListDrives();
            gop = new OperatorEx();
            gop.Prepare(grid);
            gop.AddColumn(@"FileName", @"ファイル名");
            gop.CreateColumns();
            top = new FileSystemTreeEx();
            top.Prepare(tree);
            top.SetGrid(gop);
            top.Tree(@"C:\");

            drive.SelectionChanged += DriveChanged;
            grid.SelectedCellsChanged += CellChanged;
            button.Click += ButtonClick;

            path = string.Empty;
            selected = false;
        }

        private void ListDrives() {
            Environment.GetLogicalDrives().ToList().ForEach(v => {
                drive.Items.Add(v);
            });
        }

        #endregion -- Private Methods --

        #region -- Private Class : OperatorEx --

        private class OperatorEx : Operator {

            #region -- Public Methods --

            public void DisplayDirectory(FileSystemNode arg) {
                Blank();
                IEnumerable<string> files = TryGetFiles(arg.GetPath());
                if (files == null)
                    return;
                foreach (string item in files) {
                    RowEntity add = new RowEntity();
                    add.TrySetMember(Column(0).GetBindName(), item);
                    AddRow(add);
                }
                Refresh();
            }

            #endregion -- Public Methods --

            #region -- Private Methods --

            private IEnumerable<string> TryGetFiles(string path) {
                try {
                    return System.IO.Directory.EnumerateFiles(path);
                } catch (UnauthorizedAccessException) {
                    return null;
                }
            }

            #endregion -- Private Methods --
        }

        #endregion -- Private Class : OperatorEx --

        #region -- Private Class : FileSystemTreeEx --

        private class FileSystemTreeEx : FileSystemTree {

            #region -- Private Fields --

            private OperatorEx grid;

            #endregion -- Private Fields --

            #region -- Setter --

            public void SetGrid(OperatorEx arg) {
                grid = arg;
            }

            #endregion -- Setter --

            #region -- Events --

            private void OnSelect(object sender, RoutedEventArgs e) {
                try {
                    TreeView senderObj = sender as TreeView;
                    FileSystemNode item = senderObj.SelectedItem as FileSystemNode;
                    grid.DisplayDirectory(item);
                } catch (Exception ex) {
                    Console.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
                }
            }

            #endregion -- Events --

            #region -- Public Methods --

            public new void Tree(string path) {
                base.Tree(path);
                GetTree().SelectedItemChanged += OnSelect;
            }

            #endregion -- Public Methods --
        }

        #endregion -- Private Class : FileSystemTreeEx --
    }
}