﻿using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Text;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PCPW2
{
    class Parser
    {

        public async Task<List<ParsedProduct>> ParseLink(string link)
        {
            List<ParsedProduct> products = new List<ParsedProduct>();

            // Cheking link for correctness
            if (!ValidateLink(link))
            {
                MessageBox.Show("Error: Link is not valid or empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            IConfiguration config = Configuration.Default.WithDefaultLoader();

            // Loading page HTML code
            IDocument document = await BrowsingContext.New(config).OpenAsync(link);

            if (document.StatusCode != System.Net.HttpStatusCode.OK) return null;

            // Selecting data with selectors
            IHtmlCollection<IElement> parsedPrices = document.QuerySelectorAll("td.model-hot-prices-td [id^=price], [class$=ib] span:first-child");
            IHtmlCollection<IElement> parsedNames = document.QuerySelectorAll("td.model-short-info table span.u");

            for (int i = 0; i < parsedPrices.Length; i++)
            {
                // Removing &nbsp;
                parsedPrices[i].TextContent = RemoveSpace(parsedPrices[i].Text());
            }

            // Adding data to list of produtcs
            for (int i = 0; i < parsedNames.Length && i < parsedPrices.Length; i++)
            {
                products.Add(new ParsedProduct(parsedNames[i].Text(), int.Parse(parsedPrices[i].Text())));
            }

            return products;
        }

        private string RemoveSpace(string input)
        {
            string result = null;
            for (int z = 0; z < input.Length; z++)
            {
                if (input[z].IsDigit())
                {
                    result += input[z];
                }
            }
            return result;
        }

        private bool ValidateLink(string link)
        {
            return Uri.TryCreate(link, UriKind.Absolute, out Uri uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}
