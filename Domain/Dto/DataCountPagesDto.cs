namespace Domain.Dto
{
    public class DataCountPagesDto<T>
    {
        public T Data { get; set; }
        public int Count { get; set; }
        public int Pages { get; set; }
    }
}
