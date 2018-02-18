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

        #region -- Private Fields --

        private NodeEntity node;

        private XMLNode parent;

        private XMLTree tree;

        #endregion -- Private Fields --

        #region -- Setter --

        public void SetNode(NodeEntity arg) {
            node = arg;
        }

        public void SetParent(XMLNode arg) {
            parent = arg;
        }

        public void SetTree(XMLTree arg) {
            tree = arg;
        }

        #endregion -- Setter --

        #region -- Getter --

        public NodeEntity GetNode() {
            return node;
        }

        public XMLNode Root() {
            if (parent == null) {
                return this;
            }
            return parent.Root();
        }

        #endregion -- Getter --

        #region -- Constructor --

        public XMLNode() {
            node = null;
            parent = null;
            tree = null;
        }

        #endregion -- Constructor --

        #region -- Public Methods --

        public void Tree() {
            if (Items != null) {
                Items.Clear();
            }
            Header = node.GetNodeName();
            Name = @"Node" + node.GetNodeID().ToString();
            PrepareMouseGesture();
            foreach (NodeEntity item in node.GetChildren()) {
                Tree(this, item);
            }
        }

        #endregion -- Public Methods --

        #region -- Private Methods --

        private void Tree(XMLNode arg1, NodeEntity arg2) {
            XMLNode add = new XMLNode();
            add.Header = arg2.GetNodeName();
            add.Name = @"Node" + arg2.GetNodeID().ToString();
            add.SetNode(arg2);
            add.SetTree(tree);
            add.PrepareMouseGesture();
            foreach (NodeEntity item in arg2.GetChildren()) {
                Tree(add, item);
            }
            arg1.Items.Add(add);
        }

        #endregion -- Private Methods --

        #region -- Mouse Gesture --

        private void PrepareMouseGesture() {
            AllowDrop = true;
            Drop += OnDrop;
        }

        private void OnDrop(object sender, System.Windows.DragEventArgs e) {
            try {
                e.Handled = true;
                e.Effects = System.Windows.DragDropEffects.None;
                XMLNode dropNode = e.Data.GetData(typeof(XMLNode)) as XMLNode;
                XMLNode catchNode = sender as XMLNode;
                if (dropNode == catchNode) {
                    return;
                }

                node.MoveByID(dropNode.GetNode().GetNodeID(), catchNode.GetNode().GetNodeID());
                tree.Refresh(node.Root());
            } catch (System.Exception ex) {
                System.Console.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        #endregion -- Mouse Gesture --
    }
}