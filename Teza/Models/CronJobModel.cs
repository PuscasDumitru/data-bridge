namespace Teza.Models
{
     public class CronJobModel
     {
          public string ConnectionString { get; set; }
          public int DbType { get; set; }
          public string EmailList { get; set; }
          public string CronExpresion { get; set; }
          public string Query { get; set; }
     }
}
