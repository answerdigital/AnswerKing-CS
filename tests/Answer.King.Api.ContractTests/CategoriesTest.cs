using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Alba;
using Answer.King.Api.IntegrationTests.Common;
using Answer.King.Api.IntegrationTests.Common.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PactNet;
using PactNet.Infrastructure.Outputters;
using PactNet.Models;
using PactNet.Verifier;
using PactNet.Verifier.Messaging;
using Xunit;
using Xunit.Abstractions;
using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using IPAddress = System.Net.IPAddress;

namespace Answer.King.Api.ContractTests;

public class AnswerKingVerifierTest
{
    private readonly PactVerifierConfig config;

    public AnswerKingVerifierTest(ITestOutputHelper output)
    {
        this.config = new PactVerifierConfig
        {
            Outputters = new List<IOutput> { new XUnitOutput(output) },
            LogLevel = PactLogLevel.Debug
        };
    }

    [Fact]
    public void VerifyContract()
    {
        string pactPath = Path.Combine("contracts", "answer-king-api-answer-king-ui.json");
        IPactVerifier verifier = new PactVerifier(this.config);
        verifier.ServiceProvider("AnswerKingCS-API", new Uri("http://localhost:5000"))
            .WithFileSource(new FileInfo(pactPath))
            .WithRequestTimeout(TimeSpan.FromSeconds(2))
            .WithSslVerificationDisabled()
            .Verify();
    }
}
