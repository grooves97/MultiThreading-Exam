using System;
using System.Data.Entity;
using System.Linq;
using Exam.Models;

namespace Exam.DataAcces
{
    public class DataContext : DbContext
    {
        public DataContext()
            : base("name=DataContext")
        {
        }
        public DbSet<DownloadFile> DownloadFiles { get; set; }
    }
}