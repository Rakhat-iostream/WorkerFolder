using Microsoft.EntityFrameworkCore;
using QueueAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QueueAPI.Data
{
    public class DataContext : DbContext
    {
        public DbSet<Message> MessageQueue { get; set; }
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
    }
}
