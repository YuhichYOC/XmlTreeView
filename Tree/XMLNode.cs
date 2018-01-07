/*
*
* XMLNode.cs
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

    public class XMLNode : TreeViewItem {

        #region -- Fields --

        private NodeEntity node;

        #endregion -- Fields --

        #region -- Getter, Setter --

        public void SetNode(NodeEntity arg) {
            node = arg;
        }

        public NodeEntity GetNode() {
            return node;
        }

        #endregion -- Getter, Setter --

        #region -- Constructor --

        public XMLNode() {
        }

        #endregion -- Constructor --

        #region -- Public --

        public void Tree() {
            Header = node.GetNodeName();
            Name = node.GetNodeName();
            Tag = node.CloneWithoutChildren();
            foreach (NodeEntity item in node.GetChildren()) {
                Tree(this, item);
            }
        }

        #endregion -- Public --

        #region -- Private --

        private void Tree(XMLNode arg1, NodeEntity arg2) {
            XMLNode add = new XMLNode();
            add.Header = arg2.GetNodeName();
            add.Name = arg2.GetNodeName();
            add.Tag = arg2.CloneWithoutChildren();
            foreach (NodeEntity item in arg2.GetChildren()) {
                Tree(add, item);
            }
            arg1.Items.Add(add);
        }

        #endregion -- Private --
    }
}