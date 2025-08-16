namespace GymFitnessTracker.ErrorHandling
{
    public class ErrorResponse
    {
        public string Error { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }

        public ErrorResponse(string error, int statusCode, string message)
        {
            Error = error;
            StatusCode = statusCode;
            Message = message;
        }
    }
}
