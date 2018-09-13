namespace DumbSearchEngine
{
    public  class DocumentRef
    {
        public string Path { get; set; }

        public override int GetHashCode() => Path.GetHashCode();

        public override bool Equals(object obj) => this == obj as DocumentRef;

        public static bool operator ==(DocumentRef left, DocumentRef right) => left?.Path == right?.Path;
        public static bool operator !=(DocumentRef left, DocumentRef right) => left?.Path != right?.Path;
    }
}