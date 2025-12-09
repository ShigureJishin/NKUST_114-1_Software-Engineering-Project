
# NKUST_114-1_Software-Engineering-Project

## 團隊人員

- 作者：ShigureJishin、lyc940626
- 學號：C112152325、C112152305

---

## 專案簡介

本專案為「軟體工程專題」課程作品，包含：
- C# 主控台程式（ConsoleApp1）：讀取台灣上市個股日成交資訊 JSON 檔案，反序列化並寫入 LocalDB 資料庫。
- ASP.NET Core 網頁前端（WebApplication1）：提供股票資料查詢、篩選、分頁、排序等功能，並以 API 方式存取資料。

---

## 資料來源

- [台灣證券交易所公開 API](https://openapi.twse.com.tw/v1/exchangeReport/STOCK_DAY_ALL)

---

## 專案結構

- `ConsoleApp1/Program.cs`：主控台程式入口，負責 JSON 讀取、資料庫寫入。
- `ConsoleApp1/StockInfoEntity.cs`：股票資料 Entity 定義。
- `ConsoleApp1/StockDbContext.cs`：EF Core 資料庫上下文。
- `ConsoleApp1/Migrations/`：資料庫 migration 檔案。
- `ConsoleApp1/SourceData/STOCK_DAY_ALL.json`：原始 JSON 檔案。
- `WebApplication1/Controllers/StocksController.cs`：股票資料 API 與查詢邏輯。
- `WebApplication1/Models/Stock.cs`：股票資料模型。
- `WebApplication1/Data/ApplicationDbContext.cs`：Web 前端資料庫上下文。
- `WebApplication1/Views/Stocks/`：股票查詢頁面。

---

## 主要功能

- 讀取 JSON 檔案並反序列化為物件
- 寫入股票資料至 LocalDB 資料庫
- 提供 API 查詢股票資料（分頁、排序、篩選）
- 前端 UI：網頁查詢、瀏覽、詳細資料顯示

---

## 執行方式

### ConsoleApp1
1. 請確認 `SourceData/STOCK_DAY_ALL.json` 已放置於專案資料夾。
2. 使用 Visual Studio 或 `dotnet run` 執行 `ConsoleApp1` 專案。
3. 程式會自動讀取 JSON 檔案，反序列化並寫入資料庫。

### WebApplication1
1. 使用 Visual Studio 或 `dotnet run` 執行 `WebApplication1` 專案。
2. 於瀏覽器開啟首頁，進行股票資料查詢、篩選、分頁等操作。

---

## JSON 資料格式範例

```json
{
  "Date": "1141105",
  "Code": "0050",
  "Name": "元大台灣50",
  "TradeVolume": "229413552",
  "TradeValue": "14391151897",
  "OpeningPrice": "62.95",
  "HighestPrice": "63.15",
  "LowestPrice": "62.30",
  "ClosingPrice": "63.15",
  "Change": "-0.8000",
  "Transaction": "276109"
}
```

---

## 已完成功能

- 資料庫設計與資料匯入
- API 查詢與資料篩選
- 前端 UI 查詢與顯示

---

## 未來功能

- API擴充：新增更多查詢條件、資料分析功能
- 前端 UI 優化：圖表、互動式介面