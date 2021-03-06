namespace AutomatonAlgorithms.DataStructures.Graphs.Nodes
{
    public class BasicNode : INode
    {
        public string Id { get; set; }

        public bool Equals(INode other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return other.Id == Id;
        }

        protected bool Equals(BasicNode other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((BasicNode) obj);
        }

        public override int GetHashCode()
        {
            return Id != null ? Id.GetHashCode() : 0;
        }
    }
}