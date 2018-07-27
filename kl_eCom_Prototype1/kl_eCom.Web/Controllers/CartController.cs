using kl_eCom.Web.Entities;
using kl_eCom.Web.Models;
using kl_eCom.Web.Utilities;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace kl_eCom.Web.Controllers
{
    public class CartController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Cart
        public ActionResult Index(bool flag = false, string returnUrl = "", bool checkoutErr = false)
        {
            var cart = GetCart();
            var total = 0.0f;
            var prices = new Dictionary<CartItem, float>();
            var names = new Dictionary<CartItem, string>();
            ViewBag.Flag = flag;
            ViewBag.CheckoutErr = checkoutErr;
            foreach (var itm in cart.CartItems)
            {
                var stock = db.Stocks.FirstOrDefault(m => m.Id == itm.StockId);
                var cost = itm.Qty * stock.Price;
                var prod = db.Products.FirstOrDefault(m => m.Id == stock.ProductId);
                names.Add(itm, prod.Name);
                prices.Add(itm, cost);
                total += cost;
            }

            if (Url.IsLocalUrl(returnUrl))
            {
                ViewBag.ReturnUrl = returnUrl;
            }
            else
            {
                ViewBag.ReturnUrl = "";
            }

            return View(new CartIndexViewModel {
                Cart = cart,
                TotalCost = total,
                Prices = prices,
                ProductNames = names
            });
        }

        public ActionResult Delete(int? id, string return_Url)
        {
            if (id == null) return RedirectToAction("Index");
            else
            {
                var cart = GetCart();
                CartItem cartItm = null;
                if (User.Identity.IsAuthenticated)
                {
                    cartItm = db.CartItems.FirstOrDefault(m => m.Id == id);
                    var entry = db.Entry(cartItm);
                    if (entry.State == EntityState.Detached)
                        db.CartItems.Attach(cartItm);
                    db.CartItems.Remove(cartItm);
                    db.SaveChanges();
                }
                else
                {
                    cartItm = cart.CartItems.FirstOrDefault(m => m.Id == id);
                    cart.CartItems.Remove(cartItm);
                }

                if (User.Identity.IsAuthenticated)
                {
                    db.Entry(cart).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    if (HttpContext.Request.Cookies["guestCart"] is HttpCookie cookie)
                    {
                        var cartCookie = JsonConvert.SerializeObject(cart);
                        cookie.Value = cartCookie;
                        cookie.Expires = DateTime.Now.AddMinutes(60);
                        Response.Cookies.Add(cookie);
                    }
                }
                return RedirectToAction("Index", new { flag = true, returnUrl = return_Url });
            }
        }

        [Authorize(Roles = "Customer, Vendor")]
        public ActionResult Checkout(bool addrErr = false)
        {
            ViewBag.AddrErr = addrErr;
            var usrId = User.Identity.GetUserId();
            var cart = GetCart();
            if (cart.CartItems.Count == 0)
            {
                return RedirectToAction("Index", new { checkoutErr = true });
            }
            var addrs = db.Addresses
                        .Where(m => m.ApplicationUserId == usrId)
                        .ToList();
            var totalPrice = 0.0f;
            var cartItems = cart.CartItems.ToList();
            var prices = new Dictionary<int, float>();
            foreach (var itm in cartItems)
            {
                var miniTotal = itm.Qty * itm.Stock.Price;
                prices.Add(itm.Id, miniTotal);
                totalPrice += miniTotal;
            }
            return View(new CheckoutViewModel {
                Addresses = addrs,
                CartItems = cartItems,
                CustomerName = User.Identity.GetUserName(),
                TotalPrice = totalPrice,
                Prices = prices
            });
        }


        [Authorize(Roles = "Customer, Vendor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Checkout(CheckoutViewModel model)
        {
            var usrId = User.Identity.GetUserId();
            var addrId = int.Parse(Request.Form["SelectedAddress"] ?? "0");
            if (addrId != 0)
            {
                var addr = db.Addresses
                    .FirstOrDefault(m => m.Id == addrId
                    && m.ApplicationUserId == usrId);
                if (addr != null)
                {
                    var cart = GetCart();

                    var hash = DateTime.Now.GetHashCode();
                    hash = (hash > 0) ? hash : -hash;

                    var order = db.Orders.Add(new Order
                    {
                        ApplicationUserId = usrId,
                        OrderDate = DateTime.Now,
                        TotalCost = 0.0f,
                        OrderNumber = hash,
                        AddressId = addrId
                    });
                    db.SaveChanges();

                    var orderPerVendor = new Dictionary<string, List<int>>();

                    string subj = "Order Confirmation for Order #" + order.OrderNumber;
                    string msg = "Auto-genrated mail confirming the order you just placed via Khushlife E-Com,\n\n";
                    msg += "Order Details:\n";

                    foreach (var itm in cart.CartItems)
                    {
                        var vendorId = itm.Stock.Store.ApplicationUserId;

                        var orderItem = db.CartItems
                            .Include(m => m.Stock)
                            .Include(m => m.Stock.Store)
                            .Include(m => m.Stock.Product)
                            .FirstOrDefault(m => m.Id == itm.Id);

                        var dbOrderItm = db.OrderItems.Add(new OrderItem {
                            OrderId = order.Id,
                            Order = order,
                            Qty = itm.Qty,
                            Price = itm.Stock.Price,
                            ProductName = itm.Stock.Product.Name,
                            StockId = itm.StockId,
                            FinalCost = itm.Qty * itm.Stock.Price,
                            ApplicationUserId = vendorId,
                            Status = OrderStatus.NewOrder
                        });

                        var stock = db.Stocks.FirstOrDefault(m => m.Id == itm.StockId);
                        // stock.CurrentStock -= itm.Qty;

                        order.TotalCost += dbOrderItm.FinalCost;
                        db.Entry(order).State = EntityState.Modified;
                        db.Entry(stock).State = EntityState.Modified;
                        db.SaveChanges();

                        msg += "\tProduct: " + dbOrderItm.ProductName + ", Quantity: " + dbOrderItm.Qty +
                                ", Value = Rs. " + dbOrderItm.FinalCost + " (" + dbOrderItm.Price + " x " 
                                + dbOrderItm.Qty + ")\n";


                        if (db.Refferals.FirstOrDefault(
                            m => m.CustomerId == usrId && m.VendorId == vendorId)
                            is Refferal refferal)
                        {
                            if (refferal.IsBuyer == false)
                            {
                                refferal.IsBuyer = true;
                                refferal.DateBuyerAdded = DateTime.Now;
                                db.Entry(refferal).State = EntityState.Modified;
                            }
                        }
                        else
                        {
                            db.Refferals.Add(new Refferal
                            {
                                CustomerId = usrId,
                                VendorId = vendorId,
                                DateBuyerAdded = DateTime.Now,
                                IsBuyer = true,
                                IsRegisteredUser = false,
                                DateOfRegistration = null
                            });
                        }
                        db.SaveChanges();

                        if (orderPerVendor.Keys.Contains(vendorId))
                        {
                            orderPerVendor[vendorId].Add(dbOrderItm.Id);
                        }
                        else
                        {
                            orderPerVendor.Add(vendorId, new List<int>());
                            orderPerVendor[vendorId].Add(dbOrderItm.Id);
                        }
                    }

                    msg += "\nRegards,\nKhushlife E-Com Team";
                    FireEmail(db.Users.FirstOrDefault(m => m.Id == usrId).Email,
                        subj, msg);

                    foreach (var vndrId in orderPerVendor.Keys.ToList())
                    {
                        var vendor = db.Users.FirstOrDefault(m => m.Id == vndrId);
                        var customer = db.Users.FirstOrDefault(m => m.Id == usrId);
                        var klEmail = "khushlifeecommerce@gmail.com";
                        var klPass = "klEcom1234";
                        using (MailMessage mm = new MailMessage(klEmail, vendor.Email))
                        {
                            mm.Subject = "New Order Recieved: #" + order.OrderNumber;
                            mm.Body = "Order Details:\n";
                            mm.Body += "Order Date: " + order.OrderDate.ToLongDateString() + "\n";
                            mm.Body += ", Customer Name: " + customer.FirstName + " " + customer.LastName;
                            mm.Body += ", Contact Info: " + customer.PhoneNumber + "\n\n";

                            var i = 0;
                            foreach(var orderItmId in orderPerVendor[vendor.Id])
                            {
                                var orderItm = db.OrderItems
                                    .Include(m => m.StockProduct)
                                    .Include(m => m.StockProduct.Product)
                                    .FirstOrDefault(m => m.Id == orderItmId);

                                mm.Body += "\tItem #" + ++i + ": Product Name - " + orderItm.ProductName +
                                    ", Quantity - " + orderItm.Qty + ", Cost - Rs. " + orderItm.FinalCost + "\n";
                            }
                            mm.Body += "\nTotal Cost = Rs. " + order.TotalCost + "\n";
                            mm.Body += "\n\nRegards,\nKhushlife E-Com";
                            mm.IsBodyHtml = false;
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

                    var cartItems = cart.CartItems.ToList();
                    foreach (var cartItm in cartItems)
                    {
                        db.Entry(cartItm).State = EntityState.Deleted;
                    }
                    db.SaveChanges();
                    return RedirectToAction("MyOrders", "Customer");
                }
                else
                {
                    return View("Error");
                }
            }
            return RedirectToAction("Checkout", new { addrErr = true });
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

        public ActionResult ContinueShopping(string returnUrl = "")
        {
            if(returnUrl != "")
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", controllerName: "Market");
        }

        private Cart GetCart(bool fromUpdate = false)
        {
            Cart cart;
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();
                // Get Cart and update it
                cart = db.Carts
                    .Include(m => m.CartItems)
                    .FirstOrDefault(m => m.ApplicationUserId == userId);

                if (cart is null)
                {
                    cart = db.Carts.Add(new Cart
                    {
                        ApplicationUserId = userId,
                        CartItems = new List<CartItem>()
                    });
                    db.SaveChanges();
                }

                if (cart.CartItems == null) cart.CartItems = new List<CartItem>();

                foreach (var itm in cart.CartItems)
                {
                    itm.Stock = db.Stocks.Include(m => m.Product).FirstOrDefault(m => m.Id == itm.StockId);
                }

                if (!fromUpdate) UpdateCart(ref cart);
            }
            else
            {
                // Get guest Cart or create it if it doesn't exist
                if (HttpContext.Request.Cookies["guestCart"] is HttpCookie cookie)
                    cart = JsonConvert.DeserializeObject<Cart>(cookie.Value);
                else
                {
                    cart = new Cart() { CartItems = new List<CartItem>() };
                    var cartCookie = JsonConvert.SerializeObject(cart);
                    cookie = new HttpCookie("guestCart");
                    cookie.Expires = DateTime.Now.AddMinutes(60);
                    cookie.Value = cartCookie;
                    Response.Cookies.Add(cookie);
                }

                if (cart.CartItems == null) cart.CartItems = new List<CartItem>();
            }
            return cart;
        }

        private int AddToCart(CartAddViewModel model, bool fromUpdate = false)
        {
            var cart = GetCart(fromUpdate);

            var stock = db.Stocks.FirstOrDefault(m => m.Id == model.StockId);
            if (stock is null) return -1;

            var isPresent = false;
            foreach (CartItem itm in cart.CartItems)
            {
                if (itm.StockId == model.StockId)
                {
                    isPresent = true;
                    if ((itm.Qty + model.Qty) <= stock.CurrentStock)
                    {
                        cart.CartItems.First(m => m.Id == itm.Id).Qty += model.Qty;
                    }
                    else
                        return -2;
                }
            }

            if (!isPresent)
            {
                cart.CartItems.Add(new CartItem
                {
                    StockId = model.StockId,
                    Qty = model.Qty
                });
            }

            if (User.Identity.IsAuthenticated)
            {
                db.Entry(cart).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                if (HttpContext.Request.Cookies["guestCart"] is HttpCookie cookie)
                {
                    var cartCookie = JsonConvert.SerializeObject(cart);
                    cookie.Value = cartCookie;
                    cookie.Expires = DateTime.Now.AddMinutes(60);
                    Response.Cookies.Add(cookie);
                }
                else
                {
                    return -3;
                }
            }

            return 0;
        }

        private int UpdateCart(ref Cart cart)
        {
            if (HttpContext.Request.Cookies["guestCart"] is HttpCookie cookie)
            {
                var guestCart = JsonConvert.DeserializeObject<Cart>(cookie.Value);
                foreach (var itm in guestCart.CartItems)
                {
                    AddToCart(new CartAddViewModel { StockId = itm.StockId, Qty = itm.Qty }, true);
                }
                db.Entry(cart).State = EntityState.Modified;
                db.SaveChanges();
                cookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(cookie);
            }
            return 0;
        }
    }
}