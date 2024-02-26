namespace EcommerceDotNetCore.Configurations;

public class Smtp
{
    public string MailHost { get; set; }=string.Empty;
    
    public int MailPort { get; set; } 
    
    public string MailUsername { get; set; }=string.Empty;
    
    public string MailPassword { get; set; }=string.Empty;
    
    public string MailEncryption { get; set; }=string.Empty;
    
    public string MailFromAddress { get; set; }=string.Empty;
    
    public string MailFromName { get; set; }=string.Empty;
    

}