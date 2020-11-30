using System;
using System.Linq;
using System.Threading;
using DMicroservices.DataAccess.Tests.Models;
using DMicroservices.DataAccess.UnitOfWork;
using DMicroservices.Utils.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DMicroservices.DataAccess.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            using (UnitOfWork<MasterContext> uow = new UnitOfWork<MasterContext>())
            {
                var repo = uow.GetRepository<ClassModel>();
                var repoSt = uow.GetRepository<StudentModel>();

                
                repo.Add(new ClassModel()
                {
                    Id = 2,
                    Name = "test2"
                });

                repo.Add(new ClassModel()
                {
                    Id = 1,
                    Name = "tes2"
                });
                uow.SaveChanges();

            }

           
            Console.WriteLine("Hello World!");
            Console.ReadLine();
        }
    }
}
