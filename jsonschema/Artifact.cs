using System;

namespace jsonschema
{
    public class Artifact
    {
        [Newtonsoft.Json.JsonProperty(PropertyName = "ssn")]
        public bool ShouldSerializeNull { get; set; }

        [Newtonsoft.Json.JsonProperty(PropertyName = "sni")]
        public int? SomeNullableInt { get; set; }
        public bool ShouldSerializeSomeNullableInt() => ShouldSerializeNull || SomeNullableInt.HasValue;

        [Newtonsoft.Json.JsonProperty(PropertyName = "sdt")]
        public DateTime SomeDateTime { get; set; }
        public bool ShouldSerializeSomeDateTime() => true;

        [Newtonsoft.Json.JsonProperty(PropertyName = "sndt")]
        public DateTime? SomeNullableDateTime { get; set; }
        public bool ShouldSerializeSomeNullableDateTime() => ShouldSerializeNull || SomeNullableDateTime.HasValue;

        [Newtonsoft.Json.JsonProperty(PropertyName = "sd")]
        public decimal SomeDecimal { get; set; }
        public bool ShouldSerializeSomeDecimal() => true;

        [Newtonsoft.Json.JsonProperty(PropertyName = "snd")]
        public decimal? SomeNullableDecimal { get; set; }
        public bool ShouldSerializeSomeNullableDecimal() => ShouldSerializeNull || SomeNullableDecimal.HasValue;
        
        [Newtonsoft.Json.JsonProperty(PropertyName = "ss")]
        public string SomeString { get;set; }
        public bool ShouldSerializeSomeString() => ShouldSerializeNull || SomeString != null;

        [Newtonsoft.Json.JsonProperty(PropertyName = "suv")]
        public UniqueValues SomeUniqueValue { get; set; }
        public bool ShouldSerializeSomeUniqueValue() => ShouldSerializeNull;
    }

    public enum UniqueValues
    {
        Zero = 0,
        One = 1,
        Two = 2,
        Three = 3
    }
}