using System;
using FluentAssertions;
using Xunit;

namespace jsonschema
{
    public class DeserializeDecimalTests
    {
        
        Newtonsoft.Json.JsonSerializer jsr;

        Newtonsoft.Json.Schema.JSchemaValidatingReader vr;

        public DeserializeDecimalTests()
        {
            
        }

        Artifact Act(string schema, string json)
        {
            Newtonsoft.Json.Schema.JSchema js = 
                Newtonsoft.Json.Schema.JSchema.Parse(schema);

            System.IO.StringReader sr = 
                new System.IO.StringReader(json);

            Newtonsoft.Json.JsonTextReader jr = 
                new Newtonsoft.Json.JsonTextReader(sr);

            this.vr =
                new Newtonsoft.Json.Schema.JSchemaValidatingReader(jr);

            vr.Schema = js;

            this.jsr =
                new Newtonsoft.Json.JsonSerializer();

            return jsr.Deserialize<Artifact>(vr);
        }

        string escapedSchema = $@"
            {{
                ""$schema"": ""https://json-schema.org/draft/2019-09/schema"",
                ""title"": ""Artifact"",
                ""type"": ""object"",
                ""properties"": {{
                    ""sd"": {{
                        ""required"": true,
                        ""type"": [
                            ""number""
                        ],
                        ""exclusiveMinimum"": true,
                        ""exclusiveMaximum"": false,
                        ""minimum"": 0,
                        ""maximum"": {decimal.MaxValue}
                    }}
                }}
            }}";

        [Fact]
        void TestMissingValue()
        {
            string json = @"{}"; 

            Func<Artifact> act = () => this.Act(escapedSchema, json);

            act.Should().Throw<Newtonsoft.Json.Schema.JSchemaValidationException>()
                .WithMessage("Required properties are missing from object: sd. Path '', line 1, position 2.");
        }

        [Fact]
        void TestNullValue()
        {
            string json = @"{""sd"":null}"; 

            Func<Artifact> act = () => this.Act(escapedSchema, json);

            act.Should().Throw<Newtonsoft.Json.Schema.JSchemaValidationException>()
                .WithMessage("Invalid type. Expected Number but got Null. Path 'sd', line 1, position 10.");
        }

        [Fact]
        void TestAcceptableValue()
        {
            string json = @"{""sd"":1.2}"; 

            Func<Artifact> act = () => this.Act(escapedSchema, json);

            act().SomeDecimal.Should().Be(1.2m);
        }

        [Fact]
        void TestZeroValue()
        {
            string json = @"{""sd"":0.0}"; 

            Func<Artifact> act = () => this.Act(escapedSchema, json);

            act.Should().Throw<Newtonsoft.Json.Schema.JSchemaValidationException>()
                .WithMessage("Float 0.0 equals minimum value of 0 and exclusive minimum is true. Path 'sd', line 1, position 9.");
        }

        [Fact]
        void TestOverrunValue()
        {
            string json = $@"{{""sd"":{decimal.MaxValue.ToString() + "0.0"}}}"; 

            Func<Artifact> act = () => this.Act(escapedSchema, json);

            act.Should().Throw<Newtonsoft.Json.Schema.JSchemaValidationException>()
                .WithMessage("Float 7.922816251426434E+29 exceeds maximum value of 7.922816251426433E+28. Path 'sd', line 1, position 38.");
        }

        [Fact]
        void TestNegativeValue()
        {
            string json = @"{""sd"":-1.2}"; 

            Func<Artifact> act = () => this.Act(escapedSchema, json);

            act.Should().Throw<Newtonsoft.Json.Schema.JSchemaValidationException>()
                .WithMessage("Float -1.2 is less than minimum value of 0. Path 'sd', line 1, position 10.");
        }
    }
}        