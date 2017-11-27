using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToDoApi.Model;

namespace ToDoApi
{
    public class TodoContext : DbContext
    {
        public DbSet<ToDoItem> Items { get; set; }

        public TodoContext(DbContextOptions<TodoContext> options)
            : base(options) { }
    }
}
