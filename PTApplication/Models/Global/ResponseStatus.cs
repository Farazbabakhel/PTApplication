namespace PTApplication.Models.Global
{
    enum ResponseStatus
    {
        Success = 200,
        Created = 201,
        NotFound = 404,
        BadRequest = 400,
        InternalServerError = 500,
        Fails = 525,
        UnAuthorize = 401,
        Forbidden = 403,
        Timeout = 408,
        Conflict = 409
    }


    enum TransactionStatus{
        Recharge = 1,
        Deduction= 2,
        Transfer= 3
    }


    enum RequestReplyStatus
    {
        ApprovalPending = 1,
        Approved = 2,
        Rejected = 3

    }
}
