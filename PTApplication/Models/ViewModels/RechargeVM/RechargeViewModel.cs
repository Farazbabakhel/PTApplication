using Org.BouncyCastle.Asn1.Ocsp;
using PTApplication.Models.DBContext;
using PTApplication.Models.Global;
using PTApplication.Models.ORM;
using System;
using System.ComponentModel;
using System.Linq;

namespace PTApplication.Models.ViewModels.RechargeVM
{
    public class RechargeViewModel
    {
        private readonly PTAppDBContext db;
        private readonly IConfiguration configuration;
        private readonly HttpContext httpContext;
        public RechargeViewModel(PTAppDBContext _db, IConfiguration _configuration, HttpContext _httpContext)
        {
            db = _db;
            configuration = _configuration;
            httpContext = _httpContext;
        }


        public Response GetMyBalance()
        {
            Response response;
            try
            {
                Guid? uID = Utilities.GetIdentity(httpContext).userID;
                response = new Response();
                Balance balance = db.Balance.Where(x=>x.userID==uID).FirstOrDefault();
                if (balance != null)
                {
                    balance.rechargeHistory = db.Recharge.Where(a => a.userID == uID && a.isActive == true).ToList();
                }
                else
                {
                    balance = new Balance();
                }    
                response.responseObject = balance;
            }
            catch (Exception ex)
            {
                response = new Response(ex);
            }
            return response;
        }
        public Response GetMyBalanceByRechargeID(Guid rechargeID)
        {
            Response response;
            try
            {
                Guid? uID = Utilities.GetIdentity(httpContext).userID;
                response = new Response();
                Balance balance = db.Balance.Where(x => x.userID == uID).FirstOrDefault();
                if (balance != null)
                {
                    balance.rechargeHistory = db.Recharge.Where(a => a.userID == uID && a.rechargeID== rechargeID && a.isActive == true).ToList();
                }
                response.responseObject = balance;
            }
            catch (Exception ex)
            {
                response = new Response(ex);
            }
            return response;
        }
        public Response GetUserBalance(Guid userID)
        {
            Response response;
            try
            {
                response = new Response();
                Balance balance = db.Balance.Where(x => x.userID == userID).FirstOrDefault();
                if (balance != null)
                {
                    balance.rechargeHistory = db.Recharge.Where(a => a.userID == userID && a.isActive == true).ToList();
                }
                response.responseObject = balance;
            }
            catch (Exception ex)
            {
                response = new Response(ex);
            }
            return response;
        }

        public Response RechargeMyBalance(Recharge recharge)
        {
            Response response;
            try
            {
                response = new Response();
                if (recharge.amount > 0)
                {
                    Guid? uID = Utilities.GetIdentity(httpContext).userID;
                    Balance balance = db.Balance.Where(x => x.userID == uID).FirstOrDefault();
                    Package package = db.Packages.Where(x => x.code == recharge.packageCode).FirstOrDefault();
                    if (package.price != recharge.amount)
                    {
                        throw new Exception("Package Invalid Package Amount");
                    }
                    if (package != null && package.credits!=0)
                    {
                        recharge.credits= package.credits;
                    }
                    

                    if (balance == null)
                    {
                        Balance Temp = new Balance();
                        Temp.balanceID = Guid.NewGuid();
                        Temp.userID = uID;
                        Temp.amount = recharge.amount;
                        Temp.credits = package.credits;

                        Temp.createdBy = uID;
                        Temp.creationDate = Convert.ToDateTime(System.DateTime.Now);
                        db.Balance.Add(Temp);
                        
                        recharge.userID=uID;
                        recharge.createdBy = uID;
                        recharge.rechargeType = "Recharge";
                        recharge.rechargeDate = System.DateTime.Now.ToShortDateString();
                        recharge.creationDate= Convert.ToDateTime(System.DateTime.Now);

                        recharge.rechargeID = Guid.NewGuid();
                        recharge.balanceID = Temp.balanceID;
                        recharge.openingBalance = Temp.amount;
                        recharge.closingBalance = 0;

                        recharge.openingCredits = Temp.credits;
                        recharge.closingCredits = 0;


                        recharge.transactionType = TransactionStatus.Recharge.ToString();
                        recharge.rechargeStatus = "Recharged";

                        db.Recharge.Add(recharge);
                        db.SaveChanges();
                        response.message = "Balance Rechange Successfully";
                        response.summary= "Balance Rechange Successfully";
                    }
                    else
                    {
                        recharge.closingBalance = balance.amount;
                        balance.amount=balance.amount+recharge.amount;
                        recharge.openingBalance = balance.amount;

                        recharge.closingCredits = balance.credits;
                        balance.credits = balance.credits + recharge.credits;
                        recharge.openingCredits = balance.credits;

                        db.Entry(balance).CurrentValues.SetValues(balance);
                        db.SaveChanges();
                        recharge.userID = uID;
                        recharge.createdBy = uID;
                        recharge.rechargeType = "Recharge";
                        recharge.rechargeDate = System.DateTime.Now.ToShortDateString();
                        recharge.creationDate = Convert.ToDateTime(System.DateTime.Now);
                        recharge.rechargeID = Guid.NewGuid();
                        recharge.balanceID = balance.balanceID;
                        recharge.transactionType = TransactionStatus.Recharge.ToString();
                        recharge.rechargeStatus = "Recharged";


                        db.Recharge.Add(recharge);
                        db.SaveChanges();

                        response.message = "Balance Rechange Successfully";
                        response.summary = "Balance Rechange Successfully";
                    }
                }
                else
                {
                    response.status = (int)ResponseStatus.Forbidden;
                    response.isSuccess = false;
                    response.message = "Invalid Recharge Amount.";
                    response.summary = "Invalid Recharge Amount.";
                }
            }
            catch (Exception ex)
            {
                response = new Response(ex);
            }
            return response;
        }
        
