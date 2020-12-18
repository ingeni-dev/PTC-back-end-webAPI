namespace WebApi.models
{
    public class BookTax
    {
        public string numTax { get; set; }
        public string memDate { get; set; }
        public string memUser { get; set; }
    }
    public class GoodOrder
    {
        public string barCode { get; set; }
        public string lot { get; set; }
        public string number { get; set; }
        public string weight { get; set; }
    }
    public class Goods
    {
        public string barCode { get; set; }
        public string numProduct { get; set; }
        public string saleLot { get; set; }
        public string pallet { get; set; }
        public string partNum { get; set; }
        public string number { get; set; }
        public string weight { get; set; }
    }
    public class InvoiceTools
    {
        public string assetImage { get; set; }
        public string message { get; set; }
        public string code { get; set; }
    }
    public class Orders
    {
        public string numBuy { get; set; }
        public string returnDate { get; set; }
        public string goodsCode { get; set; }
        public string goodsName { get; set; }
        public string number { get; set; }
        public string weight { get; set; }
        public string unit { get; set; }
    }
    public class TaxCard
    {
        public string codeTax { get; set; }
        public string nameTax { get; set; }
        public string numTax { get; set; }
    }
}