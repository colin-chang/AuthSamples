namespace ColinChang.ApiSample.Models
{
    public class JwtOptions
    {
        public string ValidIssuer { get; set; }
        public string ValidAudience { get; set; }
        public string IssuerSigningKey { get; set; }
        public int Expires { get; set; }
    }
}