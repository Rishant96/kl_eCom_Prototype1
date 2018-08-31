using kl_eCom.Web.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using kl_eCom.Web.Areas.Vendors.Models;
using kl_eCom.Web.Entities;
using System.Net.Mail;
using System.Net;

namespace kl_eCom.Web.Areas.Vendors.Controllers
{
    [Authorize(Roles = "Vendor")]
    public class CustomersController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        // GET: Vendors/Customers
        public ActionResult Index()
        {
            var vndrId = User.Identity.GetUserId();

            var vendor = db.EcomUsers
                           .FirstOrDefault(m => m.ApplicationUserId == vndrId);
            var customers = db.Refferals
                .Where(m => m.VendorId == vendor.Id)
                .Select(m => m.CustomerId)
                .ToList();

            var model = new CustomersIndexViewModel
            {
                Customers = db.EcomUsers
                    .Include(m => m.User)
                    .Where(m => customers.Contains(m.Id))
                    .OrderBy(m => m.User.FirstName + " " + m.User.LastName)
                    .ToList(),
                Buyers = new Dictionary<string, bool>(),
                Registrations = new Dictionary<string, bool>()
            };

            foreach (var customer in model.Customers)
            {
                var rel = db.Refferals
                    .FirstOrDefault(m => m.CustomerId == customer.Id 
                    && m.VendorId == vendor.Id);

                if (rel.IsBuyer == true)
                    model.Buyers.Add(customer.ApplicationUserId, true);
                else
                    model.Buyers.Add(customer.ApplicationUserId, false);

                if (rel.IsRegisteredUser == true)
                    model.Registrations.Add(customer.ApplicationUserId, true);
                else
                    model.Registrations.Add(customer.ApplicationUserId, false);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delivery(string userId)
        {
            var bulkDec = Request.Form["DeliverAll"];
            if (!string.IsNullOrEmpty(bulkDec))
            {
                var vendorId = User.Identity.GetUserId();
                var ecomUser = db.EcomUsers
                    .FirstOrDefault(m => 
                    m.ApplicationUserId == vendorId);
                var paramUser = db.EcomUsers.FirstOrDefault(m => m.ApplicationUserId
                        == userId);
                var allactives = (paramUser != null) ?
                                db.OrderItems
                                    .Include(m => m.Order)
                                    .Include(m => m.Order.Customer)
                                    .Include(m => m.Order.Customer.User)
                                    .Where(m => m.EcomUserId == ecomUser.Id
                                    && (m.Status == Utilities.OrderStatus.ActiveOrder
                                        || m.Status == Utilities.OrderStatus.NewOrder))
                                    .OrderByDescending(m => m.Order.OrderDate)
                                    .ToList()
                                :
                                db.OrderItems
                                    .Include(m => m.Order)
                                    .Include(m => m.Order.Customer)
                                    .Include(m => m.Order.Customer.User)
                                    .Where(m => m.EcomUserId == ecomUser.Id
                                    && m.Order.EcomUserId == paramUser.Id
                                    && (m.Status == Utilities.OrderStatus.ActiveOrder
                                        || m.Status == Utilities.OrderStatus.NewOrder))
                                    .OrderByDescending(m => m.Order.OrderDate)
                                    .ToList();
                
                if (bulkDec == "all")
                {
                    foreach (var itm in allactives)
                    {
                        string subject = "Khushlife Order #" + itm.Order.OrderNumber + ": Delivery Confirmation";
                        string message = "Auto generated mail confirming successful delivery of the following product:\n\n";


                        itm.Status = Utilities.OrderStatus.Delivered;
                        db.Entry(itm).State = EntityState.Modified;

                        int refId = db.Refferals
                                      .Include(m => m.Customer)
                                      .Include(m => m.Vendor)
                                      .FirstOrDefault(
                                        m => m.Customer.Id == itm.Order.EcomUserId
                                        && m.Vendor.Id == itm.EcomUserId).Id;

                        db.OrderInformation.Add(
                            new Utilities.OrderStateInfo
                            {
                                Type = Utilities.ChangeType.Delivered,
                                InitialDate = DateTime.Now,
                                OrderItemId = itm.Id,
                                RefferalId = refId
                            }
                        );

                        message += "\tProduct: " + itm.ProductName + ", Quantity: " + itm.Qty +
                         ", Value = Rs. " + itm.FinalCost + " (" + itm.Price + " x " + itm.Qty + ")\n";

                        message += "Regards,\nKhuslife E-com Team";

                        FireEmail(itm.Order.Customer.User.Email, subject, message);
                    }
                }
            }
            else
            {
                var idStr = Request.Form["ActiveIds"];
                if (idStr == null) idStr = "";

                var idArr = idStr.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(sValue => sValue.Trim()).ToArray() as string[];
                
                for (int i = 0; i < idArr.Count(); i++)
                {
                    switch (Request.Form["ActiveCheck" + idArr[i]])
                    {
                        case "1": { break; }
                        case "2":
                            {
                                var itmId = int.Parse(idArr[i]);
                                var itm = db.OrderItems
                                    .Include(m => m.Order)
                                    .Include(m => m.Order.Customer)
                                    .Include(m => m.Order.Customer.User)
                                    .FirstOrDefault(m => m.Id
                                        == itmId);
                                if (itm == null) return View("Error");
                                itm.Status = Utilities.OrderStatus.Delivered;
                                db.Entry(itm).State = EntityState.Modified;

                                int refId = db.Refferals
                                        .Include(m => m.Customer)
                                        .Include(m => m.Vendor)
                                        .FirstOrDefault(
                                            m => m.Customer.Id == itm.Order.EcomUserId
                                            && m.Vendor.Id == itm.EcomUserId).Id;

                                db.OrderInformation.Add(
                                    new Utilities.OrderStateInfo
                                    {
                                        Type = Utilities.ChangeType.Delivered,
                                        InitialDate = DateTime.Now,
                                        OrderItemId = itm.Id,
                                        RefferalId = refId
                                    }
                                );

                                string subject = "Khushlife Order #" +itm.Order.OrderNumber + ": Delivery Confirmation";
                                string message = "Auto generated mail confirming successful delivery of the following product:\n\n";

                                message += "\tProduct: " + itm.ProductName + ", Quantity: " + itm.Qty +
                                  ", Value = Rs. " + itm.FinalCost + " (" + itm.Price + " x " + itm.Qty + ")\n";

                                message += "Regards,\nKhuslife E-com Team";

                                FireEmail(itm.Order.Customer.User.Email, subject, message);

                                break;
                            }
                        case "3":
                            {
                                var itmId = int.Parse(idArr[i]);
                                var itm = db.OrderItems
                                    .Include(m => m.Order)
                                    .Include(m => m.Order.Customer)
                                    .FirstOrDefault(m => m.Id
                                        == itmId);
                                if (itm == null) return View("Error");
                                itm.Status = Utilities.OrderStatus.Undelivered;
                                db.Entry(itm).State = EntityState.Modified;

                                int refId = db.Refferals
                                    .Include(m => m.Customer)
                                    .Include(m => m.Customer.User)
                                    .Include(m => m.Vendor)
                                    .FirstOrDefault(
                                        m => m.Customer.Id == itm.Order.EcomUserId
                                        && m.Vendor.Id == itm.EcomUserId).Id;

                                db.OrderInformation.Add(
                                    new Utilities.OrderStateInfo
                                    {
                                        Type = Utilities.ChangeType.DeliveryFailed,
                                        InitialDate = DateTime.Now,
                                        OrderItemId = itm.Id,
                                        RefferalId = refId
                                    }
                                );

                                string subject = "Khushlife Order #" + itm.Order.OrderNumber + ": Delivery Failed";
                                string message = "Auto generated mail confirming unsuccessful delivery of the following product:\n\n";

                                message += "\tProduct: " + itm.ProductName + ", Quantity: " + itm.Qty +
                                  ", Value = Rs. " + itm.FinalCost + " (" + itm.Price + " x " + itm.Qty + ")\n";

                                message += "Regards,\nKhuslife E-com Team";

                                FireEmail(itm.Order.Customer.User.Email, subject, message);

                                break;
                            }
                        default:
                            { break; }
                    }
                }
            }
            db.SaveChanges();

            return RedirectToAction("Orders", new { id = userId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cancellation(string userId)
        {
            var bulkDec = Request.Form["CancelCheck"];
            if (!string.IsNullOrEmpty(bulkDec))
            {
                var vendorId = User.Identity.GetUserId();
                var allrequests = (string.IsNullOrEmpty(userId)) ?
                                db.OrderItems
                                    .Include(m => m.Vendor)
                                    .Include(m => m.Order)
                                    .Include(m => m.Order.Customer)
                                    .Include(m => m.Order.Customer.User)
                                    .Where(m => m.Vendor.ApplicationUserId == vendorId
                                    && m.Status == Utilities.OrderStatus.CancellationRequested)
                                    .OrderByDescending(m => m.Order.OrderDate)
                                    .ToList()
                                :
                                db.OrderItems
                                    .Include(m => m.Vendor)
                                    .Include(m => m.Order)
                                    .Include(m => m.Order.Customer)
                                    .Include(m => m.Order.Customer.User)
                                    .Where(m => m.Vendor.ApplicationUserId == vendorId
                                    && m.Order.Customer.ApplicationUserId == userId
                                    && m.Status == Utilities.OrderStatus.CancellationRequested)
                                    .OrderByDescending(m => m.Order.OrderDate)
                                    .ToList();

                if (bulkDec == "all")
                {
                    foreach (var itm in allrequests)
                    {
                        itm.Status = Utilities.OrderStatus.Cancelled;
                        db.Entry(itm).State = EntityState.Modified;

                        var state = db.OrderInformation.FirstOrDefault(m => m.OrderItemId
                            == itm.Id);
                        state.IsChangePostive = true;
                        state.FinalDate = DateTime.Now;
                        db.Entry(state).State = EntityState.Modified;

                        string subject = "Khushlife Order #" + itm.Order.OrderNumber + ": Cancellation Request Accepted";
                        string message = "Auto generated mail confirming successful cancellaton of the following product:\n\n";

                        message += "\tProduct: " + itm.ProductName + ", Quantity: " + itm.Qty +
                          ", Value = Rs. " + itm.FinalCost + " (" + itm.Price + " x " + itm.Qty + ")\n";

                        message += "Regards,\nKhuslife E-com Team";

                        FireEmail(itm.Order.Customer.User.Email, subject, message);
                    }
                }
                else if (bulkDec == "none")
                {
                    foreach (var itm in allrequests)
                    {
                        itm.Status = Utilities.OrderStatus.ActiveOrder;
                        db.Entry(itm).State = EntityState.Modified;

                        var state = db.OrderInformation.FirstOrDefault(m => m.OrderItemId
                            == itm.Id);
                        state.IsChangePostive = false;
                        state.FinalDate = DateTime.Now;
                        db.Entry(state).State = EntityState.Modified;

                        string subject = "Khushlife Order #" + itm.Order.OrderNumber + ": Cancellation Failed";
                        string message = "Auto generated mail confirming rejection of the cancellaton request for the following product:\n\n";

                        message += "\tProduct: " + itm.ProductName + ", Quantity: " + itm.Qty +
                          ", Value = Rs. " + itm.FinalCost + " (" + itm.Price + " x " + itm.Qty + ")\n";

                        message += "Regards,\nKhuslife E-com Team";

                        FireEmail(itm.Order.Customer.User.Email, subject, message);
                    }
                }

            }
            else
            {

                var idStr = Request.Form["CancelIds"];
                if (idStr == null) idStr = "";

                var idArr = idStr.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(sValue => sValue.Trim()).ToArray() as string[];

                var slctdStr = Request.Form["CancelOption"];
                if (slctdStr == null) slctdStr = "";

                var slctdArr = slctdStr.Split(',')
                    .Select(sValue => sValue.Trim()).ToArray() as string[];

                for (int i = 0; i < idArr.Count(); i++)
                {
                    switch (slctdArr[i])
                    {
                        case "0": { break; }
                        case "1":
                            {
                                var itmId = int.Parse(idArr[i]);
                                var itm = db.OrderItems
                                    .Include(m => m.Order)
                                    .Include(m => m.Order.Customer)
                                    .Include(m => m.Order.Customer.User)
                                    .FirstOrDefault(m => m.Id
                                        == itmId);
                                if (itm == null) return View("Error");
                                itm.Status = Utilities.OrderStatus.Cancelled;
                                db.Entry(itm).State = EntityState.Modified;


                                var state = db.OrderInformation.FirstOrDefault(m => m.OrderItemId
                                    == itm.Id);
                                state.IsChangePostive = true;
                                state.FinalDate = DateTime.Now;
                                db.Entry(state).State = EntityState.Modified;

                                string subject = "Khushlife Order #" + itm.Order.OrderNumber + ": Cancellation Request Accepted";
                                string message = "Auto generated mail confirming successful cancellaton of the following product:\n\n";

                                message += "\tProduct: " + itm.ProductName + ", Quantity: " + itm.Qty +
                                  ", Value = Rs. " + itm.FinalCost + " (" + itm.Price + " x " + itm.Qty + ")\n";

                                message += "Regards,\nKhuslife E-com Team";

                                FireEmail(itm.Order.Customer.User.Email, subject, message);

                                break;
                            }
                        case "2":
                            {
                                var itmId = int.Parse(idArr[i]);
                                var itm = db.OrderItems
                                    .Include(m => m.Order)
                                    .Include(m => m.Order.Customer)
                                    .Include(m => m.Order.Customer.User)
                                    .FirstOrDefault(m => m.Id
                                        == itmId);
                                if (itm == null) return View("Error");
                                itm.Status = Utilities.OrderStatus.ActiveOrder;
                                db.Entry(itm).State = EntityState.Modified;


                                var state = db.OrderInformation.FirstOrDefault(m => m.OrderItemId
                                    == itm.Id);
                                state.IsChangePostive = false;
                                state.FinalDate = DateTime.Now;
                                db.Entry(state).State = EntityState.Modified;

                                string subject = "Khushlife Order #" + itm.Order.OrderNumber + ": Cancellation Failed";
                                string message = "Auto generated mail confirming rejection of the cancellaton request for the following product:\n\n";

                                message += "\tProduct: " + itm.ProductName + ", Quantity: " + itm.Qty +
                                  ", Value = Rs. " + itm.FinalCost + " (" + itm.Price + " x " + itm.Qty + ")\n";

                                message += "Regards,\nKhuslife E-com Team";

                                FireEmail(itm.Order.Customer.User.Email, subject, message);

                                break;
                            }
                        default:
                            { break; }
                    }
                }
            }
            db.SaveChanges();

            return RedirectToAction("Orders", new { id = userId });
        }

        public ActionResult Orders(string id)
        {
            ViewBag.UserId = id;
            var vendorId = User.Identity.GetUserId();
            var allOrders = (string.IsNullOrEmpty(id)) ? 
                            db.OrderItems
                                .Include(m => m.Order)
                                .Include(m => m.Vendor)
                                .Where(m => m.Vendor.ApplicationUserId == vendorId)
                                .OrderByDescending(m => m.Order.OrderDate)
                                .ToList() 
                            : 
                            db.OrderItems
                                .Include(m => m.Order)
                                .Include(m => m.Order.Customer)
                                .Include(m => m.Vendor)
                                .Where(m => m.Vendor.ApplicationUserId == vendorId
                                && m.Order.Customer.ApplicationUserId == id)
                                .OrderByDescending(m => m.Order.OrderDate)
                                .ToList();

            CustomersOrdersViewModel model = new CustomersOrdersViewModel {
                ActiveOrders = new List<Utilities.OrderItem>(),
                CancellationRequested = new List<Utilities.OrderItem>(),
                PastOrders = new List<Utilities.OrderItem>(),
                OtherOrders = new List<Utilities.OrderItem>(),
                NewOrders = new List<int>()
            };

            if (allOrders == null || allOrders.Count == 0)
                return View(model);
            else
            {
                foreach (var orderItm in allOrders)
                {
                    switch (orderItm.Status)
                    {
                        case Utilities.OrderStatus.ActiveOrder: {
                                model.ActiveOrders.Add(orderItm);
                                break;
                            }

                        case Utilities.OrderStatus.CancellationRequested: {
                                model.CancellationRequested.Add(orderItm);
                                break;
                            }

                        case Utilities.OrderStatus.Delivered: {
                                model.PastOrders.Add(orderItm);
                                break;
                            }

                        case Utilities.OrderStatus.NewOrder:
                            {
                                model.ActiveOrders.Add(orderItm);

                                if (orderItm.Order.OrderDate.AddDays(1) <
                                    DateTime.Now)
                                {
                                    orderItm.Status = Utilities.OrderStatus.ActiveOrder;
                                    db.Entry(orderItm).State = EntityState.Modified;
                                    db.SaveChanges();

                                    int refId = db.Refferals
                                        .Include(m => m.Customer)
                                        .Include(m => m.Vendor)
                                        .FirstOrDefault(
                                            m => m.Customer.Id == orderItm.Order.EcomUserId
                                            && m.Vendor.Id == orderItm.EcomUserId).Id;

                                    db.OrderInformation.Add(
                                        new Utilities.OrderStateInfo
                                        {
                                            Type = Utilities.ChangeType.Activated,
                                            InitialDate = DateTime.Now,
                                            OrderItemId = orderItm.Id,
                                            RefferalId = refId
                                        }
                                    );
                                }
                                else
                                {
                                    model.NewOrders.Add(orderItm.Id);
                                }

                                break;
                            }

                        case Utilities.OrderStatus.Cancelled:
                            {
                                model.PastOrders.Add(orderItm);
                                
                                break;
                            }

                        case Utilities.OrderStatus.Undelivered:
                            {
                                model.PastOrders.Add(orderItm);
                                break;
                            }

                        default:
                            {
                                model.OtherOrders.Add(orderItm);
                                break;
                            }
                    }
                }
            }

            return View(model);
        }

        public ActionResult OrderDetails(int? id, string userId)
        {
            if (id == null) return View("Error");
            ViewBag.UserId = userId;

            var orderItem = db.OrderItems
                            .Include(m => m.Order)
                            .Include(m => m.Order.Customer)
                            .Include(m => m.Order.Address)
                            .FirstOrDefault(m => m.Id == id);
            if (orderItem == null) return View("Error");

            var stateInfoItms = db.OrderInformation
                            .Include(m => m.Actors)
                            .Where(m => m.OrderItemId == id)
                            .ToList() ?? new List<Utilities.OrderStateInfo>();

            var model = new CustomersOrderDetailsViewModel {
                OrderItem = orderItem,
                StateInfo = stateInfoItms
            };

            return View(model);
        }

        private void FireEmail(string to, string subject, string message, bool isBodyHtml = false)
        {
            var klEmail = "khushlifeecommerce@gmail.com";
            var klPass = "klEcom1234";
            using (MailMessage mm = new MailMessage(klEmail, to))
            {
                mm.Subject = subject;
                mm.Body = message;
                mm.IsBodyHtml = isBodyHtml;
                using (SmtpClient smtp = new SmtpClient())
                {
                    smtp.Host = "smtp.gmail.com";
                    smtp.EnableSsl = true;
                    NetworkCredential NetworkCred = new NetworkCredential(klEmail, klPass);
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = NetworkCred;
                    smtp.Port = 587;
                    smtp.Send(mm);
                }
            }
        }
    }
}