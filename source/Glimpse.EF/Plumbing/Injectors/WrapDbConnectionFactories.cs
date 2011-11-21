﻿using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection;
using Glimpse.Ado.Plumbing;
using Glimpse.Core.Extensibility;
using Microsoft.CSharp;

namespace Glimpse.EF.Plumbing.Injectors
{
    public class WrapDbConnectionFactories
    {
        private IGlimpseLogger Logger { get; set; }

        public WrapDbConnectionFactories(IGlimpseLogger logger)
        {
            Logger = logger;
        }

        public void Inject()
        {
            //TODO: Clean this up, what are we *really* testing here?
            var type = Type.GetType("System.Data.Entity.Database, EntityFramework", false);
            if (type != null && type.GetProperty("DefaultConnectionFactory") != null)
            {
                Logger.Info("AdoPipelineInitiator for EF: Starting to inject ConnectionFactory");

                var code = GetEmbeddedResource(GetType().Assembly, "Glimpse.Ef.Plumbing.Profiler.GlimpseProfileDbConnectionFactory.cs");
                var assembliesToReference = new[] { type.Assembly, typeof(DbConnection).Assembly, typeof(TypeConverter).Assembly, typeof(ProviderStats).Assembly };

                var generatedAssembly = CreateAssembly(code, assembliesToReference);
                var generatedType = generatedAssembly.GetType("Glimpse.Ado.Plumbing.Profiler.GlimpseProfileDbConnectionFactory");
                generatedType.GetMethod("Initialize").Invoke(null, null);

                Logger.Info("AdoPipelineInitiator for EF: Finished to inject ConnectionFactory");
            }

            Logger.Info("AdoPipelineInitiator for EF: Finished trying to injecting DbConnectionFactory");
        }

        public static Assembly CreateAssembly(string code, IEnumerable<Assembly> referenceAssemblies)
        {
            //See http://stackoverflow.com/questions/3032391/csharpcodeprovider-doesnt-return-compiler-warnings-when-there-are-no-errors
            var provider = new CSharpCodeProvider(new Dictionary<string, string> { { "CompilerVersion", "v4.0" } });

            var compilerParameters = new CompilerParameters { GenerateExecutable = false, GenerateInMemory = true };
            compilerParameters.ReferencedAssemblies.AddRange(referenceAssemblies.Select(a => a.Location).ToArray());

            var results = provider.CompileAssemblyFromSource(compilerParameters, code);
            if (results.Errors.HasErrors)
                throw new InvalidOperationException(results.Errors[0].ErrorText);

            return results.CompiledAssembly;
        }

        private static string GetEmbeddedResource(Assembly assembly, string resourceName)
        {
            //See http://stackoverflow.com/questions/3314140/how-to-read-embedded-resource-text-file
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}