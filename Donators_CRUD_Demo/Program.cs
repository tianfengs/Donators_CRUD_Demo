using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Donators_CRUD_Demo
{
    class Program
    {
        static void PrintDonators(IQueryable<Donator> donators)
        {
            Console.WriteLine("Id\t\t姓名\t\t金额\t\t打赏日期");
            foreach (var donator in donators)
            {
                Console.WriteLine("{0,-10}\t{1,-10}\t{2,-10}\t{3,-10}", donator.Id, donator.Name, donator.Amount, donator.DonateDate.ToShortDateString());
            }
        }

        static void Main(string[] args)
        {
            // 
            using(var db = new DonatorsContext())
            {
                //IEnumerable<Donator> donators;
                IQueryable<Donator> donators;
                //var donators;

                IQueryable<Province> province;

                #region 1.0 执行简单的查询

                Console.WriteLine("1.0 执行简单的查询");
                //1.查询语法
                donators = from d in db.Donators
                               where d.Amount == 50
                               select d;

                //2.方法语法
                donators = db.Donators.Where(d => d.Amount == 50);

                #endregion

                #region 2.0 使用导航属性

                Console.WriteLine();                
                Console.WriteLine("2.0 使用导航属性");

                // 获取山东省打赏超过5元的所有人
                //1 查询语法 
                donators = from pro in db.Provinces
                           where pro.ProvinceName == "山东省"
                           from d in pro.Donators
                           where d.Amount==50
                           select d;

                //2 方法语法
                donators = db.Provinces.Where(pro => pro.ProvinceName == "山东省")
                    .SelectMany(p => p.Donators)
                    .Where(d => d.Amount > 5);

                // 获取打赏者"雪茄"的省份
                //1 查询语法 
                province = from don in db.Donators
                           where don.Name == "雪茄"
                           select don.Province;


                //2 方法语法
                province = db.Donators.Where(don => don.Name == "雪茄").Select(p => p.Province);

                Console.WriteLine("Id\t姓名\t金额\t打赏日期");
                foreach (var donator in donators)
                {
                    Console.WriteLine("{0}\t{1,-10}\t{2}\t{3}", donator.Id, donator.Name, donator.Amount, donator.DonateDate.ToShortDateString());
                }
                Console.WriteLine("\nId\t省份名称");
                foreach (var pro in province)
                {
                    Console.WriteLine("{0}\t{1}", pro.Id, pro.ProvinceName);
                }
                #endregion

                #region 4.0 LINQ投影:查出“所有省的所有打赏者”

                Console.WriteLine();
                Console.WriteLine("4.0 LINQ投影:查出“所有省的所有打赏者”");

                //1 查询语法
                var donatorsPro = from pro in db.Provinces
                                  select new
                                  {
                                      ProV = pro.ProvinceName,
                                      DonatorList = pro.Donators
                                  };

                //2 方法语法
                donatorsPro = db.Provinces.Select(p => new
                {
                    ProV = p.ProvinceName,
                    DonatorList = p.Donators
                });

                //3 返回一个对象的方法语法: 直接利用 ViewModel类
                var donatorsPro1 = db.Provinces.Select(p => new DonatorsProvinceViewModel
                {
                    ProV = p.ProvinceName,
                    DonatorList = p.Donators
                });

                //4 返回一个对象的查询语法: 直接利用 ViewModel类
                donatorsPro1 = from pro in db.Provinces
                               select new DonatorsProvinceViewModel
                               {
                                   ProV = pro.ProvinceName,
                                   DonatorList = pro.Donators
                               };


                Console.WriteLine("\n省份\t打赏者");
                foreach (var donator in donatorsPro1)
                {
                    foreach (var donator1 in donator.DonatorList)
                    {
                        Console.WriteLine("{0}\t{1}", donator.ProV, donator1.Name);
                    }

                }
                #endregion

                #region 5.0 分组Group

                Console.WriteLine();
                Console.WriteLine("5.0 分组Group");

                //1 查询语法
                var donatorsWithProvince = from donator in db.Donators
                                           group donator by donator.Province.ProvinceName
                                           into donatorGroup
                                           select new
                                           {
                                               ProvinceName = donatorGroup.Key,
                                               Donators = donatorGroup
                                           };

                //2 方法语法
                donatorsWithProvince = db.Donators.GroupBy(d => d.Province.ProvinceName)
                    .Select(donatorGroup => new
                    {
                        ProvinceName = donatorGroup.Key,
                        Donators = donatorGroup
                    });

                foreach (var dwp in donatorsWithProvince)
                {
                    Console.WriteLine("{0}的打赏者如下：", dwp.ProvinceName);
                    foreach (var d in dwp.Donators)
                    {
                        Console.WriteLine("{0,-10}\t\t{1}", d.Name, d.Amount);
                    }
                }

                #endregion

                #region 6.0 排序Ordering

                Console.WriteLine();
                Console.WriteLine("6.0 排序Ordering");

                //1 升序查询语法
                donators = from donator in db.Donators
                               orderby donator.Amount ascending //ascending可省略
                               select donator;

                //2 升序方法语法
                donators = db.Donators.OrderBy(d => d.Amount);
                //#endregion

                //1 降序查询语法
                donators = from donator in db.Donators
                               orderby donator.Amount descending
                               select donator;

                //2 降序方法语法
                donators = db.Donators.OrderByDescending(d => d.Amount);

                Console.WriteLine("Id\t姓名\t\t金额\t打赏日期");
                foreach (var donator in donators)
                {
                    Console.WriteLine("{0}\t{1,-10}\t{2}\t{3}", donator.Id, donator.Name, donator.Amount, donator.DonateDate.ToShortDateString());
                }
                #endregion

                #region 7.0 聚合操作

                Console.WriteLine();
                Console.WriteLine("7.0 聚合操作: Count, Sum, Min, Max, Average");

                // 查询山东省的捐献认识
                var count = (from donator in db.Donators
                             where donator.Province.ProvinceName == "山东省"
                             select donator).Count();

                var count2 = db.Donators.Count(d => d.Province.ProvinceName == "山东省");
                Console.WriteLine("查询语法Count={0}，方法语法Count={1}", count, count2);

                // 查询山东省的捐献总金额
                var sum1 = (from don in db.Donators
                           where don.Province.ProvinceName == "山东省"
                           select don.Amount).Sum();
                
                var sum2 = db.Donators.Where(don => don.Province.ProvinceName == "山东省").Sum(a => a.Amount);

                Console.WriteLine("查询语法：山东捐献总金额={0}\n方法语法：山东捐献总金额={1}", sum1, sum2);
                #endregion

                #region 8.0 分页Paging: Skip, Take

                Console.WriteLine();
                Console.WriteLine("8.0 分页Paging: Skip");

                var donatorsBefore = db.Donators;
                var donatorsAfter = db.Donators.OrderBy(d => d.Id).Skip(2);
                var donatorsAfter2 = db.Donators.OrderBy(d => d.Id).Take(3);
                Console.WriteLine("原始数据打印结果：");
                PrintDonators(donatorsBefore);
                Console.WriteLine("Skip(2)之后的结果：");
                PrintDonators(donatorsAfter);
                Console.WriteLine("Take(3)之后的结果：");
                PrintDonators(donatorsAfter2);

                //分页实现
                while (true)
                {
                    Console.WriteLine("您要看第几页数据");
                    string pageStr = Console.ReadLine() ?? "1";
                    int page = int.Parse(pageStr);
                    const int pageSize = 3;
                    if (page > 0 && page < 4)
                    {
                        donators = db.Donators.OrderBy(d => d.Id).Skip((page - 1) * pageSize).Take(pageSize);
                        PrintDonators(donators);
                    }
                    else
                    {
                        break;
                    }
                }

                #endregion

                #region 9.0实现多表连接

                Console.WriteLine();
                Console.WriteLine("9.0实现多表连接");

                var join1 = from p in db.Provinces
                            join donator in db.Donators on p.Id equals donator.Province.Id
                            into donatorList//注意，这里的donatorList是属于某个省份的所有打赏者，很多人会误解为这是两张表join之后的结果集
                            select new
                            {
                                ProvinceName = p.ProvinceName,
                                DonatorList = donatorList
                            };

                var join2 = db.Provinces.GroupJoin(db.Donators,//Provinces集合要连接的Donators实体集合
                    p => p.Id,//左表要连接的键
                    donator => donator.Province.Id,//右表要连接的键
                    (p, donatorGroup) => new//返回的结果集
    {
                        ProvinceName = p.ProvinceName,
                        DonatorList = donatorGroup
                    }
                    );
                foreach (var dwp in join1)
                {
                    Console.WriteLine("{0}的打赏者如下：", dwp.ProvinceName);
                    foreach (var d in dwp.DonatorList)
                    {
                        Console.WriteLine("{0,-10}\t\t{1}", d.Name, d.Amount);
                    }
                }
                #endregion

            }

            Console.WriteLine("Operation completed!");
            Console.Read();
        }
    }
}
