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

        #region -- Private Fields --

        private TreeView tree;

        private XMLNode root;

        #endregion -- Private Fields --

        #region -- Getter --

        public TreeView GetTree() {
            return tree;
        }

        #endregion -- Getter --

        #region -- Constructor --

        public XMLTree() {
        }

        #endregion -- Constructor --

        #region -- Public Methods --

        public virtual void Prepare(TreeView arg) {
            tree = arg;
            PrepareMouseGesture(arg);
            PrepareContextMenu(arg);
        }

        public virtual void Tree(NodeEntity arg) {
            if (tree.Items != null) {
                tree.Items.Clear();
            }
            root = new XMLNode();
            root.SetNode(arg);
            root.SetTree(this);
            root.Tree();
            tree.Items.Add(root);
        }

        public void Refresh(NodeEntity arg) {
            TreeViewItem oldRoot = root;
            Tree(arg);
            XMLNode oldNode = oldRoot as XMLNode;
            XMLNode newNode = root as XMLNode;
            Refresh(oldNode, newNode);
        }

        #endregion -- Public Methods --

        #region -- Private Methods --

        private void Refresh(XMLNode oldNode, XMLNode newNode) {
            if (oldNode.IsExpanded) {
                newNode.IsExpanded = true;
                int count = oldNode.Items.Count;
                if (newNode.Items.Count == 0) {
                    return;
                }

                for (int i = 0; i < count; i++) {
                    XMLNode oldChild = oldNode.Items[i] as XMLNode;
                    if (newNode.Items.Count < i) {
                        return;
                    }

                    XMLNode newChild = newNode.Items[i] as XMLNode;
                    if (oldChild != null && newChild != null) {
                        Refresh(oldChild, newChild);
                    }
                }
            }
        }

        #endregion -- Private Methods --

        #region -- Mouse Gesture --

        private void PrepareMouseGesture(TreeView arg) {
            arg.AllowDrop = true;
            arg.PreviewMouseLeftButtonDown += OnPreviewLeftDown;
            arg.MouseMove += OnMove;
        }

        private System.Windows.Point p;

        private void OnPreviewLeftDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            try {
                p = e.GetPosition(tree);
            } catch (System.Exception ex) {
                System.Console.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        private void OnMove(object sender, System.Windows.Input.MouseEventArgs e) {
            try {
                if (e.LeftButton == System.Windows.Input.MouseButtonState.Released) {
                    return;
                }

                System.Windows.Point senderPos = e.GetPosition(tree);
                double moveX = p.X - senderPos.X;
                double moveY = p.Y - senderPos.Y;
                if (moveX < 0) {
                    moveX = moveX * -1;
                }
                if (moveY < 0) {
                    moveY = moveY * -1;
                }
                if (moveY / System.Math.Sin(System.Math.Atan(moveY / moveX)) > 10) {
                    System.Windows.DragDrop.DoDragDrop(tree, tree.SelectedItem, System.Windows.DragDropEffects.Move);
                }
            } catch (System.Exception ex) {
                System.Console.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        #endregion -- Mouse Gesture --

        #region -- Context Menu --

        private void PrepareContextMenu(TreeView arg) {
            ContextMenu m = new ContextMenu();

            MenuItem add1 = new MenuItem();
            add1.Name = @"Context_Upward";
            add1.Header = @"Upward";
            add1.Click += Upward_Click;
            m.Items.Add(add1);

            MenuItem add2 = new MenuItem();
            add2.Name = @"Context_Downward";
            add2.Header = @"Downward";
            add2.Click += Downward_Click;
            m.Items.Add(add2);

            arg.ContextMenu = m;
        }

        private void Upward_Click(object sender, System.Windows.RoutedEventArgs e) {
            try {
                XMLNode n = tree.SelectedItem as XMLNode;
                if (n == null) {
                    return;
                }

                NodeEntity newRoot = root.GetNode().Clone();
                newRoot.MoveUpByID(n.GetNode().GetNodeID());
                Refresh(newRoot);
            } catch (System.Exception ex) {
                System.Console.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        private void Downward_Click(object sender, System.Windows.RoutedEventArgs e) {
            try {
                XMLNode n = tree.SelectedItem as XMLNode;
                if (n == null) {
                    return;
                }

                NodeEntity newRoot = root.GetNode().Clone();
                newRoot.MoveDownByID(n.GetNode().GetNodeID());
                Refresh(newRoot);
            } catch (System.Exception ex) {
                System.Console.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        #endregion -- Context Menu --
    }
}