namespace Experimentation.Models.Graphs.Transitions
{
    public class Transition<TLabel>
    {
        public bool Exists { get; set; }

        public TLabel Label { get; set; } = default;
    }
}