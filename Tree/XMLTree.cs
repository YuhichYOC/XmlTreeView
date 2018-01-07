/*
*
* XMLTree.cs
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

using SAXWrapper;
using System.Windows.Controls;

namespace Tree {

    public class XMLTree {

        #region -- Fields --

        private TreeView tree;

        private XMLNode root;

        #endregion -- Fields --

        #region -- Getter, Setter --

        public TreeView GetTree() {
            return tree;
        }

        #endregion -- Getter, Setter --

        #region -- Constructor --

        public XMLTree() {
        }

        #endregion -- Constructor --

        #region -- Public --

        public void Prepare(TreeView arg) {
            tree = arg;
        }

        public void Tree(NodeEntity arg) {
            root = new XMLNode();
            root.SetNode(arg);
            root.Tree();
            tree.Items.Add(root);
        }

        #endregion -- Public --
    }
}