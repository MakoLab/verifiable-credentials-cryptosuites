namespace TestWebAPI
{
    public class MockData
    {
        public static string Controller { get; set; } = "https://example.edu/issuers/565049";
        public static string PublicKeyMultibase { get; set; } = "zDnaekGZTbQBerwcehBSXLqAg6s55hVEBms1zFy89VHXtJSa9";
        public static string SecretKeyMultibase { get; set; } = "z42tqZ5smVag3DtDhjY9YfVwTMyVHW6SCHJi2ZMrD23DGYS3";
        public static string Id { get; set; } = $"{Controller}#{PublicKeyMultibase}";
    }
}
