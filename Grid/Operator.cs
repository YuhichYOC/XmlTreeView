/*
*
* Operator.cs
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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace Grid {

    public class Operator {

        #region -- Fields --

        private DataGrid grid;

        private List<ColumnDefinition> columns;

        private ObservableCollection<RowEntity> rows;

        #endregion -- Fields --

        #region -- Constructor --

        public Operator() {
        }

        #endregion -- Constructor --

        #region -- Public --

        public virtual void Prepare(DataGrid arg) {
            grid = arg;
        }

        public void AddColumn(string bindName, string title) {
            if (columns == null) {
                columns = new List<ColumnDefinition>();
            }
            ColumnDefinition add = new ColumnDefinition();
            add.SetBindName(bindName);
            add.SetTitle(title);
            columns.Add(add);
        }

        public void AddColumn(string bindName, string title, double width, bool star) {
            if (columns == null) {
                columns = new List<ColumnDefinition>();
            }
            ColumnDefinition add = new ColumnDefinition();
            add.SetBindName(bindName);
            add.SetTitle(title);
            add.SetWidth(width, star);
            columns.Add(add);
        }

        public void CreateColumns() {
            grid.CanUserAddRows = false;
            grid.Columns.Clear();
            DataGridTextColumn index = new DataGridTextColumn();
            index.Header = string.Empty;
            index.Binding = new System.Windows.Data.Binding(@"index");
            index.Width = new DataGridLength(0.0D, DataGridLengthUnitType.Pixel);
            index.Visibility = System.Windows.Visibility.Hidden;
            grid.Columns.Add(index);
            foreach (ColumnDefinition item in columns) {
                item.AddColumn(grid);
            }
        }

        public void Refresh() {
            grid.ItemsSource = rows;
        }

        public void Blank() {
            rows = new ObservableCollection<RowEntity>();
            grid.ItemsSource = rows;
        }

        public object Value(string bindName) {
            object ret = null;
            (grid.CurrentItem as RowEntity).TryGetMember(bindName, out ret);
            return ret;
        }

        #endregion -- Public --

        #region -- Protected --

        protected void AddRow(RowEntity arg) {
            arg.TrySetMember(@"index", LastIndex() + 1);
            rows.Add(arg);
        }

        protected void RemoveRow(int index) {
            ObservableCollection<RowEntity> newRows = new ObservableCollection<RowEntity>();
            foreach (RowEntity r in rows) {
                object i = null;
                r.TryGetMember(@"index", out i);
                if (i != null) {
                    if (index != (int)i) {
                        newRows.Add(r.Clone());
                    }
                }
            }
            rows = newRows;
            Refresh();
        }

        protected ColumnDefinition Column(int i) {
            return columns[i];
        }

        protected int LastIndex() {
            int ret = -1;
            foreach (RowEntity r in rows) {
                object i = null;
                r.TryGetMember(@"index", out i);
                if (i != null) {
                    if (ret < (int)i) {
                        ret = (int)i;
                    }
                }
            }
            return ret;
        }

        #endregion -- Protected --
    }
}