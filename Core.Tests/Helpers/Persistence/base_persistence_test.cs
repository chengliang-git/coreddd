﻿using Core.Domain;
using Core.Infrastructure;
using Core.Utilities.Extensions;
using NHibernate;
using NUnit.Framework;

namespace Core.Tests.Helpers.Persistence
{
    public abstract class base_persistence_test
    {
        protected ISession Session;

        protected abstract void Context();

        [TestFixtureSetUp]
        public void SetUp()
        {
            Session = UnitOfWork.CurrentSession;
            ClearDatabase();
            Context();
        }

        protected void ClearDatabase()
        {
            using (var tx = Session.BeginTransaction())
            {
                Session.Delete("from Email");
                Session.Delete("from EmailTemplate");
                Session.Delete("from Recipient");
                Session.Delete("from User");
                tx.Commit();
            }  
        }

        protected void Save(params IAggregateRoot[] aggregateRootEntitites)
        {
            aggregateRootEntitites.Each(e => Session.SaveOrUpdate(e));
            Session.Flush();
        }

        protected TAggregateRootEntity Get<TAggregateRootEntity>(int id) where TAggregateRootEntity : IAggregateRoot
        {
            return Session.Get<TAggregateRootEntity>(id);
        }
    }
}