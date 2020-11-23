# jsonschema
## How to validate JSON in C# using Newtonsoft.Json.Schema
As the need for security increases one of the things we can do to avoid potential problems is to validate any incoming JSON against a schema before attempting to deserialize to an object. One of the ways to accomplish that is to use Newtonsoft.Json.Schema.

At the time of writing [2019-09, formerly known as draft 8](https://json-schema.org/specification-links.html#2019-09-formerly-known-as-draft-8) is the latest published incarnation of the specification documents for JSON schema validation. Yes, that's right - draft. As such, use the following with caution.

Please note that the examples that follow use Newtonsoft.Json.Schema which requires an appropriate license [from Json.NET Schema](https://www.newtonsoft.com/jsonschema).

The following code demonstrates how to validate JSON against a schema using some simple examples against an Artifact class/object and UniqueValues enum.
```
    string schema = "";
    string json = "";

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

    jsr.Deserialize<Artifact>(vr);
```
### Validating decimals

Using the following schema we can validate decimals
```
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
```
#### Missing value

Validating `string json = @"{}"; ` will fail since the required value property is set to true.

#### Null value

Validating `string json = @"{""sd"":null}";` will fail since the required value property is set to true.

#### Acceptable value

Validating `string json = @"{""sd"":1.2}";` will pass since a.2 is a valid decimal value.

#### Zero value

Validating `string json = @"{""sd"":0.0}";` will fail since the minimum property is set to 0 and the exclusiveMinimum property is set to true.

#### Overrun value

Validating `string json = $@"{{""sd"":{decimal.MaxValue.ToString() + "0.0"}}}";` will fail since the supplied number is an order of magnitude greater than the value allowed, governed by the maximum property value and exclusiveMaximum property set to true.

#### Negative value

Validating `string json = @"{""sd"":-1.2}";` will fail since the minimum property is set to 0 and the exclusiveMinimum property is set to true.

### Validating string enums

Using the following schema we can validate string enums
```
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
```

#### Acceptable value

Validating `string json = @"{""suv"":""Two""}";` will pass.

#### Unacceptable value

Validating `string json = @"{""suv"":""Thousand""}";` will fail.

### Validating nullable integers

Using the following schema we can validate nullable integers
```
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
```

#### Missing value

Validating `string json = @"{}";` will pass since the required property is set to false. The default value for `Nullable<int>` is null.

#### Null value

Validating `string json = @"{""sni"":null}";` will pass since the required property is set to false, and one of the type values is null.

#### Acceptable value

Validating `string json = @"{""sni"":1}";` will pass since the minimum property is set to zero.

#### Zero value

Validating `string json = @"{""sni"":0}";` will fail since (although the minimum property is set to zero) the exclusiveMinimum property is set to true.

#### Overrun value

Validating `string json = $@"{{""sni"":{int.MaxValue.ToString() + "0"}}}";` will fail since the supplied number is an order of magnitude greater than the value allowed, governed by the maximum property value and exclusiveMaximum property set to true.

#### Negative value

Validating `string json = @"{""sni"":-1}";` will fail since the minimum property is set to 0 and the exclusiveMinimum property is set to true.

### Validating strings

Using the following schema we can validate nullable integers
```
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
```

#### Missing value

Validating `string json = @"{}";` will pass since the required property is set to false.

#### Null value

Validating `string json = @"{""ss"":null}";` will pass since the required property is set to false, and one of the type values is null.

#### Acceptable value

Validating `string json = @"{""ss"":""ABC""}";` will pass since a value is supplied.

### Exceptions

Validation failures will throw a `Newtonsoft.Json.Schema.JSchemaValidationException`.