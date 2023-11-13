namespace JsonLdExtensions
{
    internal class IdentifierIssuer
    {
        internal string Prefix {get; private set;}
        int IdentifierCounter { get; set; } = 0;
        Dictionary<string, string> IssuedIdentifiers { get; set; } = new Dictionary<string, string>();

        internal IdentifierIssuer(string prefix)
        {
            Prefix = prefix;
        }

        internal IdentifierIssuer(IdentifierIssuer issuer)
        {
            Prefix = issuer.Prefix;
            IdentifierCounter = issuer.IdentifierCounter;
            IssuedIdentifiers = new Dictionary<string, string>(issuer.IssuedIdentifiers);
        }

        // 4.5 Issue Identifier Algorithm
        internal string IssueIdentifier(string identifier)
        {
            // 1) If there is already an issued identifier for existing identifier in issued identifiers list, return it.
            if (IssuedIdentifiers.ContainsKey(identifier))
            {
                return IssuedIdentifiers[identifier];
            }
            // 2) Generate issued identifier by concatenating identifier prefix with the string value of identifier counter.
            var issuedIdentifier = $"{Prefix}{IdentifierCounter}";
            // 3) Append an item to issued identifiers list that maps existing identifier to issued identifier.
            IssuedIdentifiers.Add(identifier, issuedIdentifier);
            // 4) Increment identifier counter.
            IdentifierCounter++;
            // 5) Return issued identifier.
            return issuedIdentifier;
        }

        internal string? GetIdentifier(string identifier)
        {
            if (IssuedIdentifiers.ContainsKey(identifier))
            {
                return IssuedIdentifiers[identifier];
            }
            return null;
        }

        internal bool IsIssued(string identifier)
        {
            return IssuedIdentifiers.ContainsKey(identifier);
        }
    }
}
