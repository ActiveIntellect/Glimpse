﻿using System.Collections.Generic;
using System.Web;
using Glimpse.Core;
using Glimpse.Core.Configuration;
using Glimpse.Core.Extensibility;
using Glimpse.Core.Handlers;
using Moq;
using NUnit.Framework;

namespace Glimpse.Test.Core
{
    [TestFixture]
    public class ModuleTest
    {
        [Test]
		[Ignore("TODO: Remove ignore and fix implementation")]
        public void Module_PostMapRequestHandler_ReturnsGlimpseClientJs()
        {
            Context.Setup(ctx => ctx.Request.Path).Returns("/virDir/Glimpse/glimpseClient.js");
            Context.SetupSet(ctx => ctx.Items[GlimpseConstants.ValidPath] = false);
            Context.SetupSet(ctx => ctx.Items[GlimpseConstants.ValidPath] = true);

            Module.Configuration = new GlimpseConfiguration {RootUrlPath = "glimpse"};
            Module.Handlers = new List<IGlimpseHandler> {new JavascriptClient()};
            //Module.PostMapRequestHandler(Context.Object);

            Context.VerifySet(ctx => ctx.Items[GlimpseConstants.ValidPath] = true);
        }

        public Mock<HttpContextBase> Context { get; set; }

        [SetUp]
        public void Setup()
        {
            Context = new Mock<HttpContextBase>();
        }
    }
}
