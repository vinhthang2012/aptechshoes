using AptechShoseShop.Models.Admin;
using AptechShoseShop.Models.Entites;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AptechShoseShop.Areas.Admin.Controllers
{
    public class ProductsController : Controller
    {
        readonly AptechShoseShopDbContext db = new AptechShoseShopDbContext();
        // GET: Admin/Products
        public ActionResult Index()
        {
            var product = db.Products.OrderByDescending(x => x.Id).ToList();
            return View(product);
        }
        // GET: Admin/Details
        // GET: Admin/Product/Details/5
        public ActionResult Details(int id)
        {
            //color
            List<ProductColor> findColor = db.ProductColors.Where(x => x.ProductId == id).ToList();
            List<string> listFindColor = new List<string>();

            for (int i = 0; i < findColor.Count; i++)
            {
                listFindColor.Add(findColor[i].Color.ColorName);
            }

            ViewBag.ListFindColor = listFindColor;
            //size
            List<ProductSize> findSize = db.ProductSizes.Where(x => x.ProductId == id).ToList();
            List<string> listFindSize = new List<string>();

            for (int i = 0; i < findSize.Count; i++)
            {
                listFindSize.Add(findSize[i].Size.SizeName);
            }

            ViewBag.listFindSize = listFindSize;

            return View(db.Products.Where(s => s.Id == id).FirstOrDefault());
        }
        //Create
        [HttpGet]
        public ActionResult Create()
        {
            var showCate = db.Categories.OrderBy(x => x.Position);

            List<StatusProduct> statusPro = db.StatusProducts.ToList();
            SelectList statusProList = new SelectList(statusPro, "Id", "StatusName");
            ViewBag.StatusPro = statusProList;

            List<Size> size = db.Sizes.ToList();
            SelectList sizeList = new SelectList(size, "Id", "SizeName");
            ViewBag.Size = sizeList;

            List<Color> color = db.Colors.ToList();
            SelectList colorList = new SelectList(color, "Id", "ColorName");
            ViewBag.Color = colorList;

            string date = DateTime.Today.ToString("yyyy-MM-dd");
            ViewBag.date = date;

            return View(showCate);
        }

        [HttpPost]
        public ActionResult Create(ProductVM product, HttpPostedFileBase[] ProductImageId, int sizeId, int colorId)
        {
            Product newPro = new Product()
            {
                ProductName = product.ProductName,
                UnitPrice = product.UnitPrice,
                DiscountRatio = product.DiscountRatio,
                DiscountExpiry = product.DiscountExpiry,
                CategoryId = product.CategoryId,
                Description = product.Description,
                CreatedDate = product.CreatedDate,
                CreatedBy = product.CreatedBy,
                StatusId = product.StatusId
            };
            db.Products.Add(newPro);
            db.SaveChanges();

            ProductSize proSize = new ProductSize()
            {
                ProductId = newPro.Id,
                SizeId = sizeId
            };
            db.ProductSizes.Add(proSize);
            db.SaveChanges();

            ProductColor proColor = new ProductColor()
            {
                ProductId = newPro.Id,
                ColorId = colorId
            };
            db.ProductColors.Add(proColor);
            db.SaveChanges();

            if (ProductImageId[0] == null)
            {
                return RedirectToAction("Index");
            }
            ///Lưu vào bảng ProIamge
            ProductImage proImage = new ProductImage();

            foreach (var item in ProductImageId)
            {
                proImage.ImageUrl = item.FileName;
                proImage.ProductId = newPro.Id;
                db.ProductImages.Add(proImage);
                db.SaveChanges();
            }

            ///Lấy id của url đầu tiên của Pro
            string findUrl = ProductImageId[0].FileName;
            var defaultImage = db.ProductImages.Where(x => x.ImageUrl == findUrl).FirstOrDefault();
            newPro.ProductImageId = defaultImage.Id;
            db.SaveChanges();

            string strFolder = Server.MapPath("~/Data/Products/" + newPro.Id);

            if (!Directory.Exists(strFolder))
            {
                Directory.CreateDirectory(strFolder);
            }
            foreach (var item in ProductImageId)
            {
                item.SaveAs(strFolder + @"\" + item.FileName);
            }

            return RedirectToAction("Index");
        }
        // Edit
        
        [HttpGet]
        public ActionResult Edit(int id)
        {
            ///var showCate = db.Categories.OrderBy(x => x.Position);
            var pro = db.Products.Find(id);
            ViewBag.DateExpi = pro.DiscountExpiry.ToString("yyyy-MM-dd");
            

            var showCate = db.Categories.ToList();
            ViewBag.showCate = showCate;

            var showStaPro = db.StatusProducts.ToList();
            ViewBag.showStaPro = showStaPro;

            var showSize = db.Sizes.ToList();
            ViewBag.showSize = showSize;

            var findProSize = db.ProductSizes.Where(x => x.ProductId == id).FirstOrDefault();
            ViewBag.findProSize = findProSize.Size.SizeName;




            string date = DateTime.Today.ToString("yyyy-MM-dd");
            ViewBag.date = date;

            return View(pro);
        }

        [HttpPost]
        public ActionResult Edit(ProductVM product, HttpPostedFileBase[] ProductImageId, int sizeId, int colorId)
        {
            Product newPro = new Product()
            {
                ProductName = product.ProductName,
                UnitPrice = product.UnitPrice,
                DiscountRatio = product.DiscountRatio,
                DiscountExpiry = product.DiscountExpiry,
                CategoryId = product.CategoryId,
                Description = product.Description,
                CreatedDate = product.CreatedDate,
                CreatedBy = product.CreatedBy,
                StatusId = product.StatusId
            };
            db.Products.Add(newPro);
            db.SaveChanges();

            ProductSize proSize = new ProductSize()
            {
                ProductId = newPro.Id,
                SizeId = sizeId
            };
            //db.ProductSizes.Add(proSize);
            db.ProductSizes.Add(proSize);
            db.SaveChanges();

            ProductColor proColor = new ProductColor()
            {
                ProductId = newPro.Id,
                ColorId = colorId
            };
            db.ProductColors.Add(proColor);
            //db.Entry(proColor).State = EntityState.Modified;
            db.SaveChanges();

            if (ProductImageId[0] == null)
            {
                return RedirectToAction("Index");
            }
            ///Lưu vào bảng ProIamge
            ProductImage proImage = new ProductImage();

            foreach (var item in ProductImageId)
            {
                proImage.ImageUrl = item.FileName;
                proImage.ProductId = newPro.Id;
                db.ProductImages.Add(proImage);
                db.SaveChanges();
            }

            ///Lấy id của url đầu tiên của Pro
            string findUrl = ProductImageId[0].FileName;
            var defaultImage = db.ProductImages.Where(x => x.ImageUrl == findUrl).FirstOrDefault();
            newPro.ProductImageId = defaultImage.Id;
            db.SaveChanges();

            string strFolder = Server.MapPath("~/Data/Products/" + newPro.Id);

            if (!Directory.Exists(strFolder))
            {
                Directory.CreateDirectory(strFolder);
            }
            foreach (var item in ProductImageId)
            {
                item.SaveAs(strFolder + @"\" + item.FileName);
            }

            return RedirectToAction("Index");
        }


    }
}