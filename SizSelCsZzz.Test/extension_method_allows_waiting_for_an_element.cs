﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NUnit.Framework;
using OpenQA.Selenium;
using SizSelCsZzz.Extras;
using SizSelCsZzz.Test.jquerySource;

namespace SizSelCsZzz.Test
{
    public class extension_method_allows_waiting_for_an_element : SpecificationForAllBrowsers
    {
        public override void SpecifyForBrowser(IWebDriver browser)
        {
            var server = beforeAll(() => new StaticServer("127.0.0.3", 8083)
                {
                    {"jquery.js", JQueryUtil.GetJQuerySource()}
                }.Start());

            beforeAll(() => server.Add("delay.html", 
                JQueryUtil.HtmlLoadingJQuery(server.UrlFor("jquery.js"), "<div> Hello, world. </div>")));

            describe("WaitForElement", delegate
            {
                arrange(() => browser.Navigate().GoToUrl(server.UrlFor("delay.html")));

                it("can find an element on the page", delegate
                {
                    expect(() => browser.WaitForElement(BySizzle.CssSelector("div:contains('Hello, world')")) != null);
                });

                it("reports a useful error if the element is not eventually found", delegate
                {
                    var expectedMessage = Assert.Throws<NoSuchElementException>(delegate
                    {
                        browser.FindElement(By.CssSelector("div div li ul ol lol.so img.nothappening"));
                    }).Message;

                    var e = Assert.Throws<NoSuchElementException>(delegate
                    {
                        browser.WaitForElement(By.CssSelector("div div li ul ol lol.so img.nothappening"));
                    });

                    expect(() => expectedMessage == e.Message);
                });

                it("will wait for elements", delegate
                {
                    (browser as IJavaScriptExecutor).ExecuteScript(@"
setTimeout(function() { $('body').append('<div>Better late than never.</div>'); }, 2000);
");

                    expect(() => browser.WaitForElement(BySizzle.CssSelector("div:contains('Better late than never.')")) != null);
                });

                beforeAll(() => server.Add("somepage.html", "<html><body><ul><li>LIST ITEM</li></ul></body></html>"));

                it("can be used transitiviely", delegate
                {
                    browser.Navigate().GoToUrl(server.UrlFor("somepage.html"));
                    expect(() => browser.WaitForElement(BySizzle.CssSelector("ul")).WaitForElement(BySizzle.CssSelector("li:contains('LIST ITEM')")) != null);
                });
            });
        }
    }
}
