using System;
using System.Collections.Generic;
using System.IO;
using System.Linq; // 為了使用 Take()
using System.Text;
using System.Text.Json;


namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8; // 設定控制台輸出編碼為 UTF-8
            Console.WriteLine("上市個股日成交資訊 - JSON 反序列化測試並寫入資料庫");

            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string jsonFilePath = Path.Combine(baseDirectory, "SourceData", "STOCK_DAY_ALL.json");

            Console.WriteLine($"嘗試讀取檔案: {jsonFilePath}");

            if (!File.Exists(jsonFilePath))
            {
                Console.WriteLine($"錯誤：找不到檔案 {jsonFilePath}");
                Console.WriteLine("\n按任意鍵結束...");
                Console.ReadKey();
                return;
            }

            try
            {
                // 讀取 JSON
                Console.WriteLine("正在讀取 JSON 檔案...");
                string jsonString = File.ReadAllText(jsonFilePath, Encoding.UTF8);
                Console.WriteLine($"檔案大小: {new FileInfo(jsonFilePath).Length / 1024.0:F2} KB");


                // 反序列化
                Console.WriteLine("正在進行反序列化...");
                var stockList = JsonSerializer.Deserialize<List<StockInfo>>(jsonString);

                if (stockList != null && stockList.Count > 0)
                {
                    Console.WriteLine($"成功反序列化 {stockList.Count} 筆股票資料\n");

                    int displayCount = Math.Min(10, stockList.Count);
                    Console.WriteLine($"顯示前 {displayCount} 筆資料：\n");

                    for (int i = 0; i < displayCount; i++)
                    {
                        var stock = stockList[i];
                        Console.WriteLine($"第 {i + 1} 筆資料:");
                        Console.WriteLine($"日期: {stock.StockDate}");
                        Console.WriteLine($"股票代碼: {stock.StockCode}");
                        Console.WriteLine($"股票名稱: {stock.StockName}");
                        Console.WriteLine($"成交股數: {stock.TradeVolume}");
                        Console.WriteLine($"成交金額: {stock.TradeValue}");
                        Console.WriteLine($"開盤價: {stock.OpeningPrice}");
                        Console.WriteLine($"最高價: {stock.HighestPrice}");
                        Console.WriteLine($"最低價: {stock.LowestPrice}");
                        Console.WriteLine($"收盤價: {stock.ClosingPrice}");
                        Console.WriteLine($"漲跌: {stock.Change}");
                        Console.WriteLine($"成交筆數: {stock.Transaction}");
                        Console.WriteLine(new string('-', 50));
                    }

                    if (stockList.Count > 10)
                    {
                        Console.WriteLine($"... 還有 {stockList.Count - 10} 筆資料未顯示\n");
                    }

                    // 寫入資料庫
                    Console.WriteLine("正在寫入資料庫...");
                    using (var db = new StockDbContext())
                    {
                        foreach (var stock in stockList)
                        {
                            // 資料型別轉換
                            var entity = new StockInfoEntity
                            {
                                StockCode = stock.StockCode,
                                StockName = stock.StockName,
                                StockDate = DateTime.TryParseExact(stock.StockDate, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out var date) ? date : DateTime.Now,
                                TradeVolume = long.TryParse(stock.TradeVolume, out var tv) ? tv : 0,
                                TradeValue = long.TryParse(stock.TradeValue, out var tval) ? tval : 0,
                                OpeningPrice = decimal.TryParse(stock.OpeningPrice, out var op) ? op : 0,
                                HighestPrice = decimal.TryParse(stock.HighestPrice, out var hp) ? hp : 0,
                                LowestPrice = decimal.TryParse(stock.LowestPrice, out var lp) ? lp : 0,
                                ClosingPrice = decimal.TryParse(stock.ClosingPrice, out var cp) ? cp : 0,
                                Change = decimal.TryParse(stock.Change, out var ch) ? ch : 0,
                                Transaction = int.TryParse(stock.Transaction, out var tr) ? tr : 0
                            };
                            db.Stocks.Add(entity);
                        }
                        db.SaveChanges();
                    }
                    Console.WriteLine("資料庫寫入完成！");
                }
                else
                {
                    Console.WriteLine("JSON 內容為空或格式錯誤。");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"讀取 JSON 發生錯誤: {ex.Message}");
            }

            Console.WriteLine("\n按任意鍵結束...");
            Console.ReadKey();
        }
    }
}
