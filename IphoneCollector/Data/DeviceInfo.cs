using SQLite;

namespace IphoneCollector.Data
{
    [Table("DeviceInfo")]
    public class DeviceInfo
    {

        [PrimaryKey]
        [AutoIncrement]
        [Column("id")]
        public int Id { get; set; }

        [Column("custodian_name")]
        public string CustodianName { get; set; }

        [Column("iphone_model")]
        public string IphoneModel { get; set; }

        [Column("phone_Number")]
        public string PhoneNumber { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("notes")]
        public string Notes { get; set; }

    }
}