        public Response DeductBalance(Recharge recharge)
        {
            Response response;
            try
            {
                int TotalCredits = 0;
                List<UserBioAnswer> bioAnswers = db.UserBioAnswers.Where(x => x.userID == recharge.clientID && x.isActive == true).ToList();
                foreach (var ans in bioAnswers)
                {
                    BioOptions option = db.BioOptions.Where(x => x.optionID == ans.optionID).First();
                    if (option != null)
                    {
                        TotalCredits = TotalCredits+ (option.points??0);
                    }
                }
                if (TotalCredits != 0)
                {
                    recharge.credits = TotalCredits;
                }

                if (recharge.amount > 0)
                {
                    recharge.amount = recharge.amount * (-1);
                }
                if(recharge.credits>0)
                {
                    recharge.credits = recharge.credits * (-1);
                }

                response = new Response();
                
                if ( recharge.credits<0) // recharge.amount < 0 &&
                {
                    Guid? uID = Utilities.GetIdentity(httpContext).userID;
                    Balance balance = db.Balance.Where(x => x.userID == uID).FirstOrDefault();
                    Conversion conversion = db.Conversions.Where(x => x.clientID == recharge.clientID && x.ptID == uID && x.isActive == true).FirstOrDefault();
                    if (conversion == null)
                    {
                        if (balance != null)
                        {
                            if (!(db.Conversions.Any(x => x.clientID == recharge.clientID && x.ptID == uID && x.isActive == true)))
                            {
                                User user = db.Users.Where(x => x.userID == recharge.clientID && x.isActive == true).FirstOrDefault();
                                if (user!=null)
                                {
                                    /* ------------------------------------- DISCOUNT --------------------------------------------------  */
                                    
                                        user.isDiscountApplicable = true;
                                        try
                                        {
                                            if (!db.RequestReply.Any(x => x.clientID == user.userID && x.pTID == uID && x.statusID == (int)RequestReplyStatus.Approved && x.isActive == true))
                                            {
                                                user.isDiscountApplicable = false;
                                            }
                                            if (db.Conversions.Count(x => x.clientID == user.userID && x.isActive == true) >= 5)
                                            {
                                                user.isDiscountApplicable = false;
                                            }

                                            DateTime startDate = Convert.ToDateTime(user.profileCompleteDate);
                                            DateTime endDate = System.DateTime.Now;
                                            double days = (endDate - startDate).TotalDays;
                                            int totalTillNow = db.Conversions.Where(x => x.clientID == recharge.clientID && x.isActive == true).Count();
                                            if (days >= 8 && user.isDiscountApplicable == true)
                                            {
                                                user.isDiscountApplicable = true;
                                                decimal discount = (decimal)((recharge.credits * 30) / 100);
                                                recharge.credits = recharge.credits - discount;

                                        }
                                            else
                                            {
                                                user.isDiscountApplicable = false;
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            user.isDiscountApplicable = false;
                                        }
                                    
                                    /*else
                                    {
                                        RequestReply req_rep = db.RequestReply.Where(x => x.clientID == recharge.clientID && x.pTID == uID && x.statusID== (int)RequestReplyStatus.Approved && x.isActive == true).FirstOrDefault();
                                        if (req_rep != null)
                                        {
                                            try
                                            {
                                                DateTime startDate = Convert.ToDateTime(user.profileCompleteDate);
                                                DateTime endDate = System.DateTime.Now;
                                                double days = (endDate - startDate).TotalDays;
                                                if (days >= 7)
                                                {
                                                    decimal discount = (decimal)((recharge.credits * 30) / 100);
                                                    recharge.credits = recharge.credits - discount;
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                            }
                                        }

                                    }*/
                                    /* -----------------------------------------------------------------------------------------------*/


                                    if (balance.credits >= recharge.credits)
                                    {
                                        

                                        Balance Temp = new Balance();

                                        recharge.closingBalance = balance.amount;
                                        balance.amount = balance.amount + recharge.amount;
                                        recharge.openingBalance = balance.amount;

                                        recharge.closingCredits = balance.credits;
                                        balance.credits = balance.credits + recharge.credits;
                                        recharge.openingCredits = balance.credits;

                                        db.Entry(balance).CurrentValues.SetValues(balance);
                                        db.SaveChanges();

                                        recharge.userID = uID;
                                        recharge.createdBy = uID;
                                        recharge.rechargeType = "Recharge";
                                        recharge.rechargeDate = System.DateTime.Now.ToShortDateString();
                                        recharge.creationDate = Convert.ToDateTime(System.DateTime.Now);
                                        recharge.rechargeID = Guid.NewGuid();
                                        recharge.balanceID = Temp.balanceID;
                                        recharge.openingBalance = Temp.amount;
                                        recharge.closingBalance = 0;
                                        recharge.transactionType = TransactionStatus.Deduction.ToString();
                                        recharge.rechargeStatus = "Deduction";
                                        db.Recharge.Add(recharge);
                                        db.SaveChanges();

                                        Conversion newConversion = new Conversion();
                                        newConversion.conversionID = Guid.NewGuid();
                                        newConversion.clientID = recharge.clientID;
                                        newConversion.ptID = recharge.userID;
                                        newConversion.rechargeID = recharge.rechargeID;
                                        newConversion.createdBy = recharge.userID;
                                        newConversion.creationDate = Convert.ToDateTime(System.DateTime.Now);
                                        db.Conversions.Add(newConversion);
                                        db.SaveChanges();

                                        int total=db.Conversions.Where(x=>x.clientID==recharge.clientID && x.isActive==true).Count();
                                        if(total>=5)
                                        {
                                            User requestor = db.Users.Where(x => x.userID == recharge.clientID).FirstOrDefault();
                                            if (requestor != null)
                                            {
                                                requestor.isDiscountApplicable = false;
                                                db.Entry(requestor).CurrentValues.SetValues(requestor);
                                                db.SaveChanges();
                                            }
                                        }
                                        response.message = "Balance Deducted Successfully";
                                        response.summary = "Balance Deducted Successfully";
                                    }
                                    else
                                    {
                                        response.status = (int)ResponseStatus.NotFound;
                                        response.isSuccess = false;
                                        response.message = "Low Balance. Please Recharge Your Account.";
                                        response.summary = "Low Balance. Please Recharge Your Account.";
                                    }
                                }
                                else
                                {
                                    response.status = (int)ResponseStatus.NotFound;
                                    response.isSuccess = false;
                                    response.message = "Client Account Not Fount.";
                                    response.summary = "Client Account Not Fount.";
                                }
                            }
                            else
                            {
                                response.status = (int)ResponseStatus.Forbidden;
                                response.isSuccess = false;
                                response.message = "Already Paid.";
                                response.summary = "You Have Already Paid For This Client.";
                            }

                        }
                        else
                        {
                            response.status = (int)ResponseStatus.NotFound;
                            response.isSuccess = false;
                            response.message = "Account Not Fount.";
                            response.summary = "Account Not Fount.";
                        }
                    }
                    else
                    {
                        response.status = (int)ResponseStatus.Conflict;
                        response.isSuccess = false;
                        response.message = "This Client Is Already In Your Conversion List.";
                        response.summary = "This Client Is Already In Your Conversion List.";
                    }
                }
                else
                {
                    response.status = (int)ResponseStatus.Forbidden;
                    response.isSuccess = false;
                    response.message = "Invalid Deducted Credit.";
                    response.summary = "Invalid Deducted Credit.";
                }
            }
            catch (Exception ex)
            {
                response = new Response(ex);
            }
            return response;
        }

    }
}
