using System;
using FluentAssertions;
using Xunit;

namespace jsonschema
{
    public class DeserializeStringTests
    {
        Newtonsoft.Json.JsonSerializer jsr;

        Newtonsoft.Json.Schema.JSchemaValidatingReader vr;

        public DeserializeStringTests()
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
                    ""ss"": {{
                        ""required"": false,
                        ""type"": [
                            ""string"",
                            ""null""
                        ]
                    }}
                }}
            }}";

        [Fact]
        void TestMissingValue()
        {
            string json = @"{}"; 

            Func<Artifact> act = () => this.Act(escapedSchema, json);

            act().SomeString.Should().BeNull();
        }

        [Fact]
        void TestNullValue()
        {
            string json = @"{""ss"":null}"; 

            Func<Artifact> act = () => this.Act(escapedSchema, json);

            act().SomeString.Should().BeNull();
        }

        [Fact]
        void TestAcceptableValue()
        {
            string json = @"{""ss"":""ABC""}"; 

            Func<Artifact> act = () => this.Act(escapedSchema, json);

            act().SomeString.Should().Be("ABC");
        }
    }
}        