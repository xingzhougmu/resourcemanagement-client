﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lithnet.ResourceManagement.Client;

namespace Lithnet.ResourceManagement.Client.UnitTests
{
    [TestClass]
    public class XpathPredicateGroupTests
    {
        private ResourceManagementClient client = new ResourceManagementClient();

        public XpathPredicateGroupTests()
        {
            client.DeleteResources(client.GetResources("/" + UnitTestHelper.ObjectTypeUnitTestObjectName));
        }

        [TestMethod]
        public void XpathPredicateGroupSingleValueTest()
        {
            string testValue1 = "test1";
            string nonMatchValue = "test3";
            XPathQuery predicate1 = new XPathQuery(UnitTestHelper.AttributeStringSV, ComparisonOperator.Equals, testValue1);
            XPathQueryGroup group = new XPathQueryGroup(GroupOperator.And, predicate1);

            string expected = string.Format("({0} = '{1}')", UnitTestHelper.AttributeStringSV, testValue1);

            ResourceObject matchObject = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeStringSV, testValue1);
            ResourceObject nonMatchObject = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeStringSV, nonMatchValue);

            try
            {
                this.SubmitXpath(group, expected, matchObject);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchObject, nonMatchObject);
            }
        }

        [TestMethod]
        public void XpathPredicateGroupAndTest()
        {
            string testValue1 = "test1";
            string testValue2 = "test2";
            string nonMatchValue = "test3";
            XPathQuery predicate1 = new XPathQuery(UnitTestHelper.AttributeStringSV, ComparisonOperator.Equals, testValue1);
            XPathQuery predicate2 = new XPathQuery(UnitTestHelper.AttributeStringMV, ComparisonOperator.Equals, testValue2);
            XPathQueryGroup group = new XPathQueryGroup(GroupOperator.And, predicate1, predicate2);

            string expected = string.Format("(({0} = '{1}') and ({2} = '{3}'))", UnitTestHelper.AttributeStringSV, testValue1, UnitTestHelper.AttributeStringMV, testValue2);

            ResourceObject matchObject = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeStringSV, testValue1);
            ResourceObject nonMatchObject = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeStringSV, nonMatchValue);
            matchObject.Attributes[UnitTestHelper.AttributeStringMV].SetValue(testValue2);
            matchObject.Save();

            try
            {
                this.SubmitXpath(group, expected, matchObject);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchObject, nonMatchObject);
            }
        }

        [TestMethod]
        public void XpathPredicateGroupOrTest()
        {
            string testValue1 = "test1";
            string testValue2 = "test2";
            string nonMatchValue = "test3";
            XPathQuery predicate1 = new XPathQuery(UnitTestHelper.AttributeStringSV, ComparisonOperator.Equals, testValue1);
            XPathQuery predicate2 = new XPathQuery(UnitTestHelper.AttributeStringMV, ComparisonOperator.Equals, testValue2);
            XPathQueryGroup group = new XPathQueryGroup(GroupOperator.Or, predicate1, predicate2);

            string expected = string.Format("(({0} = '{1}') or ({2} = '{3}'))", UnitTestHelper.AttributeStringSV, testValue1, UnitTestHelper.AttributeStringMV, testValue2);

            ResourceObject matchObject = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeStringSV, testValue1);
            ResourceObject nonMatchObject = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeStringSV, nonMatchValue);

            try
            {
                this.SubmitXpath(group, expected, matchObject);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchObject, nonMatchObject);
            }
        }

        [TestMethod]
        public void XpathPredicateGroupNestedTest()
        {
            string testValue1 = "test1";
            string testValue2 = "test2";
            long testValue3 = 55L;
            string nonMatchValue = "test3";
            XPathQuery predicate1 = new XPathQuery(UnitTestHelper.AttributeStringSV, ComparisonOperator.Equals, testValue1);
            XPathQuery predicate2 = new XPathQuery(UnitTestHelper.AttributeStringMV, ComparisonOperator.Equals, testValue2);
            XPathQuery predicate3 = new XPathQuery(UnitTestHelper.AttributeIntegerSV, ComparisonOperator.Equals, testValue3);
            XPathQueryGroup childGroup = new XPathQueryGroup(GroupOperator.Or, predicate1, predicate2);
            XPathQueryGroup group = new XPathQueryGroup(GroupOperator.And, predicate3, childGroup);

            string expected = string.Format("(({4} = {5}) and (({0} = '{1}') or ({2} = '{3}')))", UnitTestHelper.AttributeStringSV, testValue1, UnitTestHelper.AttributeStringMV, testValue2, UnitTestHelper.AttributeIntegerSV, testValue3);

            ResourceObject matchObject = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeStringSV, testValue1);
            ResourceObject nonMatchObject = UnitTestHelper.CreateTestResource(UnitTestHelper.AttributeStringSV, nonMatchValue);
            matchObject.Attributes[UnitTestHelper.AttributeIntegerSV].SetValue(testValue3);
            matchObject.Save();

            try
            {
                this.SubmitXpath(group, expected, matchObject);
            }
            finally
            {
                UnitTestHelper.CleanupTestResources(matchObject, nonMatchObject);
            }
        }

        private void SubmitXpath(XPathQueryGroup group, string expectedXpath, params ResourceObject[] matchResources)
        {
            Assert.AreEqual(expectedXpath, group.ToString());

            XPathExpression expression = new XPathExpression(UnitTestHelper.ObjectTypeUnitTestObjectName, group);

            ISearchResultCollection results = client.GetResources(expression.ToString());

            if (results.Count == 0)
            {
                if (matchResources != null && matchResources.Length > 0)
                {
                    Assert.Fail("The query returned no results");
                }
            }

            if (matchResources == null || matchResources.Length == 0)
            {
                Assert.Fail("The query returned results where none were expectedXpath");
            }

            if (results.Count != matchResources.Length)
            {
                Assert.Fail("The query returned an unexpected number of results");
            }

            if (!results.All(t => matchResources.Any(u => u.ObjectID == t.ObjectID)))
            {
                Assert.Fail("The query did not return the correct results");
            }
        }
    }
}