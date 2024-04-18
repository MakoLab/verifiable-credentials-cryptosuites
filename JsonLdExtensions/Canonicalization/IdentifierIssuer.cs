using System.Collections.Specialized;

namespace JsonLdExtensions.Canonicalization
{
    internal class IdentifierIssuer
    {
        internal string Prefix { get; private set; }
        int IdentifierCounter { get; set; } = 0;
        internal OrderedDictionary IssuedIdentifiers { get; private set; } = new();

        internal IdentifierIssuer(string prefix)
        {
            Prefix = prefix;
        }

        internal IdentifierIssuer(IdentifierIssuer issuer)
        {
            Prefix = issuer.Prefix;
            IdentifierCounter = issuer.IdentifierCounter;
            foreach (var key in issuer.IssuedIdentifiers.Keys)
            {
                IssuedIdentifiers.Add(key, issuer.IssuedIdentifiers[key]);
            }
        }

        // 4.5 Issue Identifier Algorithm
        internal string IssueIdentifier(string identifier)
        {
            // 1) If there is already an issued identifier for existing identifier in issued identifiers list, return it.
            if (IssuedIdentifiers.Contains(identifier))
            {
                return (string)IssuedIdentifiers[identifier]!;
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
            if (IssuedIdentifiers.Contains(identifier))
            {
                return (string)IssuedIdentifiers[identifier]!;
            }
            return null;
        }

        internal bool IsIssued(string identifier)
        {
            return IssuedIdentifiers.Contains(identifier);
        }

        internal IEnumerable<string> GetExistingIdentifiers()
        {
            return IssuedIdentifiers.Keys.Cast<string>();
        }
    }
}
