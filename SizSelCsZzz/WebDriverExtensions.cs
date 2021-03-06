﻿using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using OpenQA.Selenium;

namespace SizSelCsZzz
{
    public static class WebDriverExtensions
    {
        public static int MaxWaitMS = 5000;
        public static int WaitIntervalMS = 100;

        public static IWebElement WaitForElement(this ISearchContext node, By condition)
        {
            return node.WaitForElementEx(condition);
        }

        public static IWebElement WaitForElementEx(this ISearchContext node, By condition, int? maxWaitMS = null, int? waitIntervalMS = null)
        {
            maxWaitMS = maxWaitMS ?? MaxWaitMS;
            waitIntervalMS = waitIntervalMS ?? WaitIntervalMS;

            DateTime finishTime = DateTime.UtcNow.AddMilliseconds(maxWaitMS.Value);

            do
            {
                DateTime timeAttempted = DateTime.UtcNow;
                try
                {
                    return node.FindElement(condition);
                }
                catch (Exception)
                {
                    if (timeAttempted > finishTime)
                    {
                        throw;
                    }
                }

                Thread.Sleep(waitIntervalMS.Value);
            } while (true);
        }

        public static string GetBodyText(this IWebDriver browser)
        {
            if (browser.FindElements(By.TagName("body")).Count() == 0)
            {
                return "";
            }

            return browser.FindElement(By.TagName("body")).Text;
        }

        public static bool ContainsText(this IWebDriver browser, string text)
        {
            return browser.GetBodyText().Contains(text);
        }

        public static int CountElementsMatching(this IWebDriver browser, string cssSelector)
        {
            return browser.FindElements(BySizzle.CssSelector(cssSelector)).Count();
        }

        public static void ClearThenSendKeys(this IWebElement element, string text)
        {
            element.Clear();
            element.SendKeys(text);
        }
    }
}
