using System;
using FluentAssertions;
using Xunit;

namespace jsonschema
{
    public class DeserializeNullableIntegerTests
    {
        Newtonsoft.Json.JsonSerializer jsr;

        Newtonsoft.Json.Schema.JSchemaValidatingReader vr;

        public DeserializeNullableIntegerTests()
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
                    ""sni"": {{
                        ""required"": false,
                        ""type"": [
                            ""integer"",
                            ""null""
                        ],
                        ""exclusiveMinimum"": true,
                        ""exclusiveMaximum"": false,
                        ""minimum"": 0,
                        ""maximum"": {int.MaxValue}
                    }}
                }}
            }}";

        [Fact]
        void TestMissingValue()
        {
            string json = @"{}"; 

            Func<Artifact> act = () => this.Act(escapedSchema, json);

            act().SomeNullableInt.Should().BeNull();
        }

        [Fact]
        void TestNullValue()
        {
            string json = @"{""sni"":null}"; 

            Func<Artifact> act = () => this.Act(escapedSchema, json);

            act().SomeNullableInt.Should().BeNull();
        }

        [Fact]
        void TestAcceptableValue()
        {
            string json = @"{""sni"":1}"; 

            Func<Artifact> act = () => this.Act(escapedSchema, json);

            act().SomeNullableInt.Should().Be(1);
        }

        [Fact]
        void TestZeroValue()
        {
            string json = @"{""sni"":0}"; 

            Func<Artifact> act = () => this.Act(escapedSchema, json);

            act.Should().Throw<Newtonsoft.Json.Schema.JSchemaValidationException>()
                .WithMessage("Integer 0 equals minimum value of 0 and exclusive minimum is true. Path 'sni', line 1, position 8.");
        }

        [Fact]
        void TestOverrunValue()
        {
            string json = $@"{{""sni"":{int.MaxValue.ToString() + "0"}}}"; 

            Func<Artifact> act = () => this.Act(escapedSchema, json);

            act.Should().Throw<Newtonsoft.Json.Schema.JSchemaValidationException>()
                .WithMessage("Integer 21474836470 exceeds maximum value of 2147483647. Path 'sni', line 1, position 18.");
        }

        [Fact]
        void TestNegativeValue()
        {
            string json = @"{""sni"":-1}"; 

            Func<Artifact> act = () => this.Act(escapedSchema, json);

            act.Should().Throw<Newtonsoft.Json.Schema.JSchemaValidationException>()
                .WithMessage("Integer -1 is less than minimum value of 0. Path 'sni', line 1, position 9.");
        }
    }
}        