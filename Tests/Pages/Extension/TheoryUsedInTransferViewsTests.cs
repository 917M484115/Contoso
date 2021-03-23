using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq.Expressions;

namespace Tests.Pages.Extension
{
    public static class TestClass {
        public static string TypeParameters<TLabel, TValue>(TLabel label, TValue value) {
            // https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/generics/generic-type-parameters
            var s = $"{label} {value}";
            return s;
        }
        public static TResult FunctionParameters<TResult, TValue>(Func<TValue, TResult> func, TValue value) {
            return func(value);
        }
        public static TResult ExpressionParameters<TResult, TValue>(Expression<Func<TValue, TResult>> expression, TValue value) {
            var r = expression.Compile();
            return r.Invoke(value);
        }
        public static string ExtensionMethod<TObject>(this TObject s) {
            return s.ToString();
        }
    }


    [TestClass]
    public class TheoryUsedInTransferViewsTests
    {

        [TestMethod]
        public void TypeParametersTest() {
            Assert.AreEqual("A B", TestClass.TypeParameters("A", "B"));
            Assert.AreEqual("1 2", TestClass.TypeParameters(1, 2));
            Assert.AreEqual("1.4 2.5", TestClass.TypeParameters(1.4, 2.5));
            Assert.AreEqual("A 2.5", TestClass.TypeParameters("A", 2.5));
        }

        private string intFunction(int i) => i.ToString();
        private string strFunction(string s) => s;
        private string doubleFunction(double d) => d.ToString();

        [TestMethod]
        public void ShowFunctionTest() {
            Assert.AreEqual("123", TestClass.FunctionParameters(intFunction, 123));
            Assert.AreEqual("ABC", TestClass.FunctionParameters(strFunction, "ABC"));
            Assert.AreEqual("1.23456", TestClass.FunctionParameters(doubleFunction, 1.23456));
        }
        [TestMethod]
        public void ShowExpressionTest() {
            Assert.AreEqual("123", TestClass.ExpressionParameters(x => intFunction(x), 123));
            Assert.AreEqual("ABC", TestClass.ExpressionParameters(x => strFunction(x), "ABC"));
            Assert.AreEqual("1.23456", TestClass.ExpressionParameters(x => doubleFunction(x), 1.23456));
        }
        [TestMethod]
        public void ShowStringTest() {
            Assert.AreEqual("123", 123.ExtensionMethod());
            Assert.AreEqual("ABC", "ABC".ExtensionMethod());
            Assert.AreEqual("1.23456", 1.23456.ExtensionMethod());
        }
    }
}
