using SQLite;


namespace IphoneCollector.Data
{
    [Table("case")]
    public class Case
    {
        [PrimaryKey]
        [AutoIncrement]
        [Column("id")]
        public int Id { get; set; }
        
        [Column("case_name")]
        public string CaseName { get; set; }
        
        [Column("examiner_name")]
        public string ExaminerName { get; set; }
        
        [Column("matter_number")]
        public string MatterNumber { get; set; }
        
        [Column("storage_location")]
        public string StorageLocation { get; set; }
    }
}
