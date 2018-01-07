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

        public void Prepare(DataGrid arg) {
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

        public void CreateColumns() {
            grid.CanUserAddRows = false;
            grid.Columns.Clear();
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

        #endregion -- Public --

        #region -- Protected --

        protected void AddRow(RowEntity arg) {
            rows.Add(arg);
        }

        protected ColumnDefinition Column(int i) {
            return columns[i];
        }

        #endregion -- Protected --
    }
}