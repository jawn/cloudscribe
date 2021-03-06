﻿// Copyright (c) Source Tree Solutions, LLC. All rights reserved.
// Licensed under the Apache License, Version 2.0.
// Author:                  Joe Audette
// Created:                 2017-01-04
// Last Modified:           2017-10-30
// 

using cloudscribe.Web.Common.Analytics;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;
using System.Text;

namespace cloudscribe.Web.Common.TagHelpers
{
    public class GoogleAnalyticsTagHelper : TagHelper
    {
        public GoogleAnalyticsTagHelper(
            IOptions<GoogleAnalyticsOptions> optionsAccessor
            )
        {
            _options = optionsAccessor.Value;
        }

        private GoogleAnalyticsOptions _options;

        private const string ProfileIdAttributeName = "profile-id";
        private const string UserIdAttributeName = "user-id";
        private const string AllowAnchorAttributeName = "allow-anchor";

        [ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName(ProfileIdAttributeName)]
        public string ProfileId { get; set; }

        [HtmlAttributeName(UserIdAttributeName)]
        public string UserId { get; set; } = string.Empty;

        [HtmlAttributeName(AllowAnchorAttributeName)]
        public bool AllowAnchor { get; set; } = false;

        [HtmlAttributeName("track-after-page-load")]
        public bool TrackAfterPageLoad { get; set; } = false;

        [HtmlAttributeName("debug")]
        public bool Debug { get; set; } = false;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if(string.IsNullOrEmpty(ProfileId))
            {
                output.SuppressOutput();
                return;
            }

            output.TagName = "script";    // Replaces <google-analytics> with <script> tag

            var sb = new StringBuilder();
            sb.AppendLine("");
            sb.AppendLine("(function(i,s,o,g,r,a,m){i['GoogleAnalyticsObject']=r;i[r]=i[r]||function(){");
            sb.AppendLine("(i[r].q=i[r].q||[]).push(arguments)},i[r].l=1*new Date();a=s.createElement(o),");
            sb.AppendLine("m=s.getElementsByTagName(o)[0];a.async=1;a.src=g;m.parentNode.insertBefore(a,m)");
            sb.AppendLine("})(window,document,'script','https://www.google-analytics.com/analytics.js','ga');");

            if(TrackAfterPageLoad)
            {
                sb.AppendLine("window.addEventListener(\"load\", function(){");
                if(Debug) sb.AppendLine("console.log(\"analytics\");");
            }

            //sb.Append("alert(window.CookieConsentStatus === undefined || window.CookieConsentStatus.DidConsent);");
            sb.Append("var gacanUseCookies = window.CookieConsentStatus === undefined || window.CookieConsentStatus.DidConsent;");
            if(Debug)
            {
                sb.Append("if(gacanUseCookies) { console.log('cookie ok'); } else { console.log('no cookies'); }");
            }
            

            sb.Append("ga('create',");
            sb.Append("'" + ProfileId + "', 'auto'");
            
            sb.Append(", { ");

            var comma = "";

            //if (_options.TrackUserId && !string.IsNullOrWhiteSpace(UserId))
            //{
            //    sb.Append("'userId': " + "'" + UserId + "'");
            //    comma = ",";
            //}

            if (AllowAnchor)
            {
                sb.Append(comma);
                sb.Append(" 'allowAnchor': true ");
            }
            
            sb.Append(" }");
            
            sb.Append(");");
            
            sb.AppendLine("");

            
            sb.Append("if(gacanUseCookies) {");

            if (_options.TrackUserId && !string.IsNullOrWhiteSpace(UserId))
            {
                sb.Append("ga('set','userId'," + "'" + UserId + "');");
            }
            sb.Append("ga('require', 'displayfeatures');");
            sb.Append("ga('set', 'anonymizeIp', undefined);");

            if(Debug)
            {
                sb.Append("console.log('ga tracked with cookies'); ");
            }

            sb.Append("} else {"); //can't use cookies

            sb.Append("ga('set', 'displayFeaturesTask', null);");
            sb.Append("ga('set', 'anonymizeIp', true);");

            if (Debug)
            {
                sb.Append("console.log('ga tracked without cookies'); ");
            }

            sb.Append("}");


            sb.AppendLine("ga('send', 'pageview');");

            var eventList = ViewContext.TempData.GetGoogleAnalyticsEvents();
            
            foreach (var e in eventList)
            {
                if(e.IsValid())
                {
                    sb.Append("ga('send', 'event'");
                    sb.Append(", '" + e.Category + "'");
                    sb.Append(", '" + e.Action + "'");

                    if(!string.IsNullOrWhiteSpace(e.Label))
                    {
                        sb.Append(", '" + e.Label + "'");
                    }
                    else
                    {
                        sb.Append(", 'undefined'");
                    }

                    if (!string.IsNullOrWhiteSpace(e.Value))
                    {
                        sb.Append(", '" + e.Value + "'");
                    }

                    if(e.Fields.Count > 0)
                    {
                        sb.Append(",{ ");
                         comma = "";
                        foreach(var f in e.Fields)
                        {
                            sb.Append(comma + "'" + f.Key + "' : '" + f.Value + "'");
                            comma = ",";
                        }
                        sb.Append("}");
                    }

                    sb.Append(");");
                    sb.AppendLine("");
                }
            }

            if (TrackAfterPageLoad)
            {
                sb.Append("});"); //end add event listener
            }


            output.Content.SetHtmlContent(sb.ToString());


        }
    }
}
