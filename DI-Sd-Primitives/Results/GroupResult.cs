namespace DI_Sd_Primitives.Results
{
    public class GroupResult
    {
        public required IDictionary<int, string> Matching { get; set; }
        public required IDictionary<int, string> NonMatching { get; set; }
        public required IList<string> DeskolemizedNQuads { get; set; }
    }
}
