using System;
using System.Collections.Generic;
using System.Linq;
using DumbSearchEngine;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class Tests
    {
        [TestMethod]
        public void Stemmer_ok()
        {
            CollectionAssert.AreEqual(new List<string> { "m", "ma", "mat", "at", "t" }, Stemmer.Stem("mat").Select(s => s.Word).ToList());
            CollectionAssert.AreEqual(new List<double> { 1 / 3.0, 2 / 3.0, 3 / 3.0, 2 / 3.0, 1 / 3.0 }, Stemmer.Stem("mat").Select(s => s.Score).ToList());
        }


        [TestMethod]
        public void Docx_ok()
        {
            var doc = new DocxExtractor().Extract(@"C:\Users\Bartłomiej\SkyDrive\Dokumenty\Opis reklamy.docx");
            Assert.IsTrue(doc.Content.Contains("kanapę"));
        }


        [TestMethod]
        public void Index_ok()
        {
            var index = new Index();
            index.AddDocument(new Document { Content = "abc def", Reference = new DocumentRef { Path = "a" } });

            Assert.AreEqual(1, index.Find("abc").Select(n => n.Document.Path).Count());
            Assert.AreEqual(0, index.Find("xoxoxoxo").Select(n => n.Document.Path).Count());
        }

        [TestMethod]
        public void Parser_ok()
        {
            var result = QueryParser.ParseExpression("abc");
            Assert.IsInstanceOfType(result, typeof(TermQuery));

            result = QueryParser.ParseExpression("abc OR def");
            Assert.IsInstanceOfType(result, typeof(OrQuery));

            result = QueryParser.ParseExpression("abc AND def");
            Assert.IsInstanceOfType(result, typeof(AndQuery));

            result = QueryParser.ParseExpression("NOT def");
            Assert.IsInstanceOfType(result, typeof(NotQuery));

            result = QueryParser.ParseExpression("abc AND cde OR NOT xxx");
            Assert.IsInstanceOfType(result, typeof(OrQuery));
        }

    }
}
