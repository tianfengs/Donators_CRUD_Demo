using System;
using System.Collections.Generic;
using System.Data.Entity;
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
            // 查询数据
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

                //Console.WriteLine();
                //Console.WriteLine("7.0 聚合操作: Count, Sum, Min, Max, Average");

                //// 查询山东省的捐献认识
                //var count = (from donator in db.Donators
                //             where donator.Province.ProvinceName == "山东省"
                //             select donator).Count();

                //var count2 = db.Donators.Count(d => d.Province.ProvinceName == "山东省");
                //Console.WriteLine("查询语法Count={0}，方法语法Count={1}", count, count2);

                //// 查询山东省的捐献总金额
                //var sum1 = (from don in db.Donators
                //           where don.Province.ProvinceName == "山东省"
                //           select don.Amount).Sum();
                
                //var sum2 = db.Donators.Where(don => don.Province.ProvinceName == "山东省").Sum(a => a.Amount);

                //Console.WriteLine("查询语法：山东捐献总金额={0}\n方法语法：山东捐献总金额={1}", sum1, sum2);
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

                ////分页实现
                //while (true)
                //{
                //    Console.WriteLine("您要看第几页数据");
                //    string pageStr = Console.ReadLine() ?? "1";
                //    int page = int.Parse(pageStr);
                //    const int pageSize = 3;
                //    if (page > 0 && page < 4)
                //    {
                //        donators = db.Donators.OrderBy(d => d.Id).Skip((page - 1) * pageSize).Take(pageSize);
                //        PrintDonators(donators);
                //    }
                //    else
                //    {
                //        break;
                //    }
                //}

                #endregion

                #region 9.0实现多表连接

    //            Console.WriteLine();
    //            Console.WriteLine("9.0实现多表连接");

    //            var join1 = from p in db.Provinces
    //                        join donator in db.Donators on p.Id equals donator.Province.Id
    //                        into donatorList//注意，这里的donatorList是属于某个省份的所有打赏者，很多人会误解为这是两张表join之后的结果集
    //                        select new
    //                        {
    //                            ProvinceName = p.ProvinceName,
    //                            DonatorList = donatorList
    //                        };

    //            var join2 = db.Provinces.GroupJoin(db.Donators,//Provinces集合要连接的Donators实体集合
    //                p => p.Id,//左表要连接的键
    //                donator => donator.Province.Id,//右表要连接的键
    //                (p, donatorGroup) => new//返回的结果集
    //{
    //                    ProvinceName = p.ProvinceName,
    //                    DonatorList = donatorGroup
    //                }
    //                );
    //            foreach (var dwp in join1)
    //            {
    //                Console.WriteLine("{0}的打赏者如下：", dwp.ProvinceName);
    //                foreach (var d in dwp.DonatorList)
    //                {
    //                    Console.WriteLine("{0,-10}\t\t{1}", d.Name, d.Amount);
    //                }
    //            }
                #endregion

            }

            // 插入数据            
            using (var db = new DonatorsContext())
            {
                #region 11.0 插入数据

                //Console.WriteLine();
                //Console.WriteLine("11.0 插入数据");

                ////方法一：用Add方法
                //var province = new Province { ProvinceName = "浙江省" };
                //province.Donators.Add(new Donator
                //{
                //    Name = "星空夜焰",
                //    Amount = 50m,
                //    DonateDate = DateTime.Parse("2016-5-30")
                //});

                //province.Donators.Add(new Donator
                //{
                //    Name = "伟涛",
                //    Amount = 25m,
                //    DonateDate = DateTime.Parse("2016-5-25")
                //});

                //db.Provinces.Add(province);
                //db.SaveChanges();

                ////方法二：直接设置对象的状态
                //var province2 = new Province { ProvinceName = "广东省" };
                //province2.Donators.Add(new Donator
                //{
                //    Name = "邱宇",
                //    Amount = 30,
                //    DonateDate = DateTime.Parse("2016-04-25")
                //});

                //db.Entry(province2).State = EntityState.Added;
                //db.SaveChanges();

                #endregion
            }

            // 更新数据
            #region 12.0 更新数据
            using (var db = new DonatorsContext())
            {
                

                //    // 方法一、使用Find查询修改：在写桌面应用的话，可以使用Find方法先找到实体，再修改，最后提交，这是没问题的
                //    Console.WriteLine();
                //    Console.WriteLine("12.0 更新数据");

                //    var donator = db.Donators.Find(3);
                //    donator.Name = "醉千秋";//我想把“醉、千秋”中的顿号去掉
                //    db.SaveChanges();

                //    Console.WriteLine("Id\t姓名\t\t捐赠数\t捐赠日期");
                //    foreach(var d in db.Donators)
                //    {
                //        Console.WriteLine("{0}\t{1,-10}\t{2}\t{3}", d.Id, d.Name, d.Amount, d.DonateDate.ToShortDateString());
                //    }
            }

            //using (var db = new DonatorsContext())
            //{
            //    // 方法二、直接修改状态：但是在Web应用中就不行了，因为不能在两个web服务器调用之间保留原始的上下文。
            //    // 我们也没必要寻找一个实体两次，第一次用于展示给用户，第二次用于更新。相反，我们可以直接修改实体的状态达到目的。
            //    var prov = new Province { Id = 1, ProvinceName = "山东省更新" };
            //    prov.Donators.Add(new Donator
            //    {
            //        Name = "醉、千秋",
            //        Id = 3,
            //        Amount = 12.00m,
            //        DonateDate = DateTime.Parse("2016/4/13"),
            //    });

            //    db.Entry(prov).State = EntityState.Modified;
            //    foreach (var donator in prov.Donators)
            //    {
            //        db.Entry(donator).State = EntityState.Modified;
            //    }
            //    db.SaveChanges();
            //}
            #endregion

            #region 13.0 删除数据
            
            // 方法一: 用Find查询，删除
            using (var db = new DonatorsContext())
            {
                PrintAllDonators(db);

                Console.WriteLine("删除后的数据如下：");

                
                var toDelete = db.Provinces.Find(3);
                if (toDelete != null)
                {
                    toDelete.Donators.ToList().ForEach(
                                        d => db.Donators.Remove(d));
                    db.Provinces.Remove(toDelete);
                    db.SaveChanges();
                    // 上面的代码会删除每个子实体，然后再删除根实体
                }

                

                PrintAllDonators(db);
            }

            // 方法二：设置状态方式，删除
            //var toDeleteProvince = new Province { Id = 1 };
            //toDeleteProvince.Donators.Add(new Donator
            //{
            //    Id = 1
            //});
            //toDeleteProvince.Donators.Add(new Donator
            //{
            //    Id = 2
            //});
            //toDeleteProvince.Donators.Add(new Donator
            //{
            //    Id = 3
            //});

            //using (var db = new DonatorsContext())
            //{
            //    PrintAllDonators(db);//删除前先输出现有的数据,不能写在下面的using语句中，否则Attach方法会报错，原因我相信你已经可以思考出来了
            //}

            //using (var db = new DonatorsContext())
            //{
            //    db.Provinces.Attach(toDeleteProvince);
            //    foreach (var donator in toDeleteProvince.Donators.ToList())
            //    {
            //        db.Entry(donator).State = EntityState.Deleted;
            //    }
            //    db.Entry(toDeleteProvince).State = EntityState.Deleted;//删除完子实体再删除父实体
            //    db.SaveChanges();
            //    Console.WriteLine("删除之后的数据如下：\r\n");
            //    PrintAllDonators(db);//删除后输出现有的数据
            //}

            #endregion


            Console.WriteLine("Operation completed!");
            Console.Read();
        }

        //输出所有的打赏者
        private static void PrintAllDonators(DonatorsContext db)
        {
            // “懒加载”---> “预加载”
            var provinces = db.Provinces.Include(d => d.Donators); // % 此处不能用默认“懒加载”，因为不加载数据，没法删除
                                                                   // % 此处用Include“预加载”Donators才能进行删除操作
            foreach (var province in provinces)
            {
                Console.WriteLine("{0}:{1}的打赏者如下：", province.Id, province.ProvinceName);
                foreach (var don in province.Donators)
                {
                    Console.WriteLine("{0,-10}\t{1,-10}\t{2,-10}\t{3,-10}", don.Id, don.Name, don.Amount,
                        don.DonateDate.ToShortDateString());
                }
            }
        }
    }
}
