using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper.Internal;
using cfcusaga.data;
using Cfcusaga.Web.ViewModels;

namespace Cfcusaga.Web.Controllers
{
    public class StoreController : Controller
    {
        PortalDbContext storeDB = new PortalDbContext();

        //
        // GET: /Store/

        public ActionResult Index()
        {
            var catagories = storeDB.Catagories.ToList();

            return View(catagories);
        }

        //
        // GET: /Store/Browse?genre=Disco

        public ActionResult Browse(string catagorie)
        {
            // Retrieve Genre and its Associated Items from database
            var catagorieModel = storeDB.Catagories.Include("Items")
                .Single(g => g.Name == catagorie);

            return View(catagorieModel);
        }

        //
        // GET: /Store/Details/5
        public ActionResult Details(int id)
        {
            ViewBag.Title = "Item";
            var cartItem = storeDB.Carts.Find(id);
            var item = storeDB.Items.Find(cartItem.ItemId);

            if (item.IsRequireTshirtSize != null && item.IsRequireTshirtSize.Value)
            {
                //TODO: Remove this duplication from other pages
                var list = ItemRegistrationsController.GetTShirtSizesList();
                ViewBag.TshirtSizes = list;
            }

            var cartItemModel = new CartItemViewModel();
            cartItemModel.CategoryId = item.CatagoryID;
            cartItemModel.Name = item.Name;
            cartItemModel.ItemPictureUrl = item.ItemPictureUrl;
            cartItemModel.Id = cartItem.ID;
            cartItemModel.ItemId = cartItem.ItemId;
            cartItemModel.Price = item.Price;
            cartItemModel.IsRequireTshirtSize = item.IsRequireTshirtSize;
            cartItemModel.TshirtSize = cartItem.TshirtSize;
            return View(cartItemModel);
        }

        //
        // GET: /Store/GenreMenu
        [ChildActionOnly]
        public ActionResult CatagorieMenu()
        {
            var catagories = storeDB.Catagories.ToList();

            return PartialView(catagories);
        }
    }
}