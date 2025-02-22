using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication3.Data;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    public class CartController : Controller
    {
       
        // GET: Cart
        public ActionResult Index()
        {
            
            return View();
        }
        public List<CartItem> GetCart()
        {
            List<CartItem> myCart = Session["GioHang"] as
            List<CartItem>;
            //Nếu giỏ hàng chưa tồn tại thì tạo mới và đưa vào Session
            if (myCart == null)
            {
                myCart = new List<CartItem>();
                Session["GioHang"] = myCart;
            }
            return myCart;
        }
        public ActionResult AddToCart(int id)
        {
            //Lấy giỏ hàng hiện tại
            List<CartItem> myCart = GetCart();
            CartItem currentProduct = myCart.FirstOrDefault(p => p.ProductID == id);
            if (currentProduct == null)
            {
                currentProduct = new CartItem(id);
                myCart.Add(currentProduct);
            }
            else
            {
                currentProduct.Number++; //Sản phẩm đã có trong giỏ thì tăng số lượng lên 1
            }
            return RedirectToAction("ConfirmCart", "Cart", new
            {
                id = id
            });
        }
        public ActionResult AddToCart2(int id)
        {
            //Lấy giỏ hàng hiện tại
            List<CartItem> myCart = GetCart();
            CartItem currentProduct = myCart.FirstOrDefault(p => p.ProductID == id);
            if (currentProduct == null)
            {
                currentProduct = new CartItem(id);
                myCart.Add(currentProduct);
            }
            else
            {
                currentProduct.Number++; //Sản phẩm đã có trong giỏ thì tăng số lượng lên 1
            }
            return RedirectToAction("GetCartInfo", "Cart", new
            {
                id = id
            });
        }
        private int GetTotalNumber()
        {
            int totalNumber = 0;
            List<CartItem> myCart = GetCart();
            if (myCart != null)
                totalNumber = myCart.Sum(sp => sp.Number);
            return totalNumber;
        }
        private decimal GetTotalPrice()
        {
            decimal totalPrice = 0;
            List<CartItem> myCart = GetCart();
            if (myCart != null)
                totalPrice = myCart.Sum(sp => sp.FinalPrice());
            return totalPrice;
        }
        public ActionResult GetCartInfo()
        {
            List<CartItem> myCart = GetCart();
            //Nếu giỏ hàng trống thì trả về trang ban đầu
            if (myCart == null || myCart.Count == 0)
            {
                return RedirectToAction("Index", "CustomerProducts");
            }
            ViewBag.TotalNumber = GetTotalNumber();
            ViewBag.TotalPrice = GetTotalPrice();
          
            return View(myCart); //Trả về View hiển thị thông tin giỏ hàng
        }
        public ActionResult DeleteCartItem(int id)
        {
            List<CartItem> myCart = GetCart();
            //Lấy sản phẩm trong giỏ hàng
            var currentProduct = myCart.FirstOrDefault(p => p.ProductID == id);
            if (currentProduct != null)
            {
                myCart.RemoveAll(p => p.ProductID == id);
                return RedirectToAction("GetCartInfo"); //Quay về trang giỏ hàng
            }
            if (myCart.Count == 0) //Quay về trang chủ nếu giỏ hàng không có gì
                return RedirectToAction("Index", "CustomerProducts");
            return RedirectToAction("GetCartInfo"); //Quay về trang giỏ hàng
        }
        public ActionResult UpdateCartItem(int id, int Number)
        {
            List<CartItem> myCart = GetCart();
            //Lấy sản phẩm trong giỏ hàng
            var currentProduct = myCart.FirstOrDefault(p => p.ProductID == id);
            if (currentProduct != null)
            {
                //Cập nhật lại số lượng tương ứng
                //Lưu ý số lượng phải lớn hơn hoặc bằng 1
                currentProduct.Number = Number;
            }
            return RedirectToAction("GetCartInfo"); //Quay về trang giỏ hàng
        }
        public ActionResult ConfirmCart()
        {
            if (Session["TaiKhoan"] == null) //Chưa đăng nhập
                return RedirectToAction("Login", "Users");
            List<CartItem> myCart = GetCart();
            if (myCart == null || myCart.Count == 0) //Chưa có giỏ hàng hoặc chưa có sp
                return RedirectToAction("Index", "CustomerProducts");
            ViewBag.TotalNumber = GetTotalNumber();
            ViewBag.TotalPrice = GetTotalPrice();
            return View(myCart); //Trả về View xác nhận đặt hàng

        }
        DAPMEntities database = new DAPMEntities();
        public ActionResult AgreeCart()
        {
            // Lấy thông tin khách hàng từ session
            Customer khach = Session["TaiKhoan"] as Customer;
            if (khach != null)
            {
                List<CartItem> myCart = GetCart(); // Lấy giỏ hàng từ session
                OrderPro DonHang = new OrderPro(); // Tạo mới đơn đặt hàng
                DonHang.IDCus = khach.IDCus;
                DonHang.DateOrder = DateTime.Now;
                // Sử dụng địa chỉ giao hàng của khách hàng từ thông tin đăng ký tài khoản
                DonHang.AddressDelivery = khach.AddressCus;
                database.OrderProes.Add(DonHang);
                database.SaveChanges();
                // Lần lượt thêm từng chi tiết cho đơn hàng
                //foreach (var product in myCart)
                //{
                //    OrderDetail chitiet = new OrderDetail();
                //    chitiet.IDOrder = DonHang.ID;
                //    chitiet.IDProduct = product.ProductID;
                //    chitiet.Quantity = product.Number;
                //    chitiet.UnitPrice = (double)product.Price;
                //    database.OrderDetails.Add(chitiet);
                //}

                IIterator iterator = new CartIIterator(myCart);
                var item = iterator.First();
                while (iterator.IsDone)
                {
                    OrderDetail chitiet = new OrderDetail();
                    chitiet.IDOrder = DonHang.ID;
                    chitiet.IDProduct = item.ProductID;
                    chitiet.Quantity = item.Number;
                    chitiet.UnitPrice = (double)item.Price;
                    database.OrderDetails.Add(chitiet);
                    item = iterator.Next();
                }
                database.SaveChanges();
                // Xóa giỏ hàng sau khi đặt hàng thành công
                Session["GioHang"] = null;
                return RedirectToAction("Index", "CustomerProducts");
            }
            else
            {
                // Xử lý khi không tìm thấy thông tin khách hàng trong session
                // Ví dụ: hiển thị thông báo lỗi và chuyển hướng đến trang đăng nhập
                return RedirectToAction("Login", "Account");
            }
        }


        public ActionResult ViewOrders()
        {
            // Kiểm tra xem khách hàng đã đăng nhập hay chưa
            if (Session["TaiKhoan"] == null)
                return RedirectToAction("Login", "Users");

            // Lấy thông tin khách hàng từ session
            Customer khach = Session["TaiKhoan"] as Customer;
            if (khach == null)
                return RedirectToAction("Login", "Users");

            // Lấy danh sách đơn hàng của khách hàng
            var orders = database.OrderProes.Where(o => o.IDCus == khach.IDCus).ToList();

            return View(orders);
        }

        public ActionResult ViewOrderDetails(int orderId)
        {
            // Kiểm tra xem khách hàng đã đăng nhập hay chưa
            if (Session["TaiKhoan"] == null)
                return RedirectToAction("Login", "Users");

            // Lấy thông tin đơn hàng
            var order = database.OrderProes.FirstOrDefault(o => o.ID == orderId);
            if (order == null)
                return RedirectToAction("ViewOrders");

            // Lấy danh sách chi tiết đơn hàng
            var orderDetails = database.OrderDetails.Where(od => od.IDOrder == orderId).ToList();

            ViewBag.Order = order;
            return View(orderDetails);
        }

        public ActionResult CancelOrder(int orderId)
        {
            // Kiểm tra xem khách hàng đã đăng nhập hay chưa
            if (Session["TaiKhoan"] == null)
                return RedirectToAction("Login", "Users");

            // Lấy thông tin đơn hàng
            var order = database.OrderProes.FirstOrDefault(o => o.ID == orderId);
            if (order == null)
                return RedirectToAction("ViewOrders");

            // Xóa chi tiết đơn hàng trước
            var orderDetails = database.OrderDetails.Where(od => od.IDOrder == orderId).ToList();
            database.OrderDetails.RemoveRange(orderDetails);

            // Xóa đơn hàng
            database.OrderProes.Remove(order);
            database.SaveChanges();

            return RedirectToAction("CancelSuccess");
        }






        private DAPMEntities db = new DAPMEntities();
        public ActionResult ApplyVoucher(int? productId, int voucherId)
        {
            // Lấy giỏ hàng từ Session
            List<CartItem> myCart = GetCart();

            // Tìm sản phẩm trong giỏ hàng
            CartItem product = myCart.FirstOrDefault(p => p.ProductID == productId);
            if (product == null)
            {
                // Xử lý khi không tìm thấy sản phẩm
                return RedirectToAction("GetCartInfo");
            }

            //  bảng Voucher trong cơ sở dữ liệu với các thông tin về mã giảm giá
            var voucher = db.Vouchers.FirstOrDefault(v => v.VoucherID == voucherId);
           
            if (voucher != null)
            {
                // Lưu ID của voucher và số tiền giảm giá vào sản phẩm tương ứng
                product.VoucherID = voucher.VoucherID;
                product.DiscountAmount = (int?)voucher.DiscountPercent;
               
            }

            // Lưu lại giỏ hàng đã cập nhật
            Session["GioHang"] = myCart;

            // Chuyển hướng về trang hiển thị thông tin giỏ hàng
            return RedirectToAction("GetCartInfo");
        }

        public ActionResult CancelSuccess()
        {
            return View();
        }

        public ActionResult checkOut_Success()
        {
            return View();
        }

    }
}