/*
*
* ColumnDefinition.cs
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

using System.Windows.Controls;

namespace Grid {

    public class ColumnDefinition {

        #region -- Fields --

        private string bindName;

        private string title;

        private double width;

        private bool star;

        #endregion -- Fields --

        #region -- Getter, Setter --

        public void SetBindName(string arg) {
            bindName = arg;
        }

        public string GetBindName() {
            return bindName;
        }

        public void SetTitle(string arg) {
            title = arg;
        }

        public void SetWidth(double arg, bool star) {
            width = arg;
            this.star = star;
        }

        #endregion -- Getter, Setter --

        #region -- Public --

        public ColumnDefinition() {
            width = 1.0D;
            star = true;
        }

        public void AddColumn(DataGrid grid) {
            DataGridTextColumn add = new DataGridTextColumn();
            add.Header = title;
            add.Binding = new System.Windows.Data.Binding(bindName);
            if (star) {
                add.Width = new DataGridLength(width, DataGridLengthUnitType.Star);
            } else {
                add.Width = new DataGridLength(width, DataGridLengthUnitType.Pixel);
            }
            grid.Columns.Add(add);
        }

        #endregion -- Public --
    }
}