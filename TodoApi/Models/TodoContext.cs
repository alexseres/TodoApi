using System;
using Microsoft.EntityFrameworkCore;

namespace TodoApi.Models
{
    public class TodoContext : DbContext
    {
        public TodoContext(DbContextOptions<TodoContext> options) : base(options)
        {
            //TodoItems.Add(new TodoItem("maths", true));
        }

        public DbSet<TodoItem> TodoItems { get; set; }


    }
}
