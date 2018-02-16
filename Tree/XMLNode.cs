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

        private XMLTree parent;

        #endregion -- Fields --

        #region -- Getter, Setter --

        public void SetNode(NodeEntity arg) {
            node = arg;
        }

        public NodeEntity GetNode() {
            return node;
        }

        public void SetParent(XMLTree arg) {
            parent = arg;
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
            PrepareMouseGesture();
            foreach (NodeEntity item in node.GetChildren()) {
                Tree(this, item);
            }
        }

        public NodeEntity Restore() {
            NodeEntity ret = Tag as NodeEntity;
            foreach (XMLNode item in Items) {
                ret.AddChild(Restore(item));
            }
            return ret;
        }

        public NodeEntity UpwardByID(int id) {
            NodeEntity ret = Restore();
            NodeEntity parent = FindParentByID(ret, id);
            if (parent == null) { return ret; }
            parent = UpwardByID(parent, id);
            return ret;
        }

        public NodeEntity DownwardByID(int id) {
            NodeEntity ret = Restore();
            NodeEntity parent = FindParentByID(ret, id);
            if (parent == null) { return ret; }
            parent = DownwardByID(parent, id);
            return ret;
        }

        public NodeEntity MoveByEachNode(NodeEntity dropNode, NodeEntity catchNode) {
            NodeEntity ret = Restore();
            NodeEntity parent = FindParentByID(ret, catchNode.GetNodeID());
            if (parent == null || parent != FindParentByID(ret, dropNode.GetNodeID())) { return ret; }
            parent = MoveByEachNode(parent, dropNode, catchNode);
            return ret;
        }

        #endregion -- Public --

        #region -- Private --

        private void Tree(XMLNode arg1, NodeEntity arg2) {
            XMLNode add = new XMLNode();
            add.Header = arg2.GetNodeName();
            add.Name = arg2.GetNodeName();
            add.Tag = arg2.CloneWithoutChildren();
            add.SetParent(parent);
            add.PrepareMouseGesture();
            foreach (NodeEntity item in arg2.GetChildren()) {
                Tree(add, item);
            }
            arg1.Items.Add(add);
        }

        private NodeEntity Restore(XMLNode arg) {
            NodeEntity ret = arg.Tag as NodeEntity;
            foreach (XMLNode item in arg.Items) {
                ret.AddChild(Restore(item));
            }
            return ret;
        }

        private NodeEntity FindParentByID(NodeEntity arg, int id) {
            NodeEntity ret = null;
            foreach (NodeEntity child in arg.GetChildren()) {
                if (child.GetNodeID() == id) {
                    ret = arg;
                } else {
                    ret = FindParentByID(child, id);
                }
                if (ret != null) {
                    break;
                }
            }
            return ret;
        }

        private NodeEntity UpwardByID(NodeEntity arg, int id) {
            int count = arg.GetChildren().Count;
            int index = 0;
            for (index = 0; index < count; index++) {
                if (arg.GetChildren()[index].GetNodeID() == id) {
                    break;
                }
            }
            if (index == 0) {
                return arg;
            } else {
                NodeEntity up = arg.GetChildren()[index];
                arg.GetChildren().RemoveAt(index);
                arg.GetChildren().Insert(index - 1, up);
                return arg;
            }
        }

        private NodeEntity DownwardByID(NodeEntity arg, int id) {
            int count = arg.GetChildren().Count;
            int index = 0;
            for (index = 0; index < count; index++) {
                if (arg.GetChildren()[index].GetNodeID() == id) {
                    break;
                }
            }
            if (index == count - 1) {
                return arg;
            } else {
                NodeEntity down = arg.GetChildren()[index];
                arg.GetChildren().RemoveAt(index);
                arg.GetChildren().Insert(index + 1, down);
                return arg;
            }
        }

        private NodeEntity MoveByEachNode(NodeEntity parent, NodeEntity dropNode, NodeEntity catchNode) {
            int count = parent.GetChildren().Count;
            int index = 0;
            for (index = 0; index < count; index++) {
                if (parent.GetChildren()[index].GetNodeID() == catchNode.GetNodeID()) {
                    break;
                }
            }
            parent.GetChildren().Remove(dropNode);
            parent.GetChildren().Insert(index, dropNode);
            return parent;
        }

        #endregion -- Private --

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
                if (dropNode == catchNode)
                    return;

                parent.MoveByEachNode(dropNode, catchNode);
            } catch (System.Exception ex) {
                System.Console.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        #endregion -- Mouse Gesture --
    }
}