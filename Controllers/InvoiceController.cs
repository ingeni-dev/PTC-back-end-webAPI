using System.Collections.Generic;
using WebApi.data;
using WebApi.models;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        public InvoiceController()
        {
        }

        //GET
        [HttpGet("invoicetools")]
        public List<InvoiceTools> InvoiceToolsModels()
        {
            List<InvoiceTools> result = new Dummy().resultTool;
            return result;
        }
        [HttpGet("booktax")]
        public List<BookTax> BookTaxModels()
        {
            List<BookTax> result = new Dummy().resultBookTax;
            return result;
        }
        [HttpGet("goodorder")]
        public List<GoodOrder> GoodOrderModels()
        {
            List<GoodOrder> result = new Dummy().resultGoodOrder;
            return result;
        }
        [HttpGet("goods")]
        public List<Goods> GoodsModels()
        {
            List<Goods> result = new Dummy().resultGoods;
            return result;
        }
        [HttpGet("taxcard")]
        public List<TaxCard> TaxCardModels()
        {
            List<TaxCard> result = new Dummy().resultTaxCard;
            return result;
        }
        [HttpGet("orders")]
        public List<Orders> OrdersModels()
        {
            List<Orders> result = new Dummy().resultOrders;
            return result;
        }

        //POST
        [HttpPost("invoicetools")]
        public List<InvoiceTools> InvoiceToolsPostModels()
        {
            List<InvoiceTools> result = new Dummy().resultTool;
            return result;
        }
        [HttpPost("booktax")]
        public List<BookTax> BookTaxPostModels()
        {
            List<BookTax> result = new Dummy().resultBookTax;
            return result;
        }
        [HttpPost("goodorder")]
        public List<GoodOrder> GoodOrderPostModels()
        {
            List<GoodOrder> result = new Dummy().resultGoodOrder;
            return result;
        }
        [HttpPost("goods")]
        public List<Goods> GoodsPostModels()
        {
            List<Goods> result = new Dummy().resultGoods;
            return result;
        }
        [HttpPost("taxcard")]
        public List<TaxCard> TaxCardPostModels()
        {
            List<TaxCard> result = new Dummy().resultTaxCard;
            return result;
        }
        [HttpPost("orders")]
        public List<Orders> OrdersPostModels()
        {
            List<Orders> result = new Dummy().resultOrders;
            return result;
        }
    }
}