/*
*
* FileSystemNode.cs
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

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Tree {

    public class FileSystemNode : TreeViewItem {

        #region -- Fields --

        private string name;

        private string path;

        #endregion -- Fields --

        #region -- Getter, Setter --

        public void SetName(string arg) {
            Header = arg;
            name = arg;
        }

        public string GetName() {
            return name;
        }

        public void SetPath(string arg) {
            path = arg;
        }

        public string GetPath() {
            return path;
        }

        #endregion -- Getter, Setter --

        #region -- Constructor --

        public FileSystemNode() {
        }

        #endregion -- Constructor --

        #region -- Events --

        private void OnExpand(object sender, RoutedEventArgs e) {
            try {
                FileSystemNode senderObj = sender as FileSystemNode;
                AppendTree(senderObj);
            } catch (Exception ex) {
                Console.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        #endregion -- Events --

        #region -- Public --

        public void Tree() {
            IEnumerable<string> subDir = TryGetDirectories(path);
            if (subDir == null)
                return;
            foreach (string d in subDir) {
                AddChild(System.IO.Path.GetFileName(d), d);
                Tree(Find(d), d);
            }
        }

        public void AppendTree(FileSystemNode arg) {
            IEnumerable<string> subDir = TryGetDirectories(arg.GetPath());
            if (subDir == null)
                return;
            foreach (string d in subDir) {
                FileSystemNode child = Find(arg, d);
                if (child != null) {
                    AppendTree(child, d);
                }
            }
        }

        public bool ChildExists(string otherPath) {
            foreach (FileSystemNode child in Items) {
                if (child.Equals(otherPath)) {
                    return true;
                }
            }
            return false;
        }

        public void AddChild(string newName, string newPath) {
            FileSystemNode add = new FileSystemNode();
            add.SetName(newName);
            add.SetPath(newPath);
            add.Expanded += OnExpand;
            Items.Add(add);
        }

        public bool Equals(string otherPath) {
            if (path.Equals(otherPath)) {
                return true;
            } else {
                return false;
            }
        }

        #endregion -- Public --

        #region -- Private --

        private void Tree(FileSystemNode arg, string path) {
            IEnumerable<string> subDir = TryGetDirectories(path);
            if (subDir == null)
                return;
            foreach (string d in subDir) {
                arg.AddChild(System.IO.Path.GetFileName(d), d);
            }
        }

        private void AppendTree(FileSystemNode arg, string path) {
            IEnumerable<string> subDir = TryGetDirectories(path);
            if (subDir == null)
                return;
            foreach (string d in subDir) {
                if (!arg.ChildExists(d)) {
                    arg.AddChild(System.IO.Path.GetFileName(d), d);
                }
            }
        }

        private IEnumerable<string> TryGetDirectories(string path) {
            try {
                return System.IO.Directory.EnumerateDirectories(path);
            } catch (UnauthorizedAccessException) {
                return null;
            }
        }

        private FileSystemNode Find(string otherPath) {
            foreach (FileSystemNode child in Items) {
                if (child.Equals(otherPath)) {
                    return child;
                }
            }
            return null;
        }

        private FileSystemNode Find(FileSystemNode arg, string otherPath) {
            foreach (FileSystemNode child in arg.Items) {
                if (child.Equals(otherPath)) {
                    return child;
                }
            }
            return null;
        }

        #endregion -- Private --
    }
}