/*
*
* XReader.cs
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
using System.Xml;

namespace SAXWrapper {

    public class XReader : SettingReader {

        public override void Parse() {
            System.IO.StreamReader sr = new System.IO.StreamReader(GetFullPath());
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Parse;
            XmlReader reader = XmlReader.Create(sr, settings);
            try {
                while (reader.Read()) {
                    ParseElement(reader);
                    ParseText(reader);
                    ParseCDATA(reader);
                    ParseEndElement(reader);
                    ParseComment(reader);
                }
            } catch (Exception) {
                throw;
            } finally {
                if (reader != null) {
                    reader.Close();
                }
                if (sr != null) {
                    sr.Close();
                }
            }
        }

        protected void ParseComment(XmlReader reader) {
            if (reader.NodeType != XmlNodeType.Comment) { return; }

        }
    }
}
