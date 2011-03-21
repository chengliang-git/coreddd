﻿using System.Collections.Generic;
using System.Linq;
using Core.TestHelper.Persistence;
using EmailMaker.Domain.Emails;
using EmailMaker.Domain.EmailTemplates;
using EmailMaker.DTO;
using EmailMaker.DTO.Emails;
using EmailMaker.Queries.Handlers;
using EmailMaker.Queries.Messages;
using NUnit.Framework;
using Shouldly;

namespace EmailMaker.IntegrationTests.DatabaseTests.QueryTests
{
    [TestFixture]
    public class when_querying_email_parts : BaseSimplePersistenceTest
    {
        private IEnumerable<EmailPartDTO> _result;
        private Email _email;

        public override void PersistenceContext()
        {
            var emailTemplate = new EmailTemplate("123");
            Save(emailTemplate);
            emailTemplate.CreateVariable(emailTemplate.Parts.First().Id, 1, 1);
            Save(emailTemplate);

            var anotherEmailTemplate = new EmailTemplate("another html");
            Save(anotherEmailTemplate);

            _email = new Email(emailTemplate);
            Save(_email);
        }

        public override void PersistenceQuery()
        {
            var query = new GetEmailPartsQuery();
            _result = query.Execute<EmailPartDTO>(new GetEmailPartsQueryMessage { EmailId = _email.Id });
        }

        [Test]
        public void email_template_parts_correctly_retrieved()
        {
            _result.Count().ShouldBe(3);

            var htmlPart = _email.Parts.First() as HtmlEmailPart;
            var partDTO = _result.First();
            partDTO.EmailId.ShouldBe(_email.Id);
            partDTO.PartId.ShouldBe(htmlPart.Id);
            partDTO.PartType.ShouldBe(PartType.Html);
            partDTO.Html.ShouldBe(htmlPart.Html);
            partDTO.VariableValue.ShouldBe(null);

            var variablePart = _email.Parts.ElementAt(1) as VariableEmailPart;
            partDTO = _result.ElementAt(1);
            partDTO.EmailId.ShouldBe(_email.Id);
            partDTO.PartId.ShouldBe(variablePart.Id);
            partDTO.PartType.ShouldBe(PartType.Variable);
            partDTO.Html.ShouldBe(null);
            partDTO.VariableValue.ShouldBe(variablePart.Value);

            htmlPart = _email.Parts.Last() as HtmlEmailPart;
            partDTO = _result.Last();
            partDTO.EmailId.ShouldBe(_email.Id);
            partDTO.PartId.ShouldBe(htmlPart.Id);
            partDTO.PartType.ShouldBe(PartType.Html);
            partDTO.Html.ShouldBe(htmlPart.Html);
            partDTO.VariableValue.ShouldBe(null);
        }
    }
}