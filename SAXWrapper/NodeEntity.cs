/*
*
* NodeEntity.cs
*
* Copyright 2016 Yuichi Yoshii
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

namespace SAXWrapper {

    public class NodeEntity {

        #region -- プロパティ --

        private string nodeName;

        public void SetNodeName(string arg) {
            nodeName = arg;
        }

        public string GetNodeName() {
            return nodeName;
        }

        private int nodeId;

        public void SetNodeID(int arg) {
            nodeId = arg;
        }

        public int GetNodeID() {
            return nodeId;
        }

        private int depth;

        public void SetDepth(int arg) {
            depth = arg;
        }

        public int GetDepth() {
            return depth;
        }

        private string nodeValue;

        public void SetNodeValue(string arg) {
            nodeValue = arg;
        }

        public string GetNodeValue() {
            return nodeValue;
        }

        private bool isComment;

        public void Comment(bool arg) {
            isComment = arg;
        }

        public bool IsComment() {
            return isComment;
        }

        private List<AttributeEntity> attrList;

        public void SetAttrList(List<AttributeEntity> arg) {
            attrList = arg;
        }

        public List<AttributeEntity> GetAttrList() {
            return attrList;
        }

        public void AddAttr(AttributeEntity arg) {
            attrList.Add(arg);
        }

        private List<NodeEntity> children;

        public void SetChildren(List<NodeEntity> arg) {
            children = arg;
        }

        public List<NodeEntity> GetChildren() {
            return children;
        }

        public void AddChild(NodeEntity arg) {
            children.Add(arg);
        }

        private NodeEntity writerSetting;

        private bool newLineAfterOpeningBracket;

        private bool newLineAfterClosingBracket;

        private bool newLineAfterAttributes;

        private bool newLineAfterNodeValue;

        private int indentSize;

        public void SetWriterSetting(NodeEntity arg) {
            writerSetting = arg;
            newLineAfterOpeningBracket = NewLineAfterOpeningBracket();
            newLineAfterClosingBracket = NewLineAfterClosingBracket();
            newLineAfterAttributes = NewLineAfterAttributes();
            newLineAfterNodeValue = NewLineAfterNodeValue();
            indentSize = IndentSize();
            if (children.Count > 0) {
                children.ForEach(c => {
                    c.SetWriterSetting(arg);
                });
            }
        }

        #endregion -- プロパティ --

        public NodeEntity() {
            nodeId = 0;
            depth = 0;
            isComment = false;
            attrList = new List<AttributeEntity>();
            children = new List<NodeEntity>();
            writerSetting = null;
            newLineAfterOpeningBracket = true;
            newLineAfterClosingBracket = true;
            newLineAfterAttributes = true;
            newLineAfterNodeValue = true;
            indentSize = 2;
        }

        #region -- メソッド --

        public bool AttrExists(string name) {
            bool ret = false;
            attrList.ForEach(v => {
                if (v.NameEquals(name)) {
                    ret = true;
                }
            });
            return ret;
        }

        public string AttrByName(string name) {
            string ret = @"";
            attrList.ForEach(v => {
                if (v.NameEquals(name)) {
                    ret = v.GetAttrValue();
                }
            });
            return ret;
        }

        public void RemoveAttrByName(string name) {
            if (AttrExists(name)) {
                for (int i = 0; i < attrList.Count; i++) {
                    if (attrList[i].GetAttrName().Equals(name)) {
                        attrList.RemoveAt(i);
                        return;
                    }
                }
            }
        }

        public bool NameEquals(string name) {
            if (nodeName.Equals(name)) {
                return true;
            } else {
                return false;
            }
        }

        public NodeEntity Find(string tagName) {
            NodeEntity ret = null;
            children.ForEach(v => {
                if (v.NameEquals(tagName)) {
                    ret = v;
                }
            });
            return ret;
        }

        public NodeEntity Find(string tagName, string attrName, string attrValue) {
            NodeEntity ret = null;
            children.ForEach(v => {
                if (v.NameEquals(tagName) && v.AttrExists(attrName) && v.AttrByName(attrName).Equals(attrValue)) {
                    ret = v;
                }
            });
            return ret;
        }

        public NodeEntity Find(string tagName, string attr1Name, string attr1Value, string attr2Name, string attr2Value) {
            NodeEntity ret = null;
            children.ForEach(v => {
                if (v.NameEquals(tagName)
                 && v.AttrExists(attr1Name)
                 && v.AttrByName(attr1Name).Equals(attr1Value)
                 && v.AttrExists(attr2Name)
                 && v.AttrByName(attr2Name).Equals(attr2Value)) {
                    ret = v;
                }
            });
            return ret;
        }

        public NodeEntity FindTail(int depth) {
            NodeEntity ret = this;
            if (depth == 1) { return ret; }
            depth--;
            int count = ret.GetChildren().Count;
            return FindTail(ret.GetChildren()[count - 1], depth);
        }

        public NodeEntity Clone() {
            NodeEntity ret = new NodeEntity();

            attrList.ForEach(v => {
                ret.AddAttr(v.Clone());
            });
            children.ForEach(v => {
                ret.AddChild(v.Clone());
            });

            ret.SetNodeName(nodeName);
            ret.SetNodeID(nodeId);
            ret.SetNodeValue(nodeValue);

            return ret;
        }

        public NodeEntity CloneWithoutChildren() {
            NodeEntity ret = new NodeEntity();

            attrList.ForEach(v => {
                ret.AddAttr(v.Clone());
            });

            ret.SetNodeName(nodeName);
            ret.SetNodeID(nodeId);
            ret.SetDepth(depth);
            ret.SetNodeValue(nodeValue);
            ret.Comment(isComment);

            return ret;
        }

        public override string ToString() {
            string ret = string.Empty;
            if (IsComment()) {
                ret += ToStringComment();
            } else if (children.Count > 0) {
                ret += ToStringStart();
                if (newLineAfterClosingBracket) {
                    ret += "\r\n";
                }
                foreach (NodeEntity item in children) {
                    ret += item.ToString() + "\r\n";
                }
                ret += ToStringEnd();
            } else if (nodeValue != null && !nodeValue.Equals(string.Empty)) {
                ret += ToStringStart();
                if (newLineAfterClosingBracket) {
                    ret += "\r\n";
                }
                ret += Indent(1) + nodeValue;
                if (newLineAfterNodeValue) {
                    ret += "\r\n";
                }
                ret += ToStringEnd();
            } else {
                ret += ToStringEmpty();
            }
            return ret;
        }

        #region -- Derivative Find --

        /// <summary>
        /// 自分自身の子ノードから type が Dir かつ name が引数に一致する最初のノードを返す
        /// </summary>
        /// <param name="name">
        /// 取得したいタグの name 属性値</param>
        /// <returns>
        /// ノード</returns>
        public NodeEntity Dir(string name) {
            return Find(@"Item", @"type", @"Dir", @"name", name);
        }

        /// <summary>
        /// 自分自身の子ノードから type が File かつ name が引数に一致する最初のノードを返す
        /// </summary>
        /// <param name="name">
        /// 取得したいタグの name 属性値</param>
        /// <returns>
        /// ノード</returns>
        public NodeEntity File(string name) {
            return Find(@"Item", @"type", @"File", @"name", name);
        }

        /// <summary>
        /// 自分自身の子ノードから type が Tag かつ name が引数に一致する最初のノードを返す
        /// </summary>
        /// <param name="name">
        /// 取得したいタグの name 属性値</param>
        /// <returns>
        /// ノード</returns>
        public NodeEntity Tag(string name) {
            return Find(@"Item", @"type", @"Tag", @"name", name);
        }

        /// <summary>
        /// 自分自身の子ノードから type が Attr かつ name が引数に一致する最初のノードを返す
        /// </summary>
        /// <param name="name">
        /// 取得したいタグの name 属性値</param>
        /// <returns>
        /// ノード</returns>
        public NodeEntity Attr(string name) {
            return Find(@"Item", @"type", @"Attr", @"name", name);
        }

        /// <summary>
        /// 自分自身の子ノードから name が引数に一致する最初の Category を返す
        /// </summary>
        /// <param name="name">
        /// 取得したいタグの name 属性値</param>
        /// <returns>
        /// ノード</returns>
        public NodeEntity SubCategory(string name) {
            return Find(@"Category", @"name", name);
        }

        /// <summary>
        /// 自分自身の孫ノードから name が引数に一致する最初の Category を返す
        /// </summary>
        /// <param name="par1Name">
        /// 取得したい子タグの name 属性値</param>
        /// <param name="par2Name">
        /// 取得したい孫タグの name 属性値</param>
        /// <returns>
        /// ノード</returns>
        public NodeEntity SubCategory(string par1Name, string par2Name) {
            return Find(@"Category", @"name", par1Name).Find(@"Category", @"name", par2Name);
        }

        /// <summary>
        /// 自分自身の曾孫ノードから name が引数に一致する最初の Category を返す
        /// </summary>
        /// <param name="par1Name">
        /// 取得したい子タグの name 属性値</param>
        /// <param name="par2Name">
        /// 取得したい孫タグの name 属性値</param>
        /// <param name="par3Name">
        /// 取得したい曾孫タグの name 属性値</param>
        /// <returns>
        /// ノード</returns>
        public NodeEntity SubCategory(string par1Name, string par2Name, string par3Name) {
            return Find(@"Category", @"name", par1Name).Find(@"Category", @"name", par2Name).Find(@"Category", @"name", par3Name);
        }

        /// <summary>
        /// 自分自身の子ノードから name が引数に一致する最初の Command を返す
        /// </summary>
        /// <param name="name">
        /// 取得したい子タグの name 属性値</param>
        /// <returns>
        /// ノード</returns>
        public NodeEntity Command(string name) {
            return Find(@"Command", @"name", name);
        }

        /// <summary>
        /// 自分自身の子ノードから name が引数に一致する最初の Param を返す
        /// </summary>
        /// <param name="name">
        /// 取得したい子タグの name 属性値</param>
        /// <returns>
        /// ノード</returns>
        public NodeEntity Param(string name) {
            return Find(@"Param", @"name", name);
        }

        /// <summary>
        /// 自分自身の子ノードから name が引数に一致する最初の Command から name が引数に一致する最初の Param を返す
        /// </summary>
        /// <param name="par1Name">
        /// 取得したい Command の name 属性値</param>
        /// <param name="par2Name">
        /// 取得したい Param の name 属性値</param>
        /// <returns>
        /// ノード</returns>
        public NodeEntity Param(string par1Name, string par2Name) {
            return Command(par1Name).Param(par2Name);
        }

        #endregion -- Derivative Find --

        #endregion -- メソッド --

        #region -- private --

        private NodeEntity FindTail(NodeEntity node, int depth) {
            if (depth == 1) {
                return node;
            } else {
                depth--;
                int count = node.GetChildren().Count;
                return FindTail(node.GetChildren()[count - 1], depth);
            }
        }

        private string ToStringStart() {
            string ret = Indent(0);
            if (attrList.Count > 0) {
                ret += @"<" + nodeName;
                if (newLineAfterOpeningBracket) {
                    ret += "\r\n";
                }
                foreach (AttributeEntity item in attrList) {
                    ret += Indent(1) + item.ToString();
                    if (newLineAfterAttributes) {
                        ret += "\r\n";
                    }
                }
                ret += Indent(1) + @">";
            } else {
                ret += @"<" + nodeName + @">";
            }
            return ret;
        }

        private string ToStringEnd() {
            string ret = Indent(0);
            ret += @"</" + nodeName + @">";
            return ret;
        }

        private string ToStringEmpty() {
            string ret = Indent(0);
            if (attrList.Count > 0) {
                ret += @"<" + nodeName;
                if (newLineAfterOpeningBracket) {
                    ret += "\r\n";
                }
                foreach (AttributeEntity item in attrList) {
                    ret += Indent(1) + item.ToString();
                    if (newLineAfterAttributes) {
                        ret += "\r\n";
                    }
                }
                ret += Indent(1) + @"/>";
            } else {
                ret += @"<" + nodeName + @"/>";
            }
            return ret;
        }

        private string ToStringComment() {
            string ret = Indent(0);
            ret += @"<!-- " + nodeValue + @" -->";
            return ret;
        }

        private string Indent(int plus) {
            string ret = string.Empty;
            for (int i = 0; i < (depth + plus) * indentSize; i++) {
                ret += @" ";
            }
            return ret;
        }

        #region -- Writer Setting --

        private bool NewLineAfterOpeningBracket() {
            if (writerSetting == null) {
                return true;
            }
            if (writerSetting.Find(@"Writer") == null) {
                return true;
            }
            if (writerSetting.Find(@"Writer").Find(@"Setting") == null) {
                return true;
            }
            if (writerSetting.Find(@"Writer").Find(@"Setting").Find(@"NewLine") == null) {
                return true;
            }
            if (writerSetting.Find(@"Writer").Find(@"Setting").Find(@"NewLine").Find(@"OpeningBracket") == null) {
                return true;
            }
            if (writerSetting.Find(@"Writer").Find(@"Setting").Find(@"NewLine").Find(@"OpeningBracket").GetNodeValue().Equals(@"YES")) {
                return true;
            } else {
                return false;
            }
        }

        private bool NewLineAfterClosingBracket() {
            if (writerSetting == null) {
                return true;
            }
            if (writerSetting.Find(@"Writer") == null) {
                return true;
            }
            if (writerSetting.Find(@"Writer").Find(@"Setting") == null) {
                return true;
            }
            if (writerSetting.Find(@"Writer").Find(@"Setting").Find(@"NewLine") == null) {
                return true;
            }
            if (writerSetting.Find(@"Writer").Find(@"Setting").Find(@"NewLine").Find(@"ClosingBracket") == null) {
                return true;
            }
            if (writerSetting.Find(@"Writer").Find(@"Setting").Find(@"NewLine").Find(@"ClosingBracket").GetNodeValue().Equals(@"YES")) {
                return true;
            } else {
                return false;
            }
        }

        private bool NewLineAfterAttributes() {
            if (writerSetting == null) {
                return true;
            }
            if (writerSetting.Find(@"Writer") == null) {
                return true;
            }
            if (writerSetting.Find(@"Writer").Find(@"Setting") == null) {
                return true;
            }
            if (writerSetting.Find(@"Writer").Find(@"Setting").Find(@"NewLine") == null) {
                return true;
            }
            if (writerSetting.Find(@"Writer").Find(@"Setting").Find(@"NewLine").Find(@"AfterAttrElements") == null) {
                return true;
            }
            if (writerSetting.Find(@"Writer").Find(@"Setting").Find(@"NewLine").Find(@"AfterAttrElements").GetNodeValue().Equals(@"YES")) {
                return true;
            } else {
                return false;
            }
        }

        private bool NewLineAfterNodeValue() {
            if (writerSetting == null) {
                return true;
            }
            if (writerSetting.Find(@"Writer") == null) {
                return true;
            }
            if (writerSetting.Find(@"Writer").Find(@"Setting") == null) {
                return true;
            }
            if (writerSetting.Find(@"Writer").Find(@"Setting").Find(@"NewLine") == null) {
                return true;
            }
            if (writerSetting.Find(@"Writer").Find(@"Setting").Find(@"NewLine").Find(@"AfterNodeValue") == null) {
                return true;
            }
            if (writerSetting.Find(@"Writer").Find(@"Setting").Find(@"NewLine").Find(@"AfterNodeValue").GetNodeValue().Equals(@"YES")) {
                return true;
            } else {
                return false;
            }
        }

        private int IndentSize() {
            if (writerSetting == null) {
                return 2;
            }
            if (writerSetting.Find(@"Writer") == null) {
                return 2;
            }
            if (writerSetting.Find(@"Writer").Find(@"Setting") == null) {
                return 2;
            }
            if (writerSetting.Find(@"Writer").Find(@"Setting").Find(@"IndentSize") == null) {
                return 2;
            }
            int ret = 0;
            if (int.TryParse(writerSetting.Find(@"Writer").Find(@"Setting").Find(@"IndentSize").GetNodeValue(), out ret)) {
                return ret;
            } else {
                return 2;
            }
        }

        #endregion -- Writer Setting --

        #endregion -- private --
    }
}