﻿using System;
using System.Collections.Generic;
using System.Reflection;
using CoreDdd.Nhibernate.Configurations;
using IntegrationTestsShared.TestEntities;
using HibernatingRhinos.Profiler.Appender.NHibernate;

namespace IntegrationTestsShared
{
    public class TestNhibernateConfigurator : BaseNhibernateConfigurator
    {
        public TestNhibernateConfigurator()
        {
#if DEBUG
            NHibernateProfiler.Initialize();
#endif
        }

        protected override Assembly[] GetAssembliesToMap()
        {
            return new[] { typeof(EqualityEntity).Assembly };
        }

        protected override IEnumerable<Type> GetIncludeBaseTypes()
        {
            yield return typeof(AbstractEntity);
        }

        protected override IEnumerable<Type> GetDiscriminatedTypes()
        {
            yield return typeof(EqualityEntity); // maps EqualityEntity and DerivedEqualityEntity into one table with a discriminator column
        }

        protected override IEnumerable<Type> GetComponentTypes()
        {
            yield return typeof(Component);
            yield return typeof(AnotherComponent);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
#if DEBUG
                NHibernateProfiler.Shutdown();
#endif
            }
            base.Dispose(disposing);
        }
    }
}