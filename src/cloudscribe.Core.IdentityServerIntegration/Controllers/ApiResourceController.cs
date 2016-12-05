﻿// Copyright (c) Source Tree Solutions, LLC. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Author:					Joe Audette
// Created:					2016-10-13
// Last Modified:			2016-10-21
// 

using cloudscribe.Core.IdentityServerIntegration.Models;
using cloudscribe.Core.IdentityServerIntegration.Services;
using cloudscribe.Core.Web.Components;
using cloudscribe.Web.Common.Extensions;
using cloudscribe.Web.Navigation;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace cloudscribe.Core.IdentityServerIntegration.Controllers
{
    [Authorize(Policy = "IdentityServerAdminPolicy")]
    public class ApiResourceController : Controller
    {
    }
}
