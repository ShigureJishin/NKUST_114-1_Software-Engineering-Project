
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
- `WebApplication1/Controllers/StocksController.cs`：股票資料網頁（MVC）查詢、分頁、排序。
- `WebApplication1/Controllers/StockApiController.cs`：股票資料 API（JSON 回傳）。
- `WebApplication1/Models/Stock.cs`：股票資料模型。
- `WebApplication1/Data/ApplicationDbContext.cs`：Web 前端資料庫上下文。
- `WebApplication1/Views/Stocks/`：股票查詢頁面。

---

## 主要功能

- 讀取 JSON 檔案並反序列化為物件
- 寫入股票資料至 LocalDB 資料庫
- 提供 API 讀取股票資料（JSON）
- 前端 UI：網頁查詢、瀏覽、詳細資料顯示

---

## API 文件（Swagger）

啟動 `WebApplication1` 後，可透過 Swagger UI 測試 API：

- Swagger UI：`/swagger`
- OpenAPI JSON：`/swagger/v1/swagger.json`

---

## 現有 API（WebApplication1）

Base URL：`/api/StockApi`

### 取得當日市場漲跌平家數

- `GET /api/StockApi/market-summary`
  - Query：`date`（可選，格式 `yyyy-MM-dd`；未提供則自動使用最近有資料日期）
- `GET /api/StockApi/market-summary/latest`

### 取得指定日期的所有股票資訊

- `GET /api/StockApi/stocks`
  - Query：`date`（可選，格式 `yyyy-MM-dd`；未提供則自動使用最近有資料日期）
- `GET /api/StockApi/stocks/latest`

---

## 執行方式

### ConsoleApp1
1. 請確認 `SourceData/STOCK_DAY_ALL.json` 已放置於專案資料夾。
2. 使用 Visual Studio 或 `dotnet run` 執行 `ConsoleApp1` 專案。
3. 程式會自動讀取 JSON 檔案，反序列化並寫入資料庫。

### WebApplication1
1. 使用 Visual Studio 或執行：

  ```bash
  dotnet run --project WebApplication1/WebApplication1.csproj
  ```

2. 於瀏覽器開啟：

  - 首頁：`/`
  - Swagger：`/swagger`

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

## 建立 Admin 帳號（開發種子或手動）

- 方法 A（建議，開發種子）：啟動 `WebApplication1`（Development），應用會以種子（seed）方式建立帳號 `admin@test.com` 並賦予 `Admin` 角色，僅供本地開發測試。
- 方法 B（手動建立）：若不使用種子，可手動產生 ASP.NET Identity 密碼雜湊，並將使用者與 `Admin` 角色匯入資料庫。

  1. 產生 ASP.NET Identity 的密碼雜湊（範例 C#）：

```csharp
using Microsoft.AspNetCore.Identity;

var user = new IdentityUser { UserName = "admin@test.com", Email = "admin@test.com" };
var hasher = new PasswordHasher<IdentityUser>();
Console.WriteLine(hasher.HashPassword(user, "admin123"));
```

  此程式可放入小型 console 專案或使用 dotnet-script 執行，會輸出 `<PASSWORD_HASH>`。

  2. 將使用者與角色寫入資料庫（範例 SQL，請依實際 schema 調整）：

```sql
INSERT INTO AspNetUsers (Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed, PasswordHash, SecurityStamp)
VALUES ('<GUID>', 'admin@test.com', 'ADMIN@TEST.COM', 'admin@test.com', 'ADMIN@TEST.COM', 1, '<PASSWORD_HASH>', NEWID());

INSERT INTO AspNetRoles (Id, Name, NormalizedName)
VALUES ('<ROLE_GUID>', 'Admin', 'ADMIN');

INSERT INTO AspNetUserRoles (UserId, RoleId)
VALUES ('<GUID>', '<ROLE_GUID>');
```

  注意：直接操作資料表有風險，請先備份；生產環境建議使用管理介面或正式腳本建立並設定強密碼。


## 未來功能

- API擴充：新增更多查詢條件、資料分析功能
- 前端 UI 優化：圖表、互動式介面