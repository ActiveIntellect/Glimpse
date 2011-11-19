﻿using System.Diagnostics.Contracts;
using Glimpse.Core2.Extensibility;

namespace Glimpse.Core2
{
    public class GlimpseConfiguration
    {
        public GlimpseLazyCollection<IGlimpseTab, IGlimpsePluginMetadata> Plugins { get; set; }
        public GlimpseCollection<IGlimpsePipelineModifier> PipelineModifiers { get; set; }
        public IFrameworkProvider FrameworkProvider { get; set; }
        public IGlimpseSerializer Serializer { get; set; }
        public IGlimpsePersistanceStore PersistanceStore { get; set; }

        public GlimpseConfiguration(IFrameworkProvider frameworkProvider)
        {
            Contract.Requires(frameworkProvider != null);
            //only use contracts if we can build them on teamcity AND the experience is nice for users that don't have the VS extension installed

            FrameworkProvider = frameworkProvider;
            Plugins = new GlimpseLazyCollection<IGlimpseTab, IGlimpsePluginMetadata>();
            PipelineModifiers = new GlimpseCollection<IGlimpsePipelineModifier>();
            Serializer = new JsonNetSerializer();
            PersistanceStore = new ApplicationPersistanceStore(frameworkProvider.HttpServerStore);
        }
    }
}
