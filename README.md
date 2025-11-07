# NKUST_114-1_Software-Engineering-Project
## 團隊人員

- 作者：ShigureJishin、lyc940626
- 學號：C112152325、C112152305

## 專案簡介

本專案為「軟體工程專題」課程 C# 主控台程式，主要功能為讀取台灣上市個股日成交資訊 JSON 檔案，反序列化後顯示於 Console，並可寫入資料庫。

---

## 專案結構

- `ConsoleApp1/Program.cs`：主程式，負責讀取 JSON、反序列化並顯示資料。
- `ConsoleApp1/StockInfoEntity.cs`：股票資料的 Entity 定義。
- `ConsoleApp1/SourceData/STOCK_DAY_ALL.json`：原始股票日成交資訊 JSON 檔案。

---

## 執行方式

1. 請確認 `SourceData/STOCK_DAY_ALL.json` 已放置於專案資料夾。
2. 使用 Visual Studio 或 `dotnet run` 執行 `ConsoleApp1` 專案。
3. 程式會自動讀取 JSON 檔案，反序列化並顯示前 10 筆資料於 Console。

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

## 主要功能

- 讀取 JSON 檔案並反序列化為物件
- 顯示股票資料於 Console
- 資料庫 Entity 設計（可擴充寫入資料庫功能）


## 未來功能

- 資料庫製作：將股票資料儲存至資料庫，支援查詢與分析。
- 前端 UI：開發網頁或桌面介面，讓使用者更方便瀏覽與操作資料。