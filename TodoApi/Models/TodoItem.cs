using System;
namespace TodoApi.Models
{
    public class TodoItem
    {
        System.Random random = new Random();

        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsComplete { get; set; }
        public string Secret { get; set; }


        //public TodoItem(string name, bool isComplete)
        //{
        //    Id = random.Next();
        //    Name = name;
        //    IsComplete = isComplete;
        //}
    }


}
