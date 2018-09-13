namespace DumbSearchEngine
{
    public class Document
    {
        public DocumentRef Reference { get; set; }

        public string Content { get; set; }

        public override int GetHashCode() => Reference.GetHashCode();
    }
}