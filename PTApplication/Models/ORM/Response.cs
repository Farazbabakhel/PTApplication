namespace PTApplication.Models.ORM
{
    
    public class Response
    {
        public string name { get; set; }
        public string heading { get; set; }
        public string summary { get; set; }
        public string message { get; set; }
        public string exceptionMessage { get; set; }
        public int status { get; set; }
        public bool isSuccess { get; set; }
        public DateTime creationTime { get; set; }
        public object responseObject { get; set; }

        public Response()
        {
            status = 200;
            isSuccess = true;
            creationTime = Convert.ToDateTime(System.DateTime.Now);
            heading = "Success";
            message = string.Empty;
            name = string.Empty;
            summary = string.Empty;
            responseObject = new object();
        }
        public Response(Exception ex)
        {
            status = 500;
            isSuccess = false;
            exceptionMessage = ex.ToString();
            heading = "Error!";
            message = ex.Message;
            if (ex.InnerException != null)
            {
                summary = ex.InnerException.ToString();
            }
            creationTime = Convert.ToDateTime(System.DateTime.Now);
            responseObject = new object();
        }

    }
}
