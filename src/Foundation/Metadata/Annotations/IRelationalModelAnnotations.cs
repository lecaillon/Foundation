namespace Foundation.Metadata.Annotations
{
    public interface IRelationalModelAnnotations
    {
        //ISequence FindSequence(string name, string schema = null);
        //IReadOnlyList<ISequence> Sequences { get; }
        string DefaultSchema { get; set; }
        string DatabaseName { get; set; }
    }
}
