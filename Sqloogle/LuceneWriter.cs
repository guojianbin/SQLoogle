#region license
// Sqloogle
// Copyright 2013-2017 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using System.IO;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Rhino.Etl.Core;
using Version = Lucene.Net.Util.Version;

namespace Sqloogle {
    public class LuceneWriter : WithLoggingMixin, IIndexWriter, IDisposable {
        private readonly string _folder;
        private readonly FSDirectory _indexDirectory;
        private readonly StandardAnalyzer _standardAnalyzer;
        private readonly IndexWriter _indexWriter;

        public LuceneWriter(string folder) {
            _folder = folder;
            _indexDirectory = FSDirectory.Open(new DirectoryInfo(folder));
            _standardAnalyzer = new StandardAnalyzer(Version.LUCENE_30);
            _indexWriter = new IndexWriter(_indexDirectory, _standardAnalyzer, IndexWriter.MaxFieldLength.UNLIMITED);
        }

        public void Clean() {
            Info("Cleaning Lucene index at {0}", _folder);
            _indexWriter.DeleteAll();
        }

        public void Add(object doc) {
            _indexWriter.AddDocument((Document)doc);
        }

        public void Delete(string id) {
            _indexWriter.DeleteDocuments(new Term("id", id));
        }

        public void Update(string id, object doc) {
            _indexWriter.UpdateDocument(new Term("id", id), (Document)doc);
        }

        public void Commit() {
            Info("Lucene Committing.");
            _indexWriter.Commit();
        }

        public void Optimize() {
            Info("Lucene Optimizing.");
            _indexWriter.Optimize();
        }

        public void Dispose() {
            Info("Lucene Disposal.");
            _indexWriter.Dispose();
            _indexDirectory.Dispose();
            _standardAnalyzer.Close();
            _standardAnalyzer.Dispose();
        }
    }
}