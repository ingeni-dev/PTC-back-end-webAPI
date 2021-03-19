namespace webAPI.Models.Elearning
{
    public class GetTrainType
    {
        public string TRAIN_TYPE_ID { get; set; }
        public string TRAIN_TYPE_DESC { get; set; }
    }
     public class SetTrainType
    {
        public string trainTypeID { get; set; }
        public string trainTypeDESC { get; set; }
    }
}