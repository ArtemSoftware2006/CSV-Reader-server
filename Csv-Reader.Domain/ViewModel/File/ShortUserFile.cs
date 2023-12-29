namespace Csv_Reader.Domain.ViewModel.File
{
    public class ShortUserFile
    {
        public string Name { get; set; }       
        public Guid Id { get; set; }
        public ShortUserFile(string name, Guid id)
        {
            Name = name;
            Id = id;
        }
    }
}