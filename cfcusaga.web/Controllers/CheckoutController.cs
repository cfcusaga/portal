﻿using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using cfcusaga.domain.Orders;
using  Cfcusaga.Web.Configuration;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Cfcusaga.Web.Models;
using SendGrid;
using Order = cfcusaga.domain.Orders.Order;

namespace  Cfcusaga.Web.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly IShoppingCartService _svc;
        AppConfigurations appConfig = new AppConfigurations();

        public List<String> CreditCardTypes { get { return appConfig.CreditCardType;} }

        public CheckoutController(IShoppingCartService svc)
        {
            _svc = svc;
        }


        // GET: /Checkout/AddressAndPayment
        public ActionResult AddressAndPaymentWithCard()
        {
            ViewBag.CreditCardTypes = CreditCardTypes;
            var previousOrder = _svc.GetOrderByIdentity(User.Identity.Name);

            if (previousOrder != null)
                return View(previousOrder);
            else
                return View();
        }

        public ActionResult AddressAndPayment()
        {
            ViewBag.CreditCardTypes = CreditCardTypes;
            //var previousOrder = storeDB.Orders.FirstOrDefault(x => x.Username == User.Identity.Name);
            var previousOrder = _svc.GetOrderByIdentity(User.Identity.Name);

            if (previousOrder != null)
                return View(previousOrder);
            else
            {
                var sessionInfo = ItemRegistrationsController.GetSessionCurrentRegistrationInfo(this.HttpContext);
                var sessionOrder = new Order();
                if (sessionInfo != null)
                {
                    sessionOrder.LastName = sessionOrder.LastName;
                    sessionOrder.FirstName = sessionOrder.FirstName;
                    sessionOrder.IsAgreeToWaiver = false;
                    sessionOrder.Country = "USA";
                    sessionOrder.Email = sessionOrder.Email;
                    return View(sessionOrder);
                }
                else
                {
                    return View();
                }
            }
        }

        //
        // POST: /Checkout/AddressAndPayment
        [HttpPost]
        public async Task<ActionResult> AddressAndPayment(FormCollection values)
        {
            var order = new cfcusaga.domain.Orders.Order();
            try
            {
                //ViewBag.CreditCardTypes = CreditCardTypes;
                //string result =  values[9];
                //if (!ModelState.IsValid) return View(values); ;
               
                TryUpdateModel(order);
                if (!ModelState.IsValid) return View(order);
                    //TODO: Need to store this?
                    //order.CreditCard = result;

                    order.Username = User.Identity.Name;
                //order.Email = User.Identity.Name;
                var timeUtc = DateTime.UtcNow;
                var easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                var easternTime = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, easternZone);
                order.OrderDate = easternTime;
                var currentUserId = User.Identity.GetUserId();

                    //TODO: force set to true
                    order.SaveInfo = true;
                    //TODO: temp solution
                    order.State = order.State.ToUpper();
                    switch (order.State)
                    {
                        case "FLORIDA":
                            order.State = "FL";
                            break;
                        case "GEORGIA":
                            order.State = "GA";
                            break;
                    }
                    if (order.SaveInfo && !order.Username.Equals("guest@guest.com"))
                    {
                        var manager =
                            new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                        var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
                        var ctx = store.Context;
                        var currentUser = manager.FindById(User.Identity.GetUserId());

                        currentUser.Address = order.Address;
                        currentUser.City = order.City;
                        currentUser.Country = order.Country;
                        currentUser.State = order.State;
                        currentUser.Phone = order.Phone;
                        currentUser.PostalCode = order.PostalCode;
                        currentUser.FirstName = order.FirstName;
                        //Save this back
                        //http://stackoverflow.com/questions/20444022/updating-user-data-asp-net-identity
                        //var result = await UserManager.UpdateAsync(currentUser);
                        await ctx.SaveChangesAsync();

                        //await storeDB.SaveChangesAsync();
                        //await _svc.SaveChangesAsync();
                    }

                    var cart = ShoppingCart.GetCart(this.HttpContext, _svc);

                    var cartItems = await cart.GetCartItems();
                    //Save Order
                    var memberId = await _svc.AddOrder(order, cartItems, cart.ShoppingCartId, currentUserId);
                    //if (memberId > 0)
                    //{
                    //    UpdateLoginUserMemberId(memberId);

                    //}

                    //order = cart.CreateOrder(order);
                    //order = await cart.CreateOrderDetails(order);
                    var emailBody = new StringBuilder(order.ToString(order));
                    emailBody.Append("<p>");
                    emailBody.Append("Thank you for registering!<br/>");
                    emailBody.Append("Please write your check payable to \"CFC-GA\". On the memo line, please write \"Family Conference\" and your registration confirmation #. Mail your check to: <br /><br />");
                    emailBody.Append("Allan/Kirsten Tejano <br />");
                    emailBody.Append("6425 Jonabell Lane <br />");
                    emailBody.Append("Cumming, GA 30040 <br />");
                    emailBody.Append("</p>");
                    await CheckoutController.SendOrderMessage_SendGrid(order.Email, "Your Registration #: " + order.OrderId,
                            emailBody.ToString(), appConfig.OrderEmail);
                        //CheckoutController.SendOrderMessage_SendGrid(order.FirstName, "New Order: " + order.OrderId,order.ToString(), appConfig.OrderEmail);
                    return RedirectToAction("Complete",
                            new { id = order.OrderId });
                
            }
            catch (Exception ex)
            {
                //Invalid - redisplay with errors
                return View(order);
            }
        }

        private async void UpdateLoginUserMemberId(int memberId)
        {
            var manager =
                new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
            var ctx = store.Context;
            var currentUser = manager.FindById(User.Identity.GetUserId());

            if (currentUser != null)
            {
                //currentUser.MemberId = memberId;
                //await ctx.SaveChangesAsync();
            }
        }

        //
        // GET: /Checkout/Complete
        public ActionResult Complete(int id)
        {
            // Validate customer owns this order
            bool isValid = _svc.IsValidOrder(id, User.Identity.Name);

            if (isValid)
            {
                return View(id);
            }
            else
            {
                return View("Error");
            }
        }

        //private static RestResponse SendOrderMessage(String toName, String subject, String body, String destination)
        //{
        //    RestClient client = new RestClient();
        //    //fix this we have this up top too
        //    AppConfigurations appConfig = new AppConfigurations();
        //    // TODO: this is free emai service
        //    client.BaseUrl = "https://api.mailgun.net/v2";
        //    client.Authenticator =
        //           new HttpBasicAuthenticator("api",
        //                                      appConfig.EmailApiKey);
        //    RestRequest request = new RestRequest();
        //    request.AddParameter("domain",
        //                        appConfig.DomainForApiKey, ParameterType.UrlSegment);
        //    request.Resource = "{domain}/messages";
        //    request.AddParameter("from", appConfig.FromName + " <" + appConfig.FromEmail + ">");
        //    request.AddParameter("to", toName + " <" + destination + ">");
        //    request.AddParameter("subject", subject);
        //    request.AddParameter("html", body);
        //    request.Method = Method.POST;
        //    IRestResponse executor = client.Execute(request);
        //    return executor as RestResponse;
        //}
        //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


        public static async  Task SendOrderMessage_SendGrid(String toName, String subject, String body, String destination)
        {
            try
            {

                var appConfig = new AppConfigurations();

                var myMessage = new SendGridMessage();
                myMessage.AddTo(toName);
                myMessage.From = new MailAddress(appConfig.FromEmail, appConfig.FromName);
                myMessage.Subject = subject;
                myMessage.Bcc = new MailAddress[] {new MailAddress("kidsforchrist.ga@gmail.com","CFC Kids GA")};
                myMessage.Html = body;

                var transportWeb = new SendGrid.Web(appConfig.EmailApiKey);

                await transportWeb.DeliverAsync(myMessage);
            }
            catch (Exception ex)
            {
                //TODO: remove this
                Console.WriteLine(ex.Message);
                //throw;
            }
        }











        //private static async void SendOrderMessage_SendGrid(String toName, String subject, String body, String destination)
        //{
        //    var myMessage = new SendGrid.SendGridMessage();
        //    myMessage.AddTo(toName);
        //    myMessage.From = new MailAddress("kidsforchrist.ga@gmail.com", "CFC Kids For Christ - GA");
        //    myMessage.Subject = subject;//"Sending with SendGrid is Fun";
        //    //myMessage.Text = body;
        //    myMessage.Html = body;

        //    var credentials = new NetworkCredential("azure_adb1c1f9b5383a3339cebd125489d765@azure.com", "sndgrdpswd1");

        //    // Create an Web transport for sending email.
        //    var transportWeb = new SendGrid.Web(credentials);

        //    //var transportWeb = new SendGrid.Web("SENDGRID_APIKEY");
        //    await transportWeb.DeliverAsync(myMessage);
        //    //transportWeb.Deliver(myMessage);//.Wait();

        //    //RestClient client = new RestClient();
        //    ////fix this we have this up top too
        //    //AppConfigurations appConfig = new AppConfigurations();
        //    //// TODO: this is free emai service
        //    //client.BaseUrl = "https://api.mailgun.net/v2";
        //    //client.Authenticator =
        //    //       new HttpBasicAuthenticator("api",
        //    //                                  appConfig.EmailApiKey);
        //    //RestRequest request = new RestRequest();
        //    //request.AddParameter("domain",
        //    //                    appConfig.DomainForApiKey, ParameterType.UrlSegment);
        //    //request.Resource = "{domain}/messages";
        //    //request.AddParameter("from", appConfig.FromName + " <" + appConfig.FromEmail + ">");
        //    //request.AddParameter("to", toName + " <" + destination + ">");
        //    //request.AddParameter("subject", subject);
        //    //request.AddParameter("html", body);
        //    //request.Method = Method.POST;
        //    //IRestResponse executor = client.Execute(request);
        //    //return executor as RestResponse;
        //}

    }
}