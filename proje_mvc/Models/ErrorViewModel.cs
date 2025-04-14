namespace proje_mvc.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
    namespace BtkAkademi.Models
    {
        public class Candidate
        {
            public string? Email { get; set; } = string.Empty;
            public string? FirstName { get; set; } = string.Empty;
            public string? LastName { get; set; } = string.Empty;
        }
    }
}