﻿/*
*
* NewAttribute.cs
*
* Copyright 2018 Yuichi Yoshii
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

using System;
using System.Windows;

namespace XmlTreeView {

    /// <summary>
    /// NewAttribute.xaml の相互作用ロジック
    /// </summary>
    public partial class NewAttribute : Window {

        #region -- Private Fields --

        private string attrName;

        private string attrValue;

        private bool addAttribute;

        #endregion -- Private Fields --

        #region -- Getter --

        public string GetAttrName() {
            return attrName;
        }

        public string GetAttrValue() {
            return attrValue;
        }

        public bool AddAttribute() {
            return addAttribute;
        }

        #endregion -- Getter --

        public NewAttribute() {
            InitializeComponent();

            attrName = string.Empty;
            attrValue = string.Empty;
            addAttribute = false;

            Cue.Click += Cue_Click;
        }

        #region -- Events --

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

        #endregion -- Events --
    }
}