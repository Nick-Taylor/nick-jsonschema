using System;
using FluentAssertions;
using Xunit;

namespace jsonschema
{
    public class DeserializeStringEnumTests
    {
        Newtonsoft.Json.JsonSerializer jsr;

        Newtonsoft.Json.Schema.JSchemaValidatingReader vr;

        public DeserializeStringEnumTests()
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
                    ""suv"": {{
                        ""type"": [
                            ""string""
                        ],
                        ""enum"": [
                            ""Zero"", ""One"", ""Two"", ""Three""
                        ]
                    }}
                }}
            }}";


        [Fact]
        void TestAcceptableValue()
        {
            string json = @"{""suv"":""Two""}"; 

            Func<Artifact> act = () => this.Act(escapedSchema, json);

            act().SomeUniqueValue.Should().Be(UniqueValues.Two);
        }

        [Fact]
        void TestUnacceptableValue()
        {
            string json = @"{""suv"":""Thousand""}"; 

            Func<Artifact> act = () => this.Act(escapedSchema, json);

            act.Should().Throw<Newtonsoft.Json.Schema.JSchemaValidationException>()
                .WithMessage(@"Value ""Thousand"" is not defined in enum. Path 'suv', line 1, position 17.");
            
        }
    }
}        